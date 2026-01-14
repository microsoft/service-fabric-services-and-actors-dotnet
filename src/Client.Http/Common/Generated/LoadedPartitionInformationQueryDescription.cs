// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents data structure that contains query information.
    /// </summary>
    public partial class LoadedPartitionInformationQueryDescription
    {
        /// <summary>
        /// Initializes a new instance of the LoadedPartitionInformationQueryDescription class.
        /// </summary>
        /// <param name="metricName">Name of the metric for which this information is provided.</param>
        /// <param name="serviceName">Name of the service this partition belongs to.</param>
        /// <param name="ordering">Ordering of partitions' load. Possible values include: 'Desc', 'Asc'
        /// 
        /// Defines the order.
        /// </param>
        /// <param name="maxResults">The maximum number of results to be returned as part of the paged queries. This parameter
        /// defines the upper bound on the number of results returned. The results returned can be less than the specified
        /// maximum results if they do not fit in the message as per the max message size restrictions defined in the
        /// configuration. If this parameter is zero or not specified, the paged query includes as many results as possible
        /// that fit in the return message.</param>
        /// <param name="continuationToken">The continuation token parameter is used to obtain next set of results. The
        /// continuation token is included in the response of the API when the results from the system do not fit in a single
        /// response. When this value is passed to the next API call, the API returns next set of results. If there are no
        /// further results, then the continuation token is not included in the response.</param>
        public LoadedPartitionInformationQueryDescription(
            string metricName = default(string),
            string serviceName = default(string),
            Ordering? ordering = Common.Ordering.Desc,
            long? maxResults = default(long?),
            ContinuationToken continuationToken = default(ContinuationToken))
        {
            this.MetricName = metricName;
            this.ServiceName = serviceName;
            this.Ordering = ordering;
            this.MaxResults = maxResults;
            this.ContinuationToken = continuationToken;
        }

        /// <summary>
        /// Gets name of the metric for which this information is provided.
        /// </summary>
        public string MetricName { get; }

        /// <summary>
        /// Gets name of the service this partition belongs to.
        /// </summary>
        public string ServiceName { get; }

        /// <summary>
        /// Gets ordering of partitions' load. Possible values include: 'Desc', 'Asc'
        /// 
        /// Defines the order.
        /// </summary>
        public Ordering? Ordering { get; }

        /// <summary>
        /// Gets the maximum number of results to be returned as part of the paged queries. This parameter defines the upper
        /// bound on the number of results returned. The results returned can be less than the specified maximum results if
        /// they do not fit in the message as per the max message size restrictions defined in the configuration. If this
        /// parameter is zero or not specified, the paged query includes as many results as possible that fit in the return
        /// message.
        /// </summary>
        public long? MaxResults { get; }

        /// <summary>
        /// Gets the continuation token parameter is used to obtain next set of results. The continuation token is included in
        /// the response of the API when the results from the system do not fit in a single response. When this value is passed
        /// to the next API call, the API returns next set of results. If there are no further results, then the continuation
        /// token is not included in the response.
        /// </summary>
        public ContinuationToken ContinuationToken { get; }
    }
}
