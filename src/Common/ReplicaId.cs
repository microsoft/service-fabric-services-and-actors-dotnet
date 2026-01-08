// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;

    /// <summary>
    /// Identifier for a service replica.
    /// </summary>
    public class ReplicaId : IEquatable<ReplicaId>
    {
        /// <summary>
        /// A read-only instance of the <see cref="ReplicaId"/> class whose value is 0.
        /// </summary>
        /// <value>Instance of <see cref="ReplicaId"/> class whose value is 0.</value>
        public static readonly ReplicaId Empty = new ReplicaId(0);

        private readonly long? id;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReplicaId"/> class by using the value represented by the specified long.
        /// </summary>
        /// <param name="id">A long value for the replica id.</param>
        public ReplicaId(long? id)
        {
            id.ThrowIfNull(nameof(id));
            this.id = id;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReplicaId"/> class by using the value represented by the specified long.
        /// </summary>
        /// <param name="id">A long value for the replica id.</param>
        /// <value>New instance of the <see cref="ReplicaId"/> class.</value>
        public static implicit operator ReplicaId(long id) => new ReplicaId(id);

        /// <summary>
        /// Indicates whether the values of two specified <see cref="ReplicaId"/> objects are equal.
        /// </summary>
        /// <param name="replicaId1">The first object to compare.</param>
        /// <param name="replicaId2">The second object to compare</param>
        /// <returns>true if replicaId1 and replicaId2 are equal; otherwise, false.</returns>
        public static bool operator ==(ReplicaId replicaId1, ReplicaId replicaId2)
            => replicaId1 is null ? replicaId2 is null : replicaId1.Equals(replicaId2);

        /// <summary>
        /// Indicates whether the values of two specified <see cref="ReplicaId"/> objects are not equal.
        /// </summary>
        /// <param name="replicaId1">The first object to compare.</param>
        /// <param name="replicaId2">The second object to compare</param>
        /// <returns>true if replicaId1 and replicaId2 are not equal; otherwise, false.</returns>
        public static bool operator !=(ReplicaId replicaId1, ReplicaId replicaId2)
            => !(replicaId1 == replicaId2);

        /// <summary>
        /// Creates a ReplicaId class instance using the string representation of the long passed in as argument.
        /// </summary>
        /// <param name="replicaId">The long to convert.</param>
        /// <returns>A class that contains the value that was parsed.</returns>
        public static ReplicaId Parse(string replicaId) => new ReplicaId(long.Parse(replicaId));

        /// <summary>
        /// Returns a string representation of the value of this instance.
        /// </summary>
        /// <returns>string representation of the value of this instance.</returns>
        public override string ToString() => this.id.ToString();

        /// <summary>
        /// Returns a value indicating whether this instance and a specified <see cref="ReplicaId"/> object represent the same value.        
        /// </summary>
        /// <param name="other">An object to compare to this instance.</param>
        /// <returns>true if this instance equal to other parameter; otherwise, false.</returns>
        public bool Equals(ReplicaId other)
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
        /// <returns>true if other object is a <see cref="ReplicaId"/> that has the same value as this instance; otherwise, false.</returns>
        public override bool Equals(object other)
        {
            if (other is null)
            {
                return false;
            }

            return (other is ReplicaId) && this.Equals((ReplicaId)other);
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>The hash code for this instance.</returns>
        public override int GetHashCode() => this.id.GetHashCode();
    }
}
