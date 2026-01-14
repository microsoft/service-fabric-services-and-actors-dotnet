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
    /// Converter for <see cref="ApplicationProcessExitedEvent" />.
    /// </summary>
    internal class ApplicationProcessExitedEventConverter
    {
        /// <summary>
        /// Deserializes the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static ApplicationProcessExitedEvent Deserialize(JsonReader reader)
        {
            return reader.Deserialize(GetFromJsonProperties);
        }

        /// <summary>
        /// Gets the object from Json properties.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The object Value.</returns>
        internal static ApplicationProcessExitedEvent GetFromJsonProperties(JsonReader reader)
        {
            var eventInstanceId = default(Guid?);
            var category = default(string);
            var timeStamp = default(DateTime?);
            var hasCorrelatedEvents = default(bool?);
            var applicationId = default(string);
            var serviceName = default(string);
            var servicePackageName = default(string);
            var servicePackageActivationId = default(string);
            var isExclusive = default(bool?);
            var codePackageName = default(string);
            var entryPointType = default(string);
            var exeName = default(string);
            var processId = default(long?);
            var hostId = default(string);
            var exitCode = default(long?);
            var unexpectedTermination = default(bool?);
            var startTime = default(DateTime?);
            var exitReason = default(string);
            var nodeId = default(NodeId);
            var nodeInstanceId = default(long?);

            do
            {
                var propName = reader.ReadPropertyName();
                if (string.Compare("EventInstanceId", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    eventInstanceId = reader.ReadValueAsGuid();
                }
                else if (string.Compare("Category", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    category = reader.ReadValueAsString();
                }
                else if (string.Compare("TimeStamp", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    timeStamp = reader.ReadValueAsDateTime();
                }
                else if (string.Compare("HasCorrelatedEvents", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    hasCorrelatedEvents = reader.ReadValueAsBool();
                }
                else if (string.Compare("ApplicationId", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    applicationId = reader.ReadValueAsString();
                }
                else if (string.Compare("ServiceName", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    serviceName = reader.ReadValueAsString();
                }
                else if (string.Compare("ServicePackageName", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    servicePackageName = reader.ReadValueAsString();
                }
                else if (string.Compare("ServicePackageActivationId", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    servicePackageActivationId = reader.ReadValueAsString();
                }
                else if (string.Compare("IsExclusive", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    isExclusive = reader.ReadValueAsBool();
                }
                else if (string.Compare("CodePackageName", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    codePackageName = reader.ReadValueAsString();
                }
                else if (string.Compare("EntryPointType", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    entryPointType = reader.ReadValueAsString();
                }
                else if (string.Compare("ExeName", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    exeName = reader.ReadValueAsString();
                }
                else if (string.Compare("ProcessId", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    processId = reader.ReadValueAsLong();
                }
                else if (string.Compare("HostId", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    hostId = reader.ReadValueAsString();
                }
                else if (string.Compare("ExitCode", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    exitCode = reader.ReadValueAsLong();
                }
                else if (string.Compare("UnexpectedTermination", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    unexpectedTermination = reader.ReadValueAsBool();
                }
                else if (string.Compare("StartTime", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    startTime = reader.ReadValueAsDateTime();
                }
                else if (string.Compare("ExitReason", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    exitReason = reader.ReadValueAsString();
                }
                else if (string.Compare("NodeId", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    nodeId = NodeIdConverter.Deserialize(reader);
                }
                else if (string.Compare("NodeInstanceId", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    nodeInstanceId = reader.ReadValueAsLong();
                }
                else
                {
                    reader.SkipPropertyValue();
                }
            }
            while (reader.TokenType != JsonToken.EndObject);

            return new ApplicationProcessExitedEvent(
                eventInstanceId: eventInstanceId,
                category: category,
                timeStamp: timeStamp,
                hasCorrelatedEvents: hasCorrelatedEvents,
                applicationId: applicationId,
                serviceName: serviceName,
                servicePackageName: servicePackageName,
                servicePackageActivationId: servicePackageActivationId,
                isExclusive: isExclusive,
                codePackageName: codePackageName,
                entryPointType: entryPointType,
                exeName: exeName,
                processId: processId,
                hostId: hostId,
                exitCode: exitCode,
                unexpectedTermination: unexpectedTermination,
                startTime: startTime,
                exitReason: exitReason,
                nodeId: nodeId,
                nodeInstanceId: nodeInstanceId);
        }

        /// <summary>
        /// Serializes the object to JSON.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="obj">The object to serialize to JSON.</param>
        internal static void Serialize(JsonWriter writer, ApplicationProcessExitedEvent obj)
        {
            // Required properties are always serialized, optional properties are serialized when not null.
            writer.WriteStartObject();
            writer.WriteProperty(obj.Kind, "Kind", FabricEventKindConverter.Serialize);
            writer.WriteProperty(obj.EventInstanceId, "EventInstanceId", JsonWriterExtensions.WriteGuidValue);
            writer.WriteProperty(obj.TimeStamp, "TimeStamp", JsonWriterExtensions.WriteDateTimeValue);
            writer.WriteProperty(obj.ApplicationId, "ApplicationId", JsonWriterExtensions.WriteStringValue);
            writer.WriteProperty(obj.ServiceName, "ServiceName", JsonWriterExtensions.WriteStringValue);
            writer.WriteProperty(obj.ServicePackageName, "ServicePackageName", JsonWriterExtensions.WriteStringValue);
            writer.WriteProperty(obj.ServicePackageActivationId, "ServicePackageActivationId", JsonWriterExtensions.WriteStringValue);
            writer.WriteProperty(obj.IsExclusive, "IsExclusive", JsonWriterExtensions.WriteBoolValue);
            writer.WriteProperty(obj.CodePackageName, "CodePackageName", JsonWriterExtensions.WriteStringValue);
            writer.WriteProperty(obj.EntryPointType, "EntryPointType", JsonWriterExtensions.WriteStringValue);
            writer.WriteProperty(obj.ExeName, "ExeName", JsonWriterExtensions.WriteStringValue);
            writer.WriteProperty(obj.ProcessId, "ProcessId", JsonWriterExtensions.WriteLongValue);
            writer.WriteProperty(obj.HostId, "HostId", JsonWriterExtensions.WriteStringValue);
            writer.WriteProperty(obj.ExitCode, "ExitCode", JsonWriterExtensions.WriteLongValue);
            writer.WriteProperty(obj.UnexpectedTermination, "UnexpectedTermination", JsonWriterExtensions.WriteBoolValue);
            writer.WriteProperty(obj.StartTime, "StartTime", JsonWriterExtensions.WriteDateTimeValue);
            writer.WriteProperty(obj.NodeId, "NodeId", NodeIdConverter.Serialize);
            writer.WriteProperty(obj.NodeInstanceId, "NodeInstanceId", JsonWriterExtensions.WriteLongValue);
            if (obj.Category != null)
            {
                writer.WriteProperty(obj.Category, "Category", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.HasCorrelatedEvents != null)
            {
                writer.WriteProperty(obj.HasCorrelatedEvents, "HasCorrelatedEvents", JsonWriterExtensions.WriteBoolValue);
            }

            if (obj.ExitReason != null)
            {
                writer.WriteProperty(obj.ExitReason, "ExitReason", JsonWriterExtensions.WriteStringValue);
            }

            writer.WriteEndObject();
        }
    }
}
