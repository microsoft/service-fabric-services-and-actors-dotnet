// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using Microsoft.ServiceFabric.Common;
using Newtonsoft.Json;

namespace Microsoft.ServiceFabric.Client.Http.Serialization
{
    static class ReplicaIdConverter
    {
        internal static ReplicaId Deserialize(JsonReader reader) =>
            new ReplicaId(reader.ReadValueAsLong());

        internal static void Serialize(JsonWriter writer, ReplicaId value) =>
            writer.WriteValue(value.ToString());
    }
}
