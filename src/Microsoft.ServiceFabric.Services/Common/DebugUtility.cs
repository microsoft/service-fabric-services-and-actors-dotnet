// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Common
{
    using System;
    using System.Diagnostics;
    using System.Threading;

    internal static class DebugUtility
    {
        static DebugUtility()
        {
            DefaultDebugAttachWaitDuration = TimeSpan.FromSeconds(180);
            DefaultDebugAttachCheckDuration = TimeSpan.FromSeconds(1);
        }

        public static TimeSpan DefaultDebugAttachWaitDuration { get; set; }

        public static TimeSpan DefaultDebugAttachCheckDuration { get; set; }

        public static void WaitForDebuggerAttach()
        {
            WaitForDebuggerAttach(DefaultDebugAttachWaitDuration, DefaultDebugAttachCheckDuration);
        }

        public static void WaitForDebuggerAttach(TimeSpan waitDuration)
        {
            WaitForDebuggerAttach(waitDuration, DefaultDebugAttachCheckDuration);
        }

        public static void WaitForDebuggerAttach(TimeSpan waitDuration, TimeSpan checkDelay)
        {
            var start = DateTime.UtcNow;
            while (!Debugger.IsAttached)
            {
                Thread.Sleep(checkDelay);

                if ((DateTime.UtcNow - start) > waitDuration)
                {
                    break;
                }
            }
        }
    }
}
