// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------
namespace Microsoft.ServiceFabric.Actors.Runtime
{
    using System;
    using System.Fabric.Common;

    /// <summary>
    /// Represents change to an actor state with a given state name.
    /// </summary>
    public sealed class ActorStateChange
    {
        private readonly string stateName;
        private readonly Type type;
        private readonly object value;
        private readonly StateChangeKind changeKind;

        /// <summary>
        /// Creates an instance of ActorStateChange class.
        /// </summary>
        /// <param name="stateName">Name of the actor state</param>
        /// <param name="type">Type of value associated with given actor state name.</param>
        /// <param name="value">Value associated with given actor state name.</param>
        /// <param name="changeKind">Kind of state change for given actor state name.</param>
        public ActorStateChange(string stateName, Type type, object value, StateChangeKind changeKind)
        {
            Requires.Argument("stateName", stateName).NotNull();

            this.stateName = stateName;
            this.type = type;
            this.value = value;
            this.changeKind = changeKind;
        }

        /// <summary>
        /// Gets name of the actor state.
        /// </summary>
        /// <value>
        /// Name of the actor state.
        /// </value>
        public string StateName
        {
            get { return this.stateName; }
        }

        /// <summary>
        /// Gets the type of value associated with given actor state name.
        /// </summary>
        /// <value>
        /// Type of value associated with given actor state name.
        /// </value>
        public Type Type
        {
            get { return this.type; }
        }

        /// <summary>
        /// Gets the value associated with given actor state name.
        /// </summary>
        /// <value>
        /// Value associated with given actor state name.
        /// </value>
        public object Value
        {
            get { return this.value; }
        }

        /// <summary>
        /// Gets the kind of state change for given actor state name.
        /// </summary>
        /// <value>
        /// Kind of state change for given actor state name.
        /// </value>
        public StateChangeKind ChangeKind
        {
            get { return this.changeKind; }
        }
    }
}
