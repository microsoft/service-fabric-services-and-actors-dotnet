// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Communication.Runtime
{
    using System;
    using System.Fabric;

    /// <summary>
    /// Represents the communication listener and its properties for a Stateful Service replica.
    /// Endpoints given out by the communication listener are associated with the name of the communication listener.
    /// </summary>
    public sealed class ServiceReplicaListener
    {
        /// <summary>
        /// <para>The default name of the Service replica listener.</para>
        /// </summary>
        /// <value>
        /// <para>The default name of the Service replica listener.</para>
        /// </value>
        public const string DefaultName = "";

        /// <summary>
        /// <para>Gets the name of the communication listener.</para>
        /// </summary>
        /// <value>
        /// <para>The name of the communication listener.</para>
        /// </value>
        public string Name { get; private set; }

        /// <summary>
        /// <para>Gets the flag that indicates if this communication listener should be opened when the replica becomes an 
        /// <see cref="System.Fabric.ReplicaRole.ActiveSecondary"/>.</para>
        /// <para>When this member is false, the communication listener is opened only when the replica becomes 
        /// <see cref="System.Fabric.ReplicaRole.Primary"/>.</para>
        /// <para>The default value is <languageKeyword>false</languageKeyword>.</para>
        /// </summary>
        /// <value>
        /// <para>The flag that indicates if this communication listener should be opened when the replica is <see cref="System.Fabric.ReplicaRole.ActiveSecondary"/>.</para>
        /// </value>
        /// <remarks>
        /// <para>This flag can be set when the primary replica is too busy to serve reads and writes efficiently and the application can tolerate reading stale (but consistent) data,
        /// then data can be read from secondary replica.</para>
        /// </remarks>
        public bool ListenOnSecondary { get; private set; }

        /// <summary>
        /// <para>Gets the factory method for creating the communication listener.</para>
        /// </summary>
        /// <value>
        /// <para>The factory method for creating the communication listener.</para>
        /// </value>
        /// <remarks>
        /// <para>The factory method takes in a <see cref="System.Fabric.StatefulServiceContext"/> and returns communication listener implementing <see cref="Microsoft.ServiceFabric.Services.Communication.Runtime.ICommunicationListener"/>.</para>
        /// </remarks>
        public Func<StatefulServiceContext, ICommunicationListener> CreateCommunicationListener { get; private set; }

        /// <summary>
        /// Creates the ServiceReplicaListener
        /// </summary>
        /// <param name="createCommunicationListener">Factory method for creating the communication listener</param>
        /// <param name="name">Name of the communication listener. This parameter is optional, if the service has only one communication listener</param>
        /// <param name="listenOnSecondary">Specifies if the communication listener needs to be opened when the replica becomes Active secondary. THis parameter is optional</param>
        public ServiceReplicaListener(
            Func<StatefulServiceContext, ICommunicationListener> createCommunicationListener,
            string name = DefaultName,
            bool listenOnSecondary = false)
        {
            this.CreateCommunicationListener = createCommunicationListener;
            this.Name = name;
            this.ListenOnSecondary = listenOnSecondary;
        }
    }
}
