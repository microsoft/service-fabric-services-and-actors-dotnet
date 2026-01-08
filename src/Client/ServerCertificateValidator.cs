// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Client
{
    using System;
    using System.Linq;
    using System.Net.Security;
    using System.Security.Cryptography.X509Certificates;
    using System.Threading;
    using Microsoft.ServiceFabric.Common;
    using Microsoft.ServiceFabric.Common.Security;

    /// <summary>
    /// Class to verify the remote Secure Sockets Layer (SSL) certificate used for authentication.
    /// </summary>
    internal class ServerCertificateValidator
    {
        /// <summary>
        /// Protects upgrading the remoteX509SecuritySettings while Cert Validation callback is in progress.
        /// </summary>
        private readonly ReaderWriterLockSlim slimRWLock = new ReaderWriterLockSlim();

        private RemoteX509SecuritySettings remoteX509SecuritySettings;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServerCertificateValidator"/> class to perform remote certificate validation using <paramref name="remoteX509SecuritySettings"/>
        /// </summary>
        /// <param name="remoteX509SecuritySettings">Settings to validate remote certificate.</param>
        public ServerCertificateValidator(RemoteX509SecuritySettings remoteX509SecuritySettings)
        {
            remoteX509SecuritySettings.ThrowIfNull(nameof(remoteX509SecuritySettings));
            this.remoteX509SecuritySettings = remoteX509SecuritySettings;
        }

        /// <summary>
        /// Updates the <see cref="Microsoft.ServiceFabric.Common.Security.RemoteX509SecuritySettings"/> to validate remote certificate.
        /// </summary>
        /// /// <param name="remoteX509SecuritySettings">Settings to validate remote certificate.</param>
        public void UpdateSecuritySettings(RemoteX509SecuritySettings remoteX509SecuritySettings)
        {
            remoteX509SecuritySettings.ThrowIfNull(nameof(remoteX509SecuritySettings));
            this.slimRWLock.EnterWriteLock();
            this.remoteX509SecuritySettings = remoteX509SecuritySettings;
            this.slimRWLock.ExitWriteLock();
        }

        /// <summary>
        /// Callback to Verify the remote Secure Sockets Layer (SSL) certificate used for authentication.
        /// </summary>
        /// <param name="sender">An object that contains state information for this validation.</param>
        /// <param name="cert">The certificate used to authenticate the remote party.</param>
        /// <param name="chain">The chain of certificate authorities associated with the remote certificate.</param>
        /// <param name="sslPolicyErrors">One or more errors associated with the remote certificate.</param>
        /// <returns>
        /// A <see cref="bool"/> value that determines whether the specified certificate is accepted for authentication.
        /// </returns>
        public bool ValidateCertificate(
            object sender,
            X509Certificate2 cert,
            X509Chain chain,
            SslPolicyErrors sslPolicyErrors)
        {
            if (sslPolicyErrors == SslPolicyErrors.None)
            {
                return true;
            }

            if (sslPolicyErrors == SslPolicyErrors.RemoteCertificateNotAvailable)
            {
                return false;
            }

            this.slimRWLock.EnterReadLock();
            try
            {
                // Call the validator function for X509Name or Thumbprints.
                if (this.remoteX509SecuritySettings.RemoteX509Names != null)
                {
                    return this.ValidateServerCertificateX509Name(cert, chain, sslPolicyErrors);
                }
                else if (this.remoteX509SecuritySettings.RemoteCertThumbprints != null)
                {
                    return this.ValidateServerCertificateWithThumbprint(cert, chain, sslPolicyErrors);
                }
            }
            finally
            {
                this.slimRWLock.ExitReadLock();
            }

            return false;
        }

        /// <summary>
        /// Function to Verify the remote certificate used using <see cref="Microsoft.ServiceFabric.Common.Security.X509Name"/>
        /// </summary>
        /// <param name="cert">The certificate used to authenticate the remote party.</param>
        /// <param name="chain">The chain of certificate authorities associated with the remote certificate.</param>
        /// <param name="sslPolicyErrors">One or more errors associated with the remote certificate.</param>
        /// <returns>
        /// A <see cref="bool"/> value that determines whether the specified certificate is accepted for authentication.
        /// </returns>
        private bool ValidateServerCertificateX509Name(X509Certificate2 cert, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            // SelfSigned certificates will only be verified with X509 name when chain build succeeds.
            // so it must be copied to TrustedRoot or TrustedPeople
            if ((sslPolicyErrors & SslPolicyErrors.RemoteCertificateChainErrors) ==
                SslPolicyErrors.RemoteCertificateChainErrors)
            {
                // When matching with subject name, Only CrlOffline can be ignored if specified in settings.
                if (!this.remoteX509SecuritySettings.IgnoreCrlOfflineError)
                {
                    return false;
                }
                else
                {
                    // if errors other than OfflineRevocation, return false;
                    if (
                        chain.ChainStatus.Any(
                            chainStatus => chainStatus.Status != X509ChainStatusFlags.OfflineRevocation))
                    {
                        return false;
                    }

                    // only OfflineRevocation was found, continue with validation.
                }
            }

            foreach (var x509Name in this.remoteX509SecuritySettings.RemoteX509Names)
            {
                if (cert.GetNameInfo(X509NameType.SimpleName, false).Equals(x509Name.Name, StringComparison.CurrentCultureIgnoreCase) ||
                    cert.GetNameInfo(X509NameType.DnsName, false).Equals(x509Name.Name, StringComparison.CurrentCultureIgnoreCase))
                {
                    // if issuer thumbprint is specified verify it.
                    if (x509Name.IssuerCertThumbprint != null)
                    {
                        // validate issuer thumbprint against all pairs of RemoteX509Names
                        if (this.IsServerCertIssuerThumbprintValid(chain, x509Name.IssuerCertThumbprint))
                        {
                            return true;
                        }
                    }
                    else
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private bool IsServerCertIssuerThumbprintValid(X509Chain chain, string expectedIssuerThumbprints)
        {
                var issuers = expectedIssuerThumbprints.ToLower().Split(',');

                // SelfSigned cert matches with index 0, CA signed matches with index 1.
                var thumbprint = chain.ChainElements[0].Certificate.Thumbprint.ToLower();

                if (thumbprint != null && issuers.Contains(thumbprint))
                {
                    return true;
                }

                // Not self-signed, check if its CA signed. Should have at least one issuer
                if (chain.ChainElements.Count < 2)
                {
                    return false;
                }

                thumbprint = chain.ChainElements[1].Certificate.Thumbprint.ToLower();

                return thumbprint != null && issuers.Contains(thumbprint);
        }

        /// <summary>
        /// Function to Verify the remote certificate using thumbprints.
        /// </summary>
        /// <param name="cert">The certificate used to authenticate the remote party.</param>
        /// <param name="chain">The chain of certificate authorities associated with the remote certificate.</param>
        /// <param name="sslPolicyErrors">One or more errors associated with the remote certificate.</param>
        /// <returns>
        /// A <see cref="bool"/> value that determines whether the specified certificate is accepted for authentication.
        /// </returns>
        private bool ValidateServerCertificateWithThumbprint(X509Certificate2 cert, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            if ((sslPolicyErrors & SslPolicyErrors.RemoteCertificateChainErrors) ==
                SslPolicyErrors.RemoteCertificateChainErrors)
            {
                // When matching with thumbprint name, following chain building errors can be ignored for validating Server certificates.
                var nonFatalError = X509ChainStatusFlags.UntrustedRoot |
                                    X509ChainStatusFlags.RevocationStatusUnknown |
                                    X509ChainStatusFlags.PartialChain;

                // Ignore CrlOffline if specified in settings.
                if (this.remoteX509SecuritySettings.IgnoreCrlOfflineError)
                {
                    nonFatalError |= X509ChainStatusFlags.OfflineRevocation;
                }

                // only ignore non-fatal chain errors.
                if (chain.ChainStatus.Any(x => (x.Status & (~nonFatalError)) != X509ChainStatusFlags.NoError))
                {
                    return false;
                }
            }

            return this.remoteX509SecuritySettings.RemoteCertThumbprints.Any(thumbprint =>
                cert.Thumbprint != null && cert.Thumbprint.Equals(thumbprint, StringComparison.OrdinalIgnoreCase));
        }
    }
}
