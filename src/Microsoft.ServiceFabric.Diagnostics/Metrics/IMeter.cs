// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Diagnostics.Metrics
{
    /// <summary>
    /// Defines a meter for recording telemetry values.
    /// </summary>
    /// <typeparam name="TValueType">The type of the value to be recorded.</typeparam>
    internal interface IMeter<TValueType>
    {
        /// <summary>
        /// Records a telemetry value.
        /// </summary>
        /// <param name="value">The value to record.</param>
        void Record(TValueType value);
    }
}
