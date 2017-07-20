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

    /// <summary>
    /// Manages context propogation for consumption by telemetry systems.
    /// </summary>
    internal class ContextPropagationManager
    {
        private const string PackageActivationIdEnvVariableName = "Fabric_ServicePackageActivationId";

        // This string is part of the contract that Application Insights (and in time other components) rely on. Do not change this string.
        private const string ServiceContextKeyName = "ServiceContext";

        private const string TraceType = "StatefulServiceReplicaAdapter";

        private const string ServiceTelemetryConfigSection = "ServiceTelemetry";
        private const string IncludeContextParamter = "IncludeServiceContext";

        private readonly Dictionary<string, string> contextDictionary;
        private readonly bool isExclusiveMode;

        private readonly bool disableContextPropogation = false;

        /// <summary>
        /// Creates and instance given the service context.
        /// </summary>
        /// <param name="serviceContext">Service Context that is to be propagated.</param>
        public ContextPropagationManager(ServiceContext serviceContext)
        {
            try
            {
                string packageActivationId = Environment.GetEnvironmentVariable(PackageActivationIdEnvVariableName);
                this.isExclusiveMode = !string.IsNullOrEmpty(packageActivationId);

                // Fetch the settings that allows you to turn off context propagation.
                var config = serviceContext.CodePackageActivationContext.GetConfigurationPackageObject("config");
                if (config.Settings.Sections.Contains(ServiceTelemetryConfigSection))
                {
                    if (config.Settings.Sections[ServiceTelemetryConfigSection].Parameters.Contains(IncludeContextParamter))
                    {
                        string includeContext = config.Settings.Sections[ServiceTelemetryConfigSection].Parameters[IncludeContextParamter].Value;

                        if (string.Compare(includeContext, "true", StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            disableContextPropogation = false;
                        }
                        else if (string.Compare(includeContext, "false", StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            disableContextPropogation = true;

                            // I am assuming when the user says IncludeContext = false, I would respect it even in exclusive mode, so let's just return without even setting the environment even if the service is running in exclusive mode.
                            return;
                        }
                        else
                        {
                            // Invalid value - pretent not configured, but let's log warning.
                            ServiceTrace.Source.WriteWarning(TraceType, SR.ErrorInvalidValueForIncludeServiceContext);
                        }
                    }
                }

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

        /// <summary>
        /// Ensures context is propagated through logical call context, when the service is running in shared mode.
        /// </summary>
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

        /// <summary>
        /// Ensures the context added on logical call context is removed.
        /// </summary>
        public void StopContextPropagation()
        {
            try
            {
                if (this.isExclusiveMode || this.disableContextPropogation)
                {
                    return;
                }

                CallContext.FreeNamedDataSlot(ServiceContextKeyName);
            }
            catch(Exception ex)
            {
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
