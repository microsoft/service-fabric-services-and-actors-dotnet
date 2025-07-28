// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using Microsoft.ServiceFabric.Services.Remoting.V2.Diagnostic;
using Xunit;

namespace Microsoft.ServiceFabric.Services.Remoting.Tests.V2.Diagnostic
{
    public abstract class IDiagnosticEventsTest
    {
        public class Signature : IDiagnosticEventsTest
        {
            [Fact]
            public void HasRequiredMethods()
            {
                var interfaceType = typeof(IDiagnosticEvents);

                var onRequestResponseBeginMethod = interfaceType.GetMethod(nameof(IDiagnosticEvents.OnRequestResponseBegin), new Type[0]);
                Assert.NotNull(onRequestResponseBeginMethod);
                Assert.Equal(typeof(void), onRequestResponseBeginMethod.ReturnType);

                var onRequestResponseEndMethod = interfaceType.GetMethod(nameof(IDiagnosticEvents.OnRequestResponseEnd), new[] { typeof(DateTime) });
                Assert.NotNull(onRequestResponseEndMethod);
                Assert.Equal(typeof(void), onRequestResponseEndMethod.ReturnType);

                var onCreateTransportMessageBeginMethod = interfaceType.GetMethod(nameof(IDiagnosticEvents.OnCreateTransportMessageBegin), new Type[0]);
                Assert.NotNull(onCreateTransportMessageBeginMethod);
                Assert.Equal(typeof(void), onCreateTransportMessageBeginMethod.ReturnType);

                var onCreateTransportMessageEndMethod = interfaceType.GetMethod(nameof(IDiagnosticEvents.OnCreateTransportMessageEnd), new[] { typeof(DateTime) });
                Assert.NotNull(onCreateTransportMessageEndMethod);
                Assert.Equal(typeof(void), onCreateTransportMessageEndMethod.ReturnType);

                var onCreateRemotingMessageBeginMethod = interfaceType.GetMethod(nameof(IDiagnosticEvents.OnRemotingRequestBegin), new Type[0]);
                Assert.NotNull(onCreateRemotingMessageBeginMethod);
                Assert.Equal(typeof(void), onCreateRemotingMessageBeginMethod.ReturnType);

                var onCreateRemotingMessageEndMethod = interfaceType.GetMethod(nameof(IDiagnosticEvents.OnRemotingRequestEnd), new[] { typeof(DateTime) });
                Assert.NotNull(onCreateRemotingMessageEndMethod);
                Assert.Equal(typeof(void), onCreateRemotingMessageEndMethod.ReturnType);
            }
        }
    }
}
