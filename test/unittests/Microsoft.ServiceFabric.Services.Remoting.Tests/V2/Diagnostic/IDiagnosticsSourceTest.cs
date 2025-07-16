// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using Microsoft.ServiceFabric.Services.Remoting.V2.Diagnostic;
using Xunit;

namespace Microsoft.ServiceFabric.Services.Remoting.Tests.V2.Diagnostic
{
    public abstract class IDiagnosticsSourceTest
    {
        public class Signiture : IDiagnosticsSourceTest
        {
            [Fact]
            public void IDiagnosticsSource_ShouldHave_RequiredMethods()
            {
                var interfaceType = typeof(IDiagnosticsSource);

                var onRequestResponseBeginMethod = interfaceType.GetMethod("OnRequestResponseBegin", new Type[0]);
                Assert.NotNull(onRequestResponseBeginMethod);
                Assert.Equal(typeof(DateTime), onRequestResponseBeginMethod.ReturnType);

                var onRequestResponseEndMethod = interfaceType.GetMethod("OnRequestResponseEnd", new[] { typeof(DateTime) });
                Assert.NotNull(onRequestResponseEndMethod);
                Assert.Equal(typeof(void), onRequestResponseEndMethod.ReturnType);

                var onCreateTransportMessageBeginMethod = interfaceType.GetMethod("OnCreateTransportMessageSerializationBegin", new Type[0]);
                Assert.NotNull(onCreateTransportMessageBeginMethod);
                Assert.Equal(typeof(DateTime), onCreateTransportMessageBeginMethod.ReturnType);

                var onCreateTransportMessageEndMethod = interfaceType.GetMethod("OnCreateTransportMessageSerializationEnd", new[] { typeof(DateTime) });
                Assert.NotNull(onCreateTransportMessageEndMethod);
                Assert.Equal(typeof(void), onCreateTransportMessageEndMethod.ReturnType);

                var onCreateRemotingMessageBeginMethod = interfaceType.GetMethod("OnRemotingRequestDeserializationBegin", new Type[0]);
                Assert.NotNull(onCreateRemotingMessageBeginMethod);
                Assert.Equal(typeof(DateTime), onCreateRemotingMessageBeginMethod.ReturnType);

                var onCreateRemotingMessageEndMethod = interfaceType.GetMethod("OnRemotingRequestDeserializationEnd", new[] { typeof(DateTime) });
                Assert.NotNull(onCreateRemotingMessageEndMethod);
                Assert.Equal(typeof(void), onCreateRemotingMessageEndMethod.ReturnType);
            }
        }
    }
}
