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
    /// Represents a Service Fabric name.
    /// </summary>
    public class FabricName : IEquatable<FabricName>
    {
        /// <summary>
        /// List of characters not allowed in ServiceFabric name
        /// </summary>
        public const string InvalidCharacters = @"?#[]+$<>\^|%";

        private readonly Uri uri;

        /// <summary>
        /// Initializes a new instance of the <see cref="FabricName"/> class by using the value represented by the specified string.
        /// </summary>
        /// <param name="name">A string for the Service Fabric name.</param>
        public FabricName(string name) 
            : this(new Uri(name.CheckNotNull(nameof(name))))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FabricName"/> class by using the value represented by the specified string.
        /// </summary>
        /// <param name="name">Service Fabric name to create this isntance from..</param>
        public FabricName(FabricName name)
            : this(name.CheckNotNull(nameof(name)).ToString())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FabricName"/> class by using the value represented by the specified uri.
        /// </summary>
        /// <param name="name">A uri for the Service Fabric name.</param>
        public FabricName(Uri name)
        {
            name.ThrowIfNull(nameof(name));
            //// FabricName must:
            ////  1. have scheme as fabric (begins with fabric:/)
            ////  2. not have authority names (fabric://)
            ////  3. not have invalid chars.
            ////  4. trailing "/", empty segments.

            if (InvalidCharacters.ToCharArray().Any(x => name.AbsolutePath.Contains(x)))
            {
                throw new ArgumentException(string.Format(SR.ErrorAppNameHasInvalidChars, InvalidCharacters), nameof(name));
            }

            if (name.AbsolutePath.EndsWith("/", StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException(SR.ErrorNameHasTrailingSlash, nameof(name));
            }

            if (!name.Scheme.Equals("fabric", StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException(SR.ErrorNameDoesntBeginWithFabric, nameof(name));
            }

            if (!name.Authority.Equals(string.Empty, StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException(SR.ErrorNameHasAuthorityName, nameof(name));
            }

            this.uri = name;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FabricName"/> class by using the value represented by the specified string.
        /// </summary>
        /// <param name="name">A string for the Service Fabric name.</param>
        /// <value>New instance of the <see cref="FabricName"/> class.</value>
        public static implicit operator FabricName(string name) => new FabricName(name);

        /// <summary>
        /// Initializes a new instance of the <see cref="FabricName"/> class by using the value represented by the specified uri.
        /// </summary>
        /// <param name="name">A string for the Service Fabric name.</param>
        /// <value>New instance of the <see cref="FabricName"/> class.</value>
        public static implicit operator FabricName(Uri name) => new FabricName(name);

        /// <summary>
        /// Indicates whether the values of two specified objects are equal.
        /// </summary>
        /// <param name="name1">The first object to compare.</param>
        /// <param name="name2">The second object to compare</param>
        /// <returns>true if name1 and name2 are equal; otherwise, false.</returns>
        public static bool operator ==(FabricName name1, FabricName name2) => name1 is null ? name2 is null : name1.Equals(name2);

        /// <summary>
        /// Indicates whether the values of two specified objects are not equal.
        /// </summary>
        /// <param name="name1">The first object to compare.</param>
        /// <param name="name2">The second object to compare</param>
        /// <returns>true if name1 and name2 are not equal; otherwise, false.</returns>
        public static bool operator !=(FabricName name1, FabricName name2)
            => !(name1 == name2);

        /// <summary>
        /// Returns a string representation of the value of this instance.
        /// </summary>
        /// <returns>string representation of the value of this instance.</returns>
        public override string ToString() => this.uri.ToString();

        /// <summary>
        /// Returns a value indicating whether this instance and a specified object represent the same value.
        /// </summary>
        /// <param name="other">An object to compare to this instance.</param>
        /// <returns>true if this instance equal to other parameter; otherwise, false.</returns>
        public bool Equals(FabricName other)
        {
            if (other is null)
            {
                return false;
            }

            return this.uri.Equals(other?.uri);
        }

        /// <summary>
        /// Returns a value that indicates whether this instance is equal to a specified object.
        /// </summary>
        /// <param name="other">The object to compare with this instance.</param>
        /// <returns>true if other object  has the same value as this instance; otherwise, false.</returns>
        public override bool Equals(object other)
        {
            if (other is null)
            {
                return false;
            }

            return (other is FabricName) && this.uri.Equals(((FabricName)other)?.uri);
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>The hash code for this instance.</returns>
        public override int GetHashCode() => this.uri.GetHashCode();

        /// <summary>
        /// Returns the name without the 'fabric:' URI scheme.
        /// </summary>
        /// <returns>FabricName without the 'fabric:' URI scheme.</returns>
        public string GetId()
        {
            return Uri.EscapeUriString(this.uri.ToString().Replace("fabric:/", string.Empty));
        }
    }
}
