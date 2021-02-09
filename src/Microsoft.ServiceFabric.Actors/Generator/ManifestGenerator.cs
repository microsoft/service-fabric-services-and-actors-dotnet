// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Generator
{
    using System;
    using System.Collections.Generic;
    using System.Fabric.Management.ServiceModel;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Xml;
    using Microsoft.ServiceFabric.Actors.Runtime;
    using Microsoft.ServiceFabric.Services.Remoting;

    /// <summary>
    /// ServiceManifestEntryPointType decides which kind of service manifest exe host is generated. By default the existing behavior of serviceName.exe will be used.
    /// </summary>
    internal enum SvcManifestEntryPointType
    {
        /// <summary>
        /// Default behavior of ServiceName.exe
        /// </summary>
        Exe,

        /// <summary>
        /// ServiceName without any extension is generated as the program in the exehost. This can be used on both windows/linux in a self contained deployment.
        /// </summary>
        NoExtension,

        /// <summary>
        /// ServiceName.dll with the external program "dotnet", is generated in the exehost. This can be used on both windows/linux in a framework dependant deployment.
        /// </summary>
        ExternalExecutable,
    }

    // generates the service manifest for the actor implementations
    internal class ManifestGenerator
    {
        private const string DefaultPartitionCount = "10";
        private const string NoneStatePersistenceTargetReplicaDefaultValue = "1";
        private const string NoneStatePersistenceMinReplicaDefaultValue = "1";
        private const string PersistedStateTargetReplicaDefaultValue = "3";
        private const string PersistedStateMinReplicaDefaultValue = "3";

        private const string ApplicationManifestFileName = "ApplicationManifest.xml";
        private const string ServiceManifestFileName = "ServiceManifest.xml";
        private const string ConfigSettingsFileName = "Settings.xml";

        private const string ExtensionSchemaNamespace = @"http://schemas.microsoft.com/2015/03/fabact-no-schema";
        private const string GeneratedServiceTypeExtensionName = "__GeneratedServiceType__";
        private const string GeneratedNamesRootElementName = "GeneratedNames";
        private const string GeneratedNamesAttributeName = "Name";

        private const string GeneratedDefaultServiceName = "DefaultService";
        private const string GeneratedServiceEndpointName = "ServiceEndpoint";
        private const string GeneratedServiceEndpointV2Name = "ServiceEndpointV2";
        private const string GeneratedServiceEndpointWrappedMessageStackName = "ServiceEndpointV2_1";
        private const string GeneratedReplicatorEndpointName = "ReplicatorEndpoint";
        private const string GeneratedReplicatorConfigSectionName = "ReplicatorConfigSection";
        private const string GeneratedReplicatorSecurityConfigSectionName = "ReplicatorSecurityConfigSection";
        private const string GeneratedStoreConfigSectionName = "StoreConfigSection";
        private const string PartitionCountParamName = "PartitionCount";
        private const string MinReplicaSetSizeParamName = "MinReplicaSetSize";
        private const string TargetReplicaSetSizeParamName = "TargetReplicaSetSize";

        private const string ParamNameFormat = "{0}_{1}";
        private const string ParamNameUsageFormat = "[{0}_{1}]";

        // Generated Id is of format Guid|StatePersistenceAttributeValue. This is to handle StatePersistenceAttribute changes and generate Target/Min replica set size parameters correctly.
        private const string GeneratedIdFormat = "{0}|{1}";
        private const char GeneratedIdDelimiter = '|';

        private const string DefaultSemanticVersion = "1.0.0";

        private static readonly Dictionary<string, Func<ActorTypeInformation, string>> GeneratedNameFunctions = new Dictionary
            <string, Func<ActorTypeInformation, string>>
            {
                { GeneratedDefaultServiceName, GetFabricServiceName },
                { GeneratedReplicatorEndpointName, GetFabricServiceReplicatorEndpointName },
                { GeneratedReplicatorConfigSectionName, GetFabricServiceReplicatorConfigSectionName },
                { GeneratedReplicatorSecurityConfigSectionName, GetFabricServiceReplicatorSecurityConfigSectionName },
                { GeneratedStoreConfigSectionName, GetLocalEseStoreConfigSectionName },
            };

        private static Context toolContext;

        internal static void Generate(Arguments arguments)
        {
            toolContext = new Context(arguments);
            toolContext.LoadExistingContents();
            var serviceManifest = CreateServiceManifest(arguments.ServiceManifestEntryPointType);
            var configSettings = CreateConfigSettings();
            var mergedServiceManifest = MergeServiceManifest(serviceManifest);

            InsertXmlCommentsAndWriteIfNeeded(
                toolContext.ServiceManifestFilePath,
                toolContext.ExistingServiceManifestContents,
                mergedServiceManifest);

            InsertXmlCommentsAndWriteIfNeeded(
                toolContext.ConfigSettingsFilePath,
                toolContext.ExistingConfigSettingsContents,
                MergeConfigSettings(configSettings));

            if (toolContext.ShouldGenerateApplicationManifest())
            {
                var applicationManifest = CreateApplicationManifest(mergedServiceManifest);

                InsertXmlCommentsAndWriteIfNeeded(
                    toolContext.ApplicationManifestFilePath,
                    toolContext.ExistingApplicationManifestContents,
                    MergeApplicationManifest(applicationManifest));
            }
        }

        #region Create and Merge Config Settings

        private static SettingsType CreateConfigSettings()
        {
            var settings = new SettingsType();

            var sections = new List<SettingsTypeSection>();
            foreach (var actorTypeInfo in toolContext.Arguments.ActorTypes)
            {
                sections.AddRange(CreateConfigSections(actorTypeInfo));
            }

            settings.Section = sections.ToArray();
            return settings;
        }

        private static SettingsType MergeConfigSettings(SettingsType configSettings)
        {
            if (string.IsNullOrEmpty(toolContext.ExistingConfigSettingsContents))
            {
                return configSettings;
            }

            var existingConfigSettings = XmlSerializationUtility.Deserialize<SettingsType>(
                toolContext.ExistingConfigSettingsContents);
            existingConfigSettings.Section = MergeConfigSections(existingConfigSettings.Section, configSettings.Section);

            return existingConfigSettings;
        }

        private static IEnumerable<SettingsTypeSection> CreateConfigSections(
            ActorTypeInformation actorTypeInfo)
        {
            var retval = new List<SettingsTypeSection>();

            // add section for the replicator settings
            var replicatorConfigSection = new SettingsTypeSection();
            retval.Add(replicatorConfigSection);

            replicatorConfigSection.Name = GetFabricServiceReplicatorConfigSectionName(actorTypeInfo);
            var replicatorConfigSectionParameters = new List<SettingsTypeSectionParameter>
            {
                new SettingsTypeSectionParameter
                {
                    Name = "ReplicatorEndpoint",
                    Value = GetFabricServiceReplicatorEndpointName(actorTypeInfo),
                },
                new SettingsTypeSectionParameter
                {
                    Name = "BatchAcknowledgementInterval",
                    Value = "0.005", // in seconds, default to 5 milliseconds to match reliable services.
                },
            };
            replicatorConfigSection.Parameter = replicatorConfigSectionParameters.ToArray();

            // add section for the replicator security settings
            var replicatorSecurityConfigSection = new SettingsTypeSection();
            retval.Add(replicatorSecurityConfigSection);
            replicatorSecurityConfigSection.Name = GetFabricServiceReplicatorSecurityConfigSectionName(actorTypeInfo);
            var replicatorSecurityConfigParameters = new List<SettingsTypeSectionParameter>
            {
                new SettingsTypeSectionParameter
                {
                    Name = ActorNameFormat.GetFabricServiceReplicatorSecurityCredentialTypeName(),
                    Value = "None",
                },
            };

            replicatorSecurityConfigSection.Parameter = replicatorSecurityConfigParameters.ToArray();
            return retval;
        }

        private static string GetLocalEseStoreConfigSectionName(ActorTypeInformation actorTypeInfo)
        {
            return ActorNameFormat.GetLocalEseStoreConfigSectionName(actorTypeInfo.ImplementationType);
        }

        private static string GetFabricServiceReplicatorConfigSectionName(ActorTypeInformation actorTypeInfo)
        {
            return ActorNameFormat.GetFabricServiceReplicatorConfigSectionName(actorTypeInfo.ImplementationType);
        }

        private static string GetFabricServiceReplicatorSecurityConfigSectionName(ActorTypeInformation actorTypeInfo)
        {
            return ActorNameFormat.GetFabricServiceReplicatorSecurityConfigSectionName(actorTypeInfo.ImplementationType);
        }

        private static SettingsTypeSection MergeConfigSection(
            SettingsTypeSection existingItem,
            SettingsTypeSection newItem)
        {
            existingItem.Parameter = MergeConfigParameters(existingItem.Parameter, newItem.Parameter);
            return existingItem;
        }

        private static SettingsTypeSection[] MergeConfigSections(
            IEnumerable<SettingsTypeSection> existingItems,
            IEnumerable<SettingsTypeSection> newItems)
        {
            return MergeItems(
                existingItems,
                newItems,
                (i1, i2) => (string.CompareOrdinal(i1.Name, i2.Name) == 0),
                MergeConfigSection,
                i1 => toolContext.ShouldKeepConfigSection(i1.Name));
        }

        private static SettingsTypeSectionParameter MergeConfigParameter(
            SettingsTypeSectionParameter existingItem,
            SettingsTypeSectionParameter newItem)
        {
            return existingItem;
        }

        private static SettingsTypeSectionParameter[] MergeConfigParameters(
            IEnumerable<SettingsTypeSectionParameter> existingItems,
            IEnumerable<SettingsTypeSectionParameter> newItems)
        {
            return MergeItems(
                existingItems,
                newItems,
                (i1, i2) => (string.CompareOrdinal(i1.Name, i2.Name) == 0),
                MergeConfigParameter);
        }

        #endregion

        #region Create and Merge Service Manifest

        private static ServiceManifestType CreateServiceManifest(SvcManifestEntryPointType serviceManifestEntryPointType)
        {
            var serviceManifest = new ServiceManifestType
            {
                Name = ActorNameFormat.GetFabricServicePackageName(toolContext.Arguments.ServicePackageNamePrefix),
                Version = GetVersion(),
            };

            var serviceTypeList = new List<ServiceTypeType>();
            var endpointResourceList = new List<EndpointType>();

            foreach (var actorTypeInfo in toolContext.Arguments.ActorTypes)
            {
                serviceTypeList.Add(CreateServiceTypeType(actorTypeInfo));
                endpointResourceList.AddRange(CreateEndpointResources(actorTypeInfo));
            }

            serviceManifest.ServiceTypes = new object[serviceTypeList.Count];
            serviceTypeList.ToArray().CopyTo(serviceManifest.ServiceTypes, 0);

            serviceManifest.CodePackage = new CodePackageType[1];
            serviceManifest.CodePackage[0] = CreateCodePackage(serviceManifestEntryPointType);

            serviceManifest.ConfigPackage = new ConfigPackageType[1];
            serviceManifest.ConfigPackage[0] = CreateConfigPackage();

            serviceManifest.Resources = new ResourcesType
            {
                Endpoints = endpointResourceList.ToArray(),
            };

            return serviceManifest;
        }

        private static ServiceManifestType MergeServiceManifest(ServiceManifestType serviceManifest)
        {
            if (string.IsNullOrEmpty(toolContext.ExistingServiceManifestContents))
            {
                return serviceManifest;
            }

            var existingServiceManifest = toolContext.ExistingServiceManifestType;

            // basic properties of the service manifest
            // Use new version, only when it doesn't exist.
            if (string.IsNullOrEmpty(existingServiceManifest.Version))
            {
                existingServiceManifest.Version = serviceManifest.Version;
            }

            existingServiceManifest.Name = serviceManifest.Name;

            // service types
            existingServiceManifest.ServiceTypes = MergeServiceTypes(
                existingServiceManifest,
                serviceManifest);

            existingServiceManifest.CodePackage = MergeCodePackages(
                existingServiceManifest.CodePackage,
                serviceManifest.CodePackage);

            // config package
            existingServiceManifest.ConfigPackage = MergeConfigPackages(
                existingServiceManifest.ConfigPackage,
                serviceManifest.ConfigPackage);

            // endpoints
            if (existingServiceManifest.Resources == null)
            {
                existingServiceManifest.Resources = serviceManifest.Resources;
            }
            else
            {
                existingServiceManifest.Resources.Endpoints = MergeEndpointResource(
                    existingServiceManifest.Resources.Endpoints,
                    serviceManifest.Resources.Endpoints);
            }

            return existingServiceManifest;
        }

        private static object[] MergeServiceTypes(
            ServiceManifestType existingServiceManifest,
            ServiceManifestType serviceManifest)
        {
            return MergeItems(
                existingServiceManifest.ServiceTypes,
                serviceManifest.ServiceTypes,
                (i1, i2) =>
                {
                    var casted1 = i1 as ServiceTypeType;
                    var casted2 = i2 as ServiceTypeType;
                    return (casted1 != null && casted2 != null) &&
                           string.CompareOrdinal(casted1.ServiceTypeName, casted2.ServiceTypeName) == 0;
                },
                MergeServiceType,
                i1 => toolContext.ShouldKeepItem(GeneratedServiceTypeExtensionName, ((ServiceTypeType)i1).ServiceTypeName));
        }

        private static object MergeServiceType(
            object existingItem,
            object newItem)
        {
            var existingCasted = existingItem as ServiceTypeType;
            var newCasted = newItem as ServiceTypeType;

            if (newCasted == null)
            {
                return existingItem;
            }

            if (existingCasted == null)
            {
                return newItem;
            }

            var mergedExtensions = MergeItems(
                existingCasted.Extensions,
                newCasted.Extensions,
                (i1, i2) => string.CompareOrdinal(i1.Name, i2.Name) == 0,
                MergeExtension);

            if (existingItem.GetType() != newItem.GetType())
            {
                newCasted.LoadMetrics = existingCasted.LoadMetrics;
                newCasted.PlacementConstraints = existingCasted.PlacementConstraints;
                newCasted.ServicePlacementPolicies = existingCasted.ServicePlacementPolicies;

                existingCasted = newCasted;
            }

            var newStateful = newCasted as StatefulServiceTypeType;
            if (existingCasted is StatefulServiceTypeType existingStateful && newStateful != null)
            {
                existingStateful.HasPersistedState = newStateful.HasPersistedState;
            }

            existingCasted.Extensions = mergedExtensions;

            return existingCasted;
        }

        private static ExtensionsTypeExtension MergeExtension(
            ExtensionsTypeExtension existingExtension,
            ExtensionsTypeExtension newExtension)
        {
            if (existingExtension.GeneratedId != null)
            {
                // Only reuse from existing extension:
                // 1. If existingExtension.GeneratedId has StatePersistence in it (as older service manifests didn't have StatePersistence in GeneratedId).
                // 2. And StatePersistence in existingExtension.GeneratedId matches the  StatePersistence in newExtension.GeneratedId

                // Generated Id is of format Guid|StatePersistenceAttributeValue
                var splitted = newExtension.GeneratedId.Split(GeneratedIdDelimiter);
                if (splitted.Length == 2)
                {
                    var newStatePersistenceValue = splitted[1];

                    if (existingExtension.GeneratedId.EndsWith(GeneratedIdDelimiter + newStatePersistenceValue))
                    {
                        newExtension.GeneratedId = existingExtension.GeneratedId;
                    }
                }
            }

            return newExtension;
        }

        private static ServiceTypeType CreateServiceTypeType(
            ActorTypeInformation actorTypeInfo)
        {
            // HasPersistedState flag in service manifest is set to true only when
            //    1. Actor [StatePersistenceAttribute] attribute has StatePersistence.Persisted.
            return new StatefulServiceTypeType
            {
                HasPersistedState = actorTypeInfo.StatePersistence.Equals(StatePersistence.Persisted),
                ServiceTypeName = ActorNameFormat.GetFabricServiceTypeName(actorTypeInfo.ImplementationType),
                Extensions = CreateServiceTypeExtensions(actorTypeInfo),
            };
        }

        private static Dictionary<string, Func<ActorTypeInformation, string>> GetGeneratedNameFunctionForServiceEndpoint(ActorTypeInformation actorTypeInfo)
        {
            var generatedNameFunctions = new Dictionary<string, Func<ActorTypeInformation, string>>();
#if !DotNetCoreClr
            if (Helper.IsRemotingV1(actorTypeInfo.RemotingListenerVersion))
            {
                generatedNameFunctions.Add(GeneratedServiceEndpointName, GetFabricServiceEndpointName);
            }
#endif
            if (Helper.IsRemotingV2(actorTypeInfo.RemotingListenerVersion))
            {
                generatedNameFunctions.Add(GeneratedServiceEndpointV2Name, GetFabricServiceV2EndpointName);
            }

            if (Helper.IsRemotingV2_1(actorTypeInfo.RemotingListenerVersion))
            {
                generatedNameFunctions.Add(GeneratedServiceEndpointWrappedMessageStackName, GetGeneratedServiceEndpointWrappedMessageStackName);
            }

            return generatedNameFunctions;
        }

        private static List<EndpointType> CreateEndpointResourceBasedOnRemotingServer(ActorTypeInformation actorTypeInfo)
        {
            var endpoints = new List<EndpointType>();
#if !DotNetCoreClr
            if (Helper.IsRemotingV1(actorTypeInfo.RemotingListenerVersion))
            {
                endpoints.Add(
                    new EndpointType()
                    {
                        Name = GetFabricServiceEndpointName(actorTypeInfo),
                    });
            }
#endif
            if (Helper.IsRemotingV2(actorTypeInfo.RemotingListenerVersion))
            {
                endpoints.Add(
                    new EndpointType()
                    {
                        Name = GetFabricServiceV2EndpointName(actorTypeInfo),
                    });
            }

            if (Helper.IsRemotingV2_1(actorTypeInfo.RemotingListenerVersion))
            {
                endpoints.Add(
                    new EndpointType()
                    {
                        Name = GetGeneratedServiceEndpointWrappedMessageStackName(actorTypeInfo),
                    });
            }

            return endpoints;
        }

        private static ExtensionsTypeExtension[] CreateServiceTypeExtensions(ActorTypeInformation actorTypeInfo)
        {
            var generatedNameFunctions = GeneratedNameFunctions.Concat(GetGeneratedNameFunctionForServiceEndpoint(actorTypeInfo)).ToDictionary(d => d.Key, d => d.Value);

            var xml = CreateServiceTypeExtension(actorTypeInfo, generatedNameFunctions);

            var extension = new ExtensionsTypeExtension
            {
                Name = GeneratedServiceTypeExtensionName,

                // GeneratedId is of format Guid|StatePersistenceAttributeValue.
                GeneratedId =
                    string.Format(
                        GeneratedIdFormat,
                        Guid.NewGuid(),
                        actorTypeInfo.StatePersistence),
                Any = xml.DocumentElement,
            };

            return new List<ExtensionsTypeExtension> { extension }.ToArray();
        }

        private static XmlDocument CreateServiceTypeExtension(ActorTypeInformation actorTypeInfo, Dictionary<string, Func<ActorTypeInformation, string>> generatedNameFunctions)
        {
            var xml = new XmlDocument();
            xml.XmlResolver = null;

            xml.AppendChild(xml.CreateElement(GeneratedNamesRootElementName, ExtensionSchemaNamespace));

            foreach (var pair in generatedNameFunctions)
            {
                var elementName = pair.Key;
                var attributeValue = pair.Value(actorTypeInfo);

                var elem = xml.CreateElement(elementName, ExtensionSchemaNamespace);
                elem.SetAttribute(GeneratedNamesAttributeName, attributeValue);
                xml.DocumentElement.AppendChild(elem);
            }

            return xml;
        }

        #region CodePackage Create and Merge

        private static CodePackageType CreateCodePackage(SvcManifestEntryPointType serviceManifestEntryPointType)
        {
            var assembly = toolContext.Arguments.InputAssembly;
            var codePackage = new CodePackageType
            {
                Name = ActorNameFormat.GetCodePackageName(),
                Version = GetVersion(),
                EntryPoint = new EntryPointDescriptionType
                {
                    Item = CreateExeHostEntryPoint(assembly, serviceManifestEntryPointType),
                },
            };

            return codePackage;
        }

        private static EntryPointDescriptionTypeExeHost CreateExeHostEntryPoint(
            Assembly assembly, SvcManifestEntryPointType serviceManifestEntryPointType)
        {
            if (serviceManifestEntryPointType == SvcManifestEntryPointType.NoExtension)
            {
                return new EntryPointDescriptionTypeExeHost
                {
                    Program = Path.GetFileNameWithoutExtension(assembly.Location),
                };
            }
            else if (serviceManifestEntryPointType == SvcManifestEntryPointType.ExternalExecutable)
            {
                return new EntryPointDescriptionTypeExeHost
                {
                    IsExternalExecutable = true,
                    Program = "dotnet",
                    Arguments = Path.GetFileNameWithoutExtension(assembly.Location) + ".dll",
                    WorkingFolder = ExeHostEntryPointTypeWorkingFolder.CodePackage,
                };
            }
            else
            {
                return new EntryPointDescriptionTypeExeHost
                {
                    Program = Path.GetFileNameWithoutExtension(assembly.Location) + ".exe",
                };
            }
        }

        private static CodePackageType MergeCodePackage(
            CodePackageType existingItem,
            CodePackageType newItem)
        {
            // Use new version, only when it doesn't exist.
            if (string.IsNullOrEmpty(existingItem.Version))
            {
                existingItem.Version = newItem.Version;
            }

            if ((existingItem.EntryPoint == null) ||
                (existingItem.EntryPoint.Item == null) ||
                (existingItem.EntryPoint.Item.GetType() != newItem.EntryPoint.Item.GetType()))
            {
                existingItem.EntryPoint = newItem.EntryPoint;
            }
            else
            {
                var existingExeHost = (ExeHostEntryPointType)existingItem.EntryPoint.Item;
                var newExeHost = (ExeHostEntryPointType)newItem.EntryPoint.Item;
                existingExeHost.Program = newExeHost.Program;
            }

            return existingItem;
        }

        private static CodePackageType[] MergeCodePackages(
            IEnumerable<CodePackageType> existingItems,
            IEnumerable<CodePackageType> newItems)
        {
            return MergeItems(
                existingItems,
                newItems,
                (i1, i2) => (string.CompareOrdinal(i1.Name, i2.Name) == 0),
                MergeCodePackage);
        }

        #endregion

        #region ConfigPackage Create and Merge

        private static ConfigPackageType CreateConfigPackage()
        {
            var configPackage = new ConfigPackageType
            {
                Name = ActorNameFormat.GetConfigPackageName(),
                Version = GetVersion(),
            };

            return configPackage;
        }

        private static ConfigPackageType MergeConfigPackage(
            ConfigPackageType existingItem,
            ConfigPackageType newItem)
        {
            // Use new version, only when it doesn't exist.
            if (string.IsNullOrEmpty(existingItem.Version))
            {
                existingItem.Version = newItem.Version;
            }

            return existingItem;
        }

        private static ConfigPackageType[] MergeConfigPackages(
            IEnumerable<ConfigPackageType> existingItems,
            IEnumerable<ConfigPackageType> newItems)
        {
            return MergeItems(
                existingItems,
                newItems,
                (i1, i2) => (string.CompareOrdinal(i1.Name, i2.Name) == 0),
                MergeConfigPackage);
        }

        #endregion

        #region EndpointResource Create and Merge

        private static IEnumerable<EndpointType> CreateEndpointResources(
            ActorTypeInformation actorTypeInfo)
        {
            var endpoints = CreateEndpointResourceBasedOnRemotingServer(actorTypeInfo);

            endpoints.Add(
                new EndpointType
                {
                    Name = GetFabricServiceReplicatorEndpointName(actorTypeInfo),
                });

            return endpoints;
        }

        private static string GetFabricServiceReplicatorEndpointName(ActorTypeInformation actorTypeInfo)
        {
            return ActorNameFormat.GetFabricServiceReplicatorEndpointName(actorTypeInfo.ImplementationType);
        }

        private static string GetFabricServiceEndpointName(ActorTypeInformation actorTypeInfo)
        {
            return ActorNameFormat.GetFabricServiceEndpointName(actorTypeInfo.ImplementationType);
        }

        private static string GetFabricServiceV2EndpointName(ActorTypeInformation actorTypeInfo)
        {
            return ActorNameFormat.GetFabricServiceV2EndpointName(actorTypeInfo.ImplementationType);
        }

        private static string GetGeneratedServiceEndpointWrappedMessageStackName(ActorTypeInformation actorTypeInfo)
        {
            return ActorNameFormat.GetFabricServiceWrappedMessageEndpointName(actorTypeInfo.ImplementationType);
        }

        private static EndpointType MergeEndpointResource(
            EndpointType existingItem,
            EndpointType newItem)
        {
            return existingItem;
        }

        private static EndpointType[] MergeEndpointResource(
            IEnumerable<EndpointType> existingItems,
            IEnumerable<EndpointType> newItems)
        {
            return MergeItems(
                existingItems,
                newItems,
                (i1, i2) => (string.CompareOrdinal(i1.Name, i2.Name) == 0),
                MergeEndpointResource,
                i1 => toolContext.ShouldKeepEndpointResource(i1.Name));
        }

        #endregion

        #endregion

        #region Create And Merge Application Manifest

        private static ApplicationManifestType CreateApplicationManifest(ServiceManifestType serviceManifest)
        {
            // application manifest properties
            var applicationManifest = new ApplicationManifestType
            {
                ApplicationTypeName = ActorNameFormat.GetFabricApplicationTypeName(toolContext.Arguments.ApplicationPrefix),
                ApplicationTypeVersion = GetVersion(),
                ServiceManifestImport = new ApplicationManifestTypeServiceManifestImport[1],
            };

            // service manifest import
            applicationManifest.ServiceManifestImport[0] = new ApplicationManifestTypeServiceManifestImport
            {
                ServiceManifestRef = new ServiceManifestRefType
                {
                    ServiceManifestName = serviceManifest.Name,
                    ServiceManifestVersion = serviceManifest.Version,
                },
            };

            // default parameters
            var defaultParameters = CreateDefaultParameter();
            if (defaultParameters != null && defaultParameters.Count > 0)
            {
                applicationManifest.Parameters = defaultParameters.ToArray();
            }

            // default services
            var defaultServices = CreateDefaultServices(serviceManifest);
            applicationManifest.DefaultServices = new DefaultServicesType
            {
                Items = new object[defaultServices.Count],
            };
            defaultServices.ToArray().CopyTo(applicationManifest.DefaultServices.Items, 0);

            return applicationManifest;
        }

        private static ApplicationManifestType MergeApplicationManifest(ApplicationManifestType applicationManifest)
        {
            if (string.IsNullOrEmpty(toolContext.ExistingApplicationManifestContents))
            {
                return applicationManifest;
            }

            var existingApplicationManifest = toolContext.ExistingApplicationManifestType;

            // Use new version, only when it doesn't exist.
            if (string.IsNullOrEmpty(existingApplicationManifest.ApplicationTypeVersion))
            {
                existingApplicationManifest.ApplicationTypeVersion = applicationManifest.ApplicationTypeVersion;
            }

            existingApplicationManifest.ServiceManifestImport = MergeServiceManifestImports(
                existingApplicationManifest.ServiceManifestImport,
                applicationManifest.ServiceManifestImport);

            existingApplicationManifest.DefaultServices = MergeDefaultServices(
                existingApplicationManifest.DefaultServices,
                applicationManifest.DefaultServices);

            existingApplicationManifest.Parameters = MergeParameters(
                existingApplicationManifest.Parameters,
                applicationManifest.Parameters);

            return existingApplicationManifest;
        }

        private static IList<DefaultServicesTypeService> CreateDefaultServices(ServiceManifestType serviceManifest)
        {
            return toolContext.Arguments.ActorTypes.Select(x => CreateDefaultService(x, serviceManifest)).ToList();
        }

        private static IList<ApplicationManifestTypeParameter> CreateDefaultParameter()
        {
            var parameterlists = toolContext.Arguments.ActorTypes.Select(
                CreateDefaultParameter).ToList();
            var parametes = new List<ApplicationManifestTypeParameter>();

            foreach (var paramlist in parameterlists)
            {
                parametes.AddRange(paramlist);
            }

            return parametes;
        }

        private static DefaultServicesType MergeDefaultServices(
            DefaultServicesType existingItem,
            DefaultServicesType newItem)
        {
            if (existingItem == null)
            {
                existingItem = new DefaultServicesType();
                existingItem.Items = newItem.Items;
            }
            else if (existingItem.Items == null)
            {
                existingItem.Items = newItem.Items;
            }
            else
            {
                existingItem.Items = MergeItems(
                    existingItem.Items,
                    newItem.Items,
                    (i1, i2) =>
                    {
                        return ((i1.GetType() == i2.GetType()) &&
                                (i1.GetType() == typeof(DefaultServicesTypeService)) &&
                                (string.CompareOrdinal(
                                    ((DefaultServicesTypeService)i1).Name,
                                    ((DefaultServicesTypeService)i2).Name) == 0));
                    },
                    (i1, i2) =>
                    {
                        return MergeDefaultService(
                            (DefaultServicesTypeService)i1,
                            (DefaultServicesTypeService)i2);
                    },
                    i1 =>
                    {
                        return toolContext.ShouldKeepItem(
                            GeneratedDefaultServiceName,
                            ((DefaultServicesTypeService)i1).Name);
                    });
            }

            return existingItem;
        }

        private static DefaultServicesTypeService CreateDefaultService(
            ActorTypeInformation actorTypeInfo, ServiceManifestType serviceManifest)
        {
            var defaultService = new DefaultServicesTypeService
            {
                Name = GetFabricServiceName(actorTypeInfo),
            };

            var partition = new ServiceTypeUniformInt64Partition
            {
                LowKey = long.MinValue.ToString(CultureInfo.InvariantCulture),
                HighKey = long.MaxValue.ToString(CultureInfo.InvariantCulture),
                PartitionCount = string.Format(ParamNameUsageFormat, defaultService.Name, PartitionCountParamName),
            };

            var service = CreateStatefulDefaultService(actorTypeInfo);

            var serviceTypeName = ActorNameFormat.GetFabricServiceTypeName(actorTypeInfo.ImplementationType);

            defaultService.Item = service;
            defaultService.Item.ServiceTypeName = serviceTypeName;
            defaultService.Item.UniformInt64Partition = partition;

            // Get GeneratedId from service manifest for the ServiceTypeName
            var serviceType = (ServiceTypeType)serviceManifest.ServiceTypes.First(x => ((ServiceTypeType)x).ServiceTypeName.Equals(serviceTypeName));
            var extension = serviceType.Extensions.First(x => x.Name.Equals(GeneratedServiceTypeExtensionName));

            defaultService.GeneratedIdRef = extension.GeneratedId;

            return defaultService;
        }

        private static IList<ApplicationManifestTypeParameter> CreateDefaultParameter(ActorTypeInformation actorTypeInfo)
        {
            var applicationManifestTypeParameterList = new List<ApplicationManifestTypeParameter>();
            CreateStatefulDefaultServiceParameters(actorTypeInfo, applicationManifestTypeParameterList);

            return applicationManifestTypeParameterList;
        }

        private static ApplicationManifestTypeParameter GetApplicationManifestTypeParameter(string name, string defaultValue)
        {
            return new ApplicationManifestTypeParameter() { Name = name, DefaultValue = defaultValue };
        }

        private static string GetFabricServiceName(ActorTypeInformation actorTypeInfo)
        {
            return ActorNameFormat.GetFabricServiceName(actorTypeInfo.InterfaceTypes.First(), actorTypeInfo.ServiceName);
        }

        private static ServiceType CreateStatefulDefaultService(
            ActorTypeInformation actorTypeInfo)
        {
            var name = GetFabricServiceName(actorTypeInfo);

            return new StatefulServiceType
            {
                MinReplicaSetSize = string.Format(ParamNameUsageFormat, name, MinReplicaSetSizeParamName),
                TargetReplicaSetSize = string.Format(ParamNameUsageFormat, name, TargetReplicaSetSizeParamName),
            };
        }

        private static void CreateStatefulDefaultServiceParameters(
            ActorTypeInformation actorTypeInfo, IList<ApplicationManifestTypeParameter> applicationManifestTypeParameters)
        {
            var name = GetFabricServiceName(actorTypeInfo);
            applicationManifestTypeParameters.Add(
                GetApplicationManifestTypeParameter(
                    string.Format(ParamNameFormat, name, PartitionCountParamName),
                    DefaultPartitionCount));

            applicationManifestTypeParameters.Add(
                GetApplicationManifestTypeParameter(
                string.Format(ParamNameFormat, name, MinReplicaSetSizeParamName),
                GetMinReplicaSetSize(actorTypeInfo)));

            applicationManifestTypeParameters.Add(
                GetApplicationManifestTypeParameter(
                string.Format(ParamNameFormat, name, TargetReplicaSetSizeParamName),
                GetTargetReplicaSetSize(actorTypeInfo)));
        }

        private static string GetMinReplicaSetSize(ActorTypeInformation actorTypeInfo)
        {
            // MinReplicaSetSize is 1 when:
            //   1. Actor has no [StatePersistenceAttribute] attribute. OR
            //   2. Actor [StatePersistenceAttribute] attribute has StatePersistence.None.

            // MinReplicaSetSize is 3 when:
            //   1. Actor [StatePersistenceAttribute] attribute has StatePersistence.Volatile OR
            //   2. Actor [StatePersistenceAttribute] attribute has StatePersistence.Persisted.
            if (actorTypeInfo.StatePersistence.Equals(StatePersistence.None))
            {
                return NoneStatePersistenceMinReplicaDefaultValue;
            }
            else
            {
                return PersistedStateMinReplicaDefaultValue;
            }
        }

        private static string GetTargetReplicaSetSize(ActorTypeInformation actorTypeInfo)
        {
            // TargetReplicaSetSize is 1 when:
            //   1. Actor has no [StatePersistenceAttribute] attribute. OR
            //   2. Actor [StatePersistenceAttribute] attribute has StatePersistence.None.

            // TargetReplicaSetSize is 3 when:
            //   1. Actor [StatePersistenceAttribute] attribute has StatePersistence.Volatile OR
            //   2. Actor [StatePersistenceAttribute] attribute has StatePersistence.Persisted.
            if (actorTypeInfo.StatePersistence.Equals(StatePersistence.None))
            {
                return NoneStatePersistenceTargetReplicaDefaultValue;
            }
            else
            {
                return PersistedStateTargetReplicaDefaultValue;
            }
        }

        private static DefaultServicesTypeService MergeDefaultService(
            DefaultServicesTypeService existingItem,
            DefaultServicesTypeService newItem)
        {
            if (existingItem.Item == null)
            {
                existingItem.Item = newItem.Item;

                return existingItem;
            }

            var existingService = existingItem.Item;
            var newService = newItem.Item;

            // merge GeneratedIdRef
            existingItem.GeneratedIdRef = newItem.GeneratedIdRef;

            // Merged type-agnostic values before (potentially) swapping the type
            var mergedPartition = MergeDefaultServicePartition(
                existingService.UniformInt64Partition,
                newService.UniformInt64Partition);

            existingService.ServiceTypeName = newService.ServiceTypeName;

            if (existingService.GetType() != newService.GetType())
            {
                newService.LoadMetrics = existingService.LoadMetrics;
                newService.PlacementConstraints = existingService.PlacementConstraints;
                newService.ServiceCorrelations = existingService.ServiceCorrelations;
                newService.ServicePlacementPolicies = existingService.ServicePlacementPolicies;

                // Type-specific values are lost
                existingService = newService;
                existingItem.Item = existingService;
            }

            existingService.UniformInt64Partition = mergedPartition;
            existingService.SingletonPartition = null;
            existingService.NamedPartition = null;

            return existingItem;
        }

        private static ServiceTypeUniformInt64Partition MergeDefaultServicePartition(
            ServiceTypeUniformInt64Partition existingItem,
            ServiceTypeUniformInt64Partition newItem)
        {
            if (existingItem == null)
            {
                return newItem;
            }
            else
            {
                existingItem.LowKey = newItem.LowKey;
                existingItem.HighKey = newItem.HighKey;
                return existingItem;
            }
        }

        private static ApplicationManifestTypeServiceManifestImport MergeServiceManifestImport(
            ApplicationManifestTypeServiceManifestImport existingItem,
            ApplicationManifestTypeServiceManifestImport newItem)
        {
            // Use new version, only when it doesn't exist.
            if (string.IsNullOrEmpty(existingItem.ServiceManifestRef.ServiceManifestVersion))
            {
                existingItem.ServiceManifestRef.ServiceManifestVersion = newItem.ServiceManifestRef.ServiceManifestVersion;
            }

            return existingItem;
        }

        private static ApplicationManifestTypeServiceManifestImport[]
            MergeServiceManifestImports(
            IEnumerable<ApplicationManifestTypeServiceManifestImport> existingItems,
            IEnumerable<ApplicationManifestTypeServiceManifestImport> newItems)
        {
            return MergeItems(
                existingItems,
                newItems,
                (i1, i2) =>
                    ((i1.ServiceManifestRef != null) && (i2.ServiceManifestRef != null) &&
                     (string.CompareOrdinal(
                         i1.ServiceManifestRef.ServiceManifestName,
                         i2.ServiceManifestRef.ServiceManifestName) == 0)),
                MergeServiceManifestImport);
        }

        private static ApplicationManifestTypeParameter MergeParameters(
            ApplicationManifestTypeParameter existingItem,
            ApplicationManifestTypeParameter newItem)
        {
            // Following scenario must be handled while merging parameters values for TargetReplicaSetSize and MinReplicaSetSize
            // 1. User could change StatePrersistence from Persisted/Volatile to None, in this case
            //    overwrite the existing parameter value.
            foreach (var actorTypeInfo in toolContext.Arguments.ActorTypes)
            {
                var name = GetFabricServiceName(actorTypeInfo);

                if (existingItem.Name.Equals(string.Format(ParamNameFormat, name, MinReplicaSetSizeParamName)) ||
                    existingItem.Name.Equals(string.Format(ParamNameFormat, name, TargetReplicaSetSizeParamName)))
                {
                    // Get GeneratedId Ref from the Default services for this actor.
                    if (toolContext.TryGetGeneratedIdRefForActorService(name, out var generatedIdRef))
                    {
                        // GeneratedIdRef is of format "Guid|StatePersistenceAttributeValue"
                        // If StatePersistence Value from GeneratedIdRef is different from the current value then override the param value.
                        var splitted = generatedIdRef.Split(GeneratedIdDelimiter);
                        if (splitted.Length == 2)
                        {
                            var statePersistenceValue = splitted[1];
                            var newPersistenceValue = actorTypeInfo.StatePersistence.ToString();

                            if (!statePersistenceValue.Equals(newPersistenceValue))
                            {
                                existingItem.DefaultValue = newItem.DefaultValue;
                            }
                        }
                    }

                    break;
                }
            }

            return existingItem;
        }

        private static ApplicationManifestTypeParameter[] MergeParameters(
            IEnumerable<ApplicationManifestTypeParameter> existingItems,
            IEnumerable<ApplicationManifestTypeParameter> newItems)
        {
            return MergeItems(
                existingItems,
                newItems,
                (i1, i2) =>
                    (string.CompareOrdinal(
                        i1.Name,
                        i2.Name) == 0),
                MergeParameters);
        }

        #endregion

        private static T[] MergeItems<T>(
            IEnumerable<T> existingItems,
            IEnumerable<T> newItems,
            Func<T, T, bool> isEquals,
            Func<T, T, T> itemMerge)
        {
            return MergeItems(
                existingItems,
                newItems,
                isEquals,
                itemMerge,
                t => true);
        }

        private static T[] MergeItems<T>(
            IEnumerable<T> existingItems,
            IEnumerable<T> newItems,
            Func<T, T, bool> isEquals,
            Func<T, T, T> itemMerge,
            Func<T, bool> shouldKeep)
        {
            var list1 = (existingItems == null ? new List<T>() : existingItems.ToList());
            var list2 = (newItems == null ? new List<T>() : newItems.ToList());
            var mergedList = new List<T>();

            // Order must be of existing list during merge.
            // If existing is 1,2,4,5 and new is 2,3 merged list should be 1,2,4,5,3 and not 2,3,1,4,5
            foreach (var item1 in list1)
            {
                // if its in list2, keep the merge
                var idx2 = list2.FindIndex(i => isEquals(i, item1));
                if (idx2 >= 0)
                {
                    mergedList.Add(itemMerge(item1, list2[idx2]));
                    list2.RemoveAt(idx2);
                }
                else
                {
                    // only keep it, if specified.
                    if (shouldKeep.Invoke(item1))
                    {
                        mergedList.Add(item1);
                    }
                }
            }

            mergedList.AddRange(list2);
            return mergedList.ToArray();
        }

        private static void InsertXmlCommentsAndWriteIfNeeded<T>(string filePath, string existingContents, T value)
            where T : class
        {
            var newContent = XmlSerializationUtility.InsertXmlComments(existingContents, value);
            Utility.WriteIfNeeded(filePath, existingContents, newContent);
        }

        private static string GetVersion()
        {
            return !string.IsNullOrEmpty(toolContext.Arguments.Version)
                ? toolContext.Arguments.Version
                : DefaultSemanticVersion;
        }

        private static string GetStatePersistenceValueForActorService(string actorService)
        {
            foreach (var actorTypeInfo in toolContext.Arguments.ActorTypes)
            {
                var serviceName = GetFabricServiceName(actorTypeInfo);

                if (serviceName.Equals(actorService))
                {
                    return actorTypeInfo.StatePersistence.ToString();
                }
            }

            return null;
        }

        public class Arguments
        {
            public string ApplicationPrefix { get; set; }

            public string ServicePackageNamePrefix { get; set; }

            public string OutputPath { get; set; }

            public string ApplicationPackagePath { get; set; }

            public string ServicePackagePath { get; set; }

            public Assembly InputAssembly { get; set; }

            public IList<ActorTypeInformation> ActorTypes { get; set; }

            public string Version { get; set; }

            public SvcManifestEntryPointType ServiceManifestEntryPointType { get; set; }
        }

        private class Context
        {
            private readonly Dictionary<string, HashSet<string>> existingGeneratedNames;

            private readonly Dictionary<string, string> existingActorServiceGeneratedIdRefNamesMap;

            public Context(Arguments arguments)
            {
                this.Arguments = arguments;
                if (this.ShouldGenerateApplicationManifest())
                {
                    this.ApplicationManifestFilePath = this.GetApplicationManifestFilePath();
                }

                this.ServiceManifestFilePath = this.GetServiceManifestFilePath();
                this.ConfigSettingsFilePath = this.GetConfigSettingsFilePath();
                this.existingGeneratedNames = new Dictionary<string, HashSet<string>>();
                this.existingActorServiceGeneratedIdRefNamesMap = new Dictionary<string, string>();
            }

            public string ApplicationManifestFilePath { get; private set; }

            public string ServiceManifestFilePath { get; private set; }

            public string ConfigSettingsFilePath { get; private set; }

            public string ExistingApplicationManifestContents { get; private set; }

            public string ExistingServiceManifestContents { get; private set; }

            public string ExistingConfigSettingsContents { get; private set; }

            public ApplicationManifestType ExistingApplicationManifestType { get; private set; }

            public ServiceManifestType ExistingServiceManifestType { get; private set; }

            public Arguments Arguments { get; private set; }

            public void LoadExistingContents()
            {
                // Load AppManifest
                if (this.ShouldGenerateApplicationManifest())
                {
                    Utility.EnsureParentFolder(this.ApplicationManifestFilePath);
                    this.ExistingApplicationManifestContents = Utility.LoadContents(this.ApplicationManifestFilePath).Trim();

                    this.ExistingApplicationManifestType = XmlSerializationUtility
                        .Deserialize<ApplicationManifestType>(
                            this.ExistingApplicationManifestContents);

                    // Create ActorService name and GeneratedIdRef map to be used while merging parameters later.
                    if (this.ExistingApplicationManifestType != null
                        && this.ExistingApplicationManifestType.DefaultServices != null
                        && this.ExistingApplicationManifestType.DefaultServices.Items != null)
                    {
                        foreach (var defaultService in this.ExistingApplicationManifestType.DefaultServices.Items)
                        {
                            var castedType = defaultService as DefaultServicesTypeService;
                            this.existingActorServiceGeneratedIdRefNamesMap.Add(castedType.Name, castedType.GeneratedIdRef);
                        }
                    }
                }

                // Load Service Manifest
                Utility.EnsureParentFolder(this.ServiceManifestFilePath);
                this.ExistingServiceManifestContents = Utility.LoadContents(this.ServiceManifestFilePath).Trim();
                this.ExistingServiceManifestType = XmlSerializationUtility
                    .Deserialize<ServiceManifestType>(
                        this.ExistingServiceManifestContents);

                // Load Config.
                Utility.EnsureParentFolder(this.ConfigSettingsFilePath);
                this.ExistingConfigSettingsContents = Utility.LoadContents(this.ConfigSettingsFilePath).Trim();

                this.LoadExistingGeneratedNames();
            }

            public void LoadExistingGeneratedNames()
            {
                if (this.ExistingServiceManifestType == null || this.ExistingServiceManifestType.ServiceTypes == null)
                {
                    return;
                }

                foreach (var serviceType in this.ExistingServiceManifestType.ServiceTypes)
                {
                    if (serviceType is ServiceTypeType castedServiceType)
                    {
                        if (castedServiceType.Extensions == null)
                        {
                            // Not a FabAct generated type
                            continue;
                        }

                        foreach (var extension in castedServiceType.Extensions)
                        {
                            if (extension.Name == GeneratedServiceTypeExtensionName)
                            {
                                if (!this.existingGeneratedNames.TryGetValue(extension.Name, out var existingTypes))
                                {
                                    existingTypes = new HashSet<string>();

                                    this.existingGeneratedNames.Add(
                                        extension.Name,
                                        existingTypes);
                                }

                                existingTypes.Add(castedServiceType.ServiceTypeName);

                                var xmlEnumerator = extension.Any.ChildNodes.GetEnumerator();
                                while (xmlEnumerator.MoveNext())
                                {
                                    var xml = xmlEnumerator.Current as XmlElement;

                                    if (xml == null)
                                    {
                                        continue;
                                    }

                                    if (!this.existingGeneratedNames.TryGetValue(xml.Name, out var existingNames))
                                    {
                                        existingNames = new HashSet<string>();

                                        this.existingGeneratedNames.Add(
                                            xml.Name,
                                            existingNames);
                                    }

                                    existingNames.Add(xml.GetAttribute(GeneratedNamesAttributeName));
                                }

                                break;
                            }
                        }
                    }
                }
            }

            public bool ShouldKeepItem(
                string name,
                string value)
            {
                return !this.IsExistingGeneratedName(name, value);
            }

            public bool ShouldKeepConfigSection(
                string value)
            {
                return
                    !(this.IsExistingGeneratedName(GeneratedStoreConfigSectionName, value) ||
                      this.IsExistingGeneratedName(GeneratedReplicatorConfigSectionName, value) ||
                      this.IsExistingGeneratedName(GeneratedReplicatorSecurityConfigSectionName, value));
            }

            public bool ShouldKeepEndpointResource(
                string value)
            {
                return
                    !(this.IsExistingGeneratedName(GeneratedServiceEndpointName, value) ||
                      this.IsExistingGeneratedName(GeneratedReplicatorEndpointName, value));
            }

            public bool ShouldGenerateApplicationManifest()
            {
                // Generates application manifest only when ApplicationPackagePath is non-empty,
                // or ApplicationPackagePath and ServicePackagePath are both empty.
                return
                    !string.IsNullOrEmpty(this.Arguments.ApplicationPackagePath) ||
                    string.IsNullOrEmpty(this.Arguments.ServicePackagePath);
            }

            public bool TryGetGeneratedIdRefForActorService(string actorService, out string generatedIdRef)
            {
                if (this.existingActorServiceGeneratedIdRefNamesMap.TryGetValue(actorService, out generatedIdRef))
                {
                    return true;
                }
                else
                {
                    generatedIdRef = null;
                    return false;
                }
            }

            private bool IsExistingGeneratedName(
                string name,
                string value)
            {
                if (this.existingGeneratedNames.TryGetValue(name, out var existingValues))
                {
                    return existingValues.Contains(value);
                }
                else
                {
                    return false;
                }
            }

            private string GetApplicationManifestFilePath()
            {
                if (!string.IsNullOrEmpty(this.Arguments.ApplicationPackagePath))
                {
                    return Path.Combine(this.Arguments.ApplicationPackagePath, ApplicationManifestFileName);
                }
                else if (this.ShouldGenerateApplicationManifest())
                {
                    var appManifestFilePath = Path.Combine(this.Arguments.OutputPath, ApplicationManifestFileName);
                    if (!File.Exists(appManifestFilePath))
                    {
                        appManifestFilePath = Path.Combine(
                            this.Arguments.OutputPath,
                            ActorNameFormat.GetFabricApplicationPackageName(this.Arguments.ApplicationPrefix),
                            ApplicationManifestFileName);
                    }

                    return appManifestFilePath;
                }

                return string.Empty;
            }

            private string GetServiceManifestFilePath()
            {
                var servicePackagePath = this.Arguments.ServicePackagePath;
                if (string.IsNullOrEmpty(servicePackagePath))
                {
                    var appManifestFilePath = this.GetApplicationManifestFilePath();

                    var appPackageFolder = Path.GetDirectoryName(appManifestFilePath) ??
                                           Path.Combine(
                                               this.Arguments.OutputPath,
                                               ActorNameFormat.GetFabricApplicationPackageName(
                                                   this.Arguments.ApplicationPrefix));

                    servicePackagePath = Path.Combine(
                        appPackageFolder,
                        ActorNameFormat.GetFabricServicePackageName(this.Arguments.ServicePackageNamePrefix));
                }

                var manifestFilePath = Path.Combine(
                    servicePackagePath,
                    ServiceManifestFileName);

                return manifestFilePath;
            }

            private string GetConfigSettingsFilePath()
            {
                var manifestFilePath = this.GetServiceManifestFilePath();

                var sevicePackageFolder = Path.GetDirectoryName(manifestFilePath) ??
                                          Path.Combine(
                                              this.Arguments.OutputPath,
                                              ActorNameFormat.GetFabricApplicationPackageName(
                                                  this.Arguments.ApplicationPrefix),
                                              ActorNameFormat.GetFabricServicePackageName(
                                                  this.Arguments.ServicePackageNamePrefix));

                var settingsFilePath = Path.Combine(
                    sevicePackageFolder,
                    ActorNameFormat.GetConfigPackageName(),
                    ConfigSettingsFileName);

                return settingsFilePath;
            }
        }
    }
}
