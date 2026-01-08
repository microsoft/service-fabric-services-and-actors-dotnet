// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents the health policy used to evaluate the health of services belonging to a service type.
    /// </summary>
    public partial class ServiceTypeHealthPolicy
    {
        /// <summary>
        /// Initializes a new instance of the ServiceTypeHealthPolicy class.
        /// </summary>
        /// <param name="maxPercentUnhealthyPartitionsPerService">The maximum allowed percentage of unhealthy partitions per
        /// service. Allowed values are Byte values from zero to 100
        /// 
        /// The percentage represents the maximum tolerated percentage of partitions that can be unhealthy before the service
        /// is considered in error.
        /// If the percentage is respected but there is at least one unhealthy partition, the health is evaluated as Warning.
        /// The percentage is calculated by dividing the number of unhealthy partitions over the total number of partitions in
        /// the service.
        /// The computation rounds up to tolerate one failure on small numbers of partitions. Default percentage is zero.
        /// </param>
        /// <param name="maxPercentUnhealthyReplicasPerPartition">The maximum allowed percentage of unhealthy replicas per
        /// partition. Allowed values are Byte values from zero to 100.
        /// 
        /// The percentage represents the maximum tolerated percentage of replicas that can be unhealthy before the partition
        /// is considered in error.
        /// If the percentage is respected but there is at least one unhealthy replica, the health is evaluated as Warning.
        /// The percentage is calculated by dividing the number of unhealthy replicas over the total number of replicas in the
        /// partition.
        /// The computation rounds up to tolerate one failure on small numbers of replicas. Default percentage is zero.
        /// </param>
        /// <param name="maxPercentUnhealthyServices">The maximum allowed percentage of unhealthy services. Allowed values are
        /// Byte values from zero to 100.
        /// 
        /// The percentage represents the maximum tolerated percentage of services that can be unhealthy before the application
        /// is considered in error.
        /// If the percentage is respected but there is at least one unhealthy service, the health is evaluated as Warning.
        /// This is calculated by dividing the number of unhealthy services of the specific service type over the total number
        /// of services of the specific service type.
        /// The computation rounds up to tolerate one failure on small numbers of services. Default percentage is zero.
        /// </param>
        public ServiceTypeHealthPolicy(
            int? maxPercentUnhealthyPartitionsPerService = 0,
            int? maxPercentUnhealthyReplicasPerPartition = 0,
            int? maxPercentUnhealthyServices = 0)
        {
            this.MaxPercentUnhealthyPartitionsPerService = maxPercentUnhealthyPartitionsPerService;
            this.MaxPercentUnhealthyReplicasPerPartition = maxPercentUnhealthyReplicasPerPartition;
            this.MaxPercentUnhealthyServices = maxPercentUnhealthyServices;
        }

        /// <summary>
        /// Gets the maximum allowed percentage of unhealthy partitions per service. Allowed values are Byte values from zero
        /// to 100
        /// 
        /// The percentage represents the maximum tolerated percentage of partitions that can be unhealthy before the service
        /// is considered in error.
        /// If the percentage is respected but there is at least one unhealthy partition, the health is evaluated as Warning.
        /// The percentage is calculated by dividing the number of unhealthy partitions over the total number of partitions in
        /// the service.
        /// The computation rounds up to tolerate one failure on small numbers of partitions. Default percentage is zero.
        /// </summary>
        public int? MaxPercentUnhealthyPartitionsPerService { get; }

        /// <summary>
        /// Gets the maximum allowed percentage of unhealthy replicas per partition. Allowed values are Byte values from zero
        /// to 100.
        /// 
        /// The percentage represents the maximum tolerated percentage of replicas that can be unhealthy before the partition
        /// is considered in error.
        /// If the percentage is respected but there is at least one unhealthy replica, the health is evaluated as Warning.
        /// The percentage is calculated by dividing the number of unhealthy replicas over the total number of replicas in the
        /// partition.
        /// The computation rounds up to tolerate one failure on small numbers of replicas. Default percentage is zero.
        /// </summary>
        public int? MaxPercentUnhealthyReplicasPerPartition { get; }

        /// <summary>
        /// Gets the maximum allowed percentage of unhealthy services. Allowed values are Byte values from zero to 100.
        /// 
        /// The percentage represents the maximum tolerated percentage of services that can be unhealthy before the application
        /// is considered in error.
        /// If the percentage is respected but there is at least one unhealthy service, the health is evaluated as Warning.
        /// This is calculated by dividing the number of unhealthy services of the specific service type over the total number
        /// of services of the specific service type.
        /// The computation rounds up to tolerate one failure on small numbers of services. Default percentage is zero.
        /// </summary>
        public int? MaxPercentUnhealthyServices { get; }
    }
}
