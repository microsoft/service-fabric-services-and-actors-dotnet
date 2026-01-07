// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;

namespace Microsoft.ServiceFabric.Services.Communication.Runtime
{
    /// <summary>
    /// Represents the communication listener and its name.
    /// </summary>
    sealed class CommunicationListenerInfo : IEquatable<CommunicationListenerInfo>
    {
        readonly string traceString;

        internal CommunicationListenerInfo(string name, ICommunicationListener listener)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Listener = listener ?? throw new ArgumentNullException(nameof(listener));
            traceString = $"{Listener.GetType().Name} '{Name}' (#{Listener.GetHashCode()})";
        }

        internal string Name { get; }

        internal ICommunicationListener Listener { get; }

        public override string ToString() => traceString;

        bool IEquatable<CommunicationListenerInfo>.Equals(CommunicationListenerInfo info) =>
            info != null && Equals(Name, info.Name) && ReferenceEquals(Listener, info.Listener);
    }
}
