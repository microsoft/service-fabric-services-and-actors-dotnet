// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Client.Http
{
    using System.Diagnostics.Tracing;

    /// <summary>
    /// Wrapper class on top of EventSource to writes events for Microsoft.ServiceFabric.Client.Http library.
    /// </summary>
    [EventSource(Guid = "c4766d6f-5414-5d26-48de-873499ff0976", Name = "ServiceFabricHttpClient")]
    internal sealed class ServiceFabricHttpClientEventSource : EventSource
    {
        /// <summary>
        /// Gets instance of <see cref="ServiceFabricHttpClientEventSource"/> class.
        /// </summary>
        internal static readonly ServiceFabricHttpClientEventSource Current = new ServiceFabricHttpClientEventSource();

        /// <summary>
        /// Prevents a default instance of the <see cref="ServiceFabricHttpClientEventSource" /> class from being created.
        /// </summary>
        private ServiceFabricHttpClientEventSource()
        {
        }

        #region Events
        
        /// <summary>
        /// Write an informational text event.
        /// </summary>
        /// <param name="id">Correlation id to help in diagnosis with other traces. It can be empty if no correlation is needed with other traces,</param>
        /// <param name="message">Message to write to event.</param>
        [Event(1, Message = "{1}", Level = EventLevel.Informational, Keywords = Keywords.Default)]
        public void InfoMessage(string id, string message)
        {
            this.WriteEvent(1, id, message);
        }

        /// <summary>
        /// Write an Warning text event.
        /// </summary>
        /// <param name="id">Correlation id to help in diagnosis with other traces. It can be empty if no correlation is needed with other traces,</param>
        /// <param name="message">Message to write to event.</param>
        [Event(2, Message = "{1}", Level = EventLevel.Warning, Keywords = Keywords.Default)]
        public void WarningMessage(string id, string message)
        {
            this.WriteEvent(2, id, message);
        }

        /// <summary>
        /// Write an Error text event.
        /// </summary>
        /// <param name="id">Correlation id to help in diagnosis with other traces. It can be empty if no correlation is needed with other traces,</param>
        /// <param name="message">Message to write to event.</param>
        [Event(3, Message = "{1}", Level = EventLevel.Error, Keywords = Keywords.Default)]
        public void ErrorMessage(string id, string message)
        {
            this.WriteEvent(3, id, message);
        }

        /// <summary>
        /// Write an Noise text event.
        /// </summary>
        /// <param name="id">Correlation id to help in diagnosis with other traces. It can be empty if no correlation is needed with other traces,</param>
        /// <param name="message">Message to write to event.</param>
        [Event(4, Message = "{1}", Level = EventLevel.Verbose, Keywords = Keywords.Default)]
        public void NoiseMessage(string id, string message)
        {
            this.WriteEvent(4, id, message);
        }

        /// <summary>
        /// Write an informational text event when request is send by client.
        /// </summary>
        /// <param name="id">Correlation id to help in diagnosis with other traces..</param>
        /// <param name="message">Message to write to event.</param>
        [Event(5, Message = "{1}", Level = EventLevel.Informational, Keywords = Keywords.Default)]
        public void Send(string id, string message)
        {
            this.WriteEvent(5, id, message);
        }

        /// <summary>
        /// Write an informational text event when successful response to a request is received by client.
        /// </summary>
        /// <param name="id">Correlation id to help in diagnosis with other traces.</param>
        /// <param name="message">Message to write to event.</param>
        [Event(6, Message = "{1}", Level = EventLevel.Informational, Keywords = Keywords.Default)]
        public void SuccessResponse(string id, string message)
        {
            this.WriteEvent(6, id, message);
        }

        /// <summary>
        /// Write a Error text event when error code is received by client.
        /// </summary>
        /// <param name="id">Correlation id to help in diagnosis with other traces.</param>
        /// <param name="message">Message to write to event.</param>
        [Event(7, Message = "{1}", Level = EventLevel.Error, Keywords = Keywords.Default)]
        public void ErrorResponse(string id, string message)
        {
            this.WriteEvent(7, id, message);
        }

        /// <summary>
        /// Write a Warning text when remote server certificate validation fails.
        /// </summary>
        /// <param name="id">Correlation id to help in diagnosis with other traces.</param>
        /// <param name="message">Message to write to event.</param>
        [Event(8, Message = "{1}", Level = EventLevel.Warning, Keywords = Keywords.Default)]
        public void RemoteCertValidationError(string id, string message)
        {
            this.WriteEvent(8, id, message);
        }

        /// <summary>
        /// Write a Warning text when client cert is  invalid.
        /// </summary>
        /// <param name="id">Correlation id to help in diagnosis with other traces.</param>
        /// <param name="message">Message to write to event.</param>
        [Event(9, Message = "{1}", Level = EventLevel.Warning, Keywords = Keywords.Default)]
        public void ClientCertInvalid(string id, string message)
        {
            this.WriteEvent(9, id, message);
        }        
        #endregion

        #region Keywords / Tasks / Opcodes
        
        /// <summary>
        /// Class containing Event Keywords.
        /// </summary>
        public static class Keywords
        {
            /// <summary>
            /// Default EventKeywords
            /// </summary>
            public const EventKeywords Default = (EventKeywords)0x0001;
        }
        #endregion
    }
}
