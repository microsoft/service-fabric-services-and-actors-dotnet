// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;

    /// <summary>
    /// A token that is used in paged data queries to move to the next page of data.
    /// </summary>
    public class ContinuationToken : IEquatable<ContinuationToken>
    { 
        /// <summary>
        /// An empty continuation token instance.
        /// </summary>
        public static readonly ContinuationToken Empty = new ContinuationToken(string.Empty);

        private readonly string continuationToken;

        /// <summary>
        /// Initializes a new instance of the <see cref="Microsoft.ServiceFabric.Common.ContinuationToken"/> class.
        /// </summary>
        /// <param name="token">token used to retrieve the next set of available results.</param>
        public ContinuationToken(string token)
        {
            this.continuationToken = token;
        }

        /// <summary>
        /// Gets a value indicating whether a paged data operation has more data available on the server.
        /// </summary>
        public bool Next => !string.IsNullOrEmpty(this.continuationToken);

        /// <summary>
        /// Determines if this ContinuationToken instance is equal to another instance.
        /// </summary>
        /// <param name="other">The object to compare with this instance.</param>
        /// <returns>true if other object is a <see cref="ContinuationToken"/> that has the same value as this instance; otherwise, false.</returns>
        public bool Equals(ContinuationToken other)
        {
            if (other == null)
            {
                return false;
            }

            return this.continuationToken.Equals(other?.continuationToken);
        }

        /// <summary>
        /// Determines if this ContinuationToken instance is equal to another object.
        /// </summary>
        /// <param name="other">The object to compare with this instance.</param>
        /// <returns>true if other object is a <see cref="ContinuationToken"/> that has the same value as this instance; otherwise, false.</returns>
        public override bool Equals(object other)
        {
            if (other == null)
            {
                return false;
            }

            if (other is ContinuationToken)
            {
                return this.Equals((ContinuationToken)other);
            }

            return false;
        }

        /// <summary>
        /// Gets the hash code of this instance.
        /// </summary>
        /// <returns>An integer hash of this instance's value.</returns>
        public override int GetHashCode()
        {
            return this.continuationToken.GetHashCode();
        }

        /// <summary>
        /// Gets the string value of this object
        /// </summary>
        /// <returns>A string representing this object's value</returns>
        public override string ToString()
        {
            return this.continuationToken;
        }
    }
}
