// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Client.Http.Serialization
{
    using Microsoft.ServiceFabric.Common;
    using Newtonsoft.Json;

    static class ServiceNameConverter
    {
        internal static ServiceName Deserialize(JsonReader reader) =>
            new ServiceName(reader.ReadValueAsString());

        internal static void Serialize(JsonWriter writer, ServiceName value) =>
            writer.WriteValue(value.ToString());
    }
}
