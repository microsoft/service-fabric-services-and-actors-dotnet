// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Diagnostics.Tracing.Util
{
    using System;
    using System.Runtime.InteropServices;

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

    /// <summary>
    /// Default implementation of IPlatformInformation that uses System.Runtime.InteropServices.RuntimeInformation.
    /// </summary>
    public class PlatformInformation : IPlatformInformation
    {
        private static readonly Lazy<PlatformInformation> LazyInstance = 
            new Lazy<PlatformInformation>(() => new PlatformInformation());
        
        /// <summary>
        /// Gets the singleton instance of PlatformInformation.
        /// </summary>
        public static PlatformInformation Instance => LazyInstance.Value;
        
        /// <summary>
        /// Prevents external instantiation. Use Instance property to get the singleton.
        /// </summary>
        protected PlatformInformation()
        {
        }

        /// <summary>
        /// Returns a value that indicates whether the current operating system is Linux
        /// by delegating to RuntimeInformation.IsOSPlatform.
        /// </summary>
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
