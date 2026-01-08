// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Client.Http.Serialization
{
    using System;
    using System.Collections.Generic;
    using Microsoft.ServiceFabric.Common;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Converter for PagedData.
    /// </summary>
    /// <typeparam name="T">Type of items contained in list.</typeparam>
    internal class PagedDataConverter<T>
    {
        /// <summary>
        /// Deserializes the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <param name="deserializeFunc">Deserializer Func for T.</param>
        /// <returns>The object Value.</returns>
        public static PagedData<T> Deserialize(JsonReader reader, Func<JsonReader, T> deserializeFunc)
        {
            reader.ReadStartObject();
            var obj = GetFromJsonProperties(reader, deserializeFunc);
            reader.ReadEndObject();
            return obj;
        }

        /// <summary>
        /// Gets the object from Json properties.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <param name="deserializeFunc">Deserializer Func for T.</param>
        /// <returns>The object Value.</returns>
        public static PagedData<T> GetFromJsonProperties(JsonReader reader, Func<JsonReader, T> deserializeFunc)
        {
            var continuationToken = default(ContinuationToken);
            var items = default(IList<T>);

            do
            {
                var propName = reader.ReadPropertyName();
                if (string.Compare("ContinuationToken", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    continuationToken = ContinuationTokenConverter.Deserialize(reader);
                }
                else if (string.Compare("Items", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    items = reader.ReadList(deserializeFunc);
                }
                else if (string.Compare("History", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    items = reader.ReadList(deserializeFunc);
                }
                else if (string.Compare("Properties", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    items = reader.ReadList(deserializeFunc);
                }
                else
                {
                    reader.SkipPropertyValue();
                }
            }
            while (reader.TokenType != JsonToken.EndObject);

            return new PagedData<T>(continuationToken, items);
        }
    }
}
