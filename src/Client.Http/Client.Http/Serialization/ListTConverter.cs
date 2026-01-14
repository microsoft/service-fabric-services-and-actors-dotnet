// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Microsoft.ServiceFabric.Client.Http.Serialization
{
    static class ListTConverter<T>
    {
        internal static void Serialize(JsonWriter writer, IEnumerable<T> sequence, Action<JsonWriter, T> serializeFunc) =>
            writer.WriteEnumerableValue(sequence, serializeFunc);
    }
}
