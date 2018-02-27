// ------------------------------------------------------------
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT License (MIT).See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Remoting.V2.Messaging
{
    using System;
    using System.IO;

    internal interface IMessageHeader : IDisposable
    {
        ArraySegment<byte> GetSendBuffer();

        Stream GetReceivedBuffer();
    }
}
