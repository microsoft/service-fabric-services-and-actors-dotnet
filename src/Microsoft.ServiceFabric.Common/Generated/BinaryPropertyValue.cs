// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Describes a Service Fabric property value of type Binary.
    /// </summary>
    public partial class BinaryPropertyValue : PropertyValue
    {
        /// <summary>
        /// Initializes a new instance of the BinaryPropertyValue class.
        /// </summary>
        /// <param name="data">Array of bytes to be sent as an integer array. Each element of array is a number between 0 and
        /// 255.</param>
        public BinaryPropertyValue(
            byte[] data)
            : base(
                Common.PropertyValueKind.Binary)
        {
            data.ThrowIfNull(nameof(data));
            this.Data = data;
        }

        /// <summary>
        /// Gets array of bytes to be sent as an integer array. Each element of array is a number between 0 and 255.
        /// </summary>
        public byte[] Data { get; }
    }
}
