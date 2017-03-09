// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------
namespace Microsoft.ServiceFabric.Actors.Diagnostics
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    internal class PerformanceCounterInstanceNameBuilder
    {
        private const int MaxCounterInstanceNameLen = 127;

        // The counter instance name for a method contains the substring "_XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX".
        // So compute the number of characters remaining for the rest of the instance name.
        private static readonly int MaxInstanceNameVariablePartsLen = MaxCounterInstanceNameLen - Guid.Empty.ToString().Length - 1;

        private Guid partitionId;
        private readonly string counterInstanceDifferentiator;
        private readonly int maxMethodInfoLen;

        internal PerformanceCounterInstanceNameBuilder(Guid partitionId, string counterInstanceDifferentiator)
        {
            this.partitionId = partitionId;

            this.counterInstanceDifferentiator = counterInstanceDifferentiator;

            // Adjust the number of characters available to hold the method information to account for the
            // counterInstanceDifferentiator that is appended at the end.
            this.maxMethodInfoLen = MaxInstanceNameVariablePartsLen - this.counterInstanceDifferentiator.Length - 1;
        }

        internal IEnumerable<KeyValuePair<long, string>> GetActorMethodCounterInstanceNames(
            IEnumerable<KeyValuePair<long, MethodInfo>> actorMethodInfo)
        {
            // The method name will be part of the performance counter instance name so that it is 
            // easy for a user to figure out which method the counter instance is for. Create the data
            // structure that will help us build the method names from the MethodInfo objects.
            var methodNameBuilders = this.CreateMethodNameBuilders(actorMethodInfo);

            // Assign a rank to each method that will be unique within the actor type.
            // The performance counter instance name will contain a string representation of the method
            // rank, to avoid collisions among counter instance names for different methods.
            this.ComputeMethodRanks(methodNameBuilders.Values);

            // Figure out the maximum possible length for the method name in the counter instance name.
            this.ComputeMaxMethodNameLength(methodNameBuilders.Values);

            // Identify overloaded methods. These methods will have the same type and method name, so
            // we'll include some parameter info in the performance counter instance names for these
            // methods. This will make it easier to differentiate between the different overloads.
            var methodOverloads = this.CreateMethodOverloadMap(methodNameBuilders.Values);

            // Compute the actual method name component of the counter instance name
            this.ComputeMethodNameInCounterInstanceName(methodNameBuilders.Values, methodOverloads);

            // Compute the counter instance names
            var counterInstanceNames = this.ComputeCounterInstanceNames(methodNameBuilders);

            return counterInstanceNames;
        }

        private Dictionary<long, MethodNameBuilder> CreateMethodNameBuilders(
            IEnumerable<KeyValuePair<long, MethodInfo>> actorMethodInfo)
        {
            var methodNameInfo = new Dictionary<long, MethodNameBuilder>();
            foreach (var kvp in actorMethodInfo)
            {
                var mi = kvp.Value;
                var mnInfo = new MethodNameBuilder() {MethodInfo = mi, Names = new string[(int) MethodNameFormat.Count]};

                // Compute method name in the format <DeclaringType>.<MethodName>
                mnInfo.Names[(int) MethodNameFormat.TypeAndMember] = String.Concat(mi.DeclaringType.Name, ".", mi.Name);

                // Compute method name in the most detailed format, i.e. <ReturnType> <DeclaringType>.<MethodName>[paramType1, paramType2, ...]
                mnInfo.Names[(int) MethodNameFormat.TypeMemberParamsAndReturn] =
                    mi.ToString().Replace('(', '[').Replace(')', ']');

                methodNameInfo[kvp.Key] = mnInfo;
            }

            return methodNameInfo;
        }

        private string FormatMethodParameters(MethodInfo methodInfo)
        {
            var paramInfo = methodInfo.GetParameters();
            string[] paramInfoStrings = paramInfo.Select(pi => string.Format(
                "{0}{1}",
                pi.IsOut ? ((pi.IsIn) ? "ref " : "out ") : String.Empty,
                pi.ParameterType.Name)).ToArray();
            return String.Join(",", paramInfoStrings);
        }

        private void ComputeMethodRanks(IEnumerable<MethodNameBuilder> methodNameBuilders)
        {
            // The most detailed method name format is one that includes type name, method name, parameter information
            // and return type. Sort the method name builders based on method names built according to the above format. 
            // After sorting, assign a rank to each method.
            // Note that the rank of each method will be unique within the actor type.
            var methodNameBuildersSorted = methodNameBuilders.OrderBy(
                mnInfo => mnInfo.Names[(int) MethodNameFormat.TypeMemberParamsAndReturn]);
            for (var i = 0; i < methodNameBuildersSorted.Count(); i++)
            {
                methodNameBuildersSorted.ElementAt(i).Rank = i;
            }
        }

        private void ComputeMaxMethodNameLength(IEnumerable<MethodNameBuilder> methodNameBuilders)
        {
            // The method information part of the counter instance name ends with "_<Rank>", so figure
            // out the number of characters remaining for the method name after accounting for the "_<Rank>"
            // suffix.
            foreach (var currentMethodBuilderInfo in methodNameBuilders)
            {
                currentMethodBuilderInfo.MethodNameMaxLength = this.maxMethodInfoLen -
                                                               currentMethodBuilderInfo.Rank.ToString("D").Length - 1;
            }
        }

        private Dictionary<string, int> CreateMethodOverloadMap(IEnumerable<MethodNameBuilder> methodNameBuilders)
        {
            var methodOverloads = new Dictionary<string, int>();
            foreach (var currentMethodBuilderInfo in methodNameBuilders)
            {
                string methodName = currentMethodBuilderInfo.Names[(int) MethodNameFormat.TypeAndMember];
                if (!methodOverloads.ContainsKey(methodName))
                {
                    methodOverloads[methodName] = 0;
                }
                methodOverloads[methodName]++;
            }
            return methodOverloads;
        }

        private void ComputeMethodNameInCounterInstanceName(IEnumerable<MethodNameBuilder> methodNameBuilders,
            Dictionary<string, int> methodOverloads)
        {
            foreach (var currentMethodBuilderInfo in methodNameBuilders)
            {
                // If the type and method (without parameters) can fit into the instance name and if the method is 
                // not overloaded (hence easy to identify), then include just the type and method name.
                string methodNameWithoutParams = currentMethodBuilderInfo.Names[(int) MethodNameFormat.TypeAndMember];
                if ((methodNameWithoutParams.Length <= currentMethodBuilderInfo.MethodNameMaxLength) &&
                    (methodOverloads[methodNameWithoutParams] == 1))
                {
                    currentMethodBuilderInfo.FormatToUse = MethodNameFormat.TypeAndMember;
                    continue;
                }

                // If the type, method, parameters and return type can all fit into the instance name, then include them all
                if (currentMethodBuilderInfo.Names[(int) MethodNameFormat.TypeMemberParamsAndReturn].Length <=
                    currentMethodBuilderInfo.MethodNameMaxLength)
                {
                    currentMethodBuilderInfo.FormatToUse = MethodNameFormat.TypeMemberParamsAndReturn;
                    continue;
                }

                // Compute a truncated version of the method name and use that in the instance name
                currentMethodBuilderInfo.FormatToUse = MethodNameFormat.Truncated;
                currentMethodBuilderInfo.Names[(int) MethodNameFormat.Truncated] =
                    this.ComputeTruncatedMethodName(currentMethodBuilderInfo);
            }
        }

        private string ComputeTruncatedMethodName(MethodNameBuilder methodNameBuilder)
        {
            var availableSpaceForTruncatedParts = methodNameBuilder.MethodNameMaxLength;
            var minLengthToQualifyForTruncation = methodNameBuilder.MethodNameMaxLength/((int) MethodNameParts.Count);

            var partsToTruncate = new List<MethodNameParts>();

            // Check if we should truncate the type component of the method name
            var typeFull = methodNameBuilder.MethodInfo.DeclaringType.Name;
            string typeTruncated;
            this.PrepareMethodNamePartForTruncation(
                typeFull,
                MethodNameParts.DeclaringType,
                minLengthToQualifyForTruncation,
                partsToTruncate,
                ref availableSpaceForTruncatedParts,
                out typeTruncated);

            // Check if we should truncate the method name component of the method name
            var methodNameFull = methodNameBuilder.MethodInfo.Name;
            string methodNameTruncated;
            this.PrepareMethodNamePartForTruncation(
                methodNameFull,
                MethodNameParts.MethodName,
                minLengthToQualifyForTruncation,
                partsToTruncate,
                ref availableSpaceForTruncatedParts,
                out methodNameTruncated);

            // Check if we should truncate the parameters component of the method name.
            var parametersFull = this.FormatMethodParameters(methodNameBuilder.MethodInfo);
            string paramsTruncated;
            this.PrepareMethodNamePartForTruncation(
                parametersFull,
                MethodNameParts.Params,
                minLengthToQualifyForTruncation,
                partsToTruncate,
                ref availableSpaceForTruncatedParts,
                out paramsTruncated);

            // Divide the available space equally among the parts that need to be truncated
            int maxSizeOfTruncatedPart = availableSpaceForTruncatedParts/partsToTruncate.Count;

            // Truncate each part that we need to
            foreach (var methodNamePart in partsToTruncate)
            {
                switch (methodNamePart)
                {
                    case MethodNameParts.DeclaringType:
                        typeTruncated = this.TruncateTypeOrMethod(typeFull, maxSizeOfTruncatedPart);
                        break;
                    case MethodNameParts.MethodName:
                        methodNameTruncated = this.TruncateTypeOrMethod(methodNameFull, maxSizeOfTruncatedPart);
                        break;
                    case MethodNameParts.Params:
                        paramsTruncated = this.TruncateParams(methodNameBuilder.MethodInfo.GetParameters(),
                            maxSizeOfTruncatedPart);
                        break;
                }
            }

            // Compute the truncated method name
            return String.Concat(
                typeTruncated,
                ".",
                methodNameTruncated,
                paramsTruncated);
        }

        private void PrepareMethodNamePartForTruncation(
            string fullNameOfMethodNamePart,
            MethodNameParts methodNamePart,
            int minLengthToQualifyForTruncation,
            List<MethodNameParts> partsToTruncate,
            ref int availableSpaceForTruncatedParts,
            out string truncatedNameOfMethodNamePart)
        {
            truncatedNameOfMethodNamePart = null;

            // For parameters, we add two to the string length because we need to account 
            // for the square brackets that the parameters are enclosed in.
            int adjustedPartLength = (methodNamePart == MethodNameParts.Params)
                ? (fullNameOfMethodNamePart.Length + 2)
                : fullNameOfMethodNamePart.Length;

            if (adjustedPartLength <= minLengthToQualifyForTruncation)
            {
                // This part of the method name is small already. No need to truncate it further.
                truncatedNameOfMethodNamePart = (methodNamePart == MethodNameParts.Params)
                    ? String.Concat("[", fullNameOfMethodNamePart, "]")
                    : fullNameOfMethodNamePart;

                // Because we are including this part of the method name as is, the space available for
                // the parts that we will truncate is reduced.
                availableSpaceForTruncatedParts = availableSpaceForTruncatedParts - adjustedPartLength;
            }
            else
            {
                // This part of the method name is big enough that we should truncate it.
                partsToTruncate.Add(methodNamePart);
            }
        }

        private string TruncateTypeOrMethod(string typeOrMethod, int maxSizeOfTruncatedPart)
        {
            // Take ((maxSizeOfTruncatedPart/2) - 1) characters from the start, and (maxSizeOfTruncatedPart/2)
            // characters from the end. Separate the starting and ending portions with a '~'. 
            string firstPart = typeOrMethod.Substring(0, ((maxSizeOfTruncatedPart/2) - 1));
            string lastPart = typeOrMethod.Substring(typeOrMethod.Length - (maxSizeOfTruncatedPart/2));
            return String.Concat(firstPart, "~", lastPart);
        }

        private string TruncateParams(ParameterInfo[] parameterInfo, int maxSizeOfTruncatedPart)
        {
            // The format of the truncated parameter string is: [...Pn-2, Pn-1, Pn]
            // So the space actually available for the parameter information is reduced by the length
            // of "[...]"
            int availableSpace = maxSizeOfTruncatedPart - "[...]".Length;
            if (availableSpace < 0)
            {
                return String.Empty;
            }

            // In the truncated string, the parameters are included from right to left, based on the
            // assumption that overloaded methods are more likely to differ in their later arguments
            // than in the initial arguments. So including parameters from right to left is likely
            // to make it easier to differentiate among overloaded methods by just looking at the
            // instance name.
            var parameters = new List<string>();
            int i;
            bool paramTypeTruncated = false;
            for (i = parameterInfo.Length - 1; (i >= 0) && (availableSpace > 0); i--)
            {
                var currentParam = parameterInfo[i].ParameterType.Name;
                if (currentParam.Length > availableSpace)
                {
                    // We have enough space to hold only a part of the current parameter
                    currentParam = currentParam.Substring(currentParam.Length - availableSpace);
                    paramTypeTruncated = true;
                }

                // Add the current parameter to the list of parameters included in the truncated string
                parameters.Add(currentParam);

                // Reduce the space available in the truncated string
                availableSpace = availableSpace - currentParam.Length;
            }

            // Reverse the list, because that's the order in which we want the parameters displayed
            parameters.Reverse();

            // Return the formatted parameter string to the caller
            // Include "..." in the formatted string only if we couldn't fit in all the parameters.
            string paramsAsString = String.Join(",", parameters.ToArray());
            return String.Concat(
                "[",
                (availableSpace == 0) && ((i >= 0) || paramTypeTruncated) ? "..." : String.Empty,
                paramsAsString,
                "]");
        }

        private IEnumerable<KeyValuePair<long, string>> ComputeCounterInstanceNames(
            Dictionary<long, MethodNameBuilder> methodNameBuilders)
        {
            // The counter instance name includes the method name (possibly truncated), the method rank, 
            // the partition ID, and the differentiator.
            //     <MethodName>_<Rank>_XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX_<Differentiator>
            var counterInstanceNames = new List<KeyValuePair<long, string>>();
            foreach (var currentMethodId in methodNameBuilders.Keys)
            {
                var methodNameBuilder = methodNameBuilders[currentMethodId];
                var counterInstanceName = String.Concat(
                    methodNameBuilder.Names[(int) methodNameBuilder.FormatToUse],
                    "_",
                    methodNameBuilder.Rank.ToString("D"),
                    "_",
                    this.partitionId.ToString(),
                    "_",
                    this.counterInstanceDifferentiator);
                counterInstanceNames.Add(new KeyValuePair<long, string>(currentMethodId, counterInstanceName));
            }
            return counterInstanceNames;
        }

        private class MethodNameBuilder
        {
            internal MethodNameFormat FormatToUse;
            internal MethodInfo MethodInfo;
            internal int MethodNameMaxLength;
            internal string[] Names;
            internal int Rank;
        }

        private enum MethodNameFormat
        {
            TypeAndMember,
            TypeMemberParamsAndReturn,
            Truncated,

            // This value does not represent a method name format. It represents the count of
            // method name formats.
            Count
        }

        private enum MethodNameParts
        {
            DeclaringType,
            MethodName,
            Params,

            // This value does not represent any part of the method name. Instead,
            // it represents the total count of method name parts.
            Count
        }
    }
}