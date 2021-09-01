// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Remoting.V2
{
    using System;
#if DotNetCoreClr
    using System.Threading;

    /// <summary>
    /// Class ActivityIdLogicalCallContext.
    /// </summary>
    public static class ActivityIdLogicalCallContext
    {
        private static AsyncLocal<string> activityIDAsyncLocal = null;

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        public static void Initialize()
        {
            activityIDAsyncLocal = new AsyncLocal<string>();
        }

        /// <summary>
        /// Determines whether this instance is present.
        /// </summary>
        /// <returns><c>true</c> if this instance is present; otherwise, <c>false</c>.</returns>
        public static bool IsPresent()
        {
            return (activityIDAsyncLocal != null);
        }

        /// <summary>
        /// Tries the get.
        /// </summary>
        /// <param name="activityIdValue">The activity identifier value.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool TryGet(out string activityIdValue)
        {
            activityIdValue = activityIDAsyncLocal.Value;
            return (activityIdValue != null);
        }

        /// <summary>
        /// Sets the specified activity identifier value.
        /// </summary>
        /// <param name="activityIdValue">The activity identifier value.</param>
        public static void Set(string activityIdValue)
        {
            activityIDAsyncLocal.Value = activityIdValue;
        }

        /// <summary>
        /// Clears this instance.
        /// </summary>
        public static void Clear()
        {
            activityIDAsyncLocal = null;
        }
    }
#else
    using System.Runtime.Remoting.Messaging;

    /// <summary>
    /// Class ActivityIdLogicalCallContext.
    /// </summary>
    public static class ActivityIdLogicalCallContext
    {
        internal const string CallContextKey = "_ActivityIdCallContext_";

        /// <summary>
        /// Determines whether this instance is present.
        /// </summary>
        /// <returns><c>true</c> if this instance is present; otherwise, <c>false</c>.</returns>
        public static bool IsPresent()
        {
            return (CallContext.LogicalGetData(CallContextKey) != null);
        }

        /// <summary>
        /// Tries the get.
        /// </summary>
        /// <param name="callContextValue">The call context value.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool TryGet(out Guid callContextValue)
        {
            callContextValue = (Guid)CallContext.LogicalGetData(CallContextKey);
            return (callContextValue != null);
        }

        /// <summary>
        /// Sets the specified call context value.
        /// </summary>
        /// <param name="callContextValue">The call context value.</param>
        public static void Set(Guid callContextValue)
        {
            CallContext.LogicalSetData(CallContextKey, callContextValue);
        }

        /// <summary>
        /// Clears this instance.
        /// </summary>
        public static void Clear()
        {
            CallContext.FreeNamedDataSlot(CallContextKey);
        }
    }
#endif
}
