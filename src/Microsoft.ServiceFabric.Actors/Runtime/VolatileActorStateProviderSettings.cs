// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Runtime
{
    using System.Fabric;
    using System.Fabric.Description;

    internal sealed class VolatileActorStateProviderSettings : ActorStateProviderSettings
    {
        public VolatileActorStateProviderSettings()
        { 
        }
        
        public static VolatileActorStateProviderSettings LoadFrom(
            ICodePackageActivationContext activationContext,
            string configPackageName,
            string sectionName)
        {
            var settings = new VolatileActorStateProviderSettings();

            settings.LoadFromSettings(activationContext, configPackageName, sectionName);

            return settings;
        }
    }
}