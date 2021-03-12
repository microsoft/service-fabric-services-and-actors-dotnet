// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Generator
{
    using System.Collections.Generic;
    using System.Fabric.Management.ServiceModel;
    using System.Linq;
    using Microsoft.ServiceFabric.Actors.Runtime;
    using Microsoft.ServiceFabric.StartupServicesUtility;

    internal class AppParameterFileUpdater
    {
        private const string ParamNameFormat = "{0}_{1}";

        private const string PartitionCountParamName = "PartitionCount";
        private const int LocalFiveNodeDefaultPartitionCount = 1;
        private const int LocalOneNodeDefaultPartitionCount = 1;

        private const string TargetReplicaSetSizeParamName = "TargetReplicaSetSize";
        private const int LocalOneNodeDefaultTargetReplicaSetSize = 1;

        private const string MinReplicaSetSizeParamName = "MinReplicaSetSize";
        private const int LocalOneNodeDefaultMinReplicaSetSize = 1;

        internal static void AddServiceParameterValuesToLocalFiveNodeParamFile(Arguments arguments)
        {
            var serviceParamFileContents = Utility.LoadContents(arguments.ServiceParamFilePath).Trim();
            var serviceInstanceDefinition = XmlSerializationUtility.Deserialize<StartupServiceInstanceDefinitionType>(serviceParamFileContents);

            var newServiceParams =
                arguments.ActorTypes.Select(
                    actorTypeInfo =>
                    {
                        var serviceName = ActorNameFormat.GetFabricServiceName(actorTypeInfo.InterfaceTypes.First(), actorTypeInfo.ServiceName);

                        return new StartupServiceInstanceDefinitionTypeParameter
                        {
                            Name = string.Format(ParamNameFormat, serviceName, PartitionCountParamName),
                            Value = LocalFiveNodeDefaultPartitionCount.ToString(),
                        };
                    });

            // Create new parameters for Actor Types and merge it with existing service Parameters.
            serviceInstanceDefinition.Parameters = MergeServiceParams(serviceInstanceDefinition.Parameters, newServiceParams);

            var newContent = XmlSerializationUtility.InsertXmlComments(serviceParamFileContents, serviceInstanceDefinition);
            Utility.WriteIfNeeded(arguments.ServiceParamFilePath, serviceParamFileContents, newContent);
        }

        internal static void AddServiceParameterValuesToLocalOneNodeParamFile(Arguments arguments)
        {
            var serviceParamFileContents = Utility.LoadContents(arguments.ServiceParamFilePath).Trim();
            var serviceInstanceDefinition = XmlSerializationUtility.Deserialize<StartupServiceInstanceDefinitionType>(serviceParamFileContents);
            var newAppParams = new List<StartupServiceInstanceDefinitionTypeParameter>();

            // Create new parameters for Actor Types and merge it with existing Parameters.
            foreach (var actorTypeInfo in arguments.ActorTypes)
            {
                var serviceName = ActorNameFormat.GetFabricServiceName(actorTypeInfo.InterfaceTypes.First(), actorTypeInfo.ServiceName);

                newAppParams.Add(
                    new StartupServiceInstanceDefinitionTypeParameter
                    {
                        Name = string.Format(ParamNameFormat, serviceName, PartitionCountParamName),
                        Value = LocalOneNodeDefaultPartitionCount.ToString(),
                    });

                newAppParams.Add(
                    new StartupServiceInstanceDefinitionTypeParameter
                    {
                        Name = string.Format(ParamNameFormat, serviceName, TargetReplicaSetSizeParamName),
                        Value = LocalOneNodeDefaultTargetReplicaSetSize.ToString(),
                    });
                newAppParams.Add(
                    new StartupServiceInstanceDefinitionTypeParameter
                    {
                        Name = string.Format(ParamNameFormat, serviceName, MinReplicaSetSizeParamName),
                        Value = LocalOneNodeDefaultMinReplicaSetSize.ToString(),
                    });
            }

            serviceInstanceDefinition.Parameters = MergeServiceParams(serviceInstanceDefinition.Parameters, newAppParams);

            var newContent = XmlSerializationUtility.InsertXmlComments(serviceParamFileContents, serviceInstanceDefinition);
            Utility.WriteIfNeeded(arguments.ServiceParamFilePath, serviceParamFileContents, newContent);
        }

        internal static void AddParameterValuesToLocalFiveNodeParamFile(Arguments arguments)
        {
            var appParamFileContents = Utility.LoadContents(arguments.AppParamFilePath).Trim();
            var appInstanceDefinition = XmlSerializationUtility.Deserialize<AppInstanceDefinitionType>(appParamFileContents);

            var newAppParams =
                arguments.ActorTypes.Select(
                    actorTypeInfo =>
                    {
                        var serviceName = ActorNameFormat.GetFabricServiceName(actorTypeInfo.InterfaceTypes.First(), actorTypeInfo.ServiceName);

                        return new AppInstanceDefinitionTypeParameter
                        {
                            Name = string.Format(ParamNameFormat, serviceName, PartitionCountParamName),
                            Value = LocalFiveNodeDefaultPartitionCount.ToString(),
                        };
                    });

            // Create new parameters for Actor Types and merge it with existing Parameters.
            appInstanceDefinition.Parameters = MergeAppParams(appInstanceDefinition.Parameters, newAppParams);

            var newContent = XmlSerializationUtility.InsertXmlComments(appParamFileContents, appInstanceDefinition);
            Utility.WriteIfNeeded(arguments.AppParamFilePath, appParamFileContents, newContent);
        }

        internal static void AddParameterValuesToLocalOneNodeParamFile(Arguments arguments)
        {
            var appParamFileContents = Utility.LoadContents(arguments.AppParamFilePath).Trim();
            var appInstanceDefinition = XmlSerializationUtility.Deserialize<AppInstanceDefinitionType>(appParamFileContents);
            var newAppParams = new List<AppInstanceDefinitionTypeParameter>();

            // Create new parameters for Actor Types and merge it with existing Parameters.
            foreach (var actorTypeInfo in arguments.ActorTypes)
            {
                var serviceName = ActorNameFormat.GetFabricServiceName(actorTypeInfo.InterfaceTypes.First(), actorTypeInfo.ServiceName);

                newAppParams.Add(
                    new AppInstanceDefinitionTypeParameter
                    {
                        Name = string.Format(ParamNameFormat, serviceName, PartitionCountParamName),
                        Value = LocalOneNodeDefaultPartitionCount.ToString(),
                    });

                newAppParams.Add(
                    new AppInstanceDefinitionTypeParameter
                    {
                        Name = string.Format(ParamNameFormat, serviceName, TargetReplicaSetSizeParamName),
                        Value = LocalOneNodeDefaultTargetReplicaSetSize.ToString(),
                    });
                newAppParams.Add(
                    new AppInstanceDefinitionTypeParameter
                    {
                        Name = string.Format(ParamNameFormat, serviceName, MinReplicaSetSizeParamName),
                        Value = LocalOneNodeDefaultMinReplicaSetSize.ToString(),
                    });
            }

            appInstanceDefinition.Parameters = MergeAppParams(appInstanceDefinition.Parameters, newAppParams);

            var newContent = XmlSerializationUtility.InsertXmlComments(appParamFileContents, appInstanceDefinition);
            Utility.WriteIfNeeded(arguments.AppParamFilePath, appParamFileContents, newContent);
        }

        internal static StartupServiceInstanceDefinitionTypeParameter[] MergeServiceParams(
            IEnumerable<StartupServiceInstanceDefinitionTypeParameter> existingItems,
            IEnumerable<StartupServiceInstanceDefinitionTypeParameter> newItems)
        {
            // Add new parameters if not already exist in the app instance definition file.
            if (existingItems == null)
            {
                return newItems.ToArray();
            }
            else
            {
                // Only add the Parameter if it doesnt exist already.
                var existingParamNames = existingItems.Select(x => x.Name);
                var updatedItemsList = existingItems.ToList();
                updatedItemsList.AddRange(newItems.Where(newParam => !existingParamNames.Contains(newParam.Name)));

                return updatedItemsList.ToArray();
            }
        }

        internal static AppInstanceDefinitionTypeParameter[] MergeAppParams(
            IEnumerable<AppInstanceDefinitionTypeParameter> existingItems,
            IEnumerable<AppInstanceDefinitionTypeParameter> newItems)
        {
            // Add new parameters if not already exist in the app instance definition file.
            if (existingItems == null)
            {
                return newItems.ToArray();
            }
            else
            {
                // Only add the Parameter if it doesnt exist already.
                var existingParamNames = existingItems.Select(x => x.Name);
                var updatedItemsList = existingItems.ToList();
                updatedItemsList.AddRange(newItems.Where(newParam => !existingParamNames.Contains(newParam.Name)));

                return updatedItemsList.ToArray();
            }
        }

        public class Arguments
        {
            public string AppParamFilePath { get; set; }

            public string ServiceParamFilePath { get; set; }

            public IList<ActorTypeInformation> ActorTypes { get; set; }
        }
    }
}
