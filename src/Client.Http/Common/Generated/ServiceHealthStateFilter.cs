// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Defines matching criteria to determine whether a service should be included as a child of an application in the
    /// cluster health chunk.
    /// The services are only returned if the parent application matches a filter specified in the cluster health chunk
    /// query description.
    /// One filter can match zero, one or multiple services, depending on its properties.
    /// </summary>
    public partial class ServiceHealthStateFilter
    {
        /// <summary>
        /// Initializes a new instance of the ServiceHealthStateFilter class.
        /// </summary>
        /// <param name="serviceNameFilter">The name of the service that matches the filter. The filter is applied only to the
        /// specified service, if it exists.
        /// If the service doesn't exist, no service is returned in the cluster health chunk based on this filter.
        /// If the service exists, it is included as the application's child if the health state matches the other filter
        /// properties.
        /// If not specified, all services that match the parent filters (if any) are taken into consideration and matched
        /// against the other filter members, like health state filter.
        /// </param>
        /// <param name="healthStateFilter">The filter for the health state of the services. It allows selecting services if
        /// they match the desired health states.
        /// The possible values are integer value of one of the following health states. Only services that match the filter
        /// are returned. All services are used to evaluate the cluster aggregated health state.
        /// If not specified, default value is None, unless the service name is specified. If the filter has default value and
        /// service name is specified, the matching service is returned.
        /// The state values are flag-based enumeration, so the value could be a combination of these values obtained using
        /// bitwise 'OR' operator.
        /// For example, if the provided value is 6, it matches services with HealthState value of OK (2) and Warning (4).
        /// 
        /// - Default - Default value. Matches any HealthState. The value is zero.
        /// - None - Filter that doesn't match any HealthState value. Used in order to return no results on a given collection
        /// of states. The value is 1.
        /// - Ok - Filter that matches input with HealthState value Ok. The value is 2.
        /// - Warning - Filter that matches input with HealthState value Warning. The value is 4.
        /// - Error - Filter that matches input with HealthState value Error. The value is 8.
        /// - All - Filter that matches input with any HealthState value. The value is 65535.
        /// </param>
        /// <param name="partitionFilters">Defines a list of filters that specify which partitions to be included in the
        /// returned cluster health chunk as children of the service. The partitions are returned only if the parent service
        /// matches a filter.
        /// If the list is empty, no partitions are returned. All the partitions are used to evaluate the parent service
        /// aggregated health state, regardless of the input filters.
        /// The service filter may specify multiple partition filters.
        /// For example, it can specify a filter to return all partitions with health state Error and another filter to always
        /// include a partition identified by its partition ID.
        /// </param>
        public ServiceHealthStateFilter(
            string serviceNameFilter = default(string),
            int? healthStateFilter = 0,
            IEnumerable<PartitionHealthStateFilter> partitionFilters = default(IEnumerable<PartitionHealthStateFilter>))
        {
            this.ServiceNameFilter = serviceNameFilter;
            this.HealthStateFilter = healthStateFilter;
            this.PartitionFilters = partitionFilters;
        }

        /// <summary>
        /// Gets the name of the service that matches the filter. The filter is applied only to the specified service, if it
        /// exists.
        /// If the service doesn't exist, no service is returned in the cluster health chunk based on this filter.
        /// If the service exists, it is included as the application's child if the health state matches the other filter
        /// properties.
        /// If not specified, all services that match the parent filters (if any) are taken into consideration and matched
        /// against the other filter members, like health state filter.
        /// </summary>
        public string ServiceNameFilter { get; }

        /// <summary>
        /// Gets the filter for the health state of the services. It allows selecting services if they match the desired health
        /// states.
        /// The possible values are integer value of one of the following health states. Only services that match the filter
        /// are returned. All services are used to evaluate the cluster aggregated health state.
        /// If not specified, default value is None, unless the service name is specified. If the filter has default value and
        /// service name is specified, the matching service is returned.
        /// The state values are flag-based enumeration, so the value could be a combination of these values obtained using
        /// bitwise 'OR' operator.
        /// For example, if the provided value is 6, it matches services with HealthState value of OK (2) and Warning (4).
        /// 
        /// - Default - Default value. Matches any HealthState. The value is zero.
        /// - None - Filter that doesn't match any HealthState value. Used in order to return no results on a given collection
        /// of states. The value is 1.
        /// - Ok - Filter that matches input with HealthState value Ok. The value is 2.
        /// - Warning - Filter that matches input with HealthState value Warning. The value is 4.
        /// - Error - Filter that matches input with HealthState value Error. The value is 8.
        /// - All - Filter that matches input with any HealthState value. The value is 65535.
        /// </summary>
        public int? HealthStateFilter { get; }

        /// <summary>
        /// Gets defines a list of filters that specify which partitions to be included in the returned cluster health chunk as
        /// children of the service. The partitions are returned only if the parent service matches a filter.
        /// If the list is empty, no partitions are returned. All the partitions are used to evaluate the parent service
        /// aggregated health state, regardless of the input filters.
        /// The service filter may specify multiple partition filters.
        /// For example, it can specify a filter to return all partitions with health state Error and another filter to always
        /// include a partition identified by its partition ID.
        /// </summary>
        public IEnumerable<PartitionHealthStateFilter> PartitionFilters { get; }
    }
}
