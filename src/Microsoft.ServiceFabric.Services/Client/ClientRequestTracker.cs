// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Client
{
 #if DotNetCoreClr
    using System.Threading;

    internal static class ClientRequestTracker
    {
        private static AsyncLocal<string> requestTracker = new AsyncLocal<string>();

        public static bool IsPresent()
        {
            return (requestTracker.Value != null);
        }

        public static bool TryGet(out string callContextValue)
        {
            callContextValue = requestTracker.Value;
            return (callContextValue != null);
        }

        public static void Set(string callContextValue)
        {
            requestTracker.Value = callContextValue;
        }
    }
#else
    using System.Runtime.Remoting.Messaging;

    internal static class ClientRequestTracker
    {
        internal const string CallContextKey = "_ServicePartitionClientRequestId_";

        public static bool IsPresent()
        {
            return (CallContext.LogicalGetData(CallContextKey) != null);
        }

        public static bool TryGet(out string callContextValue)
        {
            callContextValue = (string)CallContext.LogicalGetData(CallContextKey);
            return (callContextValue != null);
        }

        public static void Set(string callContextValue)
        {
            CallContext.LogicalSetData(CallContextKey, callContextValue);
        }
    }
#endif
}
