// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace FabActUtil.CommandLineParser
{
    using System;
    using System.Collections;
    using System.Diagnostics;
    using System.IO;
    using System.Reflection;
    using System.Text;

    /// <summary>
    /// Used to control parsing of command line arguments.
    /// </summary>
    [Flags]
    public enum CommandLineArgumentType
    {
        /// <summary>
        /// Indicates that this field is required. An error will be displayed
        /// if it is not present when parsing arguments.
        /// </summary>
        Required = 0x01,

        /// <summary>
        /// Only valid in conjunction with Multiple.
        /// Duplicate values will result in an error.
        /// </summary>
        Unique = 0x02,

        /// <summary>
        /// Indicates that the argument may be specified more than once.
        /// Only valid if the argument is a collection
        /// </summary>
        Multiple = 0x04,

        /// <summary>
        /// The default type for non-collection arguments.
        /// The argument is not required, but an error will be reported if it is specified more than once.
        /// </summary>
        AtMostOnce = 0x00,

        /// <summary>
        /// For non-collection arguments, when the argument is specified more than
        /// once no error is reported and the value of the argument is the last
        /// value which occurs in the argument list.
        /// </summary>
        LastOccurenceWins = Multiple,

        /// <summary>
        /// The default type for collection arguments.
        /// The argument is permitted to occur multiple times, but duplicate 
        /// values will cause an error to be reported.
        /// </summary>
        MultipleUnique = Multiple | Unique,
    }

    /// <summary>
    /// Allows control of command line parsing.
    /// Attach this attribute to instance fields of types used
    /// as the destination of command line argument parsing.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class CommandLineArgumentAttribute : Attribute
    {
        /// <summary>
        /// Allows control of command line parsing.
        /// </summary>
        /// <param name="type"> Specifies the error checking to be done on the argument. </param>
        public CommandLineArgumentAttribute(CommandLineArgumentType type)
        {
            this.Type = type;
            this.Description = null;
        }

        /// <summary>
        /// The error checking to be done on the argument.
        /// </summary>
        public CommandLineArgumentType Type { get; private set; }

        /// <summary>
        /// Returns true if the argument did not have an explicit short name specified.
        /// </summary>
        public bool DefaultShortName
        {
            get { return null == this.ShortName; }
        }

        /// <summary>
        /// The short name of the argument.
        /// </summary>
        public string ShortName { get; set; }

        /// <summary>
        /// Returns true if the argument did not have an explicit long name specified.
        /// </summary>
        public bool DefaultLongName
        {
            get { return null == this.longName; }
        }

        /// <summary>
        /// The long name of the argument.
        /// </summary>
        public string LongName
        {
            get
            {
                Debug.Assert(!this.DefaultLongName);
                return this.longName;
            }
            set { this.longName = value; }
        }


        public string Description { get; set; }

        private string longName;
    }

    /// <summary>
    /// Indicates that this argument is the default argument.
    /// '/' or '-' prefix only the argument value is specified.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class DefaultCommandLineArgumentAttribute : CommandLineArgumentAttribute
    {
        /// <summary>
        /// Indicates that this argument is the default argument.
        /// </summary>
        /// <param name="type"> Specifies the error checking to be done on the argument. </param>
        public DefaultCommandLineArgumentAttribute(CommandLineArgumentType type)
            : base(type)
        {
        }
    }

    /// <summary>
    /// Parser for command line arguments.
    ///
    /// The parser specification is infered from the instance fields of the object
    /// specified as the destination of the parse.
    /// Valid argument types are: int, uint, string, bool, enums
    /// Also argument types of Array of the above types are also valid.
    /// 
    /// Error checking options can be controlled by adding a CommandLineArgumentAttribute
    /// to the instance fields of the destination object.
    ///
    /// At most one field may be marked with the DefaultCommandLineArgumentAttribute
    /// indicating that arguments without a '-' or '/' prefix will be parsed as that argument.
    ///
    /// If not specified then the parser will infer default options for parsing each
    /// instance field. The default long name of the argument is the field name. The
    /// default short name is the first character of the long name. Long names and explicitly
    /// specified short names must be unique. Default short names will be used provided that
    /// the default short name does not conflict with a long name or an explicitly
    /// specified short name.
    ///
    /// Arguments which are array types are collection arguments. Collection
    /// arguments can be specified multiple times.
    /// </summary>
    public class CommandLineArgumentParser
    {
        /// <summary>
        /// Creates a new command line argument parser.
        /// </summary>
        /// <param name="argumentSpecification"> The type of object to  parse. </param>
        /// <param name="reporter"> The destination for parse errors. </param>
        public CommandLineArgumentParser(Type argumentSpecification, ErrorReporter reporter)
        {
            this.reporter = reporter;
            this.arguments = new ArrayList();
            this.argumentMap = new Hashtable();

            foreach (var field in argumentSpecification.GetFields())
            {
                if (!field.IsStatic && !field.IsInitOnly && !field.IsLiteral)
                {
                    var attribute = GetAttribute(field);
                    if (attribute is DefaultCommandLineArgumentAttribute)
                    {
                        Debug.Assert(this.defaultArgument == null);
                        this.defaultArgument = new Argument(attribute, field, reporter);
                    }
                    else
                    {
                        this.arguments.Add(new Argument(attribute, field, reporter));
                    }
                }
            }

            // add explicit names to map
            foreach (Argument argument in this.arguments)
            {
                Debug.Assert(!this.argumentMap.ContainsKey(argument.LongName));
                this.argumentMap[argument.LongName] = argument;
                if (argument.ExplicitShortName && argument.ShortName != null && argument.ShortName.Length > 0)
                {
                    Debug.Assert(!this.argumentMap.ContainsKey(argument.ShortName));
                    this.argumentMap[argument.ShortName] = argument;
                }
            }

            // add implicit names which don't collide to map
            foreach (Argument argument in this.arguments)
            {
                if (!argument.ExplicitShortName && argument.ShortName != null && argument.ShortName.Length > 0)
                {
                    if (!this.argumentMap.ContainsKey(argument.ShortName))
                    {
                        this.argumentMap[argument.ShortName] = argument;
                    }
                }
            }
        }

        private static CommandLineArgumentAttribute GetAttribute(FieldInfo field)
        {
            var attributes = field.GetCustomAttributes(typeof(CommandLineArgumentAttribute), false);
            if (attributes.Length == 1)
            {
                return (CommandLineArgumentAttribute)attributes[0];
            }

            Debug.Assert(attributes.Length == 0);
            return null;
        }

        private void ReportUnrecognizedArgument(string argument)
        {
            this.reporter(string.Format("Unrecognized command line argument '{0}'", argument));
        }

        /// <summary>
        /// Parses an argument list into an object
        /// </summary>
        /// <param name="args"></param>
        /// <param name="destination"></param>
        /// <returns> true if an error occurred </returns>
        private bool ParseArgumentList(string[] args, object destination)
        {
            var hadError = false;
            if (args != null)
            {
                foreach (var argument in args)
                {
                    if (argument.Length > 0)
                    {
                        switch (argument[0])
                        {
                            case '-':
                            case '/':
                                var endIndex = argument.IndexOfAny(new[] { ':', '+', '-' }, 1);
                                var option = argument.Substring(1, endIndex == -1 ? argument.Length - 1 : endIndex - 1);
                                string optionArgument;
                                if (endIndex == -1)
                                {
                                    optionArgument = null;
                                }
                                else if (argument.Length > 1 + option.Length && argument[1 + option.Length] == ':')
                                {
                                    optionArgument = argument.Substring(option.Length + 2);
                                }
                                else
                                {
                                    optionArgument = argument.Substring(option.Length + 1);
                                }

                                var arg = (Argument)this.argumentMap[option];
                                if (arg == null)
                                {
                                    this.ReportUnrecognizedArgument(argument);
                                    hadError = true;
                                }
                                else
                                {
                                    hadError |= !arg.SetValue(optionArgument, destination);
                                }
                                break;
                            case '@':
                                string[] nestedArguments;
                                hadError |= this.LexFileArguments(argument.Substring(1), out nestedArguments);
                                hadError |= this.ParseArgumentList(nestedArguments, destination);
                                break;
                            default:
                                if (this.defaultArgument != null)
                                {
                                    hadError |= !this.defaultArgument.SetValue(argument, destination);
                                }
                                else
                                {
                                    this.ReportUnrecognizedArgument(argument);
                                    hadError = true;
                                }
                                break;
                        }
                    }
                }
            }

            return hadError;
        }

        /// <summary>
        /// Parses an argument list.
        /// </summary>
        /// <param name="args"> The arguments to parse. </param>
        /// <param name="destination"> The destination of the parsed arguments. </param>
        /// <returns> true if no parse errors were encountered. </returns>
        public bool Parse(string[] args, object destination)
        {
            var hadError = this.ParseArgumentList(args, destination);

            // check for missing required arguments
            foreach (Argument arg in this.arguments)
            {
                hadError |= arg.Finish(destination);
            }
            if (this.defaultArgument != null)
            {
                hadError |= this.defaultArgument.Finish(destination);
            }

            return !hadError;
        }


        /// <summary>
        /// A user firendly usage string describing the command line argument syntax.
        /// </summary>
        public string Usage
        {
            get
            {
                var builder = new StringBuilder();

                var oldLength = builder.Length;

                if (this.defaultArgument != null)
                {
                    this.AppendArgumentUsage(builder, this.defaultArgument, oldLength, true);
                }

                foreach (Argument arg in this.arguments)
                {
                    oldLength = builder.Length;
                    this.AppendArgumentUsage(builder, arg, oldLength, false);
                }

                builder.Append(CommandLineUtility.NewLine);
                return builder.ToString();
            }
        }

        private void AppendArgumentUsage(StringBuilder builder, Argument arg, int oldLength, bool isDefault)
        {
            if (isDefault)
            {
                builder.Append("   [/");
            }
            else
            {
                builder.Append("    /");
            }
            builder.Append(arg.LongName);
            var valueType = arg.ValueType;
            if (valueType == typeof(int))
            {
                if (isDefault)
                {
                    builder.Append(":]<int>");
                }
                else
                {
                    builder.Append(":<int>");
                }
            }
            else if (valueType == typeof(uint))
            {
                if (isDefault)
                {
                    builder.Append(":]<uint>");
                }
                else
                {
                    builder.Append(":<uint>");
                }
            }
            else if (valueType == typeof(bool))
            {
                if (isDefault)
                {
                    builder.Append("]{+|-}");
                }
                else
                {
                    builder.Append("{+|-}");
                }
            }
            else if (valueType == typeof(string))
            {
                if (isDefault)
                {
                    builder.Append(":]<string>");
                }
                else
                {
                    builder.Append(":<string>");
                }
            }
            else
            {
                Debug.Assert(valueType.IsEnum);

                if (isDefault)
                {
                    builder.Append(":]{");
                }
                else
                {
                    builder.Append(":{");
                }

                var first = true;
                foreach (var field in valueType.GetFields())
                {
                    if (field.IsStatic)
                    {
                        if (first)
                        {
                            first = false;
                        }
                        else
                        {
                            builder.Append('|');
                        }

                        builder.Append(field.Name);
                    }
                }
                builder.Append('}');
            }

            if (arg.ShortName != arg.LongName && this.argumentMap[arg.ShortName] == arg)
            {
                builder.Append(' ', IndentLength(builder.Length - oldLength));
                builder.Append("short form /");
                builder.Append(arg.ShortName);
            }

            builder.Append(CommandLineUtility.NewLine);

            if (arg.Description != null)
            {
                builder.Append("        " + arg.Description);
                builder.Append(CommandLineUtility.NewLine);
                builder.Append(CommandLineUtility.NewLine);
            }
        }

        private static int IndentLength(int lineLength)
        {
            return Math.Max(4, 40 - lineLength);
        }

        private bool LexFileArguments(string fileName, out string[] parameters)
        {
            string args = null;

            try
            {
                using (var file = new FileStream(fileName, FileMode.Open, FileAccess.Read))
                {
                    args = (new StreamReader(file)).ReadToEnd();
                }
            }
            catch (Exception e)
            {
                this.reporter(string.Format("Error: Can't open command line argument file '{0}' : '{1}'", fileName,
                    e.Message));
                parameters = null;
                return false;
            }

            var hadError = false;
            var argArray = new ArrayList();
            var currentArg = new StringBuilder();
            var inQuotes = false;
            var index = 0;

            // while (index < args.Length)
            try
            {
                while (true)
                {
                    // skip whitespace
                    while (char.IsWhiteSpace(args[index]))
                    {
                        index += 1;
                    }

                    // # - comment to end of line
                    if (args[index] == '#')
                    {
                        index += 1;
                        while (args[index] != '\n')
                        {
                            index += 1;
                        }
                        continue;
                    }

                    // do one argument
                    do
                    {
                        if (args[index] == '\\')
                        {
                            var cSlashes = 1;
                            index += 1;
                            while (index == args.Length && args[index] == '\\')
                            {
                                cSlashes += 1;
                            }

                            if (index == args.Length || args[index] != '"')
                            {
                                currentArg.Append('\\', cSlashes);
                            }
                            else
                            {
                                currentArg.Append('\\', (cSlashes >> 1));
                                if (0 != (cSlashes & 1))
                                {
                                    currentArg.Append('"');
                                }
                                else
                                {
                                    inQuotes = !inQuotes;
                                }
                            }
                        }
                        else if (args[index] == '"')
                        {
                            inQuotes = !inQuotes;
                            index += 1;
                        }
                        else
                        {
                            currentArg.Append(args[index]);
                            index += 1;
                        }
                    } while (!char.IsWhiteSpace(args[index]) || inQuotes);
                    argArray.Add(currentArg.ToString());
                    currentArg.Length = 0;
                }
            }
            catch (IndexOutOfRangeException)
            {
                // got EOF 
                if (inQuotes)
                {
                    this.reporter(string.Format("Error: Unbalanced '\"' in command line argument file '{0}'", fileName));
                    hadError = true;
                }
                else if (currentArg.Length > 0)
                {
                    // valid argument can be terminated by EOF
                    argArray.Add(currentArg.ToString());
                }
            }

            parameters = (string[])argArray.ToArray(typeof(string));
            return hadError;
        }

        private static string LongName(CommandLineArgumentAttribute attribute, FieldInfo field)
        {
            return (attribute == null || attribute.DefaultLongName) ? field.Name : attribute.LongName;
        }

        private static string Description(CommandLineArgumentAttribute attribute)
        {
            return attribute != null ? attribute.Description : null;
        }

        private static string ShortName(CommandLineArgumentAttribute attribute, FieldInfo field)
        {
            return !ExplicitShortName(attribute) ? LongName(attribute, field).Substring(0, 1) : attribute.ShortName;
        }

        private static bool ExplicitShortName(CommandLineArgumentAttribute attribute)
        {
            return (attribute != null && !attribute.DefaultShortName);
        }

        private static Type ElementType(FieldInfo field)
        {
            if (IsCollectionType(field.FieldType))
            {
                return field.FieldType.GetElementType();
            }

            return null;
        }

        private static CommandLineArgumentType Flags(CommandLineArgumentAttribute attribute, FieldInfo field)
        {
            if (attribute != null)
            {
                return attribute.Type;
            }

            if (IsCollectionType(field.FieldType))
            {
                return CommandLineArgumentType.MultipleUnique;
            }

            return CommandLineArgumentType.AtMostOnce;
        }

        private static bool IsCollectionType(Type type)
        {
            return type.IsArray;
        }

        private static bool IsValidElementType(Type type)
        {
            return type != null && (
                type == typeof(int) ||
                type == typeof(uint) ||
                type == typeof(string) ||
                type == typeof(bool) ||
                type.IsEnum);
        }

        private class Argument
        {
            public Argument(CommandLineArgumentAttribute attribute, FieldInfo field, ErrorReporter reporter)
            {
                this.LongName = LongName(attribute, field);
                this.Description = Description(attribute);
                this.ExplicitShortName = ExplicitShortName(attribute);
                this.ShortName = ShortName(attribute, field);
                this.elementType = ElementType(field);
                this.flags = Flags(attribute, field);
                this.field = field;
                this.SeenValue = false;
                this.reporter = reporter;
                this.IsDefault = attribute != null && attribute is DefaultCommandLineArgumentAttribute;

                if (this.IsCollection)
                {
                    this.collectionValues = new ArrayList();
                }

                Debug.Assert(this.LongName != null && this.LongName.Length > 0);
                Debug.Assert(!this.IsCollection || this.AllowMultiple, "Collection arguments must have allow multiple");
                Debug.Assert(!this.Unique || this.IsCollection, "Unique only applicable to collection arguments");
                Debug.Assert(IsValidElementType(this.Type) ||
                             IsCollectionType(this.Type));
                Debug.Assert((this.IsCollection && IsValidElementType(this.elementType)) ||
                             (!this.IsCollection && this.elementType == null));
            }

            public bool Finish(object destination)
            {
                if (this.IsCollection)
                {
                    this.field.SetValue(destination, this.collectionValues.ToArray(this.elementType));
                }

                return this.ReportMissingRequiredArgument();
            }

            private bool ReportMissingRequiredArgument()
            {
                if (this.IsRequired && !this.SeenValue)
                {
                    if (this.IsDefault)
                    {
                        this.reporter(string.Format("Missing required argument '<{0}>'.", this.LongName));
                    }
                    else
                    {
                        this.reporter(string.Format("Missing required argument '/{0}'.", this.LongName));
                    }

                    return true;
                }
                return false;
            }

            private void ReportDuplicateArgumentValue(string value)
            {
                this.reporter(string.Format("Duplicate '{0}' argument '{1}'", this.LongName, value));
            }

            public bool SetValue(string value, object destination)
            {
                if (this.SeenValue && !this.AllowMultiple)
                {
                    this.reporter(string.Format("Duplicate '{0}' argument", this.LongName));
                    return false;
                }
                this.SeenValue = true;

                if (!this.ParseValue(this.ValueType, value, out var newValue))
                {
                    return false;
                }

                if (this.IsCollection)
                {
                    if (this.Unique && this.collectionValues.Contains(newValue))
                    {
                        this.ReportDuplicateArgumentValue(value);
                        return false;
                    }
                    this.collectionValues.Add(newValue);
                }
                else
                {
                    this.field.SetValue(destination, newValue);
                }

                return true;
            }

            public Type ValueType
            {
                get { return this.IsCollection ? this.elementType : this.Type; }
            }

            private void ReportBadArgumentValue(string value)
            {
                this.reporter(string.Format("'{0}' is not a valid value for the '{1}' command line option", value,
                    this.LongName));
            }

            private bool ParseValue(Type type, string stringData, out object value)
            {
                // null is only valid for bool variables
                // empty string is never valid
                if ((stringData != null || type == typeof(bool)) && (stringData == null || stringData.Length > 0))
                {
                    try
                    {
                        if (type == typeof(string))
                        {
                            value = stringData;
                            return true;
                        }
                        if (type == typeof(bool))
                        {
                            if (stringData == null || stringData == "+")
                            {
                                value = true;
                                return true;
                            }
                            if (stringData == "-")
                            {
                                value = false;
                                return true;
                            }
                        }
                        else if (type == typeof(int))
                        {
                            value = int.Parse(stringData);
                            return true;
                        }
                        else if (type == typeof(uint))
                        {
                            value = int.Parse(stringData);
                            return true;
                        }
                        else
                        {
                            Debug.Assert(type.IsEnum);
                            value = Enum.Parse(type, stringData, true);
                            return true;
                        }
                    }
                    catch
                    {
                        // catch parse errors
                    }
                }

                this.ReportBadArgumentValue(stringData);
                value = null;
                return false;
            }

            public string LongName { get; private set; }

            public string Description { get; private set; }

            public bool ExplicitShortName { get; private set; }

            public string ShortName { get; private set; }

            public bool IsRequired
            {
                get { return 0 != (this.flags & CommandLineArgumentType.Required); }
            }

            public bool SeenValue { get; private set; }

            public bool AllowMultiple
            {
                get { return 0 != (this.flags & CommandLineArgumentType.Multiple); }
            }

            public bool Unique
            {
                get { return 0 != (this.flags & CommandLineArgumentType.Unique); }
            }

            public Type Type
            {
                get { return this.field.FieldType; }
            }

            public bool IsCollection
            {
                get { return IsCollectionType(this.Type); }
            }

            public bool IsDefault { get; private set; }

            private readonly FieldInfo field;
            private readonly Type elementType;
            private readonly CommandLineArgumentType flags;
            private readonly ArrayList collectionValues;
            private readonly ErrorReporter reporter;
        }

        private readonly ArrayList arguments;
        private readonly Hashtable argumentMap;
        private readonly Argument defaultArgument;
        private readonly ErrorReporter reporter;
    }
}
