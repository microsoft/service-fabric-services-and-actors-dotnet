// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System.Diagnostics.Tracing;

namespace Microsoft.ServiceFabric.Client.Http
{
    [EventSource(Guid = "c4766d6f-5414-5d26-48de-873499ff0976", Name = "ServiceFabricHttpClient")]
    sealed class ServiceFabricHttpClientEventSource : EventSource
    {
        internal static readonly ServiceFabricHttpClientEventSource Current = new ServiceFabricHttpClientEventSource();

        // Prevents a default instance of the <see cref="ServiceFabricHttpClientEventSource" /> class from being created.
        ServiceFabricHttpClientEventSource() {}

        [Event(1, Message = "{1}", Level = EventLevel.Informational, Keywords = Keywords.Default)]
        public void InfoMessage(string id, string message) => WriteEvent(1, id, message);

        [Event(2, Message = "{1}", Level = EventLevel.Warning, Keywords = Keywords.Default)]
        public void WarningMessage(string id, string message) => WriteEvent(2, id, message);

        [Event(3, Message = "{1}", Level = EventLevel.Error, Keywords = Keywords.Default)]
        public void ErrorMessage(string id, string message) => WriteEvent(3, id, message);

        [Event(4, Message = "{1}", Level = EventLevel.Verbose, Keywords = Keywords.Default)]
        public void NoiseMessage(string id, string message) => WriteEvent(4, id, message);

        [Event(5, Message = "{1}", Level = EventLevel.Informational, Keywords = Keywords.Default)]
        public void Send(string id, string message) => WriteEvent(5, id, message);

        [Event(6, Message = "{1}", Level = EventLevel.Informational, Keywords = Keywords.Default)]
        public void SuccessResponse(string id, string message) => WriteEvent(6, id, message);

        [Event(7, Message = "{1}", Level = EventLevel.Error, Keywords = Keywords.Default)]
        public void ErrorResponse(string id, string message) => WriteEvent(7, id, message);

        [Event(8, Message = "{1}", Level = EventLevel.Warning, Keywords = Keywords.Default)]
        public void RemoteCertValidationError(string id, string message) => WriteEvent(8, id, message);

        [Event(9, Message = "{1}", Level = EventLevel.Warning, Keywords = Keywords.Default)]
        public void ClientCertInvalid(string id, string message) => WriteEvent(9, id, message);

        static class Keywords
        {
            public const EventKeywords Default = (EventKeywords)0x0001;
        }
    }
}
