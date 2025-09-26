// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;

namespace Microsoft.ServiceFabric.Services.Remoting.V2.Runtime
{
    /// <summary>
    /// Exception serialization option to use(applicable only to V2 Remoting).
    /// </summary>
    [Obsolete(DeprecationMessage.RemotingV1)]
    public enum ExceptionSerialization
    {
        /// <summary>
        /// Uses DCS to serialize exception details in service remoting message.
        /// </summary>
        Default,

        /// <summary>
        /// Uses binary formatter to serialize exception details in service remoting message.
        /// To be used in compat scenarios.
        /// </summary>
        BinaryFormatter,
    }
}