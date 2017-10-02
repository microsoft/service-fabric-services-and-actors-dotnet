// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------
namespace Microsoft.ServiceFabric.Actors.Remoting
{

#if DotNetCoreClr
    using System.Threading;

    internal static class ActorLogicalCallContext
    {
        internal static AsyncLocal<string> fabActAsyncLocal = new AsyncLocal<string>();

        public static bool IsPresent()
        {
            return (fabActAsyncLocal.Value != null);
        }

        public static bool TryGet(out string callContextValue)
        {
            callContextValue = fabActAsyncLocal.Value;
            return (callContextValue != null);
        }

        public static void Set(string callContextValue)
        {
            fabActAsyncLocal.Value = callContextValue;
        }

        public static void Clear()
        {
            fabActAsyncLocal.Value = null;
        }
    }
#else
    using System.Runtime.Remoting.Messaging;

    internal static class ActorLogicalCallContext
    {

        internal const string CallContextKey = "_FabActCallContext_";

        public static bool IsPresent()
        {
            return (CallContext.LogicalGetData(CallContextKey) != null);
        }

        public static bool TryGet(out string callContextValue)
        {
            callContextValue = (string) CallContext.LogicalGetData(CallContextKey);
            return (callContextValue != null);
        }

        public static void Set(string callContextValue)
        {
            CallContext.LogicalSetData(CallContextKey, callContextValue);
        }

        public static void Clear()
        {
            CallContext.FreeNamedDataSlot(CallContextKey);
        }
    }
#endif
}
