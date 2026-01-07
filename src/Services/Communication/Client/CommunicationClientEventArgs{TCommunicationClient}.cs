// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Communication.Client
{
    using System;

    /// <summary>
    /// Specifies the arguments for the communication client connected/disconnected events handler.
    /// </summary>
    /// <typeparam name="TCommunicationClient">Type of communication client</typeparam>
    public class CommunicationClientEventArgs<TCommunicationClient> : EventArgs
        where TCommunicationClient : ICommunicationClient
    {
        /// <summary>
        /// Gets or sets communication client for which the event is fired.
        /// </summary>
        /// <value>Communication client</value>
        public TCommunicationClient Client { get; set; }
    }
}
