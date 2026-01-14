// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;

namespace Microsoft.ServiceFabric.Common.Security
{
    /// <summary>
    /// A type to identify X509 certificate with subject common name or DNS name
    /// </summary>
    public class X509Name
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="X509Name"/> class that identifies an X509 certificate
        /// </summary>
        /// <param name="name">
        /// Subject common name or DNS name of X509 certificate
        /// </param>
        /// <param name="issuerCertThumbprint">
        /// Certificate thumbprint to identify issuer. Default value is null which means that issuer thumbprint will not be verified.
        /// for the certificate found with the common name. A Comma delimited string can be used to verify against multiple certificate issuer thumbprints
        /// </param>
        public X509Name(string name, string issuerCertThumbprint = null)
        {
            name.ThrowIfNull(nameof(name));
            Name = name;

            if (issuerCertThumbprint != null)
                IssuerCertThumbprint = issuerCertThumbprint.Replace(" ", string.Empty);
        }

        /// <summary>
        /// Gets the subject common name or DNS name of X509 certificate.
        /// </summary>
        /// <value>
        /// Subject common name or DNS name of X509 certificate
        /// </value>
        public string Name { get; }

        /// <summary>
        /// Gets the certificate thumbprint to identify issuer. This can additionally be a comma delimited string of multiple issuer certificate thumbprints
        /// </summary>
        /// <value>
        /// Certificate thumbprint to identify issuer
        /// </value>
        public string IssuerCertThumbprint { get; }

        /// <summary>
        /// Determines whether the specified object is equal to the current object
        /// </summary>
        /// <param name="obj">
        /// The object to compare with the current object
        /// </param>
        /// <returns>
        /// Returns true if the objects are equal, false otherwise.
        /// </returns>
        public override bool Equals(object obj) => Equals(obj as X509Name);

        /// <summary>
        /// Compute hash code
        /// </summary>
        /// <returns>
        /// Returns <see cref="int" /> representing the hash code.
        /// </returns>
        public override int GetHashCode() =>
            Name.ToLower().GetHashCode() ^ IssuerCertThumbprint.ToLower().GetHashCode();

        /// <summary>
        /// Determines whether the specified object is equal to the current object
        /// </summary>
        /// <param name="other">
        /// The object to compare with the current object
        /// </param>
        /// <returns>
        /// Returns true if the objects are equal, false otherwise.
        /// </returns>
        public bool Equals(X509Name other)
        {
            if (other == null)
                return false;

            if (ReferenceEquals(this, other))
                return true;

            if (GetType() != other.GetType())
                return false;

            if (Name.Equals(other.Name, StringComparison.OrdinalIgnoreCase))
            {
                if (IssuerCertThumbprint == null && other.IssuerCertThumbprint == null)
                    return true;
                else if (IssuerCertThumbprint != null && other.IssuerCertThumbprint != null)
                    return IssuerCertThumbprint.Equals(other.IssuerCertThumbprint, StringComparison.OrdinalIgnoreCase);
            }

            return false;
        }
    }
}
