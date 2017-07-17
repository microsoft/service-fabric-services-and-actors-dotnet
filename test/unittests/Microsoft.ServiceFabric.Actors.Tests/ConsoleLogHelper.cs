// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Tests
{
    using System;
    using System.Fabric;
    using System.Numerics;
    using Microsoft.ServiceFabric.Actors.Runtime;

    internal class ConsoleLogHelper
    {
        private static readonly object ConsoleLock = new object();

        internal static void LogInfo(string format, params object[] args)
        {
            Log(string.Format(format, args));
        }

        internal static void LogError(string format, params object[] args)
        {
            Log("ERROR -- " + string.Format(format, args));
        }

        private static void Log(string message)
        {
            lock (ConsoleLock)
            {
                Console.WriteLine(message);
            }
        }
    }
}