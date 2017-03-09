// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------
namespace Microsoft.ServiceFabric.Actors.Runtime
{
    using System;

    /// <summary>
    /// Represents Timer set on an Actor
    /// </summary>
    public interface IActorTimer : IDisposable
    {
        /// <summary>
        /// Time when timer is first due.
        /// </summary>
        /// <value>Time as <see cref="System.TimeSpan"/> when timer is first due.</value>
        TimeSpan DueTime { get; }

        /// <summary>
        /// Periodic time when timer will be invoked.
        /// </summary>
        /// <value>Periodic time as <see cref="System.TimeSpan"/> when timer will be invoked.</value>
        TimeSpan Period { get; }
    }
}
