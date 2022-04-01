// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Runtime.Migration
{
    using System;
    using System.Fabric;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.ServiceFabric.Actors.Generator;

    internal static class MigrationReflectionHelper
    {
        private static readonly string TraceType = typeof(MigrationReflectionHelper).Name;

        public static IMigrationOrchestrator GetMigrationOrchestrator(
            IActorStateProvider stateProvider,
            ActorTypeInformation actorTypeInfo,
            StatefulServiceContext serviceContext,
            MigrationSettings settings)
        {
            try
            {
                var migrationAttribute = StateMigrationAttribute.Get(actorTypeInfo.ImplementationType);
                if (migrationAttribute.StateMigration == StateMigration.None)
                {
                    return null;
                }

                MigrationSettings migrationSettings = settings;
                if (migrationSettings == null)
                {
                    migrationSettings = new MigrationSettings();
                    migrationSettings.LoadFrom(
                         serviceContext.CodePackageActivationContext,
                         ActorNameFormat.GetMigrationConfigSectionName(actorTypeInfo.ImplementationType));
                }

                string reflectionClassFQN;
                if (migrationAttribute.StateMigration == StateMigration.Source)
                {
                    reflectionClassFQN = migrationSettings.MigrationSourceOrchestrator;
                }
                else
                {
                    reflectionClassFQN = migrationSettings.MigrationTargetOrchestrator;
                }

                var tokens = reflectionClassFQN.Split(',');
                if (tokens.Length != 2)
                {
                    throw new ArgumentException("Migration<>Orchestrator is invalid. Valid format is <orchestrator class FQN>, <Dll name>");
                }

                var className = tokens[0].Trim();
                var assemblyName = tokens[1].Trim();
                var currentAssembly = typeof(MigrationReflectionHelper).GetTypeInfo().Assembly;
                var actorsMigrationAssembly = new AssemblyName
                {
                    Name = assemblyName,
                    Version = currentAssembly.GetName().Version,
#if !DotNetCoreClr
                    CultureInfo = currentAssembly.GetName().CultureInfo,
#endif
                    ProcessorArchitecture = currentAssembly.GetName().ProcessorArchitecture,
                };

                actorsMigrationAssembly.SetPublicKeyToken(currentAssembly.GetName().GetPublicKeyToken());

                var orchestratorType = Type.GetType($"{className}, {assemblyName}", true);

                // TODO provide a singleton pattern or initialization
                return (IMigrationOrchestrator)Activator.CreateInstance(orchestratorType, stateProvider, actorTypeInfo, serviceContext, migrationSettings);
            }
            catch (Exception ex)
            {
                ActorTrace.Source.WriteErrorWithId(
                    TraceType,
                    serviceContext.TraceId,
                    $"Error encountered while creating migration orchestrator : {ex}");
                //// TODO: Partition health warning
            }

            return null;
        }
    }
}
