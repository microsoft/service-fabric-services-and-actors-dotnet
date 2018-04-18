// ------------------------------------------------------------
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT License (MIT).See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Query
{
    using System.Runtime.Serialization;
    using Microsoft.ServiceFabric.Actors.Remoting;

    /// <summary>
    /// Represents a continuation token for query operations.
    /// </summary>
    /// <remarks>A method that may return a partial set of results via a
    /// <see cref="PagedResult{T}"/> object also returns a continuation token
    /// in the object, which can be used in a subsequent call to return the next set of available results.</remarks>
    [DataContract(Name = "ContinuationToken", Namespace = Constants.Namespace)]
    public class ContinuationToken
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Microsoft.ServiceFabric.Actors.Query.ContinuationToken"/> class.
        /// </summary>
        /// <param name="marker">A marker used to retrieve the next set of available results.</param>
        public ContinuationToken(object marker)
        {
            this.Marker = marker;
        }

        /// <summary>
        /// Gets a marker used to fetch the next set of available results.
        /// </summary>
        /// <value>A marker used to fetch the next set of available results.</value>
        [DataMember(Name = "Marker", Order = 0, IsRequired = true)]
        public object Marker { get; private set; }
    }
}
