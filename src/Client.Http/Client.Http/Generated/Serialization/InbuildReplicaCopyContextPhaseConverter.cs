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
    /// Converter for <see cref="InbuildReplicaCopyContextPhase" />.
    /// </summary>
    internal class InbuildReplicaCopyContextPhaseConverter
    {
        /// <summary>
        /// Gets the enum value by reading string value from reader.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The enum Value.</returns>
        public static InbuildReplicaCopyContextPhase? Deserialize(JsonReader reader)
        {
            var value = reader.ReadValueAsString();
            var obj = default(InbuildReplicaCopyContextPhase);

            if (string.Compare(value, "EstablishConnection", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = InbuildReplicaCopyContextPhase.EstablishConnection;
            }
            else if (string.Compare(value, "GetCopyContext", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = InbuildReplicaCopyContextPhase.GetCopyContext;
            }

            return obj;
        }

        /// <summary>
        /// Serializes the enum value.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="value">The object to serialize to JSON.</param>
        public static void Serialize(JsonWriter writer, InbuildReplicaCopyContextPhase? value)
        {
            switch (value)
            {
                case InbuildReplicaCopyContextPhase.EstablishConnection:
                    writer.WriteStringValue("EstablishConnection");
                    break;
                case InbuildReplicaCopyContextPhase.GetCopyContext:
                    writer.WriteStringValue("GetCopyContext");
                    break;
                default:
                    throw new ArgumentException($"Invalid value {value.ToString()} for enum type InbuildReplicaCopyContextPhase");
            }
        }
    }
}
