// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------
namespace Microsoft.ServiceFabric.Services.Remoting.Builder
{
    using System;
    using System.Reflection;
    using Microsoft.ServiceFabric.Services;

    /// <summary>
    /// The Attribute class to configure dyanamic code generation process for service remoting.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Interface)]
    public class CodeBuilderAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the ActorCodeBuilderAttribute class.
        /// </summary>
        public CodeBuilderAttribute()
        {
        }

        /// <summary>
        /// Gets or sets enable debugging flag for the attribute to be used by auto code generation.
        /// </summary>
        /// <value><see cref="System.Boolean"/> to get or set enable debugging flag for the attribute to be used by auto code generation.</value>
        public bool EnableDebugging { get; set; }

        internal static bool IsDebuggingEnabled(Type type = null)
        {
            var enableDebugging = false;
            var entryAssembly = Assembly.GetEntryAssembly();

            if (entryAssembly != null)
            {
                var attribute = entryAssembly.GetCustomAttribute<CodeBuilderAttribute>();
                enableDebugging = ((attribute != null) && (attribute.EnableDebugging));
            }

            if (!enableDebugging && (type != null))
            {
                var attribute = type.GetTypeInfo().Assembly.GetCustomAttribute<CodeBuilderAttribute>();
                enableDebugging = ((attribute != null) && (attribute.EnableDebugging));

                if (!enableDebugging)
                {
                    attribute = type.GetTypeInfo().GetCustomAttribute<CodeBuilderAttribute>(true);
                    enableDebugging = ((attribute != null) && (attribute.EnableDebugging));
                }
            }

            return enableDebugging;
        }
    }
}
