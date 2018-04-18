// ------------------------------------------------------------
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT License (MIT).See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Query
{
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using Microsoft.ServiceFabric.Actors.Remoting;

    /// <summary>
    /// Represents the result of actor query calls.
    /// </summary>
    /// <typeparam name="T"><see cref="System.Type"/> of the items this query result contains.</typeparam>
    [DataContract(Name = "PagedResult", Namespace = Constants.Namespace)]
    [KnownType(typeof(List<ActorInformation>))]
    public sealed class PagedResult<T>
        where T : class
    {
        /// <summary>
        /// Max number of items to return in Query Result.
        /// Default MessageSize of 4 MB with DataContract serialization can include up to 85000 items when ActorInformation only includes ActorId.
        /// Its set to 10000 to allow for custom serialization and future changes in ActorInformation.
        /// </summary>
        internal const int MaxItemsToReturn = 10000;

        /// <summary>
        /// Initializes a new instance of the <see cref="PagedResult{T}"/> class.
        /// </summary>
        public PagedResult()
        {
            this.Items = new List<T>();
            this.ContinuationToken = null;
        }

        /// <summary>
        /// Gets or sets Enumerator to iterate over the results.
        /// </summary>
        /// <value>An Enumerator, which supports a simple iteration over
        /// the collection.</value>
        [DataMember(Name = "Items", IsRequired = true, Order = 0)]
        public IEnumerable<T> Items { get; set; }

        /// <summary>
        /// Gets or sets a continuation token indicating if more items need to be fetched by calling the method again.
        /// </summary>
        /// <remarks>A null value of continuation token means that the result contains all the items
        /// and no calls to method needs to be made to fetch more items.</remarks>
        /// <value>ConinutationToken signifying if the method which returned
        /// the <see cref="PagedResult{T}"/>
        /// needs to called again to get more results </value>
        [DataMember(Name = "ContinuationToken", IsRequired = false, Order = 1)]
        public ContinuationToken ContinuationToken { get; set; }
    }
}
