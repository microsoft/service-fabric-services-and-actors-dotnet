// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Linq;
    using Microsoft.ServiceFabric.Common.Resources;

    /// <summary>
    /// Represents a Service Fabric application name.
    /// </summary>
    public sealed class ApplicationName : FabricName
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationName"/> class by using the value represented by the specified string.
        /// </summary>
        /// <param name="applicationName">A string for the application name.</param>
        public ApplicationName(string applicationName) 
            : this(new Uri(applicationName.CheckNotNull(nameof(applicationName))))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationName"/> class by using the value represented by the specified uri.
        /// </summary>
        /// <param name="applicationUri">A uri for the application name.</param>
        public ApplicationName(Uri applicationUri) 
            : base(applicationUri.CheckNotNull(nameof(applicationUri)))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationName"/> class by using the value represented by the specified string.
        /// </summary>
        /// <param name="applicationName">A string for the application name.</param>
        /// <value>New instance of the <see cref="ApplicationName"/> class.</value>
        public static implicit operator ApplicationName(string applicationName) => new ApplicationName(applicationName);

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationName"/> class by using the value represented by the specified uri.
        /// </summary>
        /// <param name="applicationName">A uri for the application name.</param>
        /// <value>New instance of the <see cref="ApplicationName"/> class.</value>
        public static implicit operator ApplicationName(Uri applicationName) => new ApplicationName(applicationName);
    }
}
