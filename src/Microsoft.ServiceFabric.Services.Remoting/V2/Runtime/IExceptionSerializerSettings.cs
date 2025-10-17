// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using Microsoft.ServiceFabric.Services.Remoting.FabricTransport.Runtime;

namespace Microsoft.ServiceFabric.Services.Remoting.V2.Runtime
{
    interface IExceptionSerializerSettings
    {
        int RemotingExceptionDepth { get; }
#pragma warning disable 618
        FabricTransportRemotingListenerSettings.ExceptionSerialization ExceptionSerializationTechnique { get; }
#pragma warning restore 618
    }
}