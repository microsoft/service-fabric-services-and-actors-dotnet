// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.KVSToRCMigration.Controllers
{
    using System.Reflection;
    using Microsoft.AspNetCore.Mvc.Controllers;

    /// <summary>
    /// MigrationControllerFeatureProvider class to override IsController to be able
    /// to load internal migration controllers
    /// </summary>
    internal class MigrationControllerFeatureProvider : ControllerFeatureProvider
    {
        protected override bool IsController(TypeInfo typeInfo)
        {
            var isMigrationController = !typeInfo.IsAbstract
                && (typeof(RcMigrationController).IsAssignableFrom(typeInfo)
                    || typeof(KvsMigrationController).IsAssignableFrom(typeInfo));
            return isMigrationController || base.IsController(typeInfo);
        }
    }
}
