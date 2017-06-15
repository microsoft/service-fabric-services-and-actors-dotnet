// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------
namespace Microsoft.ServiceFabric.Actors.Generator
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Fabric.Management.ServiceModel;
    using Microsoft.ServiceFabric.Actors.Runtime;

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
                            Value = LocalFiveNodeDefaultPartitionCount.ToString()
                        };
                    });

            // Creates new parameters for Actor Types and merge it with existing Parameters.
            appInstanceDefinition.Parameters = MergeAppParams(appInstanceDefinition.Parameters, newAppParams);

            string newContent = XmlSerializationUtility.InsertXmlComments(appParamFileContents, appInstanceDefinition);
            Utility.WriteIfNeeded(arguments.AppParamFilePath, appParamFileContents, newContent);
        }

        internal static void AddParameterValuesToLocalOneNodeParamFile(Arguments arguments)
        {
            var appParamFileContents = Utility.LoadContents(arguments.AppParamFilePath).Trim();
            var appInstanceDefinition = XmlSerializationUtility.Deserialize<AppInstanceDefinitionType>(appParamFileContents);
            var newAppParams = new List<AppInstanceDefinitionTypeParameter>();

            // Creates new parameters for Actor Types and merge it with existing Parameters.
            foreach (var actorTypeInfo in arguments.ActorTypes)
            {
                var serviceName = ActorNameFormat.GetFabricServiceName(actorTypeInfo.InterfaceTypes.First(), actorTypeInfo.ServiceName);

                newAppParams.Add(
                    new AppInstanceDefinitionTypeParameter
                    {
                        Name = string.Format(ParamNameFormat, serviceName, PartitionCountParamName),
                        Value = LocalOneNodeDefaultPartitionCount.ToString()
                    });

                newAppParams.Add(
                    new AppInstanceDefinitionTypeParameter
                    {
                        Name = string.Format(ParamNameFormat, serviceName, TargetReplicaSetSizeParamName),
                        Value = LocalOneNodeDefaultTargetReplicaSetSize.ToString()
                    });
                newAppParams.Add(
                    new AppInstanceDefinitionTypeParameter
                    {
                        Name = string.Format(ParamNameFormat, serviceName, MinReplicaSetSizeParamName),
                        Value = LocalOneNodeDefaultMinReplicaSetSize.ToString()
                    });
            }

            appInstanceDefinition.Parameters = MergeAppParams(appInstanceDefinition.Parameters, newAppParams);

            string newContent = XmlSerializationUtility.InsertXmlComments(appParamFileContents, appInstanceDefinition);
            Utility.WriteIfNeeded(arguments.AppParamFilePath, appParamFileContents, newContent);
        }

        internal static AppInstanceDefinitionTypeParameter[] MergeAppParams(IEnumerable<AppInstanceDefinitionTypeParameter> existingItems,
            IEnumerable<AppInstanceDefinitionTypeParameter> newItems)
        {
            // Adds new parameters if not already exist in the app instance definition file.
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

            public IList<ActorTypeInformation> ActorTypes { get; set; }

        }
    }
}
