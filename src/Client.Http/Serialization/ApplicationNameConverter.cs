// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using Microsoft.ServiceFabric.Common;
using Newtonsoft.Json;

namespace Microsoft.ServiceFabric.Client.Http.Serialization
{
    static class ApplicationNameConverter
    {
        internal static ApplicationName Deserialize(JsonReader reader) =>
            new ApplicationName(reader.ReadValueAsString());

        internal static void Serialize(JsonWriter writer, ApplicationName value) =>
            writer.WriteValue(value.ToString());
    }
}
