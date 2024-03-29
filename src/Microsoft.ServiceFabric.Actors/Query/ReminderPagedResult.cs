// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Query
{
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using Microsoft.ServiceFabric.Actors.Remoting;
    using Microsoft.ServiceFabric.Actors.Runtime;

    /// <summary>
    /// Represents the result of actor reminder query calls.
    /// </summary>
    /// <typeparam name="T"><see cref="System.Type"/> of the items this reminder query result contains.</typeparam>
    [DataContract(Name = "ReminderPagedResult", Namespace = Constants.Namespace)]
    [KnownType(typeof(List<KeyValuePair<ActorId, List<ActorReminderState>>>))]
    public sealed class ReminderPagedResult<T>
    {
        /// <summary>
        /// Max number of items to return by Reminder Query Result.
        /// </summary>
        private static int maxItemsToReturn = 10000;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReminderPagedResult{T}"/> class.
        /// </summary>
        public ReminderPagedResult()
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

        /// <summary>
        /// Sets the new default page size.
        /// </summary>
        /// <param name="newSize">New size.</param>
        public static void SetDefaultPageSize(int newSize)
        {
            maxItemsToReturn = newSize;
        }

        internal static int GetDefaultPageSize()
        {
            return maxItemsToReturn;
        }
    }
}
