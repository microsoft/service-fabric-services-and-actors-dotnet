// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Powershell.Http
{
    internal static class Constants
    {
        internal static readonly string ApplicationNamePropertyName = "ApplicationName";
        internal static readonly string ClusterConnectionVariableName = "SFHttpClusterConnection";
        internal static readonly string PartitionIdPropertyName = "PartitionId";
        internal static readonly string PartitionKindPropertyName = "PartitionKind";
        internal static readonly string PartitionLowKeyPropertyName = "PartitionLowKey";
        internal static readonly string PartitionHighKeyPropertyName = "PartitionHighKey";
        internal static readonly string PartitionNamePropertyName = "PartitionName";
        internal static readonly string DataLossNumberPropertyName = "DataLossNumber";
        internal static readonly string ConfigurationNumberPropertyName = "ConfigurationNumber";
        internal static readonly string ServiceName = "ServiceName";
        internal static readonly string GetImageStoreConnectionStringErrorId = "GetImageStoreConnectionStringErrorId";
        internal static readonly string NoneResultOutput = "None";
        internal static readonly string AggregatedHealthStatePropertyName = "AggregatedHealthState";
        internal static readonly string FormatObjectMethodName = "FormatObject";
        internal static readonly string ToStringMethodName = "ToString";
        internal static readonly string NodeNamePropertyName = "NodeName";
        internal static readonly string LocalDevClusterConnectionParamFileName = "ServiceFabricLocalClusterConnectionParams.json";
        internal static readonly string PowershellClientTypeHeaderValue = "SF-PS";
        internal static readonly string SFRegistryPath = "Software\\Microsoft\\Service Fabric";
        internal static readonly string SFDataRootkeyName = "FabricDataRoot";
    }
}
