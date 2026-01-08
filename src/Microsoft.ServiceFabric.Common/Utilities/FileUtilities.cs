// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common.Utilities
{
    using System;
    using System.IO;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Utility and extension methods for file operations.
    /// </summary>
    internal static class FileUtilities
    {
        /// <summary>
        /// Gets absolute path.
        /// </summary>
        /// <param name="path">Path to get absolute path for.</param>
        /// <returns>Absolute path for <paramref name="path"/>.</returns>
        internal static string GetAbsolutePath(string path)
        {
            if (!Path.IsPathRooted(path))
            {
                path = Path.Combine(Environment.CurrentDirectory, path);
            }

            return path;
        }

        /// <summary>
        /// Handles long paths for Windows OS.
        /// </summary>
        /// <param name="absolutePath">Absolute Path.</param>
        /// <returns>Path with <paramref name="absolutePath"/> appended to "\\?\" for Windows OS. </returns>
        internal static string HandleLongPath(string absolutePath)
        {
            // TODO: It must be handled by default in .net4.6.2 and .net core 2.0
            // handle long paths for windows
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) && !absolutePath.StartsWith(@"\\?\"))
            {
                absolutePath = @"\\?\" + absolutePath;
            }

            return absolutePath;
        }
    }
}
