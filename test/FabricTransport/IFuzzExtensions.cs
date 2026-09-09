// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.

using System;
using Fuzzy;

namespace Microsoft.ServiceFabric.FabricTransport;

static class IFuzzExtensions
{
    internal static FabricTransportMessage FabricTransportMessage(this IFuzz fuzzy) => new(
        new FabricTransportRequestHeader(new ArraySegment<byte>(fuzzy.Array(fuzzy.Byte)), static () => { }),
        new FabricTransportRequestBody([new ArraySegment<byte>(fuzzy.Array(fuzzy.Byte))], static () => { }));
}
