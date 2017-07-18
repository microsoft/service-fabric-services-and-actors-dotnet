// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------
namespace Microsoft.ServiceFabric.Services
{
    using System;
    using System.Collections.Generic;
    using System.Fabric;
    using System.Globalization;
    using System.Runtime.Remoting.Messaging;

    internal class ContextPropagationManager
    {
        private const string PackageActivationIdEnvVariableName = "Fabric_ServicePackageActivationId";

        // This string is part of the contract that Application Insights (and in time other components) rely on. Do not change this string.
        private const string ServiceContextKeyName = "ServiceContext";

        private const string TraceType = "StatefulServiceReplicaAdapter";

        private readonly Dictionary<string, string> contextDictionary;
        private readonly bool isExclusiveMode;

        private readonly bool disableContextPropogation = false;

        public ContextPropagationManager(ServiceContext serviceContext)
        {
            try
            {
                string packageActivationId = Environment.GetEnvironmentVariable(PackageActivationIdEnvVariableName);
                this.isExclusiveMode = !string.IsNullOrEmpty(packageActivationId);

                // Todo (nizarq): Have some setting that enables customers to say - no - I don't want auto context propogation. Load that here and set this.disableContextPropagation.

                if (this.isExclusiveMode)
                {
                    SetContextInEnvironmentForExclusiveMode(serviceContext);
                }
                else
                {
                    this.contextDictionary = BuildContextDisctionary(serviceContext);
                }
            }
            catch (Exception ex)
            {
                // Context propogation is important but non essential. If something goes wrong in here, we don't want the service to crash.
                ServiceTrace.Source.WriteExceptionAsWarning(TraceType, ex);
            }
        }

        public void PropagateContext()
        {
            try
            {
                if (this.isExclusiveMode || this.disableContextPropogation)
                {
                    // isExclusiveMode - No explicit propogation required, Context Propogation happens through environment variables, which only need to be set once for the process.
                    // disableContextPropogation - Customer has explicitly aske for context to not be propagated through CallContext.
                    return;
                }

                // This shouldn't be null but just checking in case something went wrong when originally building the dictionary.
                if (this.contextDictionary != null)
                {
                    CallContext.LogicalSetData(ServiceContextKeyName, this.contextDictionary);
                }
            }
            catch (Exception ex)
            {
                // Context propogation is important but non essential. If something goes wrong in here, we don't want the service to crash.
                ServiceTrace.Source.WriteExceptionAsWarning(TraceType, ex);
            }
        }

        private Dictionary<string, string> BuildContextDisctionary(ServiceContext context)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();

            if (context != null)
            {
                result.Add(ContextFieldNames.ServiceName, context.ServiceName.ToString());
                result.Add(ContextFieldNames.ServiceTypeName, context.ServiceTypeName);
                result.Add(ContextFieldNames.PartitionId, context.PartitionId.ToString());
                result.Add(ContextFieldNames.ApplicationName, context.CodePackageActivationContext.ApplicationName);
                result.Add(ContextFieldNames.ApplicationTypeName, context.CodePackageActivationContext.ApplicationTypeName);
                result.Add(ContextFieldNames.NodeName, context.NodeContext.NodeName);
                if (context is StatelessServiceContext)
                {
                    result.Add(ContextFieldNames.InstanceId, context.ReplicaOrInstanceId.ToString(CultureInfo.InvariantCulture));
                }

                if (context is StatefulServiceContext)
                {
                    result.Add(ContextFieldNames.ReplicaId, context.ReplicaOrInstanceId.ToString(CultureInfo.InvariantCulture));
                }
            }

            return result;
        }

        private void SetContextInEnvironmentForExclusiveMode(ServiceContext serviceContext)
        {
            Environment.SetEnvironmentVariable(ContextFieldNames.ServiceName, serviceContext.ServiceName.ToString());
            Environment.SetEnvironmentVariable(ContextFieldNames.ServiceTypeName, serviceContext.ServiceTypeName);
            Environment.SetEnvironmentVariable(ContextFieldNames.PartitionId, serviceContext.PartitionId.ToString());
            Environment.SetEnvironmentVariable(ContextFieldNames.ApplicationName, serviceContext.CodePackageActivationContext.ApplicationName);
            Environment.SetEnvironmentVariable(ContextFieldNames.ApplicationTypeName, serviceContext.CodePackageActivationContext.ApplicationTypeName);

            if (serviceContext is StatelessServiceContext)
            {
                Environment.SetEnvironmentVariable(ContextFieldNames.InstanceId, serviceContext.ReplicaOrInstanceId.ToString());
            }

            if (serviceContext is StatefulServiceContext)
            {
                Environment.SetEnvironmentVariable(ContextFieldNames.ReplicaId, serviceContext.ReplicaOrInstanceId.ToString());
            }
        }

        /// <summary>
        /// Field names for context information.
        /// These strings are part of the contract that Application Insights (and in time other components) rely on. Do not change this string.
        /// </summary>
        private class ContextFieldNames
        {
            public const string ServiceName = "Fabric_ServiceName";
            public const string ServiceTypeName = "Fabrid_ServiceTypeName";
            public const string PartitionId = "Fabric_PartitionId";
            public const string ApplicationName = "Fabric_ApplicationName";
            public const string ApplicationTypeName = "Fabric_ApplicationTypeName";
            public const string InstanceId = "Fabric_InstanceId";
            public const string ReplicaId = "Fabric_ReplicaId";
            public const string NodeName = "Fabric_NodeName";
        }
    }
}
