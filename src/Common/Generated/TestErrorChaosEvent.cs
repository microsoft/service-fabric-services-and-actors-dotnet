// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Describes a Chaos event that gets generated when an unexpected event occurs in the Chaos engine.
    /// For example, due to the cluster snapshot being inconsistent, while faulting an entity, Chaos found that the entity
    /// was already faulted -- which would be an unexpected event.
    /// </summary>
    public partial class TestErrorChaosEvent : ChaosEvent
    {
        /// <summary>
        /// Initializes a new instance of the TestErrorChaosEvent class.
        /// </summary>
        /// <param name="timeStampUtc">The UTC timestamp when this Chaos event was generated.</param>
        /// <param name="reason">Describes why TestErrorChaosEvent was generated. For example, Chaos tries to fault a partition
        /// but finds that the partition is no longer fault tolerant, then a TestErrorEvent gets generated with the reason
        /// stating that the partition is not fault tolerant.</param>
        public TestErrorChaosEvent(
            DateTime? timeStampUtc,
            string reason = default(string))
            : base(
                timeStampUtc,
                Common.ChaosEventKind.TestError)
        {
            this.Reason = reason;
        }

        /// <summary>
        /// Gets why TestErrorChaosEvent was generated. For example, Chaos tries to fault a partition but finds that the
        /// partition is no longer fault tolerant, then a TestErrorEvent gets generated with the reason stating that the
        /// partition is not fault tolerant.
        /// </summary>
        public string Reason { get; }
    }
}
