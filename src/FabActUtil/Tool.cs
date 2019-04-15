// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace FabActUtil
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Reflection;
    using Microsoft.ServiceFabric.Actors;
    using Microsoft.ServiceFabric.Actors.Generator;
    using Microsoft.ServiceFabric.Actors.Runtime;

    internal class Tool
    {
        public static void Run(ToolArguments arguments)
        {
            // create tool context
            var context = new ToolContext { Arguments = arguments };

            // process the arguments
            ProcessArguments(context);

            // process the input
            ProcessInput(context);

            // generate the output
            GenerateOutput(context);
        }

        private static void ProcessArguments(ToolContext context)
        {
            if (string.IsNullOrEmpty(context.Arguments.OutputPath))
            {
                context.Arguments.OutputPath = Directory.GetCurrentDirectory();
            }

            // validate the arguments
        }

        private static void ProcessInput(ToolContext context)
        {
            if (context.Arguments.Target == OutputTarget.Manifest)
            {
                LoadInputAssembly(context);
                LoadActors(context);
            }
        }

        private static void GenerateOutput(ToolContext context)
        {
            if (context.Arguments.Target == OutputTarget.Manifest)
            {
                GenerateManifest(context);
                AddParametersToLocalFiveNodeAppParamFile(context);
                AddParametersToLocalOneNodeAppParamFile(context);
                return;
            }
        }

        private static void GenerateManifest(ToolContext context)
        {
            var serviceManifestEntryPointType = SvcManifestEntryPointType.Exe;

            if (!Enum.TryParse(context.Arguments.ServiceManifestEntryPointType, out serviceManifestEntryPointType))
            {
                serviceManifestEntryPointType = SvcManifestEntryPointType.Exe;
            }

            var generatorArgs = new ManifestGenerator.Arguments()
            {
                ApplicationPrefix = context.Arguments.ApplicationPrefix,
                ServicePackageNamePrefix = context.Arguments.ServicePackagePrefix,
                InputAssembly = context.InputAssembly,
                ActorTypes = context.ActorTypes,
                OutputPath = context.Arguments.OutputPath,
                ApplicationPackagePath = context.Arguments.ApplicationPackagePath,
                ServicePackagePath = context.Arguments.ServicePackagePath,
                Version = context.Arguments.Version,
                ServiceManifestEntryPointType = serviceManifestEntryPointType,
            };

            ManifestGenerator.Generate(generatorArgs);
        }

        private static void AddParametersToLocalFiveNodeAppParamFile(ToolContext context)
        {
            if (string.IsNullOrEmpty(context.Arguments.Local5NodeAppParamFile))
            {
                return;
            }

            var updaterArgs = new AppParameterFileUpdater.Arguments()
            {
                ActorTypes = context.ActorTypes,
                AppParamFilePath = context.Arguments.Local5NodeAppParamFile,
            };

            AppParameterFileUpdater.AddParameterValuesToLocalFiveNodeParamFile(updaterArgs);
        }

        private static void AddParametersToLocalOneNodeAppParamFile(ToolContext context)
        {
            if (string.IsNullOrEmpty(context.Arguments.Local1NodeAppParamFile))
            {
                return;
            }

            var updaterArgs = new AppParameterFileUpdater.Arguments()
            {
                ActorTypes = context.ActorTypes,
                AppParamFilePath = context.Arguments.Local1NodeAppParamFile,
            };

            AppParameterFileUpdater.AddParameterValuesToLocalOneNodeParamFile(updaterArgs);
        }

        private static void LoadInputAssembly(ToolContext context)
        {
            context.InputAssembly = Assembly.LoadFrom(context.Arguments.InputAssembly);
        }

        private static void LoadActors(ToolContext context)
        {
            var inputAssembly = context.InputAssembly;
            var actorTypes = context.ActorTypes;
            IList<string> actorFilters = null;

            if ((context.Arguments.Actors != null) && (context.Arguments.Actors.Length > 0))
            {
                actorFilters = new List<string>(context.Arguments.Actors);
            }

            LoadActors(inputAssembly, actorFilters, actorTypes);

            // check if all specified actor types were loaded or not
            if ((actorFilters != null) && (actorFilters.Count > 0))
            {
                throw new TypeLoadException(
                    string.Format(
                        CultureInfo.CurrentCulture,
                        Microsoft.ServiceFabric.Actors.SR.ErrorNotAnActor,
                        actorFilters[0],
                        typeof(Actor).FullName));
            }
        }

        private static void LoadActors(
            Assembly inputAssembly,
            IList<string> actorFilters,
            IList<ActorTypeInformation> actorTypes)
        {
            var actorTypeInfoTable = new Dictionary<Type, ActorTypeInformation>();
            foreach (var t in inputAssembly.GetTypes())
            {
                if (!t.IsActor())
                {
                    continue;
                }

                var actorTypeInformation = ActorTypeInformation.Get(t);
                if (actorTypeInformation.IsAbstract)
                {
                    continue;
                }

                CheckForDuplicateFabricServiceName(actorTypeInfoTable, actorTypeInformation);

                if (actorFilters != null)
                {
                    if (RemoveFrom(actorFilters, t.FullName, StringComparison.OrdinalIgnoreCase))
                    {
                        actorTypes.Add(actorTypeInformation);
                    }
                }
                else
                {
                    actorTypes.Add(actorTypeInformation);
                }
            }
        }

        private static void CheckForDuplicateFabricServiceName(
            IDictionary<Type, ActorTypeInformation> actorTypeInfoTable, ActorTypeInformation actorTypeInformation)
        {
            foreach (var actorTypeInterface in actorTypeInformation.InterfaceTypes)
            {
                if (actorTypeInfoTable.ContainsKey(actorTypeInterface))
                {
                    // ensure that both of types have non-null actor service attribute with name
                    if (string.IsNullOrEmpty(actorTypeInformation.ServiceName) ||
                        string.IsNullOrEmpty(actorTypeInfoTable[actorTypeInterface].ServiceName))
                    {
                        if (actorTypeInformation.ImplementationType.IsAssignableFrom(
                            actorTypeInfoTable[actorTypeInterface].ImplementationType))
                        {
                            throw new TypeLoadException(
                                string.Format(
                                    CultureInfo.CurrentCulture,
                                    Microsoft.ServiceFabric.Actors.SR.ErrorNoActorServiceNameMultipleImplDerivation,
                                    actorTypeInterface.FullName,
                                    actorTypeInfoTable[actorTypeInterface].ImplementationType.FullName,
                                    actorTypeInformation.ImplementationType.FullName,
                                    typeof(ActorServiceAttribute).FullName));
                        }
                        else if (actorTypeInfoTable[actorTypeInterface].ImplementationType.IsAssignableFrom(
                            actorTypeInformation.ImplementationType))
                        {
                            throw new TypeLoadException(
                                string.Format(
                                    CultureInfo.CurrentCulture,
                                    Microsoft.ServiceFabric.Actors.SR.ErrorNoActorServiceNameMultipleImplDerivation,
                                    actorTypeInterface.FullName,
                                    actorTypeInformation.ImplementationType.FullName,
                                    actorTypeInfoTable[actorTypeInterface].ImplementationType.FullName,
                                    typeof(ActorServiceAttribute).FullName));
                        }
                        else
                        {
                            throw new TypeLoadException(
                                string.Format(
                                    CultureInfo.CurrentCulture,
                                    Microsoft.ServiceFabric.Actors.SR.ErrorNoActorServiceNameMultipleImpl,
                                    actorTypeInterface.FullName,
                                    actorTypeInformation.ImplementationType.FullName,
                                    actorTypeInfoTable[actorTypeInterface].ImplementationType.FullName,
                                    typeof(ActorServiceAttribute).FullName));
                        }
                    }
                }
                else
                {
                    actorTypeInfoTable.Add(actorTypeInterface, actorTypeInformation);
                }
            }
        }

        private static bool RemoveFrom(IList<string> list, string item, StringComparison comparision)
        {
            for (var i = 0; i < list.Count; i++)
            {
                if (string.Compare(list[i], item, comparision) == 0)
                {
                    list.RemoveAt(i);
                    return true;
                }
            }

            return false;
        }

        private static string GetToolPath()
        {
            var codeBase = Assembly.GetEntryAssembly().CodeBase;
            var uri = new UriBuilder(codeBase);
            return Uri.UnescapeDataString(uri.Path);
        }
    }
}
