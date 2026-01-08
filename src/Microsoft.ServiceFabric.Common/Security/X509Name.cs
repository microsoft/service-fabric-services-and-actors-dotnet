// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common.Security
{
    using System;

    /// <summary>
    ///   <para>A type to identify X509 certificate with subject common name or DNS name</para>
    /// </summary>
    public class X509Name
    {
        /// <summary>
        ///   <para>Initializes a new instance of the <see cref="X509Name"/> class that identifies an X509 certificate</para>
        /// </summary>
        /// <param name="name">
        ///   <para>Subject common name or DNS name of X509 certificate</para>
        /// </param>
        /// <param name="issuerCertThumbprint">
        ///   <para>Certificate thumbprint to identify issuer. Default value is null which means that issuer thumbprint will not be verified.
        ///   for the certificate found with the common name. A Comma delimited string can be used to verify against multiple certificate issuer thumbprints</para>
        /// </param>
        public X509Name(string name, string issuerCertThumbprint = null)
        {
            name.ThrowIfNull(nameof(name));
            this.Name = name;

            if (issuerCertThumbprint != null)
            {
                this.IssuerCertThumbprint = issuerCertThumbprint.Replace(" ", string.Empty);
            }
        }

        /// <summary>
        /// Gets the subject common name or DNS name of X509 certificate.
        /// </summary>
        /// <value>
        ///   <para>Subject common name or DNS name of X509 certificate</para>
        /// </value>
        public string Name { get; }

        /// <summary>
        /// Gets the certificate thumbprint to identify issuer. This can additionally be a comma delimited string of multiple issuer certificate thumbprints
        /// </summary>
        /// <value>
        ///   <para>Certificate thumbprint to identify issuer</para>
        /// </value>
        public string IssuerCertThumbprint { get; }

        /// <summary>
        ///   <para>Determines whether the specified object is equal to the current object</para>
        /// </summary>
        /// <param name="obj">
        ///   <para>The object to compare with the current object</para>
        /// </param>
        /// <returns>
        ///   <para>Returns true if the objects are equal, false otherwise.</para>
        /// </returns>
        public override bool Equals(object obj)
        {
            return this.Equals(obj as X509Name);
        }

        /// <summary>
        ///   <para>Compute hash code</para>
        /// </summary>
        /// <returns>
        ///   <para>Returns <see cref="int" /> representing the hash code.</para>
        /// </returns>
        public override int GetHashCode()
        {
            return this.Name.ToLower().GetHashCode() ^ this.IssuerCertThumbprint.ToLower().GetHashCode();
        }

        /// <summary>
        ///   <para>Determines whether the specified object is equal to the current object</para>
        /// </summary>
        /// <param name="other">
        ///   <para>The object to compare with the current object</para>
        /// </param>
        /// <returns>
        ///   <para>Returns true if the objects are equal, false otherwise.</para>
        /// </returns>
        public bool Equals(X509Name other)
        {
            if (other == null)
            {
                return false;
            }

            if (object.ReferenceEquals(this, other))
            {
                return true;
            }

            if (this.GetType() != other.GetType())
            {
                return false;
            }

            if (this.Name.Equals(other.Name, StringComparison.OrdinalIgnoreCase))
            {
                if (this.IssuerCertThumbprint == null && other.IssuerCertThumbprint == null)
                {
                    return true;
                }
                else if (this.IssuerCertThumbprint != null && other.IssuerCertThumbprint != null)
                {
                    return this.IssuerCertThumbprint.Equals(other.IssuerCertThumbprint, StringComparison.OrdinalIgnoreCase);
                }
            }

            return false;               
        }
    }
}
