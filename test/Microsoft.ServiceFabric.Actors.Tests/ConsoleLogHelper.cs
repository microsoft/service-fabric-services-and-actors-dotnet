// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Tests
{
    using System;

    /// <summary>
    /// Helper Class for logging to console.
    /// </summary>
    internal class ConsoleLogHelper
    {
        private static readonly object ConsoleLock = new object();

        /// <summary>
        /// Logs information messages.
        /// </summary>
        /// <param name="format">string format of message to log.</param>
        /// <param name="args">Arguments for the format.</param>
        internal static void LogInfo(string format, params object[] args)
        {
            Log(string.Format(format, args));
        }

        /// <summary>
        /// Logs error messages.
        /// </summary>
        /// <param name="format">string format of message to log.</param>
        /// <param name="args">Arguments for the format.</param>
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
