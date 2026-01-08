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
    /// Converter for <see cref="BackupEntity" />.
    /// </summary>
    internal class BackupEntityConverter
    {
        /// <summary>
        /// Deserializes the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static BackupEntity Deserialize(JsonReader reader)
        {
            return reader.Deserialize(GetFromJsonProperties);
        }

        /// <summary>
        /// Gets the object from Json properties.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static BackupEntity GetFromJsonProperties(JsonReader reader)
        {
            BackupEntity obj = null;
            var propName = reader.ReadPropertyName();
            if (!propName.Equals("EntityKind", StringComparison.OrdinalIgnoreCase))
            {
                throw new JsonReaderException($"Incorrect discriminator property name {propName}, Expected discriminator property name is EntityKind.");
            }

            var propValue = reader.ReadValueAsString();
            if (propValue.Equals("Application", StringComparison.OrdinalIgnoreCase))
            {
                obj = ApplicationBackupEntityConverter.GetFromJsonProperties(reader);
            }
            else if (propValue.Equals("Service", StringComparison.OrdinalIgnoreCase))
            {
                obj = ServiceBackupEntityConverter.GetFromJsonProperties(reader);
            }
            else if (propValue.Equals("Partition", StringComparison.OrdinalIgnoreCase))
            {
                obj = PartitionBackupEntityConverter.GetFromJsonProperties(reader);
            }
            else
            {
                throw new InvalidOperationException("Unknown EntityKind.");
            }

            return obj;
        }

        /// <summary>
        /// Serializes the object to JSON.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="obj">The object to serialize to JSON.</param>
        internal static void Serialize(JsonWriter writer, BackupEntity obj)
        {
            var kind = obj.EntityKind;
            if (kind.Equals(BackupEntityKind.Application))
            {
                ApplicationBackupEntityConverter.Serialize(writer, (ApplicationBackupEntity)obj);
            }
            else if (kind.Equals(BackupEntityKind.Service))
            {
                ServiceBackupEntityConverter.Serialize(writer, (ServiceBackupEntity)obj);
            }
            else if (kind.Equals(BackupEntityKind.Partition))
            {
                PartitionBackupEntityConverter.Serialize(writer, (PartitionBackupEntity)obj);
            }
            else
            {
                throw new InvalidOperationException("Unknown EntityKind.");
            }
        }
    }
}
