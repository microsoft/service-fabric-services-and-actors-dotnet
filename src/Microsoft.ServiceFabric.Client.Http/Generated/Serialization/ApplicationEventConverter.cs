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
    /// Converter for <see cref="ApplicationEvent" />.
    /// </summary>
    internal class ApplicationEventConverter
    {
        /// <summary>
        /// Deserializes the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static ApplicationEvent Deserialize(JsonReader reader)
        {
            return reader.Deserialize(GetFromJsonProperties);
        }

        /// <summary>
        /// Gets the object from Json properties.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static ApplicationEvent GetFromJsonProperties(JsonReader reader)
        {
            var eventInstanceId = default(Guid?);
            var category = default(string);
            var timeStamp = default(DateTime?);
            var hasCorrelatedEvents = default(bool?);
            var applicationId = default(string);

            do
            {
                var propName = reader.ReadPropertyName();
                if (propName.Equals("Kind", StringComparison.OrdinalIgnoreCase))
                {
                    var propValue = reader.ReadValueAsString();

                    if (propValue.Equals("ApplicationCreated", StringComparison.OrdinalIgnoreCase))
                    {
                        return ApplicationCreatedEventConverter.GetFromJsonProperties(reader);
                    }
                    else if (propValue.Equals("ApplicationDeleted", StringComparison.OrdinalIgnoreCase))
                    {
                        return ApplicationDeletedEventConverter.GetFromJsonProperties(reader);
                    }
                    else if (propValue.Equals("ApplicationNewHealthReport", StringComparison.OrdinalIgnoreCase))
                    {
                        return ApplicationNewHealthReportEventConverter.GetFromJsonProperties(reader);
                    }
                    else if (propValue.Equals("ApplicationHealthReportExpired", StringComparison.OrdinalIgnoreCase))
                    {
                        return ApplicationHealthReportExpiredEventConverter.GetFromJsonProperties(reader);
                    }
                    else if (propValue.Equals("ApplicationUpgradeCompleted", StringComparison.OrdinalIgnoreCase))
                    {
                        return ApplicationUpgradeCompletedEventConverter.GetFromJsonProperties(reader);
                    }
                    else if (propValue.Equals("ApplicationUpgradeDomainCompleted", StringComparison.OrdinalIgnoreCase))
                    {
                        return ApplicationUpgradeDomainCompletedEventConverter.GetFromJsonProperties(reader);
                    }
                    else if (propValue.Equals("ApplicationUpgradeRollbackCompleted", StringComparison.OrdinalIgnoreCase))
                    {
                        return ApplicationUpgradeRollbackCompletedEventConverter.GetFromJsonProperties(reader);
                    }
                    else if (propValue.Equals("ApplicationUpgradeRollbackStarted", StringComparison.OrdinalIgnoreCase))
                    {
                        return ApplicationUpgradeRollbackStartedEventConverter.GetFromJsonProperties(reader);
                    }
                    else if (propValue.Equals("ApplicationUpgradeStarted", StringComparison.OrdinalIgnoreCase))
                    {
                        return ApplicationUpgradeStartedEventConverter.GetFromJsonProperties(reader);
                    }
                    else if (propValue.Equals("DeployedApplicationNewHealthReport", StringComparison.OrdinalIgnoreCase))
                    {
                        return DeployedApplicationNewHealthReportEventConverter.GetFromJsonProperties(reader);
                    }
                    else if (propValue.Equals("DeployedApplicationHealthReportExpired", StringComparison.OrdinalIgnoreCase))
                    {
                        return DeployedApplicationHealthReportExpiredEventConverter.GetFromJsonProperties(reader);
                    }
                    else if (propValue.Equals("ApplicationProcessExited", StringComparison.OrdinalIgnoreCase))
                    {
                        return ApplicationProcessExitedEventConverter.GetFromJsonProperties(reader);
                    }
                    else if (propValue.Equals("ApplicationContainerInstanceExited", StringComparison.OrdinalIgnoreCase))
                    {
                        return ApplicationContainerInstanceExitedEventConverter.GetFromJsonProperties(reader);
                    }
                    else if (propValue.Equals("DeployedServicePackageNewHealthReport", StringComparison.OrdinalIgnoreCase))
                    {
                        return DeployedServicePackageNewHealthReportEventConverter.GetFromJsonProperties(reader);
                    }
                    else if (propValue.Equals("DeployedServicePackageHealthReportExpired", StringComparison.OrdinalIgnoreCase))
                    {
                        return DeployedServicePackageHealthReportExpiredEventConverter.GetFromJsonProperties(reader);
                    }
                    else if (propValue.Equals("ChaosCodePackageRestartScheduled", StringComparison.OrdinalIgnoreCase))
                    {
                        return ChaosCodePackageRestartScheduledEventConverter.GetFromJsonProperties(reader);
                    }
                    else if (propValue.Equals("ReadinessProbeFailed", StringComparison.OrdinalIgnoreCase))
                    {
                        return ReadinessProbeFailedEventConverter.GetFromJsonProperties(reader);
                    }
                    else if (propValue.Equals("LivenessProbeFailed", StringComparison.OrdinalIgnoreCase))
                    {
                        return LivenessProbeFailedEventConverter.GetFromJsonProperties(reader);
                    }
                    else if (propValue.Equals("ApplicationEvent", StringComparison.OrdinalIgnoreCase))
                    {
                        // kind specified as same type, deserialize using properties.
                    }
                    else
                    {
                        throw new InvalidOperationException("Unknown Discriminator.");
                    }
                }
                else
                {
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
                    else
                    {
                        reader.SkipPropertyValue();
                    }
                }
            }
            while (reader.TokenType != JsonToken.EndObject);

            return new ApplicationEvent(
                kind: Common.FabricEventKind.ApplicationEvent,
                eventInstanceId: eventInstanceId,
                category: category,
                timeStamp: timeStamp,
                hasCorrelatedEvents: hasCorrelatedEvents,
                applicationId: applicationId);
        }

        /// <summary>
        /// Serializes the object to JSON.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="obj">The object to serialize to JSON.</param>
        internal static void Serialize(JsonWriter writer, ApplicationEvent obj)
        {
            // Required properties are always serialized, optional properties are serialized when not null.
            writer.WriteStartObject();
            writer.WriteProperty(obj.Kind, "Kind", FabricEventKindConverter.Serialize);
            writer.WriteProperty(obj.EventInstanceId, "EventInstanceId", JsonWriterExtensions.WriteGuidValue);
            writer.WriteProperty(obj.TimeStamp, "TimeStamp", JsonWriterExtensions.WriteDateTimeValue);
            writer.WriteProperty(obj.ApplicationId, "ApplicationId", JsonWriterExtensions.WriteStringValue);
            if (obj.Category != null)
            {
                writer.WriteProperty(obj.Category, "Category", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.HasCorrelatedEvents != null)
            {
                writer.WriteProperty(obj.HasCorrelatedEvents, "HasCorrelatedEvents", JsonWriterExtensions.WriteBoolValue);
            }

            writer.WriteEndObject();
        }
    }
}
