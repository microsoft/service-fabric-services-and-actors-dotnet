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
    /// Converter for <see cref="FabricErrorCodes" />.
    /// </summary>
    internal class FabricErrorCodesConverter
    {
        /// <summary>
        /// Gets the enum value by reading string value from reader.
        /// </summary>
        /// <param name="reader">The <see cref="T: Newtonsoft.Json.JsonReader" /> to read from, reader must be placed at first property.</param>
        /// <returns>The enum Value.</returns>
        public static FabricErrorCodes? Deserialize(JsonReader reader)
        {
            var value = reader.ReadValueAsString();
            var obj = default(FabricErrorCodes);

            if (string.Compare(value, "FABRIC_E_INVALID_PARTITION_KEY", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = FabricErrorCodes.FABRIC_E_INVALID_PARTITION_KEY;
            }
            else if (string.Compare(value, "FABRIC_E_IMAGEBUILDER_VALIDATION_ERROR", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = FabricErrorCodes.FABRIC_E_IMAGEBUILDER_VALIDATION_ERROR;
            }
            else if (string.Compare(value, "FABRIC_E_INVALID_ADDRESS", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = FabricErrorCodes.FABRIC_E_INVALID_ADDRESS;
            }
            else if (string.Compare(value, "FABRIC_E_APPLICATION_NOT_UPGRADING", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = FabricErrorCodes.FABRIC_E_APPLICATION_NOT_UPGRADING;
            }
            else if (string.Compare(value, "FABRIC_E_APPLICATION_UPGRADE_VALIDATION_ERROR", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = FabricErrorCodes.FABRIC_E_APPLICATION_UPGRADE_VALIDATION_ERROR;
            }
            else if (string.Compare(value, "FABRIC_E_FABRIC_NOT_UPGRADING", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = FabricErrorCodes.FABRIC_E_FABRIC_NOT_UPGRADING;
            }
            else if (string.Compare(value, "FABRIC_E_FABRIC_UPGRADE_VALIDATION_ERROR", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = FabricErrorCodes.FABRIC_E_FABRIC_UPGRADE_VALIDATION_ERROR;
            }
            else if (string.Compare(value, "FABRIC_E_INVALID_CONFIGURATION", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = FabricErrorCodes.FABRIC_E_INVALID_CONFIGURATION;
            }
            else if (string.Compare(value, "FABRIC_E_INVALID_NAME_URI", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = FabricErrorCodes.FABRIC_E_INVALID_NAME_URI;
            }
            else if (string.Compare(value, "FABRIC_E_PATH_TOO_LONG", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = FabricErrorCodes.FABRIC_E_PATH_TOO_LONG;
            }
            else if (string.Compare(value, "FABRIC_E_KEY_TOO_LARGE", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = FabricErrorCodes.FABRIC_E_KEY_TOO_LARGE;
            }
            else if (string.Compare(value, "FABRIC_E_SERVICE_AFFINITY_CHAIN_NOT_SUPPORTED", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = FabricErrorCodes.FABRIC_E_SERVICE_AFFINITY_CHAIN_NOT_SUPPORTED;
            }
            else if (string.Compare(value, "FABRIC_E_INVALID_ATOMIC_GROUP", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = FabricErrorCodes.FABRIC_E_INVALID_ATOMIC_GROUP;
            }
            else if (string.Compare(value, "FABRIC_E_VALUE_EMPTY", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = FabricErrorCodes.FABRIC_E_VALUE_EMPTY;
            }
            else if (string.Compare(value, "FABRIC_E_NODE_NOT_FOUND", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = FabricErrorCodes.FABRIC_E_NODE_NOT_FOUND;
            }
            else if (string.Compare(value, "FABRIC_E_APPLICATION_TYPE_NOT_FOUND", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = FabricErrorCodes.FABRIC_E_APPLICATION_TYPE_NOT_FOUND;
            }
            else if (string.Compare(value, "FABRIC_E_APPLICATION_NOT_FOUND", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = FabricErrorCodes.FABRIC_E_APPLICATION_NOT_FOUND;
            }
            else if (string.Compare(value, "FABRIC_E_SERVICE_TYPE_NOT_FOUND", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = FabricErrorCodes.FABRIC_E_SERVICE_TYPE_NOT_FOUND;
            }
            else if (string.Compare(value, "FABRIC_E_SERVICE_DOES_NOT_EXIST", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = FabricErrorCodes.FABRIC_E_SERVICE_DOES_NOT_EXIST;
            }
            else if (string.Compare(value, "FABRIC_E_SERVICE_TYPE_TEMPLATE_NOT_FOUND", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = FabricErrorCodes.FABRIC_E_SERVICE_TYPE_TEMPLATE_NOT_FOUND;
            }
            else if (string.Compare(value, "FABRIC_E_CONFIGURATION_SECTION_NOT_FOUND", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = FabricErrorCodes.FABRIC_E_CONFIGURATION_SECTION_NOT_FOUND;
            }
            else if (string.Compare(value, "FABRIC_E_PARTITION_NOT_FOUND", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = FabricErrorCodes.FABRIC_E_PARTITION_NOT_FOUND;
            }
            else if (string.Compare(value, "FABRIC_E_REPLICA_DOES_NOT_EXIST", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = FabricErrorCodes.FABRIC_E_REPLICA_DOES_NOT_EXIST;
            }
            else if (string.Compare(value, "FABRIC_E_SERVICE_GROUP_DOES_NOT_EXIST", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = FabricErrorCodes.FABRIC_E_SERVICE_GROUP_DOES_NOT_EXIST;
            }
            else if (string.Compare(value, "FABRIC_E_CONFIGURATION_PARAMETER_NOT_FOUND", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = FabricErrorCodes.FABRIC_E_CONFIGURATION_PARAMETER_NOT_FOUND;
            }
            else if (string.Compare(value, "FABRIC_E_DIRECTORY_NOT_FOUND", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = FabricErrorCodes.FABRIC_E_DIRECTORY_NOT_FOUND;
            }
            else if (string.Compare(value, "FABRIC_E_FABRIC_VERSION_NOT_FOUND", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = FabricErrorCodes.FABRIC_E_FABRIC_VERSION_NOT_FOUND;
            }
            else if (string.Compare(value, "FABRIC_E_FILE_NOT_FOUND", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = FabricErrorCodes.FABRIC_E_FILE_NOT_FOUND;
            }
            else if (string.Compare(value, "FABRIC_E_NAME_DOES_NOT_EXIST", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = FabricErrorCodes.FABRIC_E_NAME_DOES_NOT_EXIST;
            }
            else if (string.Compare(value, "FABRIC_E_PROPERTY_DOES_NOT_EXIST", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = FabricErrorCodes.FABRIC_E_PROPERTY_DOES_NOT_EXIST;
            }
            else if (string.Compare(value, "FABRIC_E_ENUMERATION_COMPLETED", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = FabricErrorCodes.FABRIC_E_ENUMERATION_COMPLETED;
            }
            else if (string.Compare(value, "FABRIC_E_SERVICE_MANIFEST_NOT_FOUND", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = FabricErrorCodes.FABRIC_E_SERVICE_MANIFEST_NOT_FOUND;
            }
            else if (string.Compare(value, "FABRIC_E_KEY_NOT_FOUND", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = FabricErrorCodes.FABRIC_E_KEY_NOT_FOUND;
            }
            else if (string.Compare(value, "FABRIC_E_HEALTH_ENTITY_NOT_FOUND", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = FabricErrorCodes.FABRIC_E_HEALTH_ENTITY_NOT_FOUND;
            }
            else if (string.Compare(value, "FABRIC_E_APPLICATION_TYPE_ALREADY_EXISTS", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = FabricErrorCodes.FABRIC_E_APPLICATION_TYPE_ALREADY_EXISTS;
            }
            else if (string.Compare(value, "FABRIC_E_APPLICATION_ALREADY_EXISTS", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = FabricErrorCodes.FABRIC_E_APPLICATION_ALREADY_EXISTS;
            }
            else if (string.Compare(value, "FABRIC_E_APPLICATION_ALREADY_IN_TARGET_VERSION", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = FabricErrorCodes.FABRIC_E_APPLICATION_ALREADY_IN_TARGET_VERSION;
            }
            else if (string.Compare(value, "FABRIC_E_APPLICATION_TYPE_PROVISION_IN_PROGRESS", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = FabricErrorCodes.FABRIC_E_APPLICATION_TYPE_PROVISION_IN_PROGRESS;
            }
            else if (string.Compare(value, "FABRIC_E_APPLICATION_UPGRADE_IN_PROGRESS", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = FabricErrorCodes.FABRIC_E_APPLICATION_UPGRADE_IN_PROGRESS;
            }
            else if (string.Compare(value, "FABRIC_E_SERVICE_ALREADY_EXISTS", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = FabricErrorCodes.FABRIC_E_SERVICE_ALREADY_EXISTS;
            }
            else if (string.Compare(value, "FABRIC_E_SERVICE_GROUP_ALREADY_EXISTS", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = FabricErrorCodes.FABRIC_E_SERVICE_GROUP_ALREADY_EXISTS;
            }
            else if (string.Compare(value, "FABRIC_E_APPLICATION_TYPE_IN_USE", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = FabricErrorCodes.FABRIC_E_APPLICATION_TYPE_IN_USE;
            }
            else if (string.Compare(value, "FABRIC_E_FABRIC_ALREADY_IN_TARGET_VERSION", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = FabricErrorCodes.FABRIC_E_FABRIC_ALREADY_IN_TARGET_VERSION;
            }
            else if (string.Compare(value, "FABRIC_E_FABRIC_VERSION_ALREADY_EXISTS", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = FabricErrorCodes.FABRIC_E_FABRIC_VERSION_ALREADY_EXISTS;
            }
            else if (string.Compare(value, "FABRIC_E_FABRIC_VERSION_IN_USE", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = FabricErrorCodes.FABRIC_E_FABRIC_VERSION_IN_USE;
            }
            else if (string.Compare(value, "FABRIC_E_FABRIC_UPGRADE_IN_PROGRESS", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = FabricErrorCodes.FABRIC_E_FABRIC_UPGRADE_IN_PROGRESS;
            }
            else if (string.Compare(value, "FABRIC_E_NAME_ALREADY_EXISTS", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = FabricErrorCodes.FABRIC_E_NAME_ALREADY_EXISTS;
            }
            else if (string.Compare(value, "FABRIC_E_NAME_NOT_EMPTY", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = FabricErrorCodes.FABRIC_E_NAME_NOT_EMPTY;
            }
            else if (string.Compare(value, "FABRIC_E_PROPERTY_CHECK_FAILED", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = FabricErrorCodes.FABRIC_E_PROPERTY_CHECK_FAILED;
            }
            else if (string.Compare(value, "FABRIC_E_SERVICE_METADATA_MISMATCH", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = FabricErrorCodes.FABRIC_E_SERVICE_METADATA_MISMATCH;
            }
            else if (string.Compare(value, "FABRIC_E_SERVICE_TYPE_MISMATCH", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = FabricErrorCodes.FABRIC_E_SERVICE_TYPE_MISMATCH;
            }
            else if (string.Compare(value, "FABRIC_E_HEALTH_STALE_REPORT", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = FabricErrorCodes.FABRIC_E_HEALTH_STALE_REPORT;
            }
            else if (string.Compare(value, "FABRIC_E_SEQUENCE_NUMBER_CHECK_FAILED", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = FabricErrorCodes.FABRIC_E_SEQUENCE_NUMBER_CHECK_FAILED;
            }
            else if (string.Compare(value, "FABRIC_E_NODE_HAS_NOT_STOPPED_YET", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = FabricErrorCodes.FABRIC_E_NODE_HAS_NOT_STOPPED_YET;
            }
            else if (string.Compare(value, "FABRIC_E_INSTANCE_ID_MISMATCH", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = FabricErrorCodes.FABRIC_E_INSTANCE_ID_MISMATCH;
            }
            else if (string.Compare(value, "FABRIC_E_VALUE_TOO_LARGE", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = FabricErrorCodes.FABRIC_E_VALUE_TOO_LARGE;
            }
            else if (string.Compare(value, "FABRIC_E_NO_WRITE_QUORUM", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = FabricErrorCodes.FABRIC_E_NO_WRITE_QUORUM;
            }
            else if (string.Compare(value, "FABRIC_E_NOT_PRIMARY", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = FabricErrorCodes.FABRIC_E_NOT_PRIMARY;
            }
            else if (string.Compare(value, "FABRIC_E_NOT_READY", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = FabricErrorCodes.FABRIC_E_NOT_READY;
            }
            else if (string.Compare(value, "FABRIC_E_RECONFIGURATION_PENDING", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = FabricErrorCodes.FABRIC_E_RECONFIGURATION_PENDING;
            }
            else if (string.Compare(value, "FABRIC_E_SERVICE_OFFLINE", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = FabricErrorCodes.FABRIC_E_SERVICE_OFFLINE;
            }
            else if (string.Compare(value, "E_ABORT", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = FabricErrorCodes.E_ABORT;
            }
            else if (string.Compare(value, "FABRIC_E_COMMUNICATION_ERROR", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = FabricErrorCodes.FABRIC_E_COMMUNICATION_ERROR;
            }
            else if (string.Compare(value, "FABRIC_E_OPERATION_NOT_COMPLETE", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = FabricErrorCodes.FABRIC_E_OPERATION_NOT_COMPLETE;
            }
            else if (string.Compare(value, "FABRIC_E_TIMEOUT", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = FabricErrorCodes.FABRIC_E_TIMEOUT;
            }
            else if (string.Compare(value, "FABRIC_E_NODE_IS_UP", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = FabricErrorCodes.FABRIC_E_NODE_IS_UP;
            }
            else if (string.Compare(value, "E_FAIL", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = FabricErrorCodes.E_FAIL;
            }
            else if (string.Compare(value, "FABRIC_E_BACKUP_IS_ENABLED", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = FabricErrorCodes.FABRIC_E_BACKUP_IS_ENABLED;
            }
            else if (string.Compare(value, "FABRIC_E_RESTORE_SOURCE_TARGET_PARTITION_MISMATCH", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = FabricErrorCodes.FABRIC_E_RESTORE_SOURCE_TARGET_PARTITION_MISMATCH;
            }
            else if (string.Compare(value, "FABRIC_E_INVALID_FOR_STATELESS_SERVICES", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = FabricErrorCodes.FABRIC_E_INVALID_FOR_STATELESS_SERVICES;
            }
            else if (string.Compare(value, "FABRIC_E_BACKUP_NOT_ENABLED", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = FabricErrorCodes.FABRIC_E_BACKUP_NOT_ENABLED;
            }
            else if (string.Compare(value, "FABRIC_E_BACKUP_POLICY_NOT_EXISTING", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = FabricErrorCodes.FABRIC_E_BACKUP_POLICY_NOT_EXISTING;
            }
            else if (string.Compare(value, "FABRIC_E_FAULT_ANALYSIS_SERVICE_NOT_EXISTING", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = FabricErrorCodes.FABRIC_E_FAULT_ANALYSIS_SERVICE_NOT_EXISTING;
            }
            else if (string.Compare(value, "FABRIC_E_BACKUP_IN_PROGRESS", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = FabricErrorCodes.FABRIC_E_BACKUP_IN_PROGRESS;
            }
            else if (string.Compare(value, "FABRIC_E_RESTORE_IN_PROGRESS", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = FabricErrorCodes.FABRIC_E_RESTORE_IN_PROGRESS;
            }
            else if (string.Compare(value, "FABRIC_E_BACKUP_POLICY_ALREADY_EXISTING", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = FabricErrorCodes.FABRIC_E_BACKUP_POLICY_ALREADY_EXISTING;
            }
            else if (string.Compare(value, "FABRIC_E_INVALID_SERVICE_SCALING_POLICY", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = FabricErrorCodes.FABRIC_E_INVALID_SERVICE_SCALING_POLICY;
            }
            else if (string.Compare(value, "E_INVALIDARG", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = FabricErrorCodes.E_INVALIDARG;
            }
            else if (string.Compare(value, "FABRIC_E_SINGLE_INSTANCE_APPLICATION_ALREADY_EXISTS", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = FabricErrorCodes.FABRIC_E_SINGLE_INSTANCE_APPLICATION_ALREADY_EXISTS;
            }
            else if (string.Compare(value, "FABRIC_E_SINGLE_INSTANCE_APPLICATION_NOT_FOUND", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = FabricErrorCodes.FABRIC_E_SINGLE_INSTANCE_APPLICATION_NOT_FOUND;
            }
            else if (string.Compare(value, "FABRIC_E_VOLUME_ALREADY_EXISTS", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = FabricErrorCodes.FABRIC_E_VOLUME_ALREADY_EXISTS;
            }
            else if (string.Compare(value, "FABRIC_E_VOLUME_NOT_FOUND", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = FabricErrorCodes.FABRIC_E_VOLUME_NOT_FOUND;
            }
            else if (string.Compare(value, "SerializationError", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = FabricErrorCodes.SerializationError;
            }
            else if (string.Compare(value, "FABRIC_E_IMAGEBUILDER_RESERVED_DIRECTORY_ERROR", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = FabricErrorCodes.FABRIC_E_IMAGEBUILDER_RESERVED_DIRECTORY_ERROR;
            }
            else if (string.Compare(value, "FABRIC_E_CERTIFICATE_NOT_FOUND", StringComparison.OrdinalIgnoreCase) == 0)
            {
                obj = FabricErrorCodes.FABRIC_E_CERTIFICATE_NOT_FOUND;
            }

            return obj;
        }

        /// <summary>
        /// Serializes the enum value.
        /// </summary>
        /// <param name="writer">The <see cref="T: Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="value">The object to serialize to JSON.</param>
        public static void Serialize(JsonWriter writer, FabricErrorCodes? value)
        {
            switch (value)
            {
                case FabricErrorCodes.FABRIC_E_INVALID_PARTITION_KEY:
                    writer.WriteStringValue("FABRIC_E_INVALID_PARTITION_KEY");
                    break;
                case FabricErrorCodes.FABRIC_E_IMAGEBUILDER_VALIDATION_ERROR:
                    writer.WriteStringValue("FABRIC_E_IMAGEBUILDER_VALIDATION_ERROR");
                    break;
                case FabricErrorCodes.FABRIC_E_INVALID_ADDRESS:
                    writer.WriteStringValue("FABRIC_E_INVALID_ADDRESS");
                    break;
                case FabricErrorCodes.FABRIC_E_APPLICATION_NOT_UPGRADING:
                    writer.WriteStringValue("FABRIC_E_APPLICATION_NOT_UPGRADING");
                    break;
                case FabricErrorCodes.FABRIC_E_APPLICATION_UPGRADE_VALIDATION_ERROR:
                    writer.WriteStringValue("FABRIC_E_APPLICATION_UPGRADE_VALIDATION_ERROR");
                    break;
                case FabricErrorCodes.FABRIC_E_FABRIC_NOT_UPGRADING:
                    writer.WriteStringValue("FABRIC_E_FABRIC_NOT_UPGRADING");
                    break;
                case FabricErrorCodes.FABRIC_E_FABRIC_UPGRADE_VALIDATION_ERROR:
                    writer.WriteStringValue("FABRIC_E_FABRIC_UPGRADE_VALIDATION_ERROR");
                    break;
                case FabricErrorCodes.FABRIC_E_INVALID_CONFIGURATION:
                    writer.WriteStringValue("FABRIC_E_INVALID_CONFIGURATION");
                    break;
                case FabricErrorCodes.FABRIC_E_INVALID_NAME_URI:
                    writer.WriteStringValue("FABRIC_E_INVALID_NAME_URI");
                    break;
                case FabricErrorCodes.FABRIC_E_PATH_TOO_LONG:
                    writer.WriteStringValue("FABRIC_E_PATH_TOO_LONG");
                    break;
                case FabricErrorCodes.FABRIC_E_KEY_TOO_LARGE:
                    writer.WriteStringValue("FABRIC_E_KEY_TOO_LARGE");
                    break;
                case FabricErrorCodes.FABRIC_E_SERVICE_AFFINITY_CHAIN_NOT_SUPPORTED:
                    writer.WriteStringValue("FABRIC_E_SERVICE_AFFINITY_CHAIN_NOT_SUPPORTED");
                    break;
                case FabricErrorCodes.FABRIC_E_INVALID_ATOMIC_GROUP:
                    writer.WriteStringValue("FABRIC_E_INVALID_ATOMIC_GROUP");
                    break;
                case FabricErrorCodes.FABRIC_E_VALUE_EMPTY:
                    writer.WriteStringValue("FABRIC_E_VALUE_EMPTY");
                    break;
                case FabricErrorCodes.FABRIC_E_NODE_NOT_FOUND:
                    writer.WriteStringValue("FABRIC_E_NODE_NOT_FOUND");
                    break;
                case FabricErrorCodes.FABRIC_E_APPLICATION_TYPE_NOT_FOUND:
                    writer.WriteStringValue("FABRIC_E_APPLICATION_TYPE_NOT_FOUND");
                    break;
                case FabricErrorCodes.FABRIC_E_APPLICATION_NOT_FOUND:
                    writer.WriteStringValue("FABRIC_E_APPLICATION_NOT_FOUND");
                    break;
                case FabricErrorCodes.FABRIC_E_SERVICE_TYPE_NOT_FOUND:
                    writer.WriteStringValue("FABRIC_E_SERVICE_TYPE_NOT_FOUND");
                    break;
                case FabricErrorCodes.FABRIC_E_SERVICE_DOES_NOT_EXIST:
                    writer.WriteStringValue("FABRIC_E_SERVICE_DOES_NOT_EXIST");
                    break;
                case FabricErrorCodes.FABRIC_E_SERVICE_TYPE_TEMPLATE_NOT_FOUND:
                    writer.WriteStringValue("FABRIC_E_SERVICE_TYPE_TEMPLATE_NOT_FOUND");
                    break;
                case FabricErrorCodes.FABRIC_E_CONFIGURATION_SECTION_NOT_FOUND:
                    writer.WriteStringValue("FABRIC_E_CONFIGURATION_SECTION_NOT_FOUND");
                    break;
                case FabricErrorCodes.FABRIC_E_PARTITION_NOT_FOUND:
                    writer.WriteStringValue("FABRIC_E_PARTITION_NOT_FOUND");
                    break;
                case FabricErrorCodes.FABRIC_E_REPLICA_DOES_NOT_EXIST:
                    writer.WriteStringValue("FABRIC_E_REPLICA_DOES_NOT_EXIST");
                    break;
                case FabricErrorCodes.FABRIC_E_SERVICE_GROUP_DOES_NOT_EXIST:
                    writer.WriteStringValue("FABRIC_E_SERVICE_GROUP_DOES_NOT_EXIST");
                    break;
                case FabricErrorCodes.FABRIC_E_CONFIGURATION_PARAMETER_NOT_FOUND:
                    writer.WriteStringValue("FABRIC_E_CONFIGURATION_PARAMETER_NOT_FOUND");
                    break;
                case FabricErrorCodes.FABRIC_E_DIRECTORY_NOT_FOUND:
                    writer.WriteStringValue("FABRIC_E_DIRECTORY_NOT_FOUND");
                    break;
                case FabricErrorCodes.FABRIC_E_FABRIC_VERSION_NOT_FOUND:
                    writer.WriteStringValue("FABRIC_E_FABRIC_VERSION_NOT_FOUND");
                    break;
                case FabricErrorCodes.FABRIC_E_FILE_NOT_FOUND:
                    writer.WriteStringValue("FABRIC_E_FILE_NOT_FOUND");
                    break;
                case FabricErrorCodes.FABRIC_E_NAME_DOES_NOT_EXIST:
                    writer.WriteStringValue("FABRIC_E_NAME_DOES_NOT_EXIST");
                    break;
                case FabricErrorCodes.FABRIC_E_PROPERTY_DOES_NOT_EXIST:
                    writer.WriteStringValue("FABRIC_E_PROPERTY_DOES_NOT_EXIST");
                    break;
                case FabricErrorCodes.FABRIC_E_ENUMERATION_COMPLETED:
                    writer.WriteStringValue("FABRIC_E_ENUMERATION_COMPLETED");
                    break;
                case FabricErrorCodes.FABRIC_E_SERVICE_MANIFEST_NOT_FOUND:
                    writer.WriteStringValue("FABRIC_E_SERVICE_MANIFEST_NOT_FOUND");
                    break;
                case FabricErrorCodes.FABRIC_E_KEY_NOT_FOUND:
                    writer.WriteStringValue("FABRIC_E_KEY_NOT_FOUND");
                    break;
                case FabricErrorCodes.FABRIC_E_HEALTH_ENTITY_NOT_FOUND:
                    writer.WriteStringValue("FABRIC_E_HEALTH_ENTITY_NOT_FOUND");
                    break;
                case FabricErrorCodes.FABRIC_E_APPLICATION_TYPE_ALREADY_EXISTS:
                    writer.WriteStringValue("FABRIC_E_APPLICATION_TYPE_ALREADY_EXISTS");
                    break;
                case FabricErrorCodes.FABRIC_E_APPLICATION_ALREADY_EXISTS:
                    writer.WriteStringValue("FABRIC_E_APPLICATION_ALREADY_EXISTS");
                    break;
                case FabricErrorCodes.FABRIC_E_APPLICATION_ALREADY_IN_TARGET_VERSION:
                    writer.WriteStringValue("FABRIC_E_APPLICATION_ALREADY_IN_TARGET_VERSION");
                    break;
                case FabricErrorCodes.FABRIC_E_APPLICATION_TYPE_PROVISION_IN_PROGRESS:
                    writer.WriteStringValue("FABRIC_E_APPLICATION_TYPE_PROVISION_IN_PROGRESS");
                    break;
                case FabricErrorCodes.FABRIC_E_APPLICATION_UPGRADE_IN_PROGRESS:
                    writer.WriteStringValue("FABRIC_E_APPLICATION_UPGRADE_IN_PROGRESS");
                    break;
                case FabricErrorCodes.FABRIC_E_SERVICE_ALREADY_EXISTS:
                    writer.WriteStringValue("FABRIC_E_SERVICE_ALREADY_EXISTS");
                    break;
                case FabricErrorCodes.FABRIC_E_SERVICE_GROUP_ALREADY_EXISTS:
                    writer.WriteStringValue("FABRIC_E_SERVICE_GROUP_ALREADY_EXISTS");
                    break;
                case FabricErrorCodes.FABRIC_E_APPLICATION_TYPE_IN_USE:
                    writer.WriteStringValue("FABRIC_E_APPLICATION_TYPE_IN_USE");
                    break;
                case FabricErrorCodes.FABRIC_E_FABRIC_ALREADY_IN_TARGET_VERSION:
                    writer.WriteStringValue("FABRIC_E_FABRIC_ALREADY_IN_TARGET_VERSION");
                    break;
                case FabricErrorCodes.FABRIC_E_FABRIC_VERSION_ALREADY_EXISTS:
                    writer.WriteStringValue("FABRIC_E_FABRIC_VERSION_ALREADY_EXISTS");
                    break;
                case FabricErrorCodes.FABRIC_E_FABRIC_VERSION_IN_USE:
                    writer.WriteStringValue("FABRIC_E_FABRIC_VERSION_IN_USE");
                    break;
                case FabricErrorCodes.FABRIC_E_FABRIC_UPGRADE_IN_PROGRESS:
                    writer.WriteStringValue("FABRIC_E_FABRIC_UPGRADE_IN_PROGRESS");
                    break;
                case FabricErrorCodes.FABRIC_E_NAME_ALREADY_EXISTS:
                    writer.WriteStringValue("FABRIC_E_NAME_ALREADY_EXISTS");
                    break;
                case FabricErrorCodes.FABRIC_E_NAME_NOT_EMPTY:
                    writer.WriteStringValue("FABRIC_E_NAME_NOT_EMPTY");
                    break;
                case FabricErrorCodes.FABRIC_E_PROPERTY_CHECK_FAILED:
                    writer.WriteStringValue("FABRIC_E_PROPERTY_CHECK_FAILED");
                    break;
                case FabricErrorCodes.FABRIC_E_SERVICE_METADATA_MISMATCH:
                    writer.WriteStringValue("FABRIC_E_SERVICE_METADATA_MISMATCH");
                    break;
                case FabricErrorCodes.FABRIC_E_SERVICE_TYPE_MISMATCH:
                    writer.WriteStringValue("FABRIC_E_SERVICE_TYPE_MISMATCH");
                    break;
                case FabricErrorCodes.FABRIC_E_HEALTH_STALE_REPORT:
                    writer.WriteStringValue("FABRIC_E_HEALTH_STALE_REPORT");
                    break;
                case FabricErrorCodes.FABRIC_E_SEQUENCE_NUMBER_CHECK_FAILED:
                    writer.WriteStringValue("FABRIC_E_SEQUENCE_NUMBER_CHECK_FAILED");
                    break;
                case FabricErrorCodes.FABRIC_E_NODE_HAS_NOT_STOPPED_YET:
                    writer.WriteStringValue("FABRIC_E_NODE_HAS_NOT_STOPPED_YET");
                    break;
                case FabricErrorCodes.FABRIC_E_INSTANCE_ID_MISMATCH:
                    writer.WriteStringValue("FABRIC_E_INSTANCE_ID_MISMATCH");
                    break;
                case FabricErrorCodes.FABRIC_E_VALUE_TOO_LARGE:
                    writer.WriteStringValue("FABRIC_E_VALUE_TOO_LARGE");
                    break;
                case FabricErrorCodes.FABRIC_E_NO_WRITE_QUORUM:
                    writer.WriteStringValue("FABRIC_E_NO_WRITE_QUORUM");
                    break;
                case FabricErrorCodes.FABRIC_E_NOT_PRIMARY:
                    writer.WriteStringValue("FABRIC_E_NOT_PRIMARY");
                    break;
                case FabricErrorCodes.FABRIC_E_NOT_READY:
                    writer.WriteStringValue("FABRIC_E_NOT_READY");
                    break;
                case FabricErrorCodes.FABRIC_E_RECONFIGURATION_PENDING:
                    writer.WriteStringValue("FABRIC_E_RECONFIGURATION_PENDING");
                    break;
                case FabricErrorCodes.FABRIC_E_SERVICE_OFFLINE:
                    writer.WriteStringValue("FABRIC_E_SERVICE_OFFLINE");
                    break;
                case FabricErrorCodes.E_ABORT:
                    writer.WriteStringValue("E_ABORT");
                    break;
                case FabricErrorCodes.FABRIC_E_COMMUNICATION_ERROR:
                    writer.WriteStringValue("FABRIC_E_COMMUNICATION_ERROR");
                    break;
                case FabricErrorCodes.FABRIC_E_OPERATION_NOT_COMPLETE:
                    writer.WriteStringValue("FABRIC_E_OPERATION_NOT_COMPLETE");
                    break;
                case FabricErrorCodes.FABRIC_E_TIMEOUT:
                    writer.WriteStringValue("FABRIC_E_TIMEOUT");
                    break;
                case FabricErrorCodes.FABRIC_E_NODE_IS_UP:
                    writer.WriteStringValue("FABRIC_E_NODE_IS_UP");
                    break;
                case FabricErrorCodes.E_FAIL:
                    writer.WriteStringValue("E_FAIL");
                    break;
                case FabricErrorCodes.FABRIC_E_BACKUP_IS_ENABLED:
                    writer.WriteStringValue("FABRIC_E_BACKUP_IS_ENABLED");
                    break;
                case FabricErrorCodes.FABRIC_E_RESTORE_SOURCE_TARGET_PARTITION_MISMATCH:
                    writer.WriteStringValue("FABRIC_E_RESTORE_SOURCE_TARGET_PARTITION_MISMATCH");
                    break;
                case FabricErrorCodes.FABRIC_E_INVALID_FOR_STATELESS_SERVICES:
                    writer.WriteStringValue("FABRIC_E_INVALID_FOR_STATELESS_SERVICES");
                    break;
                case FabricErrorCodes.FABRIC_E_BACKUP_NOT_ENABLED:
                    writer.WriteStringValue("FABRIC_E_BACKUP_NOT_ENABLED");
                    break;
                case FabricErrorCodes.FABRIC_E_BACKUP_POLICY_NOT_EXISTING:
                    writer.WriteStringValue("FABRIC_E_BACKUP_POLICY_NOT_EXISTING");
                    break;
                case FabricErrorCodes.FABRIC_E_FAULT_ANALYSIS_SERVICE_NOT_EXISTING:
                    writer.WriteStringValue("FABRIC_E_FAULT_ANALYSIS_SERVICE_NOT_EXISTING");
                    break;
                case FabricErrorCodes.FABRIC_E_BACKUP_IN_PROGRESS:
                    writer.WriteStringValue("FABRIC_E_BACKUP_IN_PROGRESS");
                    break;
                case FabricErrorCodes.FABRIC_E_RESTORE_IN_PROGRESS:
                    writer.WriteStringValue("FABRIC_E_RESTORE_IN_PROGRESS");
                    break;
                case FabricErrorCodes.FABRIC_E_BACKUP_POLICY_ALREADY_EXISTING:
                    writer.WriteStringValue("FABRIC_E_BACKUP_POLICY_ALREADY_EXISTING");
                    break;
                case FabricErrorCodes.FABRIC_E_INVALID_SERVICE_SCALING_POLICY:
                    writer.WriteStringValue("FABRIC_E_INVALID_SERVICE_SCALING_POLICY");
                    break;
                case FabricErrorCodes.E_INVALIDARG:
                    writer.WriteStringValue("E_INVALIDARG");
                    break;
                case FabricErrorCodes.FABRIC_E_SINGLE_INSTANCE_APPLICATION_ALREADY_EXISTS:
                    writer.WriteStringValue("FABRIC_E_SINGLE_INSTANCE_APPLICATION_ALREADY_EXISTS");
                    break;
                case FabricErrorCodes.FABRIC_E_SINGLE_INSTANCE_APPLICATION_NOT_FOUND:
                    writer.WriteStringValue("FABRIC_E_SINGLE_INSTANCE_APPLICATION_NOT_FOUND");
                    break;
                case FabricErrorCodes.FABRIC_E_VOLUME_ALREADY_EXISTS:
                    writer.WriteStringValue("FABRIC_E_VOLUME_ALREADY_EXISTS");
                    break;
                case FabricErrorCodes.FABRIC_E_VOLUME_NOT_FOUND:
                    writer.WriteStringValue("FABRIC_E_VOLUME_NOT_FOUND");
                    break;
                case FabricErrorCodes.SerializationError:
                    writer.WriteStringValue("SerializationError");
                    break;
                case FabricErrorCodes.FABRIC_E_IMAGEBUILDER_RESERVED_DIRECTORY_ERROR:
                    writer.WriteStringValue("FABRIC_E_IMAGEBUILDER_RESERVED_DIRECTORY_ERROR");
                    break;
                case FabricErrorCodes.FABRIC_E_CERTIFICATE_NOT_FOUND:
                    writer.WriteStringValue("FABRIC_E_CERTIFICATE_NOT_FOUND");
                    break;
                default:
                    throw new ArgumentException($"Invalid value {value.ToString()} for enum type FabricErrorCodes");
            }
        }
    }
}
