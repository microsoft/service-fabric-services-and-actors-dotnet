// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.

using System;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using System.Xml;
using Fuzzy;
using Microsoft.ServiceFabric.Services.Communication;
using Microsoft.ServiceFabric.Services.Communication.Wcf;
using Moq;
using Xunit;

namespace Microsoft.ServiceFabric.Services.Communication.Wcf.Runtime;

public abstract class WcfGlobalErrorHandlerTest
{
    readonly IErrorHandler sut;

    // Constructor parameters
    readonly ChannelDispatcher dispatcher;

    readonly Mock<IChannelListener> listener = new();

    static readonly IFuzz fuzzy = new RandomFuzz(Environment.TickCount);

    WcfGlobalErrorHandlerTest()
    {
        dispatcher = new ChannelDispatcher(listener.Object);
        sut = new WcfGlobalErrorHandler(dispatcher);
    }

    public sealed class Constructor : WcfGlobalErrorHandlerTest
    {
        [Fact]
        public void ThrowsArgumentNullExceptionWhenDispatcherIsNull()
        {
            var actual = Assert.Throws<ArgumentNullException>(() => new WcfGlobalErrorHandler(null));
            Assert.Equal(nameof(dispatcher), actual.ParamName);
        }
    }

    public sealed class HandleError : WcfGlobalErrorHandlerTest
    {
        readonly Exception error = new InvalidOperationException(fuzzy.String());

        [Fact]
        public void ReturnsTrueWhenErrorIsNotFaultException() =>
            Assert.True(sut.HandleError(error));

        [Fact]
        public void ReturnsFalseWhenErrorIsFaultException() =>
            Assert.False(sut.HandleError(new FaultException<string>(fuzzy.String())));
    }

    public sealed class ProvideFault : WcfGlobalErrorHandlerTest
    {
        // Method parameters
        readonly Exception error = new InvalidOperationException(fuzzy.String());
        readonly MessageVersion version = MessageVersion.Soap12; // non-default proves forwarding
        Message fault;

        readonly Message expected = Message.CreateMessage(MessageVersion.None, fuzzy.String());

        public ProvideFault() => fault = expected;

        [Theory]
        [InlineData(CommunicationState.Created)]
        [InlineData(CommunicationState.Opening)]
        [InlineData(CommunicationState.Closing)]
        [InlineData(CommunicationState.Closed)]
        [InlineData(CommunicationState.Faulted)]
        public void ProvidesRetryFaultWhenListenerIsNotOpened(CommunicationState state)
        {
            _ = listener.SetupGet(_ => _.State).Returns(state);

            sut.ProvideFault(error, version, ref fault);

            Assert.Equal(version, fault.Version);
            var messageFault = MessageFault.CreateFault(fault, int.MaxValue);
            Assert.Equal(WcfRemoteExceptionInformation.FaultCodeName, messageFault.Code.Name);
            Assert.Equal(WcfRemoteExceptionInformation.FaultSubCodeRetryName, messageFault.Code.SubCode.Name);
        }

        [Fact]
        public void ProvidesFaultReasonDescribingExceptionWhenListenerIsNotOpened()
        {
            _ = listener.SetupGet(_ => _.State).Returns(CommunicationState.Faulted);

            sut.ProvideFault(error, version, ref fault);

            var messageFault = MessageFault.CreateFault(fault, int.MaxValue);
            string reason = messageFault.Reason.GetMatchingTranslation(CultureInfo.CurrentCulture).Text;
            DataContractSerializer serializer = new(typeof(ServiceExceptionData));
            using var reader = XmlReader.Create(new StringReader(reason));
            var data = (ServiceExceptionData)serializer.ReadObject(reader);
            Assert.Equal(error.GetType().FullName, data.Type);
            Assert.Contains(error.ToString(), data.Message);
        }

        [Fact]
        public void DoesNotProvideFaultWhenErrorIsFaultException()
        {
            sut.ProvideFault(new FaultException<string>(fuzzy.String()), version, ref fault);
            Assert.Same(expected, fault);
        }

        [Fact]
        public void DoesNotProvideFaultWhenListenerIsOpened()
        {
            _ = listener.SetupGet(_ => _.State).Returns(CommunicationState.Opened);
            sut.ProvideFault(error, version, ref fault);
            Assert.Same(expected, fault);
        }

        [Fact]
        public void ThrowsArgumentNullExceptionWhenErrorIsNull()
        {
            var actual = Assert.Throws<ArgumentNullException>(() => sut.ProvideFault(null, version, ref fault));
            Assert.Equal(nameof(error), actual.ParamName);
        }
    }
}
