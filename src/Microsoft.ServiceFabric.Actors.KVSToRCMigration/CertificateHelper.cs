// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.KVSToRCMigration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Security.Cryptography.X509Certificates;
    using System.Text;

    /// <summary>
    /// Helper methods related to certificate usage (loading, verifying, etc)
    /// </summary>
    internal class CertificateHelper
    {
        private static readonly string TraceType = typeof(CertificateHelper).Name;

        internal delegate bool IsCertificateAMatchForFindValue(X509Certificate2 enumeratedCert, string findValue);

        internal static List<X509Certificate2> GetCertificates(MigrationSecuritySettings securitySettings)
        {
            var storeName = securitySettings.CertificateStoreName;
            var storeLocation = securitySettings.CertificateStoreLocation;
            var findType = securitySettings.CertificateFindType;
            var findValue = securitySettings.CertificateFindValue;
            bool includeExpiredCerts = false;

            return FindMatchingCertificates(storeName, storeLocation, findType, findValue, includeExpiredCerts);
        }

        internal static List<X509Certificate2> FindMatchingCertificates(
            string storeName,
            StoreLocation storeLocation,
            X509FindType findType,
            string findValue,
            bool includeExpiredCerts)
        {
            X509Store store;
            var certificates = new List<X509Certificate2>();

            IsCertificateAMatchForFindValue matchCert;
            switch (findType)
            {
                case X509FindType.FindByThumbprint:
                    matchCert = IsMatchByThumbprint;
                    break;

                case X509FindType.FindBySubjectName:
                    matchCert = IsMatchBySubjectCommonName;
                    break;

                default:
                    throw new ArgumentException($"Unsupported X509FindType: '{findType}'; supported values are FindByThumbprint and FindBySubjectName");
            }

            store = new X509Store(storeName, storeLocation);

            X509Certificate2 selectedCert = null;
            try
            {
                bool isTimeValidCert = false;
                bool isExpiredCert = false;
                bool anyMatchFound = false;
                DateTime now = DateTime.Now;        // cert validity is presented in local time.

                store.Open(OpenFlags.ReadOnly);

                var findValues = new List<string>() { findValue };

                foreach (var value in findValues)
                {
                    ActorTrace.Source.WriteInfo(TraceType, $"Finding matching certificates for find value '{findValue}'; includeExpiredCerts = '{includeExpiredCerts}'");
                    foreach (var enumeratedCert in store.Certificates)
                    {
                        isExpiredCert = now > enumeratedCert.NotAfter;
                        isTimeValidCert = (!isExpiredCert || includeExpiredCerts)
                                        && now >= enumeratedCert.NotBefore;
                        if (matchCert(enumeratedCert, value)
                            && isTimeValidCert)
                        {
                            anyMatchFound = true;

                            ActorTrace.Source.WriteInfo(
                                TraceType,
                                $"Found matching certificate: Thumbprint {enumeratedCert.Thumbprint}, NotBefore {enumeratedCert.NotBefore}, NotAfter {enumeratedCert.NotAfter}, Subject {enumeratedCert.Subject}");

                            // Select the most recent and farthest valid matching cert.
                            // This should make it predictible if certificate is compromised and it needs to be replaced with a newer one.
                            if (selectedCert == null
                                || selectedCert.NotBefore < enumeratedCert.NotBefore)
                            {
                                selectedCert = enumeratedCert;
                            }
                            else if (selectedCert.NotBefore == enumeratedCert.NotBefore
                                && !selectedCert.Thumbprint.Equals(enumeratedCert.Thumbprint))
                            {
                                // if both were issued at the same time, prefer the farthest valid
                                selectedCert = selectedCert.NotAfter >= enumeratedCert.NotAfter ? selectedCert : enumeratedCert;
                            }
                        }
                    }
                }

                if (selectedCert != null)
                {
                    ActorTrace.Source.WriteInfo(
                        TraceType,
                        $"Selected certificate: Thumbprint {selectedCert.Thumbprint}, NotBefore {selectedCert.NotBefore}, NotAfter {selectedCert.NotAfter}, Subject {selectedCert.Subject}");

                    certificates.Add(selectedCert);
                }
                else
                {
                    ActorTrace.Source.WriteInfo(
                        TraceType,
                        $"No {(anyMatchFound ? "valid" : "matching")} certificate found: StoreName {storeName}, StoreLocation {storeLocation}, FindType {findType}, FindValue {findValue}");
                }
            }
            finally
            {
                store.Dispose();
            }

            if (!certificates.Any())
            {
                throw new InvalidOperationException("Could not load certificate");
            }

            return certificates;
        }

        internal static bool TryValidateX509Certificate(
             X509Certificate2 certificate,
             IsCertificateAMatchForFindValue matchingFn,
             string expectedFindValue,
             X509Chain chainValidator)
        {
            // This does not constitute a 'deep' validation of the certificate, as its intended usage is not available (and
            // we don't want to unnecessarily complicate this code.) The final authority on whether a certificate is valid
            // remains the runtime - this validation is meant more for pre-/first-pass scenarios. As such, the validation
            // is generally a bit more relaxed than the verification performed on the same certificate at authentication
            // time.
            //
            // This verification will assess the following:
            //   - the certificate is a valid certificate object
            //   - the certificate's chain can be built
            //   - the certificate is time-valid at a point in the near future
            //   - the matchingFn applied to the certificate returns true
            //   - the custom chain validator applied to the certificate returns true
            //
            // Application or certificate policies, as well as Enhanced Key Usages are not being validated.
            // Any chain statuses are returned to the caller for additional handling (as a public field of the chain validator.)
            if (certificate == null)
            {
                throw new ArgumentNullException(nameof(certificate));
            }

            if (string.IsNullOrWhiteSpace(expectedFindValue))
            {
                throw new ArgumentNullException(nameof(expectedFindValue));
            }

            if (chainValidator == null)
            {
                throw new ArgumentNullException(nameof(chainValidator));
            }

            bool isValidCertificate = false;
            bool isValidChain = false;

            var certSig = $"({certificate.Subject}, TP:{certificate.Thumbprint})";

            try
            {
                // first check the find value
                isValidCertificate = matchingFn(certificate, expectedFindValue);
                if (!isValidCertificate)
                {
                    ActorTrace.Source.WriteInfo(TraceType, $"certificate {certSig} does not match expected find value '{expectedFindValue}'");

                    return false;    // validation complete
                }

                // build and validate the chain
                isValidChain = chainValidator.Build(certificate);
                if (!isValidChain)
                {
                    // print a detailed error message reflecting the chain status
                    var x509ChainStatuses = chainValidator.ChainStatus;

                    StringBuilder errorStrBuilder = new StringBuilder();
                    errorStrBuilder.AppendFormat($"chain building failed for certificate {certSig}; enumerating the status for each of the {chainValidator.ChainElements.Count} elements:");
                    int idx = 0;
                    foreach (var status in chainValidator.ChainStatus)
                    {
                        errorStrBuilder.AppendFormat($"\n\telement {idx}: '{chainValidator.ChainElements[idx].Certificate.Thumbprint}': {chainValidator.ChainStatus[idx].StatusInformation} ({chainValidator.ChainStatus[idx].Status.ToString()})");
                    }

                    ActorTrace.Source.WriteError(TraceType, errorStrBuilder.ToString());
                }
            }
            catch (CryptographicException cex)
            {
                ActorTrace.Source.WriteError(
                    TraceType,
                    $"crypto exception encountered building the chain of certificate {certSig}: {cex.Message} ({cex.HResult})");
            }
            catch (Exception ex)
            {
                ActorTrace.Source.WriteError(
                    TraceType,
                    $"generic exception encountered validating certificate {certSig}: {ex.Message} ({ex.HResult})");
            }

            return isValidCertificate && isValidChain;
        }

        internal static bool IsMatchByThumbprint(X509Certificate2 enumeratedCert, string x509FindValue)
        {
            return StringComparer.OrdinalIgnoreCase.Equals(x509FindValue, enumeratedCert.Thumbprint);
        }

        internal static bool IsMatchBySubjectCommonName(X509Certificate2 enumeratedCert, string x509FindValue)
        {
            // the find value, if used with X509FindType.FindBySubject, yields erroneous matches: mismatching casing, or certificates
            // whose subject is a superstring of the one we're looking for. FindByDistinguishedName fails as well - .net does not seem
            // to support finding a cert by its own DN. At the recommendation of the CLR team, the best option is to do an exact match
            // of the find value with a certificate's SimpleName, which is the 'name' value calculated by a definitely-not-simple algorithm.
            // We're relaxing the exact match to a case-insensitive one; this isn't what the runtime is doing, but the intent here is
            // to find/compare DNS-type names.
            return StringComparer.OrdinalIgnoreCase.Equals(x509FindValue, enumeratedCert.GetNameInfo(X509NameType.SimpleName, forIssuer: false));
        }

        internal static bool IsValidRemoteCert(X509Certificate2 certificate, X509Chain chain, MigrationSecuritySettings securitySettings)
        {
            var validThumbprints = securitySettings.CertificateRemoteThumbprints?.Split(',').ToList();
            var validCommonNames = securitySettings.CertificateRemoteCommonNames?.Split(',').ToList();

            // adding CertificateFindValue as valid remote cert as well for the case where user is using same cert for client as well as server
            switch (securitySettings.CertificateFindType)
            {
                case X509FindType.FindByThumbprint:
                    validThumbprints.Add(securitySettings.CertificateFindValue);
                    break;
                case X509FindType.FindBySubjectName:
                    validCommonNames.Add(securitySettings.CertificateFindValue);
                    break;
            }

            var certSig = $"({certificate.Subject}, TP:{certificate.Thumbprint})";
            ActorTrace.Source.WriteInfo(
                        TraceType,
                        $"validThumbprints: {string.Join(",", validThumbprints)} validCommonNames: {string.Join(",", validCommonNames)} certificate: {certSig}");

            foreach (var thumbprint in validThumbprints)
            {
                if (TryValidateX509Certificate(
                        certificate,
                        IsMatchByThumbprint,
                        thumbprint,
                        chain))
                {
                    return true;
                }
            }

            foreach (var commonname in validCommonNames)
            {
                if (TryValidateX509Certificate(
                        certificate,
                        IsMatchBySubjectCommonName,
                        commonname,
                        chain))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
