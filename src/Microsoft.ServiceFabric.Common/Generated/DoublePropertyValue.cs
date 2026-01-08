// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Describes a Service Fabric property value of type Double.
    /// </summary>
    public partial class DoublePropertyValue : PropertyValue
    {
        /// <summary>
        /// Initializes a new instance of the DoublePropertyValue class.
        /// </summary>
        /// <param name="data">The data of the property value.</param>
        public DoublePropertyValue(
            double? data)
            : base(
                Common.PropertyValueKind.Double)
        {
            data.ThrowIfNull(nameof(data));
            this.Data = data;
        }

        /// <summary>
        /// Gets the data of the property value.
        /// </summary>
        public double? Data { get; }
    }
}
