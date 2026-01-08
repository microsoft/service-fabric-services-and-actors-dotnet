// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// A container event.
    /// </summary>
    public partial class ContainerEvent
    {
        /// <summary>
        /// Initializes a new instance of the ContainerEvent class.
        /// </summary>
        /// <param name="name">The name of the container event.</param>
        /// <param name="count">The count of the event.</param>
        /// <param name="firstTimestamp">Date/time of the first event.</param>
        /// <param name="lastTimestamp">Date/time of the last event.</param>
        /// <param name="message">The event message</param>
        /// <param name="type">The event type.</param>
        public ContainerEvent(
            string name = default(string),
            int? count = default(int?),
            string firstTimestamp = default(string),
            string lastTimestamp = default(string),
            string message = default(string),
            string type = default(string))
        {
            this.Name = name;
            this.Count = count;
            this.FirstTimestamp = firstTimestamp;
            this.LastTimestamp = lastTimestamp;
            this.Message = message;
            this.Type = type;
        }

        /// <summary>
        /// Gets the name of the container event.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the count of the event.
        /// </summary>
        public int? Count { get; }

        /// <summary>
        /// Gets date/time of the first event.
        /// </summary>
        public string FirstTimestamp { get; }

        /// <summary>
        /// Gets date/time of the last event.
        /// </summary>
        public string LastTimestamp { get; }

        /// <summary>
        /// Gets the event message
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// Gets the event type.
        /// </summary>
        public string Type { get; }
    }
}
