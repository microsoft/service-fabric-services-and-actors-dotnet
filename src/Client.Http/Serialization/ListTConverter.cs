// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Client.Http.Serialization
{
    using System;
    using System.Collections.Generic;
    using Newtonsoft.Json;

    /// <summary>
    /// Converter for PagedData.
    /// </summary>
    /// <typeparam name="T">Type of items contained in list.</typeparam>
    internal class ListTConverter<T>
    {
        /// <summary>
        /// Serializes the object to JSON.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="sequence">The object to serialize to JSON.</param>
        /// <param name="serializeFunc">Delegte to serialize T.</param>
        public static void Serialize(JsonWriter writer, IEnumerable<T> sequence, Action<JsonWriter, T> serializeFunc)
        {
            writer.WriteEnumerableValue(sequence, serializeFunc);
        }
    }
}
