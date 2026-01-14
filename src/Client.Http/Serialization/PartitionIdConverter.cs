// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using Microsoft.ServiceFabric.Common;
using Newtonsoft.Json;

namespace Microsoft.ServiceFabric.Client.Http.Serialization
{
    static class PartitionIdConverter
    {
        internal static PartitionId Deserialize(JsonReader reader) =>
            new PartitionId(reader.ReadValueAsGuid());

        internal static void Serialize(JsonWriter writer, PartitionId value) =>
            writer.WriteValue(value.ToString());
    }
}
