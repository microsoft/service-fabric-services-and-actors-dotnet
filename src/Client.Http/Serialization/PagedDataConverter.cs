// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Collections.Generic;
using Microsoft.ServiceFabric.Common;
using Newtonsoft.Json;

namespace Microsoft.ServiceFabric.Client.Http.Serialization
{
    static class PagedDataConverter<T>
    {
        internal static PagedData<T> Deserialize(JsonReader reader, Func<JsonReader, T> deserializeFunc)
        {
            reader.ReadStartObject();
            var obj = GetFromJsonProperties(reader, deserializeFunc);
            reader.ReadEndObject();
            return obj;
        }

        internal static PagedData<T> GetFromJsonProperties(JsonReader reader, Func<JsonReader, T> deserializeFunc)
        {
            ContinuationToken continuationToken = default;
            IList<T> items = default;

            do
            {
                string propName = reader.ReadPropertyName();
                if (string.Compare("ContinuationToken", propName, StringComparison.OrdinalIgnoreCase) == 0)
                    continuationToken = ContinuationTokenConverter.Deserialize(reader);
                else if (string.Compare("Items", propName, StringComparison.OrdinalIgnoreCase) == 0)
                    items = reader.ReadList(deserializeFunc);
                else if (string.Compare("History", propName, StringComparison.OrdinalIgnoreCase) == 0)
                    items = reader.ReadList(deserializeFunc);
                else if (string.Compare("Properties", propName, StringComparison.OrdinalIgnoreCase) == 0)
                    items = reader.ReadList(deserializeFunc);
                else
                    reader.SkipPropertyValue();
            }
            while (reader.TokenType != JsonToken.EndObject);

            return new PagedData<T>(continuationToken, items);
        }
    }
}
