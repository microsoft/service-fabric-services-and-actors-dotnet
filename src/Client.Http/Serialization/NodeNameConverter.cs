// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using Microsoft.ServiceFabric.Common;
using Newtonsoft.Json;

namespace Microsoft.ServiceFabric.Client.Http.Serialization
{
    static class NodeNameConverter
    {
        internal static NodeName Deserialize(JsonReader reader) =>
            new NodeName(reader.ReadValueAsString());

        internal static void Serialize(JsonWriter writer, NodeName value) =>
            writer.WriteValue(value.ToString());
    }
}
