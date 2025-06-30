// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System.Diagnostics.Tracing;
using Microsoft.ServiceFabric.Diagnostics.Tracing.Config;

namespace Microsoft.ServiceFabric.Diagnostics.Tracing.Writer
{
    internal class TraceEvent
    {
        private readonly string eventName;
        private readonly EventLevel level;
        private readonly EventKeywords keywords;
        private readonly TraceConfig configMgr;
        private readonly string message;
        private bool filterState;

        internal TraceEvent(
            string eventName,
            EventLevel level,
            EventKeywords keywords,
            string message,
            TraceConfig configMgr)
        {
            this.eventName = eventName;
            this.level = level;
            this.keywords = keywords;
            this.configMgr = configMgr;
            this.message = message;
            this.UpdateSinkEnabledStatus();
            configMgr.OnFilterUpdate += this.UpdateSinkEnabledStatus;
        }

        public string Message
        {
            get { return this.message; }
        }

        public string EventName
        {
            get { return this.eventName; }
        }

        public EventLevel Level
        {
            get { return this.level; }
        }

        internal void UpdateSinkEnabledStatus()
        {
            this.filterState = this.configMgr.GetEventEnabledStatus(this.level, this.keywords, this.eventName);
        }

        internal bool IsEventSinkEnabled()
        {
            return this.filterState;
        }
    }
}
