// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.

using System;
using Xunit;

namespace Microsoft.ServiceFabric.FabricTransport;

public abstract class NativeFabricTransportTest
{
    public sealed class CreateFabricTransportClient : NativeFabricTransportTest
    {
        [Fact(Explicit = true)] // TODO: SUT testability limitation. Static FabricTransport P/Invoke has no test seam.
        public void ReturnsClientCreatedByNativeRuntime() =>
            // CreateFabricTransportClient forwards every argument to PInvoke.CreateFabricTransportClient
            // and returns the out IFabricTransportClient2 after Marshal.ThrowExceptionForHR. The success
            // path can only be observed when the native FabricTransport DLL and the Service Fabric runtime
            // are installed; the static P/Invoke exposes no seam for substitution in unit tests.
            throw new NotImplementedException();

        [Fact(Explicit = true)] // TODO: SUT testability limitation. Static FabricTransport P/Invoke has no test seam.
        public void ThrowsExceptionForFailureHResultReturnedByPInvoke() =>
            // The wrapper routes the HRESULT returned by PInvoke.CreateFabricTransportClient through
            // Marshal.ThrowExceptionForHR. The wrapper exposes no seam to substitute the P/Invoke, and no
            // combination of public inputs reliably produces a non-success HRESULT without risking
            // AccessViolationException inside the native runtime.
            throw new NotImplementedException();
    }

    public sealed class CreateFabricTransportListener : NativeFabricTransportTest
    {
        [Fact(Explicit = true)] // TODO: SUT testability limitation. Static FabricTransport P/Invoke has no test seam.
        public void ReturnsListenerCreatedByNativeRuntime() =>
            // CreateFabricTransportListener forwards every argument to PInvoke.CreateFabricTransportListener
            // and returns the out IFabricTransportListener after Marshal.ThrowExceptionForHR. The success
            // path can only be observed when the native FabricTransport DLL and the Service Fabric runtime
            // are installed; the static P/Invoke exposes no seam for substitution in unit tests.
            throw new NotImplementedException();

        [Fact(Explicit = true)] // TODO: SUT testability limitation. Static FabricTransport P/Invoke has no test seam.
        public void ThrowsExceptionForFailureHResultReturnedByPInvoke() =>
            // The wrapper routes the HRESULT returned by PInvoke.CreateFabricTransportListener through
            // Marshal.ThrowExceptionForHR. The wrapper exposes no seam to substitute the P/Invoke, and no
            // combination of public inputs reliably produces a non-success HRESULT without risking
            // AccessViolationException inside the native runtime.
            throw new NotImplementedException();
    }
}
