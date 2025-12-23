// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Fabric.Interop;
using System.Fabric.Management.ServiceModel;

namespace Microsoft.ServiceFabric.FabricTransport
{
    internal class SettingsConfigParser : IFabricServiceConfigParser
    {
        public SettingsType Parse(String fileName)
        {
            return Utility.ReadXml<SettingsType>(fileName);
        }
    }
}
