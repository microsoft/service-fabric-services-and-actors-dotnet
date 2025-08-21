// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Remoting
{
    internal class Helper
    {
        public static bool IsRemotingV2(RemotingClientVersion remotingClient)
        {
            return remotingClient.HasFlag(RemotingClientVersion.V2);
        }

        public static bool IsRemotingV2(RemotingListenerVersion remotingListener)
        {
            return remotingListener.HasFlag(RemotingListenerVersion.V2);
        }

        public static bool IsRemotingV2_1(RemotingListenerVersion remotingListener)
        {
            return remotingListener.HasFlag(RemotingListenerVersion.V2_1);
        }

        public static bool IsRemotingV2_1(RemotingClientVersion remotingClient)
        {
            return remotingClient.HasFlag(RemotingClientVersion.V2_1);
        }
    }
}
