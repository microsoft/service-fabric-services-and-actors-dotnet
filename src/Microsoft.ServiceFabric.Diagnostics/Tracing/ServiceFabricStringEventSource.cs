// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System.Diagnostics.Tracing;

namespace Microsoft.ServiceFabric.Diagnostics.Tracing
{
    [EventSource(Guid = "74CF0846-E6A3-4a3e-A10F-80FD527DA5FD", Name = "StringEvent")]
    sealed class ServiceFabricStringEventSource : ServiceFabricEventSource, ITextEventSource
    {
        internal static ServiceFabricStringEventSource Instance { get; private set; } = new ServiceFabricStringEventSource();

        private ServiceFabricStringEventSource()
        {
        }

        [Event(InfoTextEventId, Message = TextEventFormat, Level = EventLevel.Informational, Keywords = Keywords.Default)]
        public void InfoText(string id, string type, string message) =>
            WriteEvent(InfoTextEventId, id, type, message);

        [Event(WarningTextEventId, Message = TextEventFormat, Level = EventLevel.Warning, Keywords = Keywords.Default)]
        public void WarningText(string id, string type, string message) =>
            WriteEvent(WarningTextEventId, id, type, message);

        [Event(ErrorTextEventId, Message = TextEventFormat, Level = EventLevel.Error, Keywords = Keywords.Default)]
        public void ErrorText(string id, string type, string message) =>
            WriteEvent(ErrorTextEventId, id, type, message);

        [Event(NoiseTextEventId, Message = TextEventFormat, Level = EventLevel.Verbose, Keywords = Keywords.Default)]
        public void NoiseText(string id, string type, string message) =>
            WriteEvent(NoiseTextEventId, id, type, message);

        public static class Keywords
        {
            public const EventKeywords Default = (EventKeywords)0x0001;
        }
    }
}
