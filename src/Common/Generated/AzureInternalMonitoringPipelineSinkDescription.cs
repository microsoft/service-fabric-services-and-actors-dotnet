// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Diagnostics settings for Geneva.
    /// </summary>
    public partial class AzureInternalMonitoringPipelineSinkDescription : DiagnosticsSinkProperties
    {
        /// <summary>
        /// Initializes a new instance of the AzureInternalMonitoringPipelineSinkDescription class.
        /// </summary>
        /// <param name="name">Name of the sink. This value is referenced by DiagnosticsReferenceDescription</param>
        /// <param name="description">A description of the sink.</param>
        /// <param name="accountName">Azure Internal monitoring pipeline account.</param>
        /// <param name="namespaceProperty">Azure Internal monitoring pipeline account namespace.</param>
        /// <param name="maConfigUrl">Azure Internal monitoring agent configuration.</param>
        /// <param name="fluentdConfigUrl">Azure Internal monitoring agent fluentd configuration.</param>
        /// <param name="autoKeyConfigUrl">Azure Internal monitoring pipeline autokey associated with the certificate.</param>
        public AzureInternalMonitoringPipelineSinkDescription(
            string name = default(string),
            string description = default(string),
            string accountName = default(string),
            string namespaceProperty = default(string),
            string maConfigUrl = default(string),
            string fluentdConfigUrl = default(string),
            string autoKeyConfigUrl = default(string))
            : base(
                Common.DiagnosticsSinkKind.AzureInternalMonitoringPipeline,
                name,
                description)
        {
            this.AccountName = accountName;
            this.NamespaceProperty = namespaceProperty;
            this.MaConfigUrl = maConfigUrl;
            this.FluentdConfigUrl = fluentdConfigUrl;
            this.AutoKeyConfigUrl = autoKeyConfigUrl;
        }

        /// <summary>
        /// Gets azure Internal monitoring pipeline account.
        /// </summary>
        public string AccountName { get; }

        /// <summary>
        /// Gets azure Internal monitoring pipeline account namespace.
        /// </summary>
        public string NamespaceProperty { get; }

        /// <summary>
        /// Gets azure Internal monitoring agent configuration.
        /// </summary>
        public string MaConfigUrl { get; }

        /// <summary>
        /// Gets azure Internal monitoring agent fluentd configuration.
        /// </summary>
        public string FluentdConfigUrl { get; }

        /// <summary>
        /// Gets azure Internal monitoring pipeline autokey associated with the certificate.
        /// </summary>
        public string AutoKeyConfigUrl { get; }
    }
}
