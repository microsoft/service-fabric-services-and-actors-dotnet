// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using Newtonsoft.Json;

namespace Microsoft.ServiceFabric.Client.Http.Serialization
{
    static class StringConverter
    {
        internal static void Serialize(JsonWriter writer, string value) =>
            writer.WriteValue(value.ToString());
    }
}
