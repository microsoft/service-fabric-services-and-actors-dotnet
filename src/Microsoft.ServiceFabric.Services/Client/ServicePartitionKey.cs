// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------
namespace Microsoft.ServiceFabric.Services.Client
{
    using System.Fabric;

    /// <summary>
    /// Defines a key to address a service partition.
    /// </summary>
    public sealed class ServicePartitionKey
    {
        private readonly ServicePartitionKind servicePartitionKind;
        private readonly object value;

        /// <summary>
        /// Returns a ServicePartitionKey that indicates a singleton partition.
        /// </summary>
        public static readonly ServicePartitionKey Singleton = new ServicePartitionKey();

        /// <summary>
        /// Instantiates a ServicePartitionKey for singleton partitioned service.
        /// </summary>
        public ServicePartitionKey()
        {
            this.value = null;
            this.servicePartitionKind = ServicePartitionKind.Singleton;
        }

        /// <summary>
        /// Instantiates a ServicePartitionKey for uniform int64 partitioned service.
        /// </summary>
        /// <param name="partitionKey">Value of the int64 partition key</param>
        public ServicePartitionKey(long partitionKey)
        {
            this.servicePartitionKind = ServicePartitionKind.Int64Range;
            this.value = partitionKey;
        }

        /// <summary>
        /// Instantiates a ServicePartitionKey for named partitioned services.
        /// </summary>
        /// <param name="partitionKey">Value of the named partition key</param>
        public ServicePartitionKey(string partitionKey)
        {
            this.servicePartitionKind = ServicePartitionKind.Named;
            this.value = partitionKey;
        }

        /// <summary>
        /// Gets the Kind of the partition key applies to.
        /// </summary>
        /// <value>Partition kind</value>
        public ServicePartitionKind Kind
        {
            get { return this.servicePartitionKind; }
        }

        /// <summary>
        /// Gets the value of the partition key. This value can be casted to the right type based on the value of the Kind property.
        /// </summary>
        /// <value>Partition key</value>
        public object Value
        {
            get { return this.value; }
        }
    }
}