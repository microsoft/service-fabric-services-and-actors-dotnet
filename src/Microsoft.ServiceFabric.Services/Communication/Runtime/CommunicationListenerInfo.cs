// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Communication.Runtime
{
    /// <summary>
    /// Represents the communication listener and its name.
    /// </summary>
    internal class CommunicationListenerInfo
    {
        internal string Name { get; set; }

        internal ICommunicationListener Listener { get; set; }
    }
}
