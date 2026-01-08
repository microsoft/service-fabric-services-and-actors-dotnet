// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// A strongly-typed chunk of a paged dataset.
    /// </summary>
    /// <typeparam name="T"><see cref="System.Type"/> of the items this PagedData contains.</typeparam>
    public class PagedData<T>
    {
        private static readonly PagedData<T> EmptyPagedData = new PagedData<T>(ContinuationToken.Empty, Enumerable.Empty<T>());

        /// <summary>
        /// Initializes a new instance of the <see cref="PagedData{T}"/> class.
        /// </summary>
        /// <param name="continuationToken">Continuation Token.</param>
        /// <param name="data">List of data.</param>
        public PagedData(ContinuationToken continuationToken, IEnumerable<T> data)
        {
            this.ContinuationToken = continuationToken;
            this.Data = data;
        }

        /// <summary>
        /// Gets an empty PagedData class.
        /// </summary>
        public static PagedData<T> Empty
        {
            get
            {
                return EmptyPagedData;
            }
        }

        /// <summary>
        /// Gets a ContinuationToken to get the next set of paged data.
        /// </summary>
        /// <value><see cref="Microsoft.ServiceFabric.Common.ContinuationToken"/> to get the next set of paged data.</value>
        public ContinuationToken ContinuationToken { get; }

        /// <summary>
        /// Gets a segment of data returned in this page of the total dataset.
        /// </summary>
        /// <value>Enumerator, which supports a simple iteration over the data.</value>
        public IEnumerable<T> Data { get; }        
    }
}
