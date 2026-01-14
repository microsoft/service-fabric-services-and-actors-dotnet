// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Linq;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using Microsoft.ServiceFabric.Common;
using Microsoft.ServiceFabric.Common.Security;

namespace Microsoft.ServiceFabric.Client
{
    abstract class ServerCertificateValidator
    {
        /// <summary>
        /// Protects upgrading the remoteX509SecuritySettings while Cert Validation callback is in progress.
        /// </summary>
        readonly ReaderWriterLockSlim slimRWLock = new();

        RemoteX509SecuritySettings remoteX509SecuritySettings;

        internal ServerCertificateValidator(RemoteX509SecuritySettings remoteX509SecuritySettings)
        {
            remoteX509SecuritySettings.ThrowIfNull(nameof(remoteX509SecuritySettings));
            this.remoteX509SecuritySettings = remoteX509SecuritySettings;
        }

        internal void UpdateSecuritySettings(RemoteX509SecuritySettings remoteX509SecuritySettings)
        {
            remoteX509SecuritySettings.ThrowIfNull(nameof(remoteX509SecuritySettings));
            slimRWLock.EnterWriteLock();
            this.remoteX509SecuritySettings = remoteX509SecuritySettings;
            slimRWLock.ExitWriteLock();
        }

        /// <summary>
        /// Callback to Verify the remote Secure Sockets Layer (SSL) certificate used for authentication.
        /// </summary>
        internal bool ValidateCertificate(object sender, X509Certificate2 cert, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            if (sslPolicyErrors == SslPolicyErrors.None)
                return true;

            if (sslPolicyErrors == SslPolicyErrors.RemoteCertificateNotAvailable)
                return false;

            slimRWLock.EnterReadLock();
            try
            {
                // Call the validator function for X509Name or Thumbprints.
                if (remoteX509SecuritySettings.RemoteX509Names != null)
                    return ValidateServerCertificateX509Name(cert, chain, sslPolicyErrors);
                else if (remoteX509SecuritySettings.RemoteCertThumbprints != null)
                    return ValidateServerCertificateWithThumbprint(cert, chain, sslPolicyErrors);
            }
            finally
            {
                slimRWLock.ExitReadLock();
            }

            return false;
        }

        bool ValidateServerCertificateX509Name(X509Certificate2 cert, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            // SelfSigned certificates will only be verified with X509 name when chain build succeeds.
            // so it must be copied to TrustedRoot or TrustedPeople
            if ((sslPolicyErrors & SslPolicyErrors.RemoteCertificateChainErrors) == SslPolicyErrors.RemoteCertificateChainErrors)
            {
                // When matching with subject name, Only CrlOffline can be ignored if specified in settings.
                if (!remoteX509SecuritySettings.IgnoreCrlOfflineError)
                    return false;

                // if errors other than OfflineRevocation, return false;
                if (chain.ChainStatus.Any(chainStatus => chainStatus.Status != X509ChainStatusFlags.OfflineRevocation))
                    return false;

                // only OfflineRevocation was found, continue with validation.
            }

            foreach (X509Name x509Name in remoteX509SecuritySettings.RemoteX509Names)
            {
                if (cert.GetNameInfo(X509NameType.SimpleName, false).Equals(x509Name.Name, StringComparison.CurrentCultureIgnoreCase) ||
                    cert.GetNameInfo(X509NameType.DnsName, false).Equals(x509Name.Name, StringComparison.CurrentCultureIgnoreCase))
                {
                    if (x509Name.IssuerCertThumbprint != null &&
                        IsServerCertIssuerThumbprintValid(chain, x509Name.IssuerCertThumbprint))
                        return true;
                }
            }

            return false;
        }

        bool IsServerCertIssuerThumbprintValid(X509Chain chain, string expectedIssuerThumbprints)
        {
            string[] issuers = expectedIssuerThumbprints.ToLower().Split(',');

            // SelfSigned cert matches with index 0, CA signed matches with index 1.
            string thumbprint = chain.ChainElements[0].Certificate.Thumbprint.ToLower();

            if (thumbprint != null && issuers.Contains(thumbprint))
                return true;

            // Not self-signed, check if its CA signed. Should have at least one issuer
            if (chain.ChainElements.Count < 2)
                return false;

            thumbprint = chain.ChainElements[1].Certificate.Thumbprint.ToLower();

            return thumbprint != null && issuers.Contains(thumbprint);
        }

        bool ValidateServerCertificateWithThumbprint(X509Certificate2 cert, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            if ((sslPolicyErrors & SslPolicyErrors.RemoteCertificateChainErrors) == SslPolicyErrors.RemoteCertificateChainErrors)
            {
                // When matching with thumbprint name, following chain building errors can be ignored for validating Server certificates.
                var nonFatalError = X509ChainStatusFlags.UntrustedRoot |
                                    X509ChainStatusFlags.RevocationStatusUnknown |
                                    X509ChainStatusFlags.PartialChain;

                // Ignore CrlOffline if specified in settings.
                if (remoteX509SecuritySettings.IgnoreCrlOfflineError)
                    nonFatalError |= X509ChainStatusFlags.OfflineRevocation;

                // only ignore non-fatal chain errors.
                if (chain.ChainStatus.Any(x => (x.Status & (~nonFatalError)) != X509ChainStatusFlags.NoError))
                    return false;
            }

            return remoteX509SecuritySettings.RemoteCertThumbprints
                .Any(thumbprint => cert.Thumbprint != null && cert.Thumbprint.Equals(thumbprint, StringComparison.OrdinalIgnoreCase));
        }
    }
}
