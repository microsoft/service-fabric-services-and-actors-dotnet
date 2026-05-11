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
    /// Converter for <see cref="KeyValueStoreESEReplicaCopyDetail" />.
    /// </summary>
    internal class KeyValueStoreESEReplicaCopyDetailConverter
    {
        /// <summary>
        /// Deserializes the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static KeyValueStoreESEReplicaCopyDetail Deserialize(JsonReader reader)
        {
            return reader.Deserialize(GetFromJsonProperties);
        }

        /// <summary>
        /// Gets the object from Json properties.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The object Value.</returns>
        internal static KeyValueStoreESEReplicaCopyDetail GetFromJsonProperties(JsonReader reader)
        {
            var primaryEpoch = default(Epoch);
            var primaryLastOperationSequenceNumber = default(string);
            var isCopyContextValid = default(bool?);
            var secondaryEpoch = default(Epoch);
            var secondaryLastOperationSequenceNumber = default(string);
            var storeFormatVersion = default(KeyValueStoreEseFormat?);
            var copyType = default(KvsReplicaCopyType?);
            var copyTypeReason = default(KvsReplicaCopyTypeReason?);
            var copyMode = default(KvsReplicaCopyMode?);
            var copyModeReason = default(KvsReplicaCopyModeReason?);

            do
            {
                var propName = reader.ReadPropertyName();
                if (string.Compare("PrimaryEpoch", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    primaryEpoch = EpochConverter.Deserialize(reader);
                }
                else if (string.Compare("PrimaryLastOperationSequenceNumber", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    primaryLastOperationSequenceNumber = reader.ReadValueAsString();
                }
                else if (string.Compare("IsCopyContextValid", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    isCopyContextValid = reader.ReadValueAsBool();
                }
                else if (string.Compare("SecondaryEpoch", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    secondaryEpoch = EpochConverter.Deserialize(reader);
                }
                else if (string.Compare("SecondaryLastOperationSequenceNumber", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    secondaryLastOperationSequenceNumber = reader.ReadValueAsString();
                }
                else if (string.Compare("StoreFormatVersion", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    storeFormatVersion = KeyValueStoreEseFormatConverter.Deserialize(reader);
                }
                else if (string.Compare("CopyType", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    copyType = KvsReplicaCopyTypeConverter.Deserialize(reader);
                }
                else if (string.Compare("CopyTypeReason", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    copyTypeReason = KvsReplicaCopyTypeReasonConverter.Deserialize(reader);
                }
                else if (string.Compare("CopyMode", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    copyMode = KvsReplicaCopyModeConverter.Deserialize(reader);
                }
                else if (string.Compare("CopyModeReason", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    copyModeReason = KvsReplicaCopyModeReasonConverter.Deserialize(reader);
                }
                else
                {
                    reader.SkipPropertyValue();
                }
            }
            while (reader.TokenType != JsonToken.EndObject);

            return new KeyValueStoreESEReplicaCopyDetail(
                primaryEpoch: primaryEpoch,
                primaryLastOperationSequenceNumber: primaryLastOperationSequenceNumber,
                isCopyContextValid: isCopyContextValid,
                secondaryEpoch: secondaryEpoch,
                secondaryLastOperationSequenceNumber: secondaryLastOperationSequenceNumber,
                storeFormatVersion: storeFormatVersion,
                copyType: copyType,
                copyTypeReason: copyTypeReason,
                copyMode: copyMode,
                copyModeReason: copyModeReason);
        }

        /// <summary>
        /// Serializes the object to JSON.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="obj">The object to serialize to JSON.</param>
        internal static void Serialize(JsonWriter writer, KeyValueStoreESEReplicaCopyDetail obj)
        {
            // Required properties are always serialized, optional properties are serialized when not null.
            writer.WriteStartObject();
            writer.WriteProperty(obj.ProviderKind, "ProviderKind", KeyValueStoreProviderKindConverter.Serialize);
            writer.WriteProperty(obj.StoreFormatVersion, "StoreFormatVersion", KeyValueStoreEseFormatConverter.Serialize);
            writer.WriteProperty(obj.CopyType, "CopyType", KvsReplicaCopyTypeConverter.Serialize);
            writer.WriteProperty(obj.CopyTypeReason, "CopyTypeReason", KvsReplicaCopyTypeReasonConverter.Serialize);
            writer.WriteProperty(obj.CopyMode, "CopyMode", KvsReplicaCopyModeConverter.Serialize);
            writer.WriteProperty(obj.CopyModeReason, "CopyModeReason", KvsReplicaCopyModeReasonConverter.Serialize);
            if (obj.PrimaryEpoch != null)
            {
                writer.WriteProperty(obj.PrimaryEpoch, "PrimaryEpoch", EpochConverter.Serialize);
            }

            if (obj.PrimaryLastOperationSequenceNumber != null)
            {
                writer.WriteProperty(obj.PrimaryLastOperationSequenceNumber, "PrimaryLastOperationSequenceNumber", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.IsCopyContextValid != null)
            {
                writer.WriteProperty(obj.IsCopyContextValid, "IsCopyContextValid", JsonWriterExtensions.WriteBoolValue);
            }

            if (obj.SecondaryEpoch != null)
            {
                writer.WriteProperty(obj.SecondaryEpoch, "SecondaryEpoch", EpochConverter.Serialize);
            }

            if (obj.SecondaryLastOperationSequenceNumber != null)
            {
                writer.WriteProperty(obj.SecondaryLastOperationSequenceNumber, "SecondaryLastOperationSequenceNumber", JsonWriterExtensions.WriteStringValue);
            }

            writer.WriteEndObject();
        }
    }
}
