// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Remoting
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    internal class Helper
    {
        public static bool IsEitherRemotingV2(RemotingClientVersion remotingClient)
        {
            return IsRemotingV2(remotingClient) || IsRemotingV2_1(remotingClient);
        }

        public static bool IsEitherRemotingV2(RemotingListenerVersion remotingListener)
        {
            return IsRemotingV2(remotingListener) || IsRemotingV2_1(remotingListener);
        }

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

#if !DotNetCoreClr
        [Obsolete(DeprecationMessage.RemotingV1)]
        public static bool IsRemotingV1(RemotingListenerVersion remotingListener)
        {
            return remotingListener.HasFlag(RemotingListenerVersion.V1);
        }

        [Obsolete(DeprecationMessage.RemotingV1)]
        public static bool IsRemotingV1(RemotingClientVersion remotingListener)
        {
            return remotingListener.HasFlag(RemotingClientVersion.V1);
        }
#endif

    }
}
