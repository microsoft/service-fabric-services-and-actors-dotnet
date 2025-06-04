// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Runtime.InteropServices;

namespace Microsoft.ServiceFabric.Diagnostics.Tracing.Util
{
    /// <summary>
    /// Interface for platform-specific information operations.
    /// Allows for dependency injection and mocking in tests.
    /// </summary>
    public interface IPlatformInformation
    {
        /// <summary>
        /// Returns a value that indicates whether the current operating system is Linux
        /// </summary>
        /// <returns>true if the current operating system is Linux; otherwise, false.</returns>
        bool IsLinuxPlatform();
    }

    internal class PlatformInformation : IPlatformInformation
    {
        private static readonly Lazy<PlatformInformation> LazyInstance = 
            new Lazy<PlatformInformation>(() => new PlatformInformation());
        
        public static PlatformInformation Instance => LazyInstance.Value;
        
        protected PlatformInformation()
        {
        }

        public bool IsLinuxPlatform()
        {
#if DotNetCoreClr
            return RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
#else
            return false;
#endif
        }
    }
}
