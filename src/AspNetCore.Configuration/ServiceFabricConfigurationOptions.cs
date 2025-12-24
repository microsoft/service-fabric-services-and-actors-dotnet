// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Fabric;
using System.Fabric.Description;
using System.Runtime.InteropServices;
using Microsoft.Extensions.Configuration;
using FabricConfigurationSection = System.Fabric.Description.ConfigurationSection;

namespace Microsoft.ServiceFabric.AspNetCore.Configuration
{
    /// <summary>
    /// The options used to configure the behavior of mapping configuration package to IConfiguration items.
    /// </summary>
    public class ServiceFabricConfigurationOptions
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceFabricConfigurationOptions"/> class.
        /// </summary>
        /// <param name="packageName">the name of the configuration package.</param>
        public ServiceFabricConfigurationOptions(string packageName)
        {
            this.PackageName = packageName ?? throw new ArgumentNullException(packageName);
            this.IncludePackageName = true;
            this.DecryptValue = false; // secure by default.

            this.ConfigAction = this.DefaultConfigAction;
            this.ExtractKeyFunc = this.DefaultExtractKeyFunc;
            this.ExtractValueFunc = this.DefaultExtractValueFunc;
        }

        /// <summary>
        /// Gets the name of the configuration package this options is used for.
        /// </summary>
        public string PackageName { get; }

        /// <summary>
        /// Gets or sets the configuration used to transform the configuration package to IConfiguration items.
        /// </summary>
        /// <remarks>
        /// This is for advanced usage scenario to take data from package and populate into the dictionary.
        /// For example, this could be used to populate json configuration files, adding logging for configuration updates, etc.
        /// The default value is a delegate which used PackageName, SectionName and ParamName as the key and value as the param value regardless of encrypted or not.
        /// </remarks>
        public Action<ConfigurationPackage, IDictionary<string, string>> ConfigAction { get; set; }

        /// <summary>
        /// Gets or sets the function to extract the IConfiguration key from the fabric configuration section and property.
        /// The return value of the fuction would be used as the key for IConfiguration in ConfigAction.
        /// </summary>
        /// <remarks>
        /// The default value is a function which used PackageName, SectionName and ParamName as the key with default IncludePackageName as true.
        /// </remarks>
        public Func<FabricConfigurationSection, ConfigurationProperty,  string> ExtractKeyFunc { get; set; }

        /// <summary>
        /// Gets or sets the function to extract the IConfiguration value from the fabric configuration section and property.
        /// The return value of the fuction would be used as the value for IConfiguration in ConfigAction.
        /// </summary>
        /// <remarks>
        /// The default value is a function which used the param value regardless of encrypted or not with default DecryptValue flag as true.
        /// </remarks>
        public Func<FabricConfigurationSection, ConfigurationProperty, string> ExtractValueFunc { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not to include the configuration package name to the section name of IConfiguration item.
        /// The default is true, and will include the package name, section name and param name in configuration package.
        /// You could set this to true for multiple config packages to avoid a setting with same section name and param name being override from other package.
        /// </summary>
        public bool IncludePackageName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not to decypt the encrypted value to unsecure string.
        /// </summary>
        /// <remarks>
        /// A typical safety guideline is to keep encrypted string encrypted in memory, and then decrypt (briefly) at time of use.
        /// With this reason, DecryptValue will default to false to treat encrypted value the same as plain text by default,
        /// user will need to handle encrypted string separately to compliant with security best practice.
        /// </remarks>
        public bool DecryptValue { get; set; }

        internal void DefaultConfigAction(ConfigurationPackage config, IDictionary<string, string> data)
        {
            foreach (var section in config.Settings.Sections)
            {
                foreach (var param in section.Parameters)
                {
                    string key = this.ExtractKeyFunc(section, param);
                    string value = this.ExtractValueFunc(section, param);
                    data[key] = value;
                }
            }
        }

        internal string DefaultExtractKeyFunc(FabricConfigurationSection section, ConfigurationProperty property)
        {
            if (this.IncludePackageName)
            {
                return $"{this.PackageName}{ConfigurationPath.KeyDelimiter}{section.Name}{ConfigurationPath.KeyDelimiter}{property.Name}";
            }
            else
            {
                return $"{section.Name}{ConfigurationPath.KeyDelimiter}{property.Name}";
            }
        }

        internal string DefaultExtractValueFunc(FabricConfigurationSection section, ConfigurationProperty property)
        {
            // see https://github.com/Azure/service-fabric-aspnetcore/issues/9
            // A typical safety guideline is to keep encrypted string encrypted in memory, and then decrypt (briefly) at time of use.
            // With this reason, will treat encrypted value the same as plain text by default,
            // user will need to handle encrypted string separately to compliant with security best practice.
            if (property.IsEncrypted && this.DecryptValue)
            {
                IntPtr unmanagedString = IntPtr.Zero;
                var secureString = property.DecryptValue();

                try
                {
                    unmanagedString = Marshal.SecureStringToGlobalAllocUnicode(secureString);
                    return Marshal.PtrToStringUni(unmanagedString);
                }
                finally
                {
                    Marshal.ZeroFreeGlobalAllocUnicode(unmanagedString);
                }
            }
            else
            {
                return property.Value;
            }
        }
    }
}
