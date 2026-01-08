// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Runtime information of a container instance.
    /// </summary>
    public partial class ContainerInstanceView
    {
        /// <summary>
        /// Initializes a new instance of the ContainerInstanceView class.
        /// </summary>
        /// <param name="restartCount">The number of times the container has been restarted.</param>
        /// <param name="currentState">Current container instance state.</param>
        /// <param name="previousState">Previous container instance state.</param>
        /// <param name="events">The events of this container instance.</param>
        public ContainerInstanceView(
            int? restartCount = default(int?),
            ContainerState currentState = default(ContainerState),
            ContainerState previousState = default(ContainerState),
            IEnumerable<ContainerEvent> events = default(IEnumerable<ContainerEvent>))
        {
            this.RestartCount = restartCount;
            this.CurrentState = currentState;
            this.PreviousState = previousState;
            this.Events = events;
        }

        /// <summary>
        /// Gets the number of times the container has been restarted.
        /// </summary>
        public int? RestartCount { get; }

        /// <summary>
        /// Gets current container instance state.
        /// </summary>
        public ContainerState CurrentState { get; }

        /// <summary>
        /// Gets previous container instance state.
        /// </summary>
        public ContainerState PreviousState { get; }

        /// <summary>
        /// Gets the events of this container instance.
        /// </summary>
        public IEnumerable<ContainerEvent> Events { get; }
    }
}
