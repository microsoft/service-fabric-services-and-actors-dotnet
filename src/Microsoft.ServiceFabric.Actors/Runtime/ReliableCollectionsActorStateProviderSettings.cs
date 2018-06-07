// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Runtime
{
    using System.Fabric;

    internal sealed class ReliableCollectionsActorStateProviderSettings : ActorStateProviderSettings
    {
        public ReliableCollectionsActorStateProviderSettings()
        {
        }

        public static ReliableCollectionsActorStateProviderSettings LoadFrom(
            ICodePackageActivationContext activationContext,
            string configPackageName,
            string sectionName)
        {
            var settings = new ReliableCollectionsActorStateProviderSettings();

            settings.LoadFromSettings(activationContext, configPackageName, sectionName);

            return settings;
        }
    }
}
