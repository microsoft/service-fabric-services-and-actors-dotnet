// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Powershell.Http
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Management.Automation;
    using System.Text;
    using Microsoft.ServiceFabric.Common;

    /// <summary>
    /// Defines custom formatters for various types of objects on the console.
    /// </summary>
    public partial class OutputFormatter
    {
        /// <summary>
        /// Invokes the format object function of the object passed, for better interpretation
        /// </summary>
        /// <param name="psObject"> Result returned by the PS cmdlet </param>
        /// <returns>
        /// Returns the formatted output
        /// </returns>
        public static string FormatObject(PSObject psObject)
        {
            var type = psObject.BaseObject.GetType();
            if (TypeToMethod.ContainsKey(type))
            {
                return TypeToMethod[type].DynamicInvoke(psObject.BaseObject) as string;
            }

            return psObject.BaseObject.ToString();
        }

        /// <summary>
        /// Overloaded ToString function for formatting the output on the console.
        /// </summary>
        /// <param name="serviceHealthState"> List of ServiceHealthState </param>
        /// <returns>
        /// Returns formatted string.
        /// </returns>
        public static string ToString(IList<ServiceHealthState> serviceHealthState)
        {
            if (serviceHealthState.Count == 0)
            {
                return "None";
            }

            var strBuilder = new StringBuilder();
            foreach (var item in serviceHealthState)
            {
                strBuilder.Append(Environment.NewLine);
                strBuilder.Append(ToString(item));
            }

            return strBuilder.ToString();
        }

        /// <summary>
        /// Overloaded ToString function for formatting the output on the console.
        /// </summary>
        /// <param name="serviceHealthState"> Object of type ServiceHealthState </param>
        /// <returns>
        /// Returns formatted string.
        /// </returns>
        public static string ToString(ServiceHealthState serviceHealthState)
        {
            var strBuilder = new StringBuilder();

            strBuilder.Append(string.Format(CultureInfo.CurrentCulture, "{0, -21} : {1}", "ServiceName", serviceHealthState.ServiceName));
            strBuilder.Append(Environment.NewLine);
            strBuilder.Append(string.Format(CultureInfo.CurrentCulture, "{0, -21} : {1}", "AggregatedHealthState", serviceHealthState.AggregatedHealthState));
            strBuilder.Append(Environment.NewLine);

            return strBuilder.ToString();
        }

        /// <summary>
        /// Overloaded ToString function for formatting the output on the console.
        /// </summary>
        /// <param name="deployedApplicationHealthState"> List of DeployedApplicationHealthState </param>
        /// <returns>
        /// Returns formatted string.
        /// </returns>
        public static string ToString(IList<DeployedApplicationHealthState> deployedApplicationHealthState)
        {
            if (deployedApplicationHealthState.Count == 0)
            {
                return "None";
            }

            var strBuilder = new StringBuilder();
            foreach (var item in deployedApplicationHealthState)
            {
                strBuilder.Append(Environment.NewLine);
                strBuilder.Append(ToString(item));
            }

            return strBuilder.ToString();
        }

        /// <summary>
        /// Overloaded ToString function for formatting the output on the console.
        /// </summary>
        /// <param name="deployedApplicationHealthState"> Object of type DeployedApplicationHealthState </param>
        /// <returns>
        /// Returns formatted string.
        /// </returns>
        public static string ToString(DeployedApplicationHealthState deployedApplicationHealthState)
        {
            var strBuilder = new StringBuilder();

            strBuilder.Append(string.Format(CultureInfo.CurrentCulture, "{0, -21} : {1}", "ApplicationName", deployedApplicationHealthState.ApplicationName));
            strBuilder.Append(Environment.NewLine);
            strBuilder.Append(string.Format(CultureInfo.CurrentCulture, "{0, -21} : {1}", "NodeName", deployedApplicationHealthState.NodeName));
            strBuilder.Append(Environment.NewLine);
            strBuilder.Append(string.Format(CultureInfo.CurrentCulture, "{0, -21} : {1}", "AggregatedHealthState", deployedApplicationHealthState.AggregatedHealthState));
            strBuilder.Append(Environment.NewLine);

            return strBuilder.ToString();
        }

        /// <summary>
        /// Overloaded ToString function for formatting the output on the console.
        /// </summary>
        /// <param name="applicationHealth"> Object of type ApplicationHealth </param>
        /// <returns>
        /// Returns formatted string.
        /// </returns>
        public static string ToString(ApplicationHealth applicationHealth)
        {
            var strBuilder = new StringBuilder();

            strBuilder.Append(string.Format(CultureInfo.CurrentCulture, "{0} : {1}", "ApplicationName", applicationHealth.Name));
            strBuilder.Append(Environment.NewLine);
            strBuilder.Append(string.Format(CultureInfo.CurrentCulture, "{0} : {1}", "ServiceHealthStates", OutputFormatter.ToString(applicationHealth.ServiceHealthStates.ToList())));
            strBuilder.Append(Environment.NewLine);
            strBuilder.Append(string.Format(CultureInfo.CurrentCulture, "{0} : {1}", "AggregatedHealthState", applicationHealth.AggregatedHealthState));
            strBuilder.Append(Environment.NewLine);
            strBuilder.Append(string.Format(CultureInfo.CurrentCulture, "{0} : {1}", "DeployedApplicationHealthStates", OutputFormatter.ToString(applicationHealth.DeployedApplicationHealthStates.ToList())));
            strBuilder.Append(Environment.NewLine);
            strBuilder.Append(string.Format(CultureInfo.CurrentCulture, "{0} : {1}", "HealthEvents", OutputFormatter.ToString(applicationHealth.HealthEvents.ToList())));
            strBuilder.Append(Environment.NewLine);
            strBuilder.Append(string.Format(CultureInfo.CurrentCulture, "{0} : {1}", "HealthStatistics", OutputFormatter.ToString(applicationHealth.HealthStatistics)));
            strBuilder.Append(Environment.NewLine);

            return strBuilder.ToString();
        }

        /// <summary>
        /// Overloaded ToString function for formatting the output on the console.
        /// </summary>
        /// <param name="applicationHealthState"> List of ApplicationHealthState </param>
        /// <returns>
        /// Returns formatted string.
        /// </returns>
        public static string ToString(IList<ApplicationHealthState> applicationHealthState)
        {
            if (applicationHealthState.Count == 0)
            {
                return "None";
            }

            var strBuilder = new StringBuilder();
            foreach (var item in applicationHealthState)
            {
                strBuilder.Append(Environment.NewLine);
                strBuilder.Append(ToString(item));
            }

            return strBuilder.ToString();
        }

        /// <summary>
        /// Overloaded ToString function for formatting the output on the console.
        /// </summary>
        /// <param name="applicationHealthState"> Object of type ApplicationHealthState </param>
        /// <returns>
        /// Returns formatted string.
        /// </returns>
        public static string ToString(ApplicationHealthState applicationHealthState)
        {
            var strBuilder = new StringBuilder();

            strBuilder.Append(string.Format(CultureInfo.CurrentCulture, "{0, -21} : {1}", "ApplicationName", applicationHealthState.Name));
            strBuilder.Append(Environment.NewLine);
            strBuilder.Append(string.Format(CultureInfo.CurrentCulture, "{0, -21} : {1}", "AggregatedHealthState", applicationHealthState.AggregatedHealthState));
            strBuilder.Append(Environment.NewLine);

            return strBuilder.ToString();
        }

        /// <summary>
        /// Overloaded ToString function for formatting the output on the console.
        /// </summary>
        /// <param name="applicationInfo"> Object of type Common.ApplicationInfo </param>
        /// <returns>
        /// Returns formatted string.
        /// </returns>
        public static string ToString(Common.ApplicationInfo applicationInfo)
        {
            var strBuilder = new StringBuilder();

            strBuilder.Append(string.Format(CultureInfo.CurrentCulture, "{0} : {1}", "ApplicationId", applicationInfo.Id));
            strBuilder.Append(Environment.NewLine);
            strBuilder.Append(string.Format(CultureInfo.CurrentCulture, "{0} : {1}", "ApplicationName", applicationInfo.Name));
            strBuilder.Append(Environment.NewLine);
            strBuilder.Append(string.Format(CultureInfo.CurrentCulture, "{0} : {1}", "ApplicationTypeName", applicationInfo.TypeName));
            strBuilder.Append(Environment.NewLine);
            strBuilder.Append(string.Format(CultureInfo.CurrentCulture, "{0} : {1}", "ApplicationTypeVersion", applicationInfo.TypeVersion));
            strBuilder.Append(Environment.NewLine);
            strBuilder.Append(string.Format(CultureInfo.CurrentCulture, "{0} : {1}", "ApplicationStatus", applicationInfo.Status));
            strBuilder.Append(Environment.NewLine);
            strBuilder.Append(string.Format(CultureInfo.CurrentCulture, "{0} : {1}", "ApplicationParameterList", applicationInfo.Parameters));
            strBuilder.Append(Environment.NewLine);
            strBuilder.Append(string.Format(CultureInfo.CurrentCulture, "{0} : {1}", "ApplicationDefinitionKind", applicationInfo.ApplicationDefinitionKind));
            strBuilder.Append(Environment.NewLine);
            strBuilder.Append(string.Format(CultureInfo.CurrentCulture, "{0} : {1}", "ManagedApplicationIdentityDescription", applicationInfo.ManagedApplicationIdentity));
            strBuilder.Append(Environment.NewLine);
            strBuilder.Append(string.Format(CultureInfo.CurrentCulture, "{0} : {1}", "ApplicationMetadata", applicationInfo.ApplicationMetadata));
            strBuilder.Append(Environment.NewLine);

            return strBuilder.ToString();
        }

        /// <summary>
        /// Overloaded ToString function for formatting the output on the console.
        /// </summary>
        /// <param name="managedKeyVaultReferenceParameter"> List of ManagedKeyVaultReferenceParameter </param>
        /// <returns>
        /// Returns formatted string.
        /// </returns>
        public static string ToString(IList<ManagedKeyVaultReferenceParameter> managedKeyVaultReferenceParameter)
        {
            if (managedKeyVaultReferenceParameter.Count == 0)
            {
                return "None";
            }

            var strBuilder = new StringBuilder();
            foreach (var item in managedKeyVaultReferenceParameter)
            {
                strBuilder.Append(Environment.NewLine);
                strBuilder.Append(ToString(item));
            }

            return strBuilder.ToString();
        }

        /// <summary>
        /// Overloaded ToString function for formatting the output on the console.
        /// </summary>
        /// <param name="managedKeyVaultReferenceParameter"> Object of type ManagedKeyVaultReferenceParameter </param>
        /// <returns>
        /// Returns formatted string.
        /// </returns>
        public static string ToString(ManagedKeyVaultReferenceParameter managedKeyVaultReferenceParameter)
        {
            var strBuilder = new StringBuilder();

            strBuilder.Append(string.Format(CultureInfo.CurrentCulture, "{0, -21} : {1}", "ParameterName", managedKeyVaultReferenceParameter.ParameterName));
            strBuilder.Append(Environment.NewLine);
            strBuilder.Append(string.Format(CultureInfo.CurrentCulture, "{0, -21} : \n{1}", "ApplicationIdentityReference", managedKeyVaultReferenceParameter.ApplicationIdentityReference));
            strBuilder.Append(Environment.NewLine);

            return strBuilder.ToString();
        }

        /// <summary>
        /// Overloaded ToString function for formatting the output on the console.
        /// </summary>
        /// <param name="applicationTypeInfo"> Object of type ApplicationTypeInfo </param>
        /// <returns>
        /// Returns formatted string.
        /// </returns>
        public static string ToString(ApplicationTypeInfo applicationTypeInfo)
        {
            var strBuilder = new StringBuilder();

            strBuilder.Append(string.Format(CultureInfo.CurrentCulture, "{0} : {1}", "ApplicationTypeName", applicationTypeInfo.Name));
            strBuilder.Append(Environment.NewLine);
            strBuilder.Append(string.Format(CultureInfo.CurrentCulture, "{0} : {1}", "ApplicationTypeVersion", applicationTypeInfo.Version));
            strBuilder.Append(Environment.NewLine);
            strBuilder.Append(string.Format(CultureInfo.CurrentCulture, "{0} : {1}", "ApplicationTypeParameterList", applicationTypeInfo.DefaultParameterList));
            strBuilder.Append(Environment.NewLine);
            strBuilder.Append(string.Format(CultureInfo.CurrentCulture, "{0} : {1}", "ApplicationTypeStatus", applicationTypeInfo.Status));
            strBuilder.Append(Environment.NewLine);
            strBuilder.Append(string.Format(CultureInfo.CurrentCulture, "{0} : {1}", "ApplicationTypeDefinitionKind", applicationTypeInfo.ApplicationTypeDefinitionKind));
            strBuilder.Append(Environment.NewLine);
            strBuilder.Append(string.Format(CultureInfo.CurrentCulture, "{0} : {1}", "ApplicationTypeMetadata", applicationTypeInfo.ApplicationTypeMetadata));
            strBuilder.Append(Environment.NewLine);
            strBuilder.Append(string.Format(CultureInfo.CurrentCulture, "{0} : {1}", "ManagedKeyVaultReferenceParameters", OutputFormatter.ToString(applicationTypeInfo.ManagedKeyVaultReferenceParameters.ToList())));
            strBuilder.Append(Environment.NewLine);

            return strBuilder.ToString();
        }

        /// <summary>
        /// Overloaded ToString function for formatting the output on the console.
        /// </summary>
        /// <param name="nodeHealthState"> List of NodeHealthState </param>
        /// <returns>
        /// Returns formatted string.
        /// </returns>
        public static string ToString(IList<NodeHealthState> nodeHealthState)
        {
            if (nodeHealthState.Count == 0)
            {
                return "None";
            }

            var strBuilder = new StringBuilder();
            foreach (var item in nodeHealthState)
            {
                strBuilder.Append(Environment.NewLine);
                strBuilder.Append(ToString(item));
            }

            return strBuilder.ToString();
        }

        /// <summary>
        /// Overloaded ToString function for formatting the output on the console.
        /// </summary>
        /// <param name="nodeHealthState"> Object of type NodeHealthState </param>
        /// <returns>
        /// Returns formatted string.
        /// </returns>
        public static string ToString(NodeHealthState nodeHealthState)
        {
            var strBuilder = new StringBuilder();

            strBuilder.Append(string.Format(CultureInfo.CurrentCulture, "{0, -21} : {1}", "NodeName", nodeHealthState.Name));
            strBuilder.Append(Environment.NewLine);
            strBuilder.Append(string.Format(CultureInfo.CurrentCulture, "{0, -21} : {1}", "AggregatedHealthState", nodeHealthState.AggregatedHealthState));
            strBuilder.Append(Environment.NewLine);

            return strBuilder.ToString();
        }

        /// <summary>
        /// Overloaded ToString function for formatting the output on the console.
        /// </summary>
        /// <param name="clusterHealth"> Object of type ClusterHealth </param>
        /// <returns>
        /// Returns formatted string.
        /// </returns>
        public static string ToString(ClusterHealth clusterHealth)
        {
            var strBuilder = new StringBuilder();

            strBuilder.Append(string.Format(CultureInfo.CurrentCulture, "{0} : {1}", "AggregatedHealthState", clusterHealth.AggregatedHealthState));
            strBuilder.Append(Environment.NewLine);
            strBuilder.Append(string.Format(CultureInfo.CurrentCulture, "{0} : {1}", "NodeHealthStates", OutputFormatter.ToString(clusterHealth.NodeHealthStates.ToList())));
            strBuilder.Append(Environment.NewLine);
            strBuilder.Append(string.Format(CultureInfo.CurrentCulture, "{0} : {1}", "ApplicationHealthStates", OutputFormatter.ToString(clusterHealth.ApplicationHealthStates.ToList())));
            strBuilder.Append(Environment.NewLine);
            strBuilder.Append(string.Format(CultureInfo.CurrentCulture, "{0} : {1}", "HealthEvents", OutputFormatter.ToString(clusterHealth.HealthEvents.ToList())));
            strBuilder.Append(Environment.NewLine);
            strBuilder.Append(string.Format(CultureInfo.CurrentCulture, "{0} : {1}", "HealthStatistics", OutputFormatter.ToString(clusterHealth.HealthStatistics)));
            strBuilder.Append(Environment.NewLine);

            return strBuilder.ToString();
        }

        /// <summary>
        /// Overloaded ToString function for formatting the output on the console.
        /// </summary>
        /// <param name="healthEvent"> List of HealthEvent </param>
        /// <returns>
        /// Returns formatted string.
        /// </returns>
        public static string ToString(IList<HealthEvent> healthEvent)
        {
            if (healthEvent.Count == 0)
            {
                return "None";
            }

            var strBuilder = new StringBuilder();
            foreach (var item in healthEvent)
            {
                strBuilder.Append(Environment.NewLine);
                strBuilder.Append(ToString(item));
            }

            return strBuilder.ToString();
        }

        /// <summary>
        /// Overloaded ToString function for formatting the output on the console.
        /// </summary>
        /// <param name="healthEvent"> Object of type HealthEvent </param>
        /// <returns>
        /// Returns formatted string.
        /// </returns>
        public static string ToString(HealthEvent healthEvent)
        {
            var strBuilder = new StringBuilder();

            strBuilder.Append(string.Format(CultureInfo.CurrentCulture, "{0, -25} : {1}", "SourceId", healthEvent.SourceId));
            strBuilder.Append(Environment.NewLine);
            strBuilder.Append(string.Format(CultureInfo.CurrentCulture, "{0, -25} : {1}", "Property", healthEvent.Property));
            strBuilder.Append(Environment.NewLine);
            strBuilder.Append(string.Format(CultureInfo.CurrentCulture, "{0, -25} : {1}", "HealthState", healthEvent.HealthState));
            strBuilder.Append(Environment.NewLine);
            strBuilder.Append(string.Format(CultureInfo.CurrentCulture, "{0, -25} : {1}", "TTL", healthEvent.TimeToLiveInMilliSeconds));
            strBuilder.Append(Environment.NewLine);
            strBuilder.Append(string.Format(CultureInfo.CurrentCulture, "{0, -25} : {1}", "Description", healthEvent.Description));
            strBuilder.Append(Environment.NewLine);
            strBuilder.Append(string.Format(CultureInfo.CurrentCulture, "{0, -25} : {1}", "SequenceNumber", healthEvent.SequenceNumber));
            strBuilder.Append(Environment.NewLine);
            strBuilder.Append(string.Format(CultureInfo.CurrentCulture, "{0, -25} : {1}", "SentAt", healthEvent.SourceUtcTimestamp));
            strBuilder.Append(Environment.NewLine);
            strBuilder.Append(string.Format(CultureInfo.CurrentCulture, "{0, -25} : {1}", "RemoveWhenExpired", healthEvent.RemoveWhenExpired));
            strBuilder.Append(Environment.NewLine);
            strBuilder.Append(string.Format(CultureInfo.CurrentCulture, "{0, -25} : {1}", "IsExpired", healthEvent.IsExpired));
            strBuilder.Append(Environment.NewLine);
            strBuilder.Append(string.Format(CultureInfo.CurrentCulture, "{0, -25} : {1}", "LastOkTransitionAt", healthEvent.LastOkTransitionAt));
            strBuilder.Append(Environment.NewLine);
            strBuilder.Append(string.Format(CultureInfo.CurrentCulture, "{0, -25} : {1}", "LastWarningTransitionAt", healthEvent.LastWarningTransitionAt));
            strBuilder.Append(Environment.NewLine);
            strBuilder.Append(string.Format(CultureInfo.CurrentCulture, "{0, -25} : {1}", "LastErrorTransitionAt", healthEvent.LastErrorTransitionAt));
            strBuilder.Append(Environment.NewLine);
            strBuilder.Append(string.Format(CultureInfo.CurrentCulture, "{0, -25} : {1}", "HealthReportId", healthEvent.HealthReportId));
            strBuilder.Append(Environment.NewLine);

            return strBuilder.ToString();
        }

        /// <summary>
        /// Overloaded ToString function for formatting the output on the console.
        /// </summary>
        /// <param name="healthStateCount"> Object of type HealthStateCount </param>
        /// <returns>
        /// Returns formatted string.
        /// </returns>
        public static string ToString(HealthStateCount healthStateCount)
        {
            var strBuilder = new StringBuilder();

            strBuilder.Append(string.Format(CultureInfo.CurrentCulture, "{0, -21} : {1}", "OkCount", healthStateCount.OkCount));
            strBuilder.Append(Environment.NewLine);
            strBuilder.Append(string.Format(CultureInfo.CurrentCulture, "{0, -21} : {1}", "WarningCount", healthStateCount.WarningCount));
            strBuilder.Append(Environment.NewLine);
            strBuilder.Append(string.Format(CultureInfo.CurrentCulture, "{0, -21} : {1}", "ErrorCount", healthStateCount.ErrorCount));
            strBuilder.Append(Environment.NewLine);

            return strBuilder.ToString();
        }

        /// <summary>
        /// Overloaded ToString function for formatting the output on the console.
        /// </summary>
        /// <param name="entityKindHealthStateCount"> List of EntityKindHealthStateCount </param>
        /// <returns>
        /// Returns formatted string.
        /// </returns>
        public static string ToString(IList<EntityKindHealthStateCount> entityKindHealthStateCount)
        {
            if (entityKindHealthStateCount.Count == 0)
            {
                return "None";
            }

            var strBuilder = new StringBuilder();
            foreach (var item in entityKindHealthStateCount)
            {
                strBuilder.Append(Environment.NewLine);
                strBuilder.Append(ToString(item));
            }

            return strBuilder.ToString();
        }

        /// <summary>
        /// Overloaded ToString function for formatting the output on the console.
        /// </summary>
        /// <param name="entityKindHealthStateCount"> Object of type EntityKindHealthStateCount </param>
        /// <returns>
        /// Returns formatted string.
        /// </returns>
        public static string ToString(EntityKindHealthStateCount entityKindHealthStateCount)
        {
            var strBuilder = new StringBuilder();

            strBuilder.Append(string.Format(CultureInfo.CurrentCulture, "{0, -21} : {1}", "EntityKind", entityKindHealthStateCount.EntityKind));
            strBuilder.Append(Environment.NewLine);
            strBuilder.Append(string.Format(CultureInfo.CurrentCulture, "{0, -21} : \n{1}", "HealthStateCount", OutputFormatter.ToString(entityKindHealthStateCount.HealthStateCount)));
            strBuilder.Append(Environment.NewLine);

            return strBuilder.ToString();
        }

        /// <summary>
        /// Overloaded ToString function for formatting the output on the console.
        /// </summary>
        /// <param name="healthStatistics"> Object of type HealthStatistics </param>
        /// <returns>
        /// Returns formatted string.
        /// </returns>
        public static string ToString(HealthStatistics healthStatistics)
        {
            var strBuilder = new StringBuilder();

            strBuilder.Append(string.Format(CultureInfo.CurrentCulture, "{0, -21} : {1}", "HealthStateCountList", OutputFormatter.ToString(healthStatistics.HealthStateCountList.ToList())));
            strBuilder.Append(Environment.NewLine);

            return strBuilder.ToString();
        }

        /// <summary>
        /// Overloaded ToString function for formatting the output on the console.
        /// </summary>
        /// <param name="partitionInformation"> Object of type PartitionInformation </param>
        /// <returns>
        /// Returns formatted string.
        /// </returns>
        public static string ToString(PartitionInformation partitionInformation)
        {
            var strBuilder = new StringBuilder();

            strBuilder.Append(string.Format(CultureInfo.CurrentCulture, "{0} : {1}", "PartitionId", partitionInformation.Id.ToString()));
            strBuilder.Append(Environment.NewLine);
            strBuilder.Append(string.Format(CultureInfo.CurrentCulture, "{0} : {1}", "PartitionKind", partitionInformation.ServicePartitionKind));
            strBuilder.Append(Environment.NewLine);

            return strBuilder.ToString();
        }

        /// <summary>
        /// Overloaded ToString function for formatting the output on the console.
        /// </summary>
        /// <param name="servicePartitionInfo"> Object of type ServicePartitionInfo </param>
        /// <returns>
        /// Returns formatted string.
        /// </returns>
        public static string ToString(ServicePartitionInfo servicePartitionInfo)
        {
            var strBuilder = new StringBuilder();

            strBuilder.Append(string.Format(CultureInfo.CurrentCulture, "{0} : {1}", "PartitionInformation", servicePartitionInfo.PartitionInformation));
            strBuilder.Append(Environment.NewLine);

            return strBuilder.ToString();
        }

        /// <summary>
        /// Overloaded ToString function for formatting the output on the console.
        /// </summary>
        /// <param name="serviceInfo"> Object of type ServiceInfo </param>
        /// <returns>
        /// Returns formatted string.
        /// </returns>
        public static string ToString(ServiceInfo serviceInfo)
        {
            var strBuilder = new StringBuilder();

            strBuilder.Append(string.Format(CultureInfo.CurrentCulture, "{0} : {1}", "ServiceId", serviceInfo.Id));
            strBuilder.Append(Environment.NewLine);
            strBuilder.Append(string.Format(CultureInfo.CurrentCulture, "{0} : {1}", "ServiceKind", serviceInfo.ServiceKind));
            strBuilder.Append(Environment.NewLine);
            strBuilder.Append(string.Format(CultureInfo.CurrentCulture, "{0} : {1}", "ServiceName", serviceInfo.Name));
            strBuilder.Append(Environment.NewLine);
            strBuilder.Append(string.Format(CultureInfo.CurrentCulture, "{0} : {1}", "ServiceTypeName", serviceInfo.TypeName));
            strBuilder.Append(Environment.NewLine);
            strBuilder.Append(string.Format(CultureInfo.CurrentCulture, "{0} : {1}", "ManifestVersion", serviceInfo.ManifestVersion));
            strBuilder.Append(Environment.NewLine);
            strBuilder.Append(string.Format(CultureInfo.CurrentCulture, "{0} : {1}", "HealthState", serviceInfo.HealthState));
            strBuilder.Append(Environment.NewLine);
            strBuilder.Append(string.Format(CultureInfo.CurrentCulture, "{0} : {1}", "ServiceStatus", serviceInfo.ServiceStatus));
            strBuilder.Append(Environment.NewLine);
            strBuilder.Append(string.Format(CultureInfo.CurrentCulture, "{0} : {1}", "IsServiceGroup", serviceInfo.IsServiceGroup));
            strBuilder.Append(Environment.NewLine);
            strBuilder.Append(string.Format(CultureInfo.CurrentCulture, "{0} : {1}", "ServiceMetadata", serviceInfo.ServiceMetadata));
            strBuilder.Append(Environment.NewLine);

            return strBuilder.ToString();
        }

        /// <summary>
        /// Overloaded ToString function for formatting the output on the console.
        /// </summary>
        /// <param name="partitionHealth"> Object of type PartitionHealth </param>
        /// <returns>
        /// Returns formatted string.
        /// </returns>
        public static string ToString(PartitionHealth partitionHealth)
        {
            var strBuilder = new StringBuilder();

            strBuilder.Append(string.Format(CultureInfo.CurrentCulture, "{0} : {1}", "PartitionId", partitionHealth.PartitionId));
            strBuilder.Append(Environment.NewLine);
            strBuilder.Append(string.Format(CultureInfo.CurrentCulture, "{0} : {1}", "AggregatedHealthState", partitionHealth.AggregatedHealthState));
            strBuilder.Append(Environment.NewLine);
            strBuilder.Append(string.Format(CultureInfo.CurrentCulture, "{0} : {1}", "HealthEvents", OutputFormatter.ToString(partitionHealth.HealthEvents.ToList())));
            strBuilder.Append(Environment.NewLine);
            strBuilder.Append(string.Format(CultureInfo.CurrentCulture, "{0} : {1}", "HealthStatistics", OutputFormatter.ToString(partitionHealth.HealthStatistics)));
            strBuilder.Append(Environment.NewLine);

            return strBuilder.ToString();
        }

        /// <summary>
        /// Overloaded ToString function for formatting the output on the console.
        /// </summary>
        /// <param name="partitionHealthState"> List of PartitionHealthState </param>
        /// <returns>
        /// Returns formatted string.
        /// </returns>
        public static string ToString(IList<PartitionHealthState> partitionHealthState)
        {
            if (partitionHealthState.Count == 0)
            {
                return "None";
            }

            var strBuilder = new StringBuilder();
            foreach (var item in partitionHealthState)
            {
                strBuilder.Append(Environment.NewLine);
                strBuilder.Append(ToString(item));
            }

            return strBuilder.ToString();
        }

        /// <summary>
        /// Overloaded ToString function for formatting the output on the console.
        /// </summary>
        /// <param name="partitionHealthState"> Object of type PartitionHealthState </param>
        /// <returns>
        /// Returns formatted string.
        /// </returns>
        public static string ToString(PartitionHealthState partitionHealthState)
        {
            var strBuilder = new StringBuilder();

            strBuilder.Append(string.Format(CultureInfo.CurrentCulture, "{0, -21} : {1}", "PartitionId", partitionHealthState.PartitionId));
            strBuilder.Append(Environment.NewLine);
            strBuilder.Append(string.Format(CultureInfo.CurrentCulture, "{0, -21} : {1}", "AggregatedHealthState", partitionHealthState.AggregatedHealthState));
            strBuilder.Append(Environment.NewLine);

            return strBuilder.ToString();
        }

        /// <summary>
        /// Overloaded ToString function for formatting the output on the console.
        /// </summary>
        /// <param name="serviceHealth"> Object of type ServiceHealth </param>
        /// <returns>
        /// Returns formatted string.
        /// </returns>
        public static string ToString(ServiceHealth serviceHealth)
        {
            var strBuilder = new StringBuilder();

            strBuilder.Append(string.Format(CultureInfo.CurrentCulture, "{0} : {1}", "ServiceName", serviceHealth.Name));
            strBuilder.Append(Environment.NewLine);
            strBuilder.Append(string.Format(CultureInfo.CurrentCulture, "{0} : {1}", "PartitionHealthStates", OutputFormatter.ToString(serviceHealth.PartitionHealthStates.ToList())));
            strBuilder.Append(Environment.NewLine);
            strBuilder.Append(string.Format(CultureInfo.CurrentCulture, "{0} : {1}", "AggregatedHealthState", serviceHealth.AggregatedHealthState));
            strBuilder.Append(Environment.NewLine);
            strBuilder.Append(string.Format(CultureInfo.CurrentCulture, "{0} : {1}", "HealthEvents", OutputFormatter.ToString(serviceHealth.HealthEvents.ToList())));
            strBuilder.Append(Environment.NewLine);
            strBuilder.Append(string.Format(CultureInfo.CurrentCulture, "{0} : {1}", "HealthStatistics", OutputFormatter.ToString(serviceHealth.HealthStatistics)));
            strBuilder.Append(Environment.NewLine);

            return strBuilder.ToString();
        }

        /// <summary>
        /// Overloaded ToString function for formatting the output on the console.
        /// </summary>
        /// <param name="serviceNameInfo"> Object of type ServiceNameInfo </param>
        /// <returns>
        /// Returns formatted string.
        /// </returns>
        public static string ToString(ServiceNameInfo serviceNameInfo)
        {
            var strBuilder = new StringBuilder();

            strBuilder.Append(string.Format(CultureInfo.CurrentCulture, "{0} : {1}", "ServiceId", serviceNameInfo.Id));
            strBuilder.Append(Environment.NewLine);
            strBuilder.Append(string.Format(CultureInfo.CurrentCulture, "{0} : {1}", "ServiceName", serviceNameInfo.Name));
            strBuilder.Append(Environment.NewLine);

            return strBuilder.ToString();
        }

        /// <summary>
        /// Overloaded ToString function for formatting the output on the console.
        /// </summary>
        /// <param name="secretResourceDescription"> Object of type SecretResourceDescription </param>
        /// <returns>
        /// Returns formatted string.
        /// </returns>
        public static string ToString(SecretResourceDescription secretResourceDescription)
        {
            var strBuilder = new StringBuilder();

            strBuilder.Append(string.Format(CultureInfo.CurrentCulture, "{0} : {1}", "Name", secretResourceDescription.Name));
            strBuilder.Append(Environment.NewLine);
            strBuilder.Append(string.Format(CultureInfo.CurrentCulture, "{0} : {1}", "Description", secretResourceDescription.Properties.Description));
            strBuilder.Append(Environment.NewLine);
            strBuilder.Append(string.Format(CultureInfo.CurrentCulture, "{0} : {1}", "Status", secretResourceDescription.Properties.Status));
            strBuilder.Append(Environment.NewLine);
            strBuilder.Append(string.Format(CultureInfo.CurrentCulture, "{0} : {1}", "ContentType", secretResourceDescription.Properties.ContentType));
            strBuilder.Append(Environment.NewLine);

            return strBuilder.ToString();
        }

        /// <summary>
        /// Overloaded ToString function for formatting the output on the console.
        /// </summary>
        /// <param name="volumeResourceDescription"> Object of type VolumeResourceDescription </param>
        /// <returns>
        /// Returns formatted string.
        /// </returns>
        public static string ToString(VolumeResourceDescription volumeResourceDescription)
        {
            var strBuilder = new StringBuilder();

            strBuilder.Append(string.Format(CultureInfo.CurrentCulture, "{0} : {1}", "Name", volumeResourceDescription.Name));
            strBuilder.Append(Environment.NewLine);
            strBuilder.Append(string.Format(CultureInfo.CurrentCulture, "{0} : {1}", "Description", volumeResourceDescription.Properties.Description));
            strBuilder.Append(Environment.NewLine);
            strBuilder.Append(string.Format(CultureInfo.CurrentCulture, "{0} : {1}", "Status", volumeResourceDescription.Properties.Status));
            strBuilder.Append(Environment.NewLine);
            strBuilder.Append(string.Format(CultureInfo.CurrentCulture, "{0} : {1}", "StatusDetails", volumeResourceDescription.Properties.StatusDetails));
            strBuilder.Append(Environment.NewLine);

            return strBuilder.ToString();
        }

        /// <summary>
        /// Overloaded ToString function for formatting the output on the console.
        /// </summary>
        /// <param name="networkResourceDescription"> Object of type NetworkResourceDescription </param>
        /// <returns>
        /// Returns formatted string.
        /// </returns>
        public static string ToString(NetworkResourceDescription networkResourceDescription)
        {
            var strBuilder = new StringBuilder();

            strBuilder.Append(string.Format(CultureInfo.CurrentCulture, "{0} : {1}", "Name", networkResourceDescription.Name));
            strBuilder.Append(Environment.NewLine);
            strBuilder.Append(string.Format(CultureInfo.CurrentCulture, "{0} : {1}", "Description", networkResourceDescription.Properties.Description));
            strBuilder.Append(Environment.NewLine);
            strBuilder.Append(string.Format(CultureInfo.CurrentCulture, "{0} : {1}", "Status", networkResourceDescription.Properties.Status));
            strBuilder.Append(Environment.NewLine);
            strBuilder.Append(string.Format(CultureInfo.CurrentCulture, "{0} : {1}", "StatusDetails", networkResourceDescription.Properties.StatusDetails));
            strBuilder.Append(Environment.NewLine);
            strBuilder.Append(string.Format(CultureInfo.CurrentCulture, "{0} : {1}", "King", networkResourceDescription.Properties.Kind));
            strBuilder.Append(Environment.NewLine);

            return strBuilder.ToString();
        }

        /// <summary>
        /// Overloaded ToString function for formatting the output on the console.
        /// </summary>
        /// <param name="gatewayResourceDescription"> Object of type GatewayResourceDescription </param>
        /// <returns>
        /// Returns formatted string.
        /// </returns>
        public static string ToString(GatewayResourceDescription gatewayResourceDescription)
        {
            var strBuilder = new StringBuilder();

            strBuilder.Append(string.Format(CultureInfo.CurrentCulture, "{0} : {1}", "Name", gatewayResourceDescription.Name));
            strBuilder.Append(Environment.NewLine);
            strBuilder.Append(string.Format(CultureInfo.CurrentCulture, "{0} : {1}", "Description", gatewayResourceDescription.Properties.Description));
            strBuilder.Append(Environment.NewLine);
            strBuilder.Append(string.Format(CultureInfo.CurrentCulture, "{0} : {1}", "Status", gatewayResourceDescription.Properties.Status));
            strBuilder.Append(Environment.NewLine);
            strBuilder.Append(string.Format(CultureInfo.CurrentCulture, "{0} : {1}", "StatusDetails", gatewayResourceDescription.Properties.StatusDetails));
            strBuilder.Append(Environment.NewLine);

            return strBuilder.ToString();
        }

        /// <summary>
        /// Overloaded ToString function for formatting the output on the console.
        /// </summary>
        /// <param name="serviceResourceDescription"> Object of type ServiceResourceDescription </param>
        /// <returns>
        /// Returns formatted string.
        /// </returns>
        public static string ToString(ServiceResourceDescription serviceResourceDescription)
        {
            var strBuilder = new StringBuilder();

            strBuilder.Append(string.Format(CultureInfo.CurrentCulture, "{0} : {1}", "Name", serviceResourceDescription.Name));
            strBuilder.Append(Environment.NewLine);
            strBuilder.Append(string.Format(CultureInfo.CurrentCulture, "{0} : {1}", "Description", serviceResourceDescription.Properties.Description));
            strBuilder.Append(Environment.NewLine);
            strBuilder.Append(string.Format(CultureInfo.CurrentCulture, "{0} : {1}", "Status", serviceResourceDescription.Properties.Status));
            strBuilder.Append(Environment.NewLine);
            strBuilder.Append(string.Format(CultureInfo.CurrentCulture, "{0} : {1}", "HealthState", serviceResourceDescription.Properties.HealthState));
            strBuilder.Append(Environment.NewLine);
            strBuilder.Append(string.Format(CultureInfo.CurrentCulture, "{0} : {1}", "StatusDetails", serviceResourceDescription.Properties.StatusDetails));
            strBuilder.Append(Environment.NewLine);
            strBuilder.Append(string.Format(CultureInfo.CurrentCulture, "{0} : {1}", "ReplicaCount", serviceResourceDescription.Properties.ReplicaCount));
            strBuilder.Append(Environment.NewLine);

            return strBuilder.ToString();
        }

        /// <summary>
        /// Overloaded ToString function for formatting the output on the console.
        /// </summary>
        /// <param name="serviceReplicaDescription"> Object of type ServiceReplicaDescription </param>
        /// <returns>
        /// Returns formatted string.
        /// </returns>
        public static string ToString(ServiceReplicaDescription serviceReplicaDescription)
        {
            var strBuilder = new StringBuilder();

            strBuilder.Append(string.Format(CultureInfo.CurrentCulture, "{0} : {1}", "ReplicaName", serviceReplicaDescription.ReplicaName));
            strBuilder.Append(Environment.NewLine);
            strBuilder.Append(string.Format(CultureInfo.CurrentCulture, "{0} : {1}", "OperatingSystem", serviceReplicaDescription.OsType));
            strBuilder.Append(Environment.NewLine);

            return strBuilder.ToString();
        }

        /// <summary>
        /// Overloaded ToString function for formatting the output on the console.
        /// </summary>
        /// <param name="applicationResourceDescription"> Object of type ApplicationResourceDescription </param>
        /// <returns>
        /// Returns formatted string.
        /// </returns>
        public static string ToString(ApplicationResourceDescription applicationResourceDescription)
        {
            var strBuilder = new StringBuilder();

            strBuilder.Append(string.Format(CultureInfo.CurrentCulture, "{0} : {1}", "Name", applicationResourceDescription.Name));
            strBuilder.Append(Environment.NewLine);
            strBuilder.Append(string.Format(CultureInfo.CurrentCulture, "{0} : {1}", "Description", applicationResourceDescription.Properties.Description));
            strBuilder.Append(Environment.NewLine);
            strBuilder.Append(string.Format(CultureInfo.CurrentCulture, "{0} : {1}", "Status", applicationResourceDescription.Properties.Status));
            strBuilder.Append(Environment.NewLine);
            strBuilder.Append(string.Format(CultureInfo.CurrentCulture, "{0} : {1}", "HealthState", applicationResourceDescription.Properties.HealthState));
            strBuilder.Append(Environment.NewLine);
            strBuilder.Append(string.Format(CultureInfo.CurrentCulture, "{0} : {1}", "StatusDetails", applicationResourceDescription.Properties.StatusDetails));
            strBuilder.Append(Environment.NewLine);

            return strBuilder.ToString();
        }

        private static bool HasOverloadForArgument(Type targetType, string functionName, object arg)
        {
            var methodInfo = targetType.GetMethod(functionName, new Type[] { arg.GetType() });
            return methodInfo != null;
        }

        private static readonly Dictionary<Type, Delegate> TypeToMethod = new Dictionary<Type, Delegate>()
        {
            { typeof(List<ServiceHealthState>), new Func<IList<ServiceHealthState>, string>(ToString) },
            { typeof(ServiceHealthState), new Func<ServiceHealthState, string>(ToString) },
            { typeof(List<DeployedApplicationHealthState>), new Func<IList<DeployedApplicationHealthState>, string>(ToString) },
            { typeof(DeployedApplicationHealthState), new Func<DeployedApplicationHealthState, string>(ToString) },
            { typeof(ApplicationHealth), new Func<ApplicationHealth, string>(ToString) },
            { typeof(List<ApplicationHealthState>), new Func<IList<ApplicationHealthState>, string>(ToString) },
            { typeof(ApplicationHealthState), new Func<ApplicationHealthState, string>(ToString) },
            { typeof(Common.ApplicationInfo), new Func<Common.ApplicationInfo, string>(ToString) },
            { typeof(List<ManagedKeyVaultReferenceParameter>), new Func<IList<ManagedKeyVaultReferenceParameter>, string>(ToString) },
            { typeof(ManagedKeyVaultReferenceParameter), new Func<ManagedKeyVaultReferenceParameter, string>(ToString) },
            { typeof(ApplicationTypeInfo), new Func<ApplicationTypeInfo, string>(ToString) },
            { typeof(List<NodeHealthState>), new Func<IList<NodeHealthState>, string>(ToString) },
            { typeof(NodeHealthState), new Func<NodeHealthState, string>(ToString) },
            { typeof(ClusterHealth), new Func<ClusterHealth, string>(ToString) },
            { typeof(List<HealthEvent>), new Func<IList<HealthEvent>, string>(ToString) },
            { typeof(HealthEvent), new Func<HealthEvent, string>(ToString) },
            { typeof(HealthStateCount), new Func<HealthStateCount, string>(ToString) },
            { typeof(List<EntityKindHealthStateCount>), new Func<IList<EntityKindHealthStateCount>, string>(ToString) },
            { typeof(EntityKindHealthStateCount), new Func<EntityKindHealthStateCount, string>(ToString) },
            { typeof(HealthStatistics), new Func<HealthStatistics, string>(ToString) },
            { typeof(PartitionInformation), new Func<PartitionInformation, string>(ToString) },
            { typeof(ServicePartitionInfo), new Func<ServicePartitionInfo, string>(ToString) },
            { typeof(ServiceInfo), new Func<ServiceInfo, string>(ToString) },
            { typeof(PartitionHealth), new Func<PartitionHealth, string>(ToString) },
            { typeof(List<PartitionHealthState>), new Func<IList<PartitionHealthState>, string>(ToString) },
            { typeof(PartitionHealthState), new Func<PartitionHealthState, string>(ToString) },
            { typeof(ServiceHealth), new Func<ServiceHealth, string>(ToString) },
            { typeof(ServiceNameInfo), new Func<ServiceNameInfo, string>(ToString) },
            { typeof(SecretResourceDescription), new Func<SecretResourceDescription, string>(ToString) },
            { typeof(VolumeResourceDescription), new Func<VolumeResourceDescription, string>(ToString) },
            { typeof(NetworkResourceDescription), new Func<NetworkResourceDescription, string>(ToString) },
            { typeof(GatewayResourceDescription), new Func<GatewayResourceDescription, string>(ToString) },
            { typeof(ServiceResourceDescription), new Func<ServiceResourceDescription, string>(ToString) },
            { typeof(ServiceReplicaDescription), new Func<ServiceReplicaDescription, string>(ToString) },
            { typeof(ApplicationResourceDescription), new Func<ApplicationResourceDescription, string>(ToString) },
        };
    }
}
