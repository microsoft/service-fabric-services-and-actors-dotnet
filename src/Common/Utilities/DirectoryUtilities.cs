// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common.Utilities
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;

    /// <summary>
    /// Utility and extension method for Directory Operations.
    /// </summary>
    internal static class DirectoryUtilities
    {
        /// <summary>
        /// Recursively copies directory.
        /// </summary>
        /// <param name="sourceDir">Absolute path to source directory.</param>
        /// <param name="destDir">Absolute path to destination directory.</param>
        internal static void CopyDirectory(string sourceDir, string destDir)
        {
            // Get the subdirectories for the specified directory.
            var directoryInfo = new DirectoryInfo(sourceDir);

            if (!directoryInfo.Exists)
            {
                throw new DirectoryNotFoundException($"Source directory does not exist: {sourceDir}");
            }
            
            if (Directory.Exists(destDir))
            {
                Directory.Delete(destDir, true);
            }

            Directory.CreateDirectory(destDir);

            // Copy files in Dir.
            var files = directoryInfo.GetFiles();
            foreach (var file in files)
            {
                var temppath = Path.Combine(destDir, file.Name);
                file.CopyTo(temppath, false);
            }

            var subdirs = directoryInfo.GetDirectories();
            foreach (var subdir in subdirs)
            {
                var temppath = Path.Combine(destDir, subdir.Name);
                CopyDirectory(subdir.FullName, temppath);
            }
        }
    }
}
