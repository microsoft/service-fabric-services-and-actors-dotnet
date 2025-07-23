// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services
{
    using System;
#if NET
    using System.Threading;

    internal class LogContext
    {
        private static AsyncLocal<LogContext> asyncLocalLogContext = new AsyncLocal<LogContext>();

        public Guid RequestId { get; set; }

        public static bool IsPresent()
        {
            return (asyncLocalLogContext.Value != null);
        }

        public static bool TryGet(out LogContext logContext)
        {
            logContext = asyncLocalLogContext.Value;
            return (logContext != null);
        }

        public static void Set(LogContext logContext)
        {
            asyncLocalLogContext.Value = logContext;
        }

        public static void Clear()
        {
            asyncLocalLogContext.Value = null;
        }

        public static Guid GetRequestIdOrDefault()
        {
            if (LogContext.TryGet(out var logContext))
            {
                return logContext.RequestId;
            }
            else
            {
                return default(Guid);
            }
        }
    }
#else
    using System.Runtime.Remoting.Messaging;

    internal class LogContext
    {
        internal const string LogContextKey = "_RemotingLogContext_";

        public Guid RequestId { get; set; } = default(Guid);

        public static bool IsPresent()
        {
            return (CallContext.LogicalGetData(LogContextKey) != null);
        }

        public static bool TryGet(out LogContext logContext)
        {
            logContext = (LogContext)CallContext.LogicalGetData(LogContextKey);
            return (logContext != null);
        }

        public static void Set(LogContext logContext)
        {
            CallContext.LogicalSetData(LogContextKey, logContext);
        }

        public static void Clear()
        {
            CallContext.FreeNamedDataSlot(LogContextKey);
        }

        public static Guid GetRequestIdOrDefault()
        {
            if (LogContext.TryGet(out var logContext))
            {
                return logContext.RequestId;
            }
            else
            {
                return default(Guid);
            }
        }
    }
#endif
}
