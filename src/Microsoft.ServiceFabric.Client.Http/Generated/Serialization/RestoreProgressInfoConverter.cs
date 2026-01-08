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
    /// Converter for <see cref="RestoreProgressInfo" />.
    /// </summary>
    internal class RestoreProgressInfoConverter
    {
        /// <summary>
        /// Deserializes the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static RestoreProgressInfo Deserialize(JsonReader reader)
        {
            return reader.Deserialize(GetFromJsonProperties);
        }

        /// <summary>
        /// Gets the object from Json properties.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The object Value.</returns>
        internal static RestoreProgressInfo GetFromJsonProperties(JsonReader reader)
        {
            var restoreState = default(RestoreState?);
            var timeStampUtc = default(DateTime?);
            var restoredEpoch = default(Epoch);
            var restoredLsn = default(string);
            var failureError = default(FabricErrorError);

            do
            {
                var propName = reader.ReadPropertyName();
                if (string.Compare("RestoreState", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    restoreState = RestoreStateConverter.Deserialize(reader);
                }
                else if (string.Compare("TimeStampUtc", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    timeStampUtc = reader.ReadValueAsDateTime();
                }
                else if (string.Compare("RestoredEpoch", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    restoredEpoch = EpochConverter.Deserialize(reader);
                }
                else if (string.Compare("RestoredLsn", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    restoredLsn = reader.ReadValueAsString();
                }
                else if (string.Compare("FailureError", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    failureError = FabricErrorErrorConverter.Deserialize(reader);
                }
                else
                {
                    reader.SkipPropertyValue();
                }
            }
            while (reader.TokenType != JsonToken.EndObject);

            return new RestoreProgressInfo(
                restoreState: restoreState,
                timeStampUtc: timeStampUtc,
                restoredEpoch: restoredEpoch,
                restoredLsn: restoredLsn,
                failureError: failureError);
        }

        /// <summary>
        /// Serializes the object to JSON.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="obj">The object to serialize to JSON.</param>
        internal static void Serialize(JsonWriter writer, RestoreProgressInfo obj)
        {
            // Required properties are always serialized, optional properties are serialized when not null.
            writer.WriteStartObject();
            writer.WriteProperty(obj.RestoreState, "RestoreState", RestoreStateConverter.Serialize);
            if (obj.TimeStampUtc != null)
            {
                writer.WriteProperty(obj.TimeStampUtc, "TimeStampUtc", JsonWriterExtensions.WriteDateTimeValue);
            }

            if (obj.RestoredEpoch != null)
            {
                writer.WriteProperty(obj.RestoredEpoch, "RestoredEpoch", EpochConverter.Serialize);
            }

            if (obj.RestoredLsn != null)
            {
                writer.WriteProperty(obj.RestoredLsn, "RestoredLsn", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.FailureError != null)
            {
                writer.WriteProperty(obj.FailureError, "FailureError", FabricErrorErrorConverter.Serialize);
            }

            writer.WriteEndObject();
        }
    }
}
