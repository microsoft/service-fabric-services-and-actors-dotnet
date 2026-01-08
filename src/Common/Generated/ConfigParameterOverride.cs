// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Information about a configuration parameter override.
    /// </summary>
    public partial class ConfigParameterOverride
    {
        /// <summary>
        /// Initializes a new instance of the ConfigParameterOverride class.
        /// </summary>
        /// <param name="sectionName">Name of the section for the parameter override.</param>
        /// <param name="parameterName">Name of the parameter that has been overridden.</param>
        /// <param name="parameterValue">Value of the overridden parameter.</param>
        /// <param name="timeout">The duration until config override is considered as valid.</param>
        /// <param name="persistAcrossUpgrade">A value that indicates whether config override will be removed on upgrade or
        /// will still be considered as valid.</param>
        public ConfigParameterOverride(
            string sectionName,
            string parameterName,
            string parameterValue,
            TimeSpan? timeout = default(TimeSpan?),
            bool? persistAcrossUpgrade = default(bool?))
        {
            sectionName.ThrowIfNull(nameof(sectionName));
            parameterName.ThrowIfNull(nameof(parameterName));
            parameterValue.ThrowIfNull(nameof(parameterValue));
            this.SectionName = sectionName;
            this.ParameterName = parameterName;
            this.ParameterValue = parameterValue;
            this.Timeout = timeout;
            this.PersistAcrossUpgrade = persistAcrossUpgrade;
        }

        /// <summary>
        /// Gets name of the section for the parameter override.
        /// </summary>
        public string SectionName { get; }

        /// <summary>
        /// Gets name of the parameter that has been overridden.
        /// </summary>
        public string ParameterName { get; }

        /// <summary>
        /// Gets value of the overridden parameter.
        /// </summary>
        public string ParameterValue { get; }

        /// <summary>
        /// Gets the duration until config override is considered as valid.
        /// </summary>
        public TimeSpan? Timeout { get; }

        /// <summary>
        /// Gets a value that indicates whether config override will be removed on upgrade or will still be considered as
        /// valid.
        /// </summary>
        public bool? PersistAcrossUpgrade { get; }
    }
}
