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
    /// Converter for <see cref="LivenessProbeFailedEvent" />.
    /// </summary>
    internal class LivenessProbeFailedEventConverter
    {
        /// <summary>
        /// Deserializes the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static LivenessProbeFailedEvent Deserialize(JsonReader reader)
        {
            return reader.Deserialize(GetFromJsonProperties);
        }

        /// <summary>
        /// Gets the object from Json properties.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The object Value.</returns>
        internal static LivenessProbeFailedEvent GetFromJsonProperties(JsonReader reader)
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
            var containerImage = default(string);
            var codePackageName = default(string);
            var containerName = default(string);
            var hostId = default(string);
            var entryPointType = default(string);
            var failureCount = default(long?);
            var exitCode = default(string);
            var stdOut = default(string);
            var stdErr = default(string);
            var errorCode = default(string);
            var message = default(string);

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
                else if (string.Compare("ContainerImage", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    containerImage = reader.ReadValueAsString();
                }
                else if (string.Compare("CodePackageName", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    codePackageName = reader.ReadValueAsString();
                }
                else if (string.Compare("ContainerName", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    containerName = reader.ReadValueAsString();
                }
                else if (string.Compare("HostId", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    hostId = reader.ReadValueAsString();
                }
                else if (string.Compare("EntryPointType", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    entryPointType = reader.ReadValueAsString();
                }
                else if (string.Compare("FailureCount", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    failureCount = reader.ReadValueAsLong();
                }
                else if (string.Compare("ExitCode", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    exitCode = reader.ReadValueAsString();
                }
                else if (string.Compare("StdOut", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    stdOut = reader.ReadValueAsString();
                }
                else if (string.Compare("StdErr", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    stdErr = reader.ReadValueAsString();
                }
                else if (string.Compare("ErrorCode", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    errorCode = reader.ReadValueAsString();
                }
                else if (string.Compare("Message", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    message = reader.ReadValueAsString();
                }
                else
                {
                    reader.SkipPropertyValue();
                }
            }
            while (reader.TokenType != JsonToken.EndObject);

            return new LivenessProbeFailedEvent(
                eventInstanceId: eventInstanceId,
                category: category,
                timeStamp: timeStamp,
                hasCorrelatedEvents: hasCorrelatedEvents,
                applicationId: applicationId,
                serviceName: serviceName,
                servicePackageName: servicePackageName,
                servicePackageActivationId: servicePackageActivationId,
                isExclusive: isExclusive,
                containerImage: containerImage,
                codePackageName: codePackageName,
                containerName: containerName,
                hostId: hostId,
                entryPointType: entryPointType,
                failureCount: failureCount,
                exitCode: exitCode,
                stdOut: stdOut,
                stdErr: stdErr,
                errorCode: errorCode,
                message: message);
        }

        /// <summary>
        /// Serializes the object to JSON.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="obj">The object to serialize to JSON.</param>
        internal static void Serialize(JsonWriter writer, LivenessProbeFailedEvent obj)
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
            writer.WriteProperty(obj.ContainerImage, "ContainerImage", JsonWriterExtensions.WriteStringValue);
            writer.WriteProperty(obj.HostId, "HostId", JsonWriterExtensions.WriteStringValue);
            writer.WriteProperty(obj.EntryPointType, "EntryPointType", JsonWriterExtensions.WriteStringValue);
            writer.WriteProperty(obj.FailureCount, "FailureCount", JsonWriterExtensions.WriteLongValue);
            writer.WriteProperty(obj.ExitCode, "ExitCode", JsonWriterExtensions.WriteStringValue);
            writer.WriteProperty(obj.StdOut, "StdOut", JsonWriterExtensions.WriteStringValue);
            writer.WriteProperty(obj.StdErr, "StdErr", JsonWriterExtensions.WriteStringValue);
            writer.WriteProperty(obj.ErrorCode, "ErrorCode", JsonWriterExtensions.WriteStringValue);
            writer.WriteProperty(obj.Message, "Message", JsonWriterExtensions.WriteStringValue);
            if (obj.Category != null)
            {
                writer.WriteProperty(obj.Category, "Category", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.HasCorrelatedEvents != null)
            {
                writer.WriteProperty(obj.HasCorrelatedEvents, "HasCorrelatedEvents", JsonWriterExtensions.WriteBoolValue);
            }

            if (obj.CodePackageName != null)
            {
                writer.WriteProperty(obj.CodePackageName, "CodePackageName", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.ContainerName != null)
            {
                writer.WriteProperty(obj.ContainerName, "ContainerName", JsonWriterExtensions.WriteStringValue);
            }

            writer.WriteEndObject();
        }
    }
}
