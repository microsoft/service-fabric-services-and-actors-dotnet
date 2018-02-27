// ------------------------------------------------------------
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT License (MIT).See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Runtime
{
    internal class SerializedStateChange
    {
        private readonly StateChangeKind changeKind;
        private readonly string key;
        private readonly byte[] serializedState;

        public SerializedStateChange(StateChangeKind changeKind, string key, byte[] serializedState)
        {
            this.changeKind = changeKind;
            this.key = key;
            this.serializedState = serializedState;
        }

        public StateChangeKind ChangeKind
        {
            get { return this.changeKind; }
        }

        public string Key
        {
            get { return this.key; }
        }

        public byte[] SerializedState
        {
            get { return this.serializedState; }
        }
    }
}
