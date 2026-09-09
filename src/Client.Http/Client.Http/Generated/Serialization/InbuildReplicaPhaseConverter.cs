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
    /// Converter for <see cref="InbuildReplicaPhase" />.
    /// </summary>
    internal class InbuildReplicaPhaseConverter
    {
        /// <summary>
        /// Gets the enum value by reading string value from reader.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The enum Value.</returns>
        public static InbuildReplicaPhase? Deserialize(JsonReader reader)
        {
            var value = reader.ReadValueAsString();
            var obj = default(InbuildReplicaPhase);

            if (string.Compare(value, "CopyContext", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = InbuildReplicaPhase.CopyContext;
            }
            else if (string.Compare(value, "CopyState", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = InbuildReplicaPhase.CopyState;
            }
            else if (string.Compare(value, "Copy", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = InbuildReplicaPhase.Copy;
            }
            else if (string.Compare(value, "CopyCatchup", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = InbuildReplicaPhase.CopyCatchup;
            }
            else if (string.Compare(value, "CopyComplete", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = InbuildReplicaPhase.CopyComplete;
            }

            return obj;
        }

        /// <summary>
        /// Serializes the enum value.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="value">The object to serialize to JSON.</param>
        public static void Serialize(JsonWriter writer, InbuildReplicaPhase? value)
        {
            switch (value)
            {
                case InbuildReplicaPhase.CopyContext:
                    writer.WriteStringValue("CopyContext");
                    break;
                case InbuildReplicaPhase.CopyState:
                    writer.WriteStringValue("CopyState");
                    break;
                case InbuildReplicaPhase.Copy:
                    writer.WriteStringValue("Copy");
                    break;
                case InbuildReplicaPhase.CopyCatchup:
                    writer.WriteStringValue("CopyCatchup");
                    break;
                case InbuildReplicaPhase.CopyComplete:
                    writer.WriteStringValue("CopyComplete");
                    break;
                default:
                    throw new ArgumentException($"Invalid value {value.ToString()} for enum type InbuildReplicaPhase");
            }
        }
    }
}
