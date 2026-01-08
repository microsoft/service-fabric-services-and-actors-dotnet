// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;

    /// <summary>
    /// Identifier for a service partition.
    /// </summary>
    public class PartitionId : IEquatable<PartitionId>
    {
        /// <summary>
        /// A read-only instance of the <see cref="PartitionId"/> class whose value is an empty GUID.
        /// </summary>
        /// <value>Instance of <see cref="PartitionId"/> class whose value is an empty GUID.</value>
        public static readonly PartitionId Empty = new PartitionId(Guid.Empty);

        private readonly Guid? id;

        /// <summary>
        /// Initializes a new instance of the <see cref="PartitionId"/> class by using the value represented by the specified GUID.
        /// </summary>
        /// <param name="id">A GUID value for the partition.</param>
        public PartitionId(Guid? id)
        {
            id.ThrowIfNull(nameof(id));
            this.id = id;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PartitionId"/> class by using the value represented by the specified GUID.
        /// </summary>
        /// <param name="id">A GUID value for the partition.</param>
        /// <value>New instance of the <see cref="PartitionId"/> class.</value>
        public static implicit operator PartitionId(Guid id) => new PartitionId(id);

        /// <summary>
        /// Indicates whether the values of two specified <see cref="PartitionId"/> objects are equal.
        /// </summary>
        /// <param name="partitionId1">The first object to compare.</param>z
        /// <param name="partitionId2">The second object to compare</param>
        /// <returns>true if partitionId1 and partitionId2 are equal; otherwise, false.</returns>
        public static bool operator ==(PartitionId partitionId1, PartitionId partitionId2)
            => partitionId1 is null ? partitionId2 is null : partitionId1.Equals(partitionId2);

        /// <summary>
        /// Indicates whether the values of two specified <see cref="PartitionId"/> objects are not equal.
        /// </summary>
        /// <param name="partitionId1">The first object to compare.</param>
        /// <param name="partitionId2">The second object to compare</param>
        /// <returns>true if partitionId1 and partitionId2 are not equal; otherwise, false.</returns>
        public static bool operator !=(PartitionId partitionId1, PartitionId partitionId2)
            => !(partitionId1 == partitionId2);

        /// <summary>
        /// Creates a PartitionId class instance using the string representation of the GUID passed in as argument.
        /// </summary>
        /// <param name="partitionId">The GUID to convert.</param>
        /// <returns>A class that contains the value that was parsed.</returns>
        public static PartitionId Parse(string partitionId) => new PartitionId(Guid.Parse(partitionId));

        /// <summary>
        /// Returns a string representation of the value of this instance.
        /// </summary>
        /// <returns>string representation of the value of this instance.</returns>
        public override string ToString() => this.id.ToString();

        /// <summary>
        /// Returns a value indicating whether this instance and a specified <see cref="PartitionId"/> object represent the same value.
        /// </summary>
        /// <param name="other">An object to compare to this instance.</param>
        /// <returns>true if this instance equal to other parameter; otherwise, false.</returns>
        public bool Equals(PartitionId other)
        {
            if (other is null)
            {
                return false;
            }

            return this.id.Equals(other.id);
        }

        /// <summary>
        /// Returns a value that indicates whether this instance is equal to a specified object.
        /// </summary>
        /// <param name="other">The object to compare with this instance.</param>
        /// <returns>true if other object is a <see cref="PartitionId"/> that has the same value as this instance; otherwise, false.</returns>
        public override bool Equals(object other)
        {
            if (other is null)
            {
                return false;
            }

            return (other is PartitionId) && this.Equals((PartitionId)other);
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>The hash code for this instance.</returns>
        public override int GetHashCode() => this.id.GetHashCode();
    }
}
