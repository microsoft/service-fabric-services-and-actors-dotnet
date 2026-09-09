// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Data
{
    using System;

    /// <summary>
    /// Wraps a <see cref="string"/> to use <see cref="StringComparison.Ordinal"/> for <see cref="IComparable{T}"/> and <see cref="IEquatable{T}"/> implementations.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Use <see cref="OrdinalString"/> instead of <see cref="string"/> as a Reliable Dictionary key to avoid data corruption and inconsistent enumeration caused by the default culture-sensitive string comparison.
    /// </para>
    /// <para>
    /// The implicit conversion from string to OrdinalString minimizes code changes when upstream code uses string.
    /// Conversion from OrdinalString to string is explicit to keep comparison behavior well-defined.
    /// </para>
    /// <para>
    /// The wrapped string can be <see langword="null"/>, which is the value of <c>default(OrdinalString)</c>. Conversions and <see cref="ToString"/> return the wrapped <see langword="null"/> unchanged, while equality and comparison apply <see cref="StringComparison.Ordinal"/> rules to a <see langword="null"/> value. <see cref="GetHashCode"/> throws because it dereferences the value.
    /// </para>
    /// </remarks>
    public struct OrdinalString : IEquatable<OrdinalString>, IComparable<OrdinalString>
    {
        private readonly string value;

        /// <summary>
        /// Initializes a new instance of the <see cref="OrdinalString"/> struct.
        /// </summary>
        /// <param name="value">The value to wrap. May be <see langword="null"/>.</param>
        public OrdinalString(string value)
        {
            this.value = value;
        }

        /// <summary>
        /// Returns the wrapped <see cref="string"/>, which may be <see langword="null"/>.
        /// </summary>
        public static explicit operator string(OrdinalString value)
        {
            return value.value;
        }

        /// <summary>
        /// Returns an <see cref="OrdinalString"/> that wraps the given <see cref="string"/>, which may be <see langword="null"/>.
        /// </summary>
        public static implicit operator OrdinalString(string value)
        {
            return new OrdinalString(value);
        }

        /// <summary>
        /// Returns <see langword="true"/> if the value of <paramref name="left"/> is the same as the value of <paramref name="right"/>; otherwise, <see langword="false"/>.
        /// </summary>
        public static bool operator ==(OrdinalString left, OrdinalString right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Returns <see langword="true"/> if the value of <paramref name="left"/> is different from the value of <paramref name="right"/>; otherwise, <see langword="false"/>.
        /// </summary>
        public static bool operator !=(OrdinalString left, OrdinalString right)
        {
            return !left.Equals(right);
        }

        /// <summary>
        /// Returns <see langword="true"/> if the value of <paramref name="left"/> is less than the value of <paramref name="right"/>; otherwise, <see langword="false"/>.
        /// </summary>
        public static bool operator <(OrdinalString left, OrdinalString right)
        {
            return left.CompareTo(right) < 0;
        }

        /// <summary>
        /// Returns <see langword="true"/> if the value of <paramref name="left"/> is greater than the value of <paramref name="right"/>; otherwise, <see langword="false"/>.
        /// </summary>
        public static bool operator >(OrdinalString left, OrdinalString right)
        {
            return left.CompareTo(right) > 0;
        }

        /// <summary>
        /// Returns <see langword="true"/> if the value of <paramref name="left"/> is less than or equal to the value of <paramref name="right"/>; otherwise, <see langword="false"/>.
        /// </summary>
        public static bool operator <=(OrdinalString left, OrdinalString right)
        {
            return left.CompareTo(right) <= 0;
        }

        /// <summary>
        /// Returns <see langword="true"/> if the value of <paramref name="left"/> is greater than or equal to the value of <paramref name="right"/>; otherwise, <see langword="false"/>.
        /// </summary>
        public static bool operator >=(OrdinalString left, OrdinalString right)
        {
            return left.CompareTo(right) >= 0;
        }

        /// <inheritdoc/>
        /// <remarks>Returns <see langword="null"/> when the wrapped <see cref="string"/> is <see langword="null"/>.</remarks>
        public override string ToString()
        {
            return this.value;
        }

        /// <inheritdoc/>
        /// <remarks>Two <see langword="null"/> wrapped values are equal.</remarks>
        public bool Equals(OrdinalString value)
        {
            return string.Equals(this.value, value.value, StringComparison.Ordinal);
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            if (obj is OrdinalString other)
            {
                return this.Equals(other);
            }

            return false;
        }

        /// <inheritdoc/>
        /// <exception cref="NullReferenceException">
        /// The <see cref="OrdinalString"/> is <c>default(OrdinalString)</c> or was created from a <see langword="null"/> <see cref="string"/>.
        /// </exception>
        public override int GetHashCode()
        {
            return this.value.GetHashCode();
        }

        /// <inheritdoc/>
        /// <remarks>A <see langword="null"/> wrapped value sorts before any non-<see langword="null"/> value.</remarks>
        public int CompareTo(OrdinalString other)
        {
            return string.CompareOrdinal(this.value, other.value);
        }
    }
}
