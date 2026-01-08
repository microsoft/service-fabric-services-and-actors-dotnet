// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common.Security
{
    using System;
    using System.Security.Cryptography.X509Certificates;
    using Microsoft.ServiceFabric.Common.Resources;

    /// <summary>
    /// Specifies the security settings that are based upon X509 certificates.
    /// </summary>
    public sealed class X509SecuritySettings : SecuritySettings
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="X509SecuritySettings" /> class.
        /// </summary>
        /// <param name="clientCertificate">The client certificate for the communication with server.</param>
        /// <param name="remoteX509SecuritySettings">Security settings to verify remote X509 certificate.</param>
        public X509SecuritySettings(X509Certificate2 clientCertificate, RemoteX509SecuritySettings remoteX509SecuritySettings)
            : base(SecurityType.X509)
        {
            clientCertificate.ThrowIfNull(nameof(clientCertificate));
            remoteX509SecuritySettings.ThrowIfNull(nameof(remoteX509SecuritySettings));

            if (!clientCertificate.HasPrivateKey)
            {
                throw new InvalidOperationException(SR.ClientCertDoesntContainPrivateKey);
            }

            this.ClientCertificate = clientCertificate;
            this.RemoteX509SecuritySettings = remoteX509SecuritySettings;
        }

        /// <summary>
        /// Gets the client certificate for communication with server.
        /// </summary>
        /// <value><see cref="System.Security.Cryptography.X509Certificates.X509Certificate2"/> to communicate with server.</value>
        public X509Certificate2 ClientCertificate { get; }

        /// <summary>
        /// Gets the settings to validate remote certificate.
        /// </summary>
        /// <value>
        /// <see cref="RemoteX509SecuritySettings"/> to validate remote certificate.
        /// </value>
        public RemoteX509SecuritySettings RemoteX509SecuritySettings { get; }
    }
}
