// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Generator
{
    using System;
    using System.Fabric;
    using System.Globalization;
    using Microsoft.ServiceFabric.Actors.Runtime;

    /// <summary>
    /// Contains static methods for generating names like service name, application name form the actor interface type.
    /// </summary>
    public static class ActorNameFormat
    {
        private static volatile string applicationName;

        /// <summary>
        /// Gets name of Actor from actorInterfaceType.
        /// </summary>
        /// <param name="actorInterfaceType">Type of the actor interface.</param>
        /// <returns>Name of Actor.</returns>
        public static string GetName(Type actorInterfaceType)
        {
            return GetName(actorInterfaceType.Name);
        }

        /// <summary>
        /// Gets name of service which hosts the actor type in Service Fabric cluster.
        /// </summary>
        /// <param name="actorInterfaceType">Type of the actor interface.</param>
        /// <param name="serviceName">Name of service hosting the actor type. If this value is null then
        /// service name is constructed using the actorInterfaceType.</param>
        /// <returns>Service Fabric service name hosting the actor type.</returns>
        public static string GetFabricServiceName(Type actorInterfaceType, string serviceName = null)
        {
            return !string.IsNullOrEmpty(serviceName) ? serviceName : GetFabricServiceName(actorInterfaceType.Name);
        }

        /// <summary>
        /// Gets service Uri which hosts the actor type in Service Fabric cluster.
        /// </summary>
        /// <param name="actorInterfaceType">Type of the actor interface.</param>
        /// <param name="applicationName">Service Fabric application name containing the actor service.
        /// If this value is null application name is obtained from <see cref="System.Fabric.CodePackageActivationContext.ApplicationName"/>.</param>
        /// <param name="serviceName">Name of service hosting the actor type. If this value is null then
        /// service name is constructed using the actorInterfaceType.</param>
        /// <returns>Service Fabric service Uri hosting the actor type.</returns>
        /// <exception cref="System.ArgumentException">
        /// When applicationName cannot be determined using <see cref="System.Fabric.CodePackageActivationContext"/>.
        /// </exception>
        /// <remarks>If applicationName is passed as null or empty string, an attempt is made to get application name from
        /// <see cref="System.Fabric.CodePackageActivationContext"/>. If the method still cannot determine application name,
        /// <see cref="System.ArgumentException"/> is thrown. </remarks>
        public static Uri GetFabricServiceUri(
            Type actorInterfaceType,
            string applicationName = null,
            string serviceName = null)
        {
            if (string.IsNullOrEmpty(applicationName))
            {
                applicationName = GetCurrentFabricApplicationName();
                if (string.IsNullOrEmpty(applicationName))
                {
                    throw new ArgumentException(SR.ErrorApplicationName, "applicationName");
                }
            }

            if (!applicationName.StartsWith("fabric:/", StringComparison.OrdinalIgnoreCase))
            {
                return new Uri(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "fabric:/{0}/{1}",
                        applicationName.TrimEnd('/'),
                        GetFabricServiceName(actorInterfaceType, serviceName)));
            }
            else
            {
                return new Uri(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "{0}/{1}",
                        applicationName.TrimEnd('/'),
                        GetFabricServiceName(actorInterfaceType, serviceName)));
            }
        }

        /// <summary>
        /// Gets service Uri which hosts the actor type in Service Fabric cluster.
        /// </summary>
        /// <param name="actorInterfaceType">Type of the actor interface.</param>
        /// <returns>Service Fabric service Uri hosting the actor type.</returns>
        /// <exception cref="System.ArgumentException">
        /// When application name cannot be determined using <see cref="System.Fabric.CodePackageActivationContext"/>.
        /// </exception>
        /// <remarks>Method will try to get application name from <see cref="System.Fabric.CodePackageActivationContext"/>.
        /// If the method still cannot determine application name, <see cref="System.ArgumentException"/> is thrown. </remarks>
        public static Uri GetFabricServiceUri(Type actorInterfaceType)
        {
            return GetFabricServiceUri(actorInterfaceType, null);
        }

        /// <summary>
        /// Gets service Uri which hosts the actor type in Service Fabric cluster.
        /// </summary>
        /// <param name="actorInterfaceType">Type of the actor interface.</param>
        /// <param name="applicationUri">Service Fabric application Uri containing the actor service.
        /// If this value is null application name is obtained from <see cref="System.Fabric.CodePackageActivationContext.ApplicationName"/>.</param>
        /// <returns>Service Fabric service Uri hosting the actor type.</returns>
        /// <exception cref="System.ArgumentException">
        /// When application name cannot be determined using <see cref="System.Fabric.CodePackageActivationContext"/>.
        /// </exception>
        /// <remarks>Method will create service name using the actorInterfaceType. If applicationUri is passed as null, an attempt is made to get application name from
        /// <see cref="System.Fabric.CodePackageActivationContext"/>. If the method still cannot determine application name,
        /// <see cref="System.ArgumentException"/> is thrown. </remarks>
        public static Uri GetFabricServiceUri(Type actorInterfaceType, Uri applicationUri)
        {
            return GetFabricServiceUri(actorInterfaceType, applicationUri, null);
        }

        /// <summary>
        /// Gets service Uri which hosts the actor type in Service Fabric cluster.
        /// </summary>
        /// <param name="actorInterfaceType">Type of the actor interface.</param>
        /// <param name="applicationUri">Service Fabric application Uri containing the actor service.
        /// If this value is null application name is obtained from <see cref="System.Fabric.CodePackageActivationContext.ApplicationName"/>.</param>
        /// <param name="serviceName">Name of service hosting the actor type. If this value is null then
        /// service name is constructed using the actorInterfaceType.</param>
        /// <returns>Service Fabric service Uri hosting the actor type.</returns>
        /// <exception cref="System.ArgumentException">
        /// When application name cannot be determined using <see cref="System.Fabric.CodePackageActivationContext"/>.
        /// </exception>
        /// <remarks>If applicationUri is passed as null, an attempt is made to get application name from
        /// <see cref="System.Fabric.CodePackageActivationContext"/>. If the method still cannot determine application name,
        /// <see cref="System.ArgumentException"/> is thrown. </remarks>
        public static Uri GetFabricServiceUri(Type actorInterfaceType, Uri applicationUri, string serviceName)
        {
            if (applicationUri == null)
            {
                return GetFabricServiceUri(actorInterfaceType, (string)null, serviceName);
            }
            else
            {
                return GetFabricServiceUri(actorInterfaceType, applicationUri.ToString(), serviceName);
            }
        }

        /// <summary>
        /// Gets service type name for the actor.
        /// </summary>
        /// <param name="actorImplementationType">Actor implementation type.</param>
        /// <returns>Service type name.</returns>
        public static string GetFabricServiceTypeName(Type actorImplementationType)
        {
            return string.Format(CultureInfo.InvariantCulture, "{0}Type", GetActorServiceName(actorImplementationType));
        }

        /// <summary>
        /// Gets service package name which is used in Service Fabric Application package for the actor.
        /// </summary>
        /// <param name="servicePackageNamePrefix">Prefix to be used for the service package name.</param>
        /// <returns>Service package name.</returns>
        public static string GetFabricServicePackageName(string servicePackageNamePrefix)
        {
            if (string.IsNullOrEmpty(servicePackageNamePrefix))
            {
                servicePackageNamePrefix = "FabricActorService";
            }

            return string.Format(CultureInfo.InvariantCulture, "{0}Pkg", servicePackageNamePrefix);
        }

        /// <summary>
        /// Gets the service endpoint for the actor type which is specified in service manifest for the actor service.
        /// </summary>
        /// <param name="actorImplementationType">Type of class implementing the actor.</param>
        /// <returns>Service endpoint name.</returns>
        public static string GetFabricServiceEndpointName(Type actorImplementationType)
        {
            return string.Format(
                CultureInfo.InvariantCulture,
                "{0}Endpoint",
                GetActorServiceName(actorImplementationType));
        }

        /// <summary>
        /// Gets the service endpoint for the actor type which is specified in service manifest for the actor service.
        /// </summary>
        /// <param name="actorImplementationType">Type of class implementing the actor.</param>
        /// <returns>Service endpoint name.</returns>
        public static string GetFabricServiceV2EndpointName(Type actorImplementationType)
        {
            return string.Format(
                CultureInfo.InvariantCulture,
                "{0}EndpointV2",
                GetActorServiceName(actorImplementationType));
        }

        /// <summary>
        /// Gets the service endpoint for the actor type which is specified in service manifest for the actor service.
        /// </summary>
        /// <param name="actorImplementationType">Type of class implementing the actor.</param>
        /// <returns>Service endpoint name.</returns>
        public static string GetFabricServiceWrappedMessageEndpointName(Type actorImplementationType)
        {
            return string.Format(
                CultureInfo.InvariantCulture,
                "{0}EndpointV2_1",
                GetActorServiceName(actorImplementationType));
        }

        /// <summary>
        /// Gets the replicator endpoint which is specified in service manifest for the actor service.
        /// </summary>
        /// <param name="actorImplementationType">Type of class implementing the actor.</param>
        /// <returns>Service replicator endpoint name.</returns>
        public static string GetFabricServiceReplicatorEndpointName(Type actorImplementationType)
        {
            return string.Format(
                CultureInfo.InvariantCulture,
                "{0}ReplicatorEndpoint",
                GetActorServiceName(actorImplementationType));
        }

        /// <summary>
        /// Gets the Kvs Migration Service endpoint which is specified in service manifest for the actor service.
        /// </summary>
        /// <param name="actorImplementationType">Type of class implementing the actor.</param>
        /// <returns>Kvs Migration Service endpoint name.</returns>
        public static string GetActorKvsMigrationEndpointName(Type actorImplementationType)
        {
            return string.Format(
                CultureInfo.InvariantCulture,
                "{0}MigrationEndpoint",
                GetActorServiceName(actorImplementationType));
        }

        /// <summary>
        /// Gets the RC Migration Service endpoint which is specified in service manifest for the actor service.
        /// </summary>
        /// <param name="actorImplementationType">Type of class implementing the actor.</param>
        /// <returns>Kvs Migration Service endpoint name.</returns>
        public static string GetActorRcMigrationEndpointName(Type actorImplementationType)
        {
            return string.Format(
                CultureInfo.InvariantCulture,
                "{0}MigrationEndpoint",
                GetActorServiceName(actorImplementationType));
        }

        /// <summary>
        /// Gets the replicator configuration section name specified in configuration package for the actor service.
        /// </summary>
        /// <param name="actorImplementationType">Type of class implementing the actor.</param>
        /// <returns>Replicator configuration section name.</returns>
        /// <remarks>Values specified in replicator configuration section are used to configure <see cref="System.Fabric.ReplicatorSettings"/>
        /// for the replication of actor state between primary and secondary replicas.
        /// </remarks>
        public static string GetFabricServiceReplicatorConfigSectionName(Type actorImplementationType)
        {
            return string.Format(
                CultureInfo.InvariantCulture,
                "{0}ReplicatorConfig",
                GetActorServiceName(actorImplementationType));
        }

        /// <summary>
        /// Gets the migration configuration section name for KVS to RC migration which is specified in configuration package for the actor service.
        /// </summary>
        /// <param name="actorImplementationType">Type of class implementing the actor.</param>
        /// <returns>Migration configuration section name.</returns>
        public static string GetMigrationConfigSectionName(Type actorImplementationType)
        {
            return string.Format(
                CultureInfo.InvariantCulture,
                "{0}MigrationConfig",
                GetActorServiceName(actorImplementationType));
        }

        /// <summary>
        /// Gets the fabrictransport configuration section name specified in configuration package for the actor service.
        /// </summary>
        /// <param name="actorImplementationType">Type of class implementing the actor.</param>
        /// <returns>FabricTransport configuration section name.</returns>
        /// <remarks>Values specified in FabricTransport configuration section are used to configure <see cref="Microsoft.ServiceFabric.Services.Remoting.FabricTransport.Runtime.FabricTransportRemotingListenerSettings"/>
        /// for the communication.
        /// </remarks>
        public static string GetFabricServiceTransportSettingsSectionName(Type actorImplementationType)
        {
            return string.Format(
                CultureInfo.InvariantCulture,
                "{0}TransportSettings",
                GetActorServiceName(actorImplementationType));
        }

        /// <summary>
        /// Gets the <see cref="Microsoft.ServiceFabric.Actors.Runtime.IActorStateProvider"/> configuration section name
        /// specified in configuration package for the actor service.
        /// </summary>
        /// <param name="actorImplementationType">
        /// Type of class implementing the actor.
        /// </param>
        /// <returns>
        /// ActorStateProvider configuration section name.
        /// </returns>
        /// <remarks>
        /// Values specified in ActorStateProvider configuration section are used to configure <see cref="Microsoft.ServiceFabric.Actors.Runtime.IActorStateProvider"/>
        /// </remarks>
        public static string GetActorStateProviderSettingsSectionName(Type actorImplementationType)
        {
            return string.Format(
                CultureInfo.InvariantCulture,
                "{0}ActorStateProviderSettings",
                GetActorServiceName(actorImplementationType));
        }

        /// <summary>
        /// Gets the replicator security configuration section name specified in configuration package for the actor service.
        /// </summary>
        /// <param name="actorImplementationType">Type of class implementing the actor.</param>
        /// <returns>Replicator security configuration section name.</returns>
        /// <remarks>Values specified in replicator security configuration section are used to configure <see cref="System.Fabric.ReplicatorSettings.SecurityCredentials"/>
        /// for the replication of actor state between primary and secondary replicas.
        /// </remarks>
        public static string GetFabricServiceReplicatorSecurityConfigSectionName(Type actorImplementationType)
        {
            return string.Format(
                CultureInfo.InvariantCulture,
                "{0}ReplicatorSecurityConfig",
                GetActorServiceName(actorImplementationType));
        }

        /// <summary>
        /// Gets local store configuration section name specified in configuration package for the actor service.
        /// </summary>
        /// <param name="actorImplementationType">Type of class implementing the actor.</param>
        /// <returns>Local store configuration section name.</returns>
        /// <remarks>Values specified in local ESE configuration section are used to configure <see cref="System.Fabric.LocalEseStoreSettings"/>
        /// for storing the state of actor.
        /// </remarks>
        public static string GetLocalEseStoreConfigSectionName(Type actorImplementationType)
        {
            return string.Format(
                CultureInfo.InvariantCulture,
                "{0}LocalStoreConfig",
                GetActorServiceName(actorImplementationType));
        }

        /// <summary>
        /// Gets Key Value store configuration section name specified in configuration package for the actor service.
        /// </summary>
        /// <param name="actorImplementationType">Type of class implementing the actor.</param>
        /// <returns>Key Value store configuration section name.</returns>
        /// <remarks>Values specified in Key Value store configuration section are used to configure <see cref="System.Fabric.KeyValueStoreReplicaSettings"/>
        /// for storing the state of actor.
        /// </remarks>
        public static string GetKeyValueStoreSettingsConfigSectionName(Type actorImplementationType)
        {
            return string.Format(
                CultureInfo.InvariantCulture,
                "{0}KeyValueStoreConfig",
                GetActorServiceName(actorImplementationType));
        }

        /// <summary>
        /// Gets the configuration package name used in service package for the actor.
        /// </summary>
        /// <param name="actorImplementationType">Type of class implementing the actor.</param>
        /// <returns>configuration package name.</returns>
        public static string GetConfigPackageName(Type actorImplementationType = null)
        {
            return "Config";
        }

        /// <summary>
        /// Gets the code package name used in service package for the actor.
        /// </summary>
        /// <param name="actorImplementationType">Type of class implementing the actor.</param>
        /// <returns>code package name.</returns>
        /// <remarks>Code package name can be accessed from within a service as <see cref="System.Fabric.CodePackageActivationContext.CodePackageName"/></remarks>
        public static string GetCodePackageName(Type actorImplementationType = null)
        {
            return "Code";
        }

        /// <summary>
        /// Gets the credential type name used in replicator security configuration section in configuration package for the actor service.
        /// </summary>
        /// <param name="actorImplementationType">Type of class implementing the actor.</param>
        /// <returns>Replicator security credential type name.</returns>
        public static string GetFabricServiceReplicatorSecurityCredentialTypeName(Type actorImplementationType = null)
        {
            return "CredentialType";
        }

        /// <summary>
        /// Gets package name used to create Service Fabric Application package for the actor.
        /// </summary>
        /// <param name="appPrefix">Prefix to be used for the application package name.</param>
        /// <returns>Application package name.</returns>
        public static string GetFabricApplicationPackageName(string appPrefix)
        {
            return string.Format(CultureInfo.InvariantCulture, "{0}Pkg", GetFabricApplicationPrefix(appPrefix));
        }

        /// <summary>
        /// Gets the application type name used in application manifest when creating Service Fabric Application package for the actor.
        /// </summary>
        /// <param name="appPrefix">Prefix to be used for the application type name.</param>
        /// <returns>Application type name.</returns>
        public static string GetFabricApplicationTypeName(string appPrefix)
        {
            return string.Format(CultureInfo.InvariantCulture, "{0}Type", GetFabricApplicationPrefix(appPrefix));
        }

        /// <summary>
        /// Gets the application name used to create application in Service Fabric cluster.
        /// </summary>
        /// <param name="appPrefix">Prefix to be used for the application name.</param>
        /// <returns>Application name.</returns>
        public static string GetFabricApplicationName(string appPrefix)
        {
            return string.Format(CultureInfo.InvariantCulture, "fabric:/{0}", GetFabricApplicationPrefix(appPrefix));
        }

        internal static string GetCurrentFabricApplicationName()
        {
            if (applicationName == null)
            {
                try
                {
                    var context = FabricRuntime.GetActivationContext();
                    applicationName = context.ApplicationName;
                }
                catch (InvalidOperationException)
                {
                    applicationName = string.Empty;
                }
            }

            return applicationName;
        }

        internal static string GetActorStateProviderOverrideSectionName()
        {
            return "ActorStateProviderOverride";
        }

        internal static string GetActorStateProviderOverrideKeyName()
        {
            return "ActorStateProvider";
        }

        private static string GetFabricServiceName(string actorInterfaceTypeName)
        {
            return string.Format(CultureInfo.InvariantCulture, "{0}Service", GetName(actorInterfaceTypeName));
        }

        private static string GetFabricApplicationPrefix(string appPrefix)
        {
            if (string.IsNullOrEmpty(appPrefix))
            {
                return "FabricActorApp";
            }
            else
            {
                return appPrefix;
            }
        }

        private static string GetActorImplName(Type actorImplType)
        {
            return GetActorImplName(actorImplType.Name);
        }

        private static string GetActorImplName(string actorImplTypeName)
        {
            var actorImplName = actorImplTypeName;
            if (!actorImplName.EndsWith("Actor", StringComparison.InvariantCultureIgnoreCase))
            {
                actorImplName = string.Format(CultureInfo.InvariantCulture, "{0}Actor", actorImplName);
            }

            return actorImplName;
        }

        private static string GetActorServiceName(Type actorImplementationType)
        {
            var actorServiceAttr = ActorServiceAttribute.Get(actorImplementationType);
            if ((actorServiceAttr != null) && (!string.IsNullOrEmpty(actorServiceAttr.Name)))
            {
                return actorServiceAttr.Name;
            }
            else
            {
                return string.Format(
                    CultureInfo.InvariantCulture,
                    "{0}Service",
                    GetActorImplName(actorImplementationType));
            }
        }

        private static string GetName(string actorInterfaceTypeName)
        {
            var actorName = actorInterfaceTypeName;
            if (!actorName.EndsWith("Actor", StringComparison.InvariantCultureIgnoreCase))
            {
                actorName = string.Format(CultureInfo.InvariantCulture, "{0}Actor", actorName);
            }

            if ((actorName[0] == 'I') && !char.IsLower(actorName[1]))
            {
                return actorName.Substring(1);
            }
            else
            {
                return string.Format(CultureInfo.InvariantCulture, actorName);
            }
        }
    }
}
