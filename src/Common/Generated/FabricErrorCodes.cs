// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    /// <summary>
    /// Defines values for FabricErrorCodes.
    /// </summary>
    public enum FabricErrorCodes
    {
        /// <summary>
        /// Unknown Error Code.
        /// </summary>
        UNKNOWN = 0,

        /// <summary>
        /// Requested resource/content/path does not exist.
        /// </summary>
        FABRIC_E_DOES_NOT_EXIST,

        /// <summary>
        /// FABRIC_E_INVALID_PARTITION_KEY.
        /// </summary>
        FABRIC_E_INVALID_PARTITION_KEY,

        /// <summary>
        /// FABRIC_E_IMAGEBUILDER_VALIDATION_ERROR.
        /// </summary>
        FABRIC_E_IMAGEBUILDER_VALIDATION_ERROR,

        /// <summary>
        /// FABRIC_E_INVALID_ADDRESS.
        /// </summary>
        FABRIC_E_INVALID_ADDRESS,

        /// <summary>
        /// FABRIC_E_APPLICATION_NOT_UPGRADING.
        /// </summary>
        FABRIC_E_APPLICATION_NOT_UPGRADING,

        /// <summary>
        /// FABRIC_E_APPLICATION_UPGRADE_VALIDATION_ERROR.
        /// </summary>
        FABRIC_E_APPLICATION_UPGRADE_VALIDATION_ERROR,

        /// <summary>
        /// FABRIC_E_FABRIC_NOT_UPGRADING.
        /// </summary>
        FABRIC_E_FABRIC_NOT_UPGRADING,

        /// <summary>
        /// FABRIC_E_FABRIC_UPGRADE_VALIDATION_ERROR.
        /// </summary>
        FABRIC_E_FABRIC_UPGRADE_VALIDATION_ERROR,

        /// <summary>
        /// FABRIC_E_INVALID_CONFIGURATION.
        /// </summary>
        FABRIC_E_INVALID_CONFIGURATION,

        /// <summary>
        /// FABRIC_E_INVALID_NAME_URI.
        /// </summary>
        FABRIC_E_INVALID_NAME_URI,

        /// <summary>
        /// FABRIC_E_PATH_TOO_LONG.
        /// </summary>
        FABRIC_E_PATH_TOO_LONG,

        /// <summary>
        /// FABRIC_E_KEY_TOO_LARGE.
        /// </summary>
        FABRIC_E_KEY_TOO_LARGE,

        /// <summary>
        /// FABRIC_E_SERVICE_AFFINITY_CHAIN_NOT_SUPPORTED.
        /// </summary>
        FABRIC_E_SERVICE_AFFINITY_CHAIN_NOT_SUPPORTED,

        /// <summary>
        /// FABRIC_E_INVALID_ATOMIC_GROUP.
        /// </summary>
        FABRIC_E_INVALID_ATOMIC_GROUP,

        /// <summary>
        /// FABRIC_E_VALUE_EMPTY.
        /// </summary>
        FABRIC_E_VALUE_EMPTY,

        /// <summary>
        /// FABRIC_E_NODE_NOT_FOUND.
        /// </summary>
        FABRIC_E_NODE_NOT_FOUND,

        /// <summary>
        /// FABRIC_E_APPLICATION_TYPE_NOT_FOUND.
        /// </summary>
        FABRIC_E_APPLICATION_TYPE_NOT_FOUND,

        /// <summary>
        /// FABRIC_E_APPLICATION_NOT_FOUND.
        /// </summary>
        FABRIC_E_APPLICATION_NOT_FOUND,

        /// <summary>
        /// FABRIC_E_SERVICE_TYPE_NOT_FOUND.
        /// </summary>
        FABRIC_E_SERVICE_TYPE_NOT_FOUND,

        /// <summary>
        /// FABRIC_E_SERVICE_DOES_NOT_EXIST.
        /// </summary>
        FABRIC_E_SERVICE_DOES_NOT_EXIST,

        /// <summary>
        /// FABRIC_E_SERVICE_TYPE_TEMPLATE_NOT_FOUND.
        /// </summary>
        FABRIC_E_SERVICE_TYPE_TEMPLATE_NOT_FOUND,

        /// <summary>
        /// FABRIC_E_CONFIGURATION_SECTION_NOT_FOUND.
        /// </summary>
        FABRIC_E_CONFIGURATION_SECTION_NOT_FOUND,

        /// <summary>
        /// FABRIC_E_PARTITION_NOT_FOUND.
        /// </summary>
        FABRIC_E_PARTITION_NOT_FOUND,

        /// <summary>
        /// FABRIC_E_REPLICA_DOES_NOT_EXIST.
        /// </summary>
        FABRIC_E_REPLICA_DOES_NOT_EXIST,

        /// <summary>
        /// FABRIC_E_SERVICE_GROUP_DOES_NOT_EXIST.
        /// </summary>
        FABRIC_E_SERVICE_GROUP_DOES_NOT_EXIST,

        /// <summary>
        /// FABRIC_E_CONFIGURATION_PARAMETER_NOT_FOUND.
        /// </summary>
        FABRIC_E_CONFIGURATION_PARAMETER_NOT_FOUND,

        /// <summary>
        /// FABRIC_E_DIRECTORY_NOT_FOUND.
        /// </summary>
        FABRIC_E_DIRECTORY_NOT_FOUND,

        /// <summary>
        /// FABRIC_E_FABRIC_VERSION_NOT_FOUND.
        /// </summary>
        FABRIC_E_FABRIC_VERSION_NOT_FOUND,

        /// <summary>
        /// FABRIC_E_FILE_NOT_FOUND.
        /// </summary>
        FABRIC_E_FILE_NOT_FOUND,

        /// <summary>
        /// FABRIC_E_NAME_DOES_NOT_EXIST.
        /// </summary>
        FABRIC_E_NAME_DOES_NOT_EXIST,

        /// <summary>
        /// FABRIC_E_PROPERTY_DOES_NOT_EXIST.
        /// </summary>
        FABRIC_E_PROPERTY_DOES_NOT_EXIST,

        /// <summary>
        /// FABRIC_E_ENUMERATION_COMPLETED.
        /// </summary>
        FABRIC_E_ENUMERATION_COMPLETED,

        /// <summary>
        /// FABRIC_E_SERVICE_MANIFEST_NOT_FOUND.
        /// </summary>
        FABRIC_E_SERVICE_MANIFEST_NOT_FOUND,

        /// <summary>
        /// FABRIC_E_KEY_NOT_FOUND.
        /// </summary>
        FABRIC_E_KEY_NOT_FOUND,

        /// <summary>
        /// FABRIC_E_HEALTH_ENTITY_NOT_FOUND.
        /// </summary>
        FABRIC_E_HEALTH_ENTITY_NOT_FOUND,

        /// <summary>
        /// FABRIC_E_APPLICATION_TYPE_ALREADY_EXISTS.
        /// </summary>
        FABRIC_E_APPLICATION_TYPE_ALREADY_EXISTS,

        /// <summary>
        /// FABRIC_E_APPLICATION_ALREADY_EXISTS.
        /// </summary>
        FABRIC_E_APPLICATION_ALREADY_EXISTS,

        /// <summary>
        /// FABRIC_E_APPLICATION_ALREADY_IN_TARGET_VERSION.
        /// </summary>
        FABRIC_E_APPLICATION_ALREADY_IN_TARGET_VERSION,

        /// <summary>
        /// FABRIC_E_APPLICATION_TYPE_PROVISION_IN_PROGRESS.
        /// </summary>
        FABRIC_E_APPLICATION_TYPE_PROVISION_IN_PROGRESS,

        /// <summary>
        /// FABRIC_E_APPLICATION_UPGRADE_IN_PROGRESS.
        /// </summary>
        FABRIC_E_APPLICATION_UPGRADE_IN_PROGRESS,

        /// <summary>
        /// FABRIC_E_SERVICE_ALREADY_EXISTS.
        /// </summary>
        FABRIC_E_SERVICE_ALREADY_EXISTS,

        /// <summary>
        /// FABRIC_E_SERVICE_GROUP_ALREADY_EXISTS.
        /// </summary>
        FABRIC_E_SERVICE_GROUP_ALREADY_EXISTS,

        /// <summary>
        /// FABRIC_E_APPLICATION_TYPE_IN_USE.
        /// </summary>
        FABRIC_E_APPLICATION_TYPE_IN_USE,

        /// <summary>
        /// FABRIC_E_FABRIC_ALREADY_IN_TARGET_VERSION.
        /// </summary>
        FABRIC_E_FABRIC_ALREADY_IN_TARGET_VERSION,

        /// <summary>
        /// FABRIC_E_FABRIC_VERSION_ALREADY_EXISTS.
        /// </summary>
        FABRIC_E_FABRIC_VERSION_ALREADY_EXISTS,

        /// <summary>
        /// FABRIC_E_FABRIC_VERSION_IN_USE.
        /// </summary>
        FABRIC_E_FABRIC_VERSION_IN_USE,

        /// <summary>
        /// FABRIC_E_FABRIC_UPGRADE_IN_PROGRESS.
        /// </summary>
        FABRIC_E_FABRIC_UPGRADE_IN_PROGRESS,

        /// <summary>
        /// FABRIC_E_NAME_ALREADY_EXISTS.
        /// </summary>
        FABRIC_E_NAME_ALREADY_EXISTS,

        /// <summary>
        /// FABRIC_E_NAME_NOT_EMPTY.
        /// </summary>
        FABRIC_E_NAME_NOT_EMPTY,

        /// <summary>
        /// FABRIC_E_PROPERTY_CHECK_FAILED.
        /// </summary>
        FABRIC_E_PROPERTY_CHECK_FAILED,

        /// <summary>
        /// FABRIC_E_SERVICE_METADATA_MISMATCH.
        /// </summary>
        FABRIC_E_SERVICE_METADATA_MISMATCH,

        /// <summary>
        /// FABRIC_E_SERVICE_TYPE_MISMATCH.
        /// </summary>
        FABRIC_E_SERVICE_TYPE_MISMATCH,

        /// <summary>
        /// FABRIC_E_HEALTH_STALE_REPORT.
        /// </summary>
        FABRIC_E_HEALTH_STALE_REPORT,

        /// <summary>
        /// FABRIC_E_SEQUENCE_NUMBER_CHECK_FAILED.
        /// </summary>
        FABRIC_E_SEQUENCE_NUMBER_CHECK_FAILED,

        /// <summary>
        /// FABRIC_E_NODE_HAS_NOT_STOPPED_YET.
        /// </summary>
        FABRIC_E_NODE_HAS_NOT_STOPPED_YET,

        /// <summary>
        /// FABRIC_E_INSTANCE_ID_MISMATCH.
        /// </summary>
        FABRIC_E_INSTANCE_ID_MISMATCH,

        /// <summary>
        /// FABRIC_E_VALUE_TOO_LARGE.
        /// </summary>
        FABRIC_E_VALUE_TOO_LARGE,

        /// <summary>
        /// FABRIC_E_NO_WRITE_QUORUM.
        /// </summary>
        FABRIC_E_NO_WRITE_QUORUM,

        /// <summary>
        /// FABRIC_E_NOT_PRIMARY.
        /// </summary>
        FABRIC_E_NOT_PRIMARY,

        /// <summary>
        /// FABRIC_E_NOT_READY.
        /// </summary>
        FABRIC_E_NOT_READY,

        /// <summary>
        /// FABRIC_E_RECONFIGURATION_PENDING.
        /// </summary>
        FABRIC_E_RECONFIGURATION_PENDING,

        /// <summary>
        /// FABRIC_E_SERVICE_OFFLINE.
        /// </summary>
        FABRIC_E_SERVICE_OFFLINE,

        /// <summary>
        /// E_ABORT.
        /// </summary>
        E_ABORT,

        /// <summary>
        /// FABRIC_E_COMMUNICATION_ERROR.
        /// </summary>
        FABRIC_E_COMMUNICATION_ERROR,

        /// <summary>
        /// FABRIC_E_OPERATION_NOT_COMPLETE.
        /// </summary>
        FABRIC_E_OPERATION_NOT_COMPLETE,

        /// <summary>
        /// FABRIC_E_TIMEOUT.
        /// </summary>
        FABRIC_E_TIMEOUT,

        /// <summary>
        /// FABRIC_E_NODE_IS_UP.
        /// </summary>
        FABRIC_E_NODE_IS_UP,

        /// <summary>
        /// E_FAIL.
        /// </summary>
        E_FAIL,

        /// <summary>
        /// FABRIC_E_BACKUP_IS_ENABLED.
        /// </summary>
        FABRIC_E_BACKUP_IS_ENABLED,

        /// <summary>
        /// FABRIC_E_RESTORE_SOURCE_TARGET_PARTITION_MISMATCH.
        /// </summary>
        FABRIC_E_RESTORE_SOURCE_TARGET_PARTITION_MISMATCH,

        /// <summary>
        /// FABRIC_E_INVALID_FOR_STATELESS_SERVICES.
        /// </summary>
        FABRIC_E_INVALID_FOR_STATELESS_SERVICES,

        /// <summary>
        /// FABRIC_E_BACKUP_NOT_ENABLED.
        /// </summary>
        FABRIC_E_BACKUP_NOT_ENABLED,

        /// <summary>
        /// FABRIC_E_BACKUP_POLICY_NOT_EXISTING.
        /// </summary>
        FABRIC_E_BACKUP_POLICY_NOT_EXISTING,

        /// <summary>
        /// FABRIC_E_FAULT_ANALYSIS_SERVICE_NOT_EXISTING.
        /// </summary>
        FABRIC_E_FAULT_ANALYSIS_SERVICE_NOT_EXISTING,

        /// <summary>
        /// FABRIC_E_BACKUP_IN_PROGRESS.
        /// </summary>
        FABRIC_E_BACKUP_IN_PROGRESS,

        /// <summary>
        /// FABRIC_E_RESTORE_IN_PROGRESS.
        /// </summary>
        FABRIC_E_RESTORE_IN_PROGRESS,

        /// <summary>
        /// FABRIC_E_BACKUP_POLICY_ALREADY_EXISTING.
        /// </summary>
        FABRIC_E_BACKUP_POLICY_ALREADY_EXISTING,

        /// <summary>
        /// FABRIC_E_INVALID_SERVICE_SCALING_POLICY.
        /// </summary>
        FABRIC_E_INVALID_SERVICE_SCALING_POLICY,

        /// <summary>
        /// E_INVALIDARG.
        /// </summary>
        E_INVALIDARG,

        /// <summary>
        /// FABRIC_E_SINGLE_INSTANCE_APPLICATION_ALREADY_EXISTS.
        /// </summary>
        FABRIC_E_SINGLE_INSTANCE_APPLICATION_ALREADY_EXISTS,

        /// <summary>
        /// FABRIC_E_SINGLE_INSTANCE_APPLICATION_NOT_FOUND.
        /// </summary>
        FABRIC_E_SINGLE_INSTANCE_APPLICATION_NOT_FOUND,

        /// <summary>
        /// FABRIC_E_VOLUME_ALREADY_EXISTS.
        /// </summary>
        FABRIC_E_VOLUME_ALREADY_EXISTS,

        /// <summary>
        /// FABRIC_E_VOLUME_NOT_FOUND.
        /// </summary>
        FABRIC_E_VOLUME_NOT_FOUND,

        /// <summary>
        /// SerializationError.
        /// </summary>
        SerializationError,

        /// <summary>
        /// FABRIC_E_IMAGEBUILDER_RESERVED_DIRECTORY_ERROR.
        /// </summary>
        FABRIC_E_IMAGEBUILDER_RESERVED_DIRECTORY_ERROR,

        /// <summary>
        /// FABRIC_E_CERTIFICATE_NOT_FOUND.
        /// </summary>
        FABRIC_E_CERTIFICATE_NOT_FOUND,
    }
}
