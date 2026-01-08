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
    /// Converter for <see cref="RepairTask" />.
    /// </summary>
    internal class RepairTaskConverter
    {
        /// <summary>
        /// Deserializes the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <returns>The object Value.</returns>
        internal static RepairTask Deserialize(JsonReader reader)
        {
            return reader.Deserialize(GetFromJsonProperties);
        }

        /// <summary>
        /// Gets the object from Json properties.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The object Value.</returns>
        internal static RepairTask GetFromJsonProperties(JsonReader reader)
        {
            var taskId = default(string);
            var version = default(string);
            var description = default(string);
            var state = default(State?);
            var flags = default(int?);
            var action = default(string);
            var target = default(RepairTargetDescriptionBase);
            var executor = default(string);
            var executorData = default(string);
            var impact = default(RepairImpactDescriptionBase);
            var resultStatus = default(ResultStatus?);
            var resultCode = default(int?);
            var resultDetails = default(string);
            var history = default(RepairTaskHistory);
            var preparingHealthCheckState = default(RepairTaskHealthCheckState?);
            var restoringHealthCheckState = default(RepairTaskHealthCheckState?);
            var performPreparingHealthCheck = default(bool?);
            var performRestoringHealthCheck = default(bool?);

            do
            {
                var propName = reader.ReadPropertyName();
                if (string.Compare("TaskId", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    taskId = reader.ReadValueAsString();
                }
                else if (string.Compare("Version", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    version = reader.ReadValueAsString();
                }
                else if (string.Compare("Description", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    description = reader.ReadValueAsString();
                }
                else if (string.Compare("State", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    state = StateConverter.Deserialize(reader);
                }
                else if (string.Compare("Flags", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    flags = reader.ReadValueAsInt();
                }
                else if (string.Compare("Action", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    action = reader.ReadValueAsString();
                }
                else if (string.Compare("Target", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    target = RepairTargetDescriptionBaseConverter.Deserialize(reader);
                }
                else if (string.Compare("Executor", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    executor = reader.ReadValueAsString();
                }
                else if (string.Compare("ExecutorData", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    executorData = reader.ReadValueAsString();
                }
                else if (string.Compare("Impact", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    impact = RepairImpactDescriptionBaseConverter.Deserialize(reader);
                }
                else if (string.Compare("ResultStatus", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    resultStatus = ResultStatusConverter.Deserialize(reader);
                }
                else if (string.Compare("ResultCode", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    resultCode = reader.ReadValueAsInt();
                }
                else if (string.Compare("ResultDetails", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    resultDetails = reader.ReadValueAsString();
                }
                else if (string.Compare("History", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    history = RepairTaskHistoryConverter.Deserialize(reader);
                }
                else if (string.Compare("PreparingHealthCheckState", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    preparingHealthCheckState = RepairTaskHealthCheckStateConverter.Deserialize(reader);
                }
                else if (string.Compare("RestoringHealthCheckState", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    restoringHealthCheckState = RepairTaskHealthCheckStateConverter.Deserialize(reader);
                }
                else if (string.Compare("PerformPreparingHealthCheck", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    performPreparingHealthCheck = reader.ReadValueAsBool();
                }
                else if (string.Compare("PerformRestoringHealthCheck", propName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    performRestoringHealthCheck = reader.ReadValueAsBool();
                }
                else
                {
                    reader.SkipPropertyValue();
                }
            }
            while (reader.TokenType != JsonToken.EndObject);

            return new RepairTask(
                taskId: taskId,
                version: version,
                description: description,
                state: state,
                flags: flags,
                action: action,
                target: target,
                executor: executor,
                executorData: executorData,
                impact: impact,
                resultStatus: resultStatus,
                resultCode: resultCode,
                resultDetails: resultDetails,
                history: history,
                preparingHealthCheckState: preparingHealthCheckState,
                restoringHealthCheckState: restoringHealthCheckState,
                performPreparingHealthCheck: performPreparingHealthCheck,
                performRestoringHealthCheck: performRestoringHealthCheck);
        }

        /// <summary>
        /// Serializes the object to JSON.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="obj">The object to serialize to JSON.</param>
        internal static void Serialize(JsonWriter writer, RepairTask obj)
        {
            // Required properties are always serialized, optional properties are serialized when not null.
            writer.WriteStartObject();
            writer.WriteProperty(obj.State, "State", StateConverter.Serialize);
            writer.WriteProperty(obj.TaskId, "TaskId", JsonWriterExtensions.WriteStringValue);
            writer.WriteProperty(obj.Action, "Action", JsonWriterExtensions.WriteStringValue);
            writer.WriteProperty(obj.ResultStatus, "ResultStatus", ResultStatusConverter.Serialize);
            writer.WriteProperty(obj.PreparingHealthCheckState, "PreparingHealthCheckState", RepairTaskHealthCheckStateConverter.Serialize);
            writer.WriteProperty(obj.RestoringHealthCheckState, "RestoringHealthCheckState", RepairTaskHealthCheckStateConverter.Serialize);
            if (obj.Version != null)
            {
                writer.WriteProperty(obj.Version, "Version", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.Description != null)
            {
                writer.WriteProperty(obj.Description, "Description", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.Flags != null)
            {
                writer.WriteProperty(obj.Flags, "Flags", JsonWriterExtensions.WriteIntValue);
            }

            if (obj.Target != null)
            {
                writer.WriteProperty(obj.Target, "Target", RepairTargetDescriptionBaseConverter.Serialize);
            }

            if (obj.Executor != null)
            {
                writer.WriteProperty(obj.Executor, "Executor", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.ExecutorData != null)
            {
                writer.WriteProperty(obj.ExecutorData, "ExecutorData", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.Impact != null)
            {
                writer.WriteProperty(obj.Impact, "Impact", RepairImpactDescriptionBaseConverter.Serialize);
            }

            if (obj.ResultCode != null)
            {
                writer.WriteProperty(obj.ResultCode, "ResultCode", JsonWriterExtensions.WriteIntValue);
            }

            if (obj.ResultDetails != null)
            {
                writer.WriteProperty(obj.ResultDetails, "ResultDetails", JsonWriterExtensions.WriteStringValue);
            }

            if (obj.History != null)
            {
                writer.WriteProperty(obj.History, "History", RepairTaskHistoryConverter.Serialize);
            }

            if (obj.PerformPreparingHealthCheck != null)
            {
                writer.WriteProperty(obj.PerformPreparingHealthCheck, "PerformPreparingHealthCheck", JsonWriterExtensions.WriteBoolValue);
            }

            if (obj.PerformRestoringHealthCheck != null)
            {
                writer.WriteProperty(obj.PerformRestoringHealthCheck, "PerformRestoringHealthCheck", JsonWriterExtensions.WriteBoolValue);
            }

            writer.WriteEndObject();
        }
    }
}
