// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Path description for the application package in the image store specified during the prior copy operation.
    /// </summary>
    public partial class ApplicationTypeImageStorePath
    {
        /// <summary>
        /// Initializes a new instance of the ApplicationTypeImageStorePath class.
        /// </summary>
        /// <param name="applicationTypeBuildPath">The relative image store path to the application package.</param>
        public ApplicationTypeImageStorePath(
            string applicationTypeBuildPath)
        {
            applicationTypeBuildPath.ThrowIfNull(nameof(applicationTypeBuildPath));
            this.ApplicationTypeBuildPath = applicationTypeBuildPath;
        }

        /// <summary>
        /// Gets the relative image store path to the application package.
        /// </summary>
        public string ApplicationTypeBuildPath { get; }
    }
}
