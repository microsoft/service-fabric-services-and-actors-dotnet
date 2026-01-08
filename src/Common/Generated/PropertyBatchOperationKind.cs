// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    /// <summary>
    /// Defines values for PropertyBatchOperationKind.
    /// </summary>
    public enum PropertyBatchOperationKind
    {
        /// <summary>
        /// Indicates the property operation is invalid. All Service Fabric enumerations have the invalid type. The value is
        /// zero.
        /// </summary>
        Invalid,

        /// <summary>
        /// The operation will create or edit a property. The value is 1.
        /// </summary>
        Put,

        /// <summary>
        /// The operation will get a property. The value is 2.
        /// </summary>
        Get,

        /// <summary>
        /// The operation will check that a property exists or doesn't exists, depending on the provided value. The value is 3.
        /// </summary>
        CheckExists,

        /// <summary>
        /// The operation will ensure that the sequence number is equal to the provided value. The value is 4.
        /// </summary>
        CheckSequence,

        /// <summary>
        /// The operation will delete a property. The value is 5.
        /// </summary>
        Delete,

        /// <summary>
        /// The operation will ensure that the value of a property is equal to the provided value. The value is 7.
        /// </summary>
        CheckValue,
    }
}
