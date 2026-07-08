// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.

using System;
using System.ServiceModel;
using System.ServiceModel.Security;
using Fuzzy;
using Microsoft.ServiceFabric.Services.Communication.Client;
using Microsoft.ServiceFabric.Services.Communication.Wcf;
using Xunit;

namespace Microsoft.ServiceFabric.Services.Communication.Wcf.Client;

public abstract class WcfExceptionHandlerTest
{
    readonly IExceptionHandler sut = new WcfExceptionHandler();

    static readonly IFuzz fuzzy = new RandomFuzz(Environment.TickCount);

    public sealed class TryHandleException : WcfExceptionHandlerTest
    {
        readonly OperationRetrySettings retrySettings;
        ExceptionHandlingResult result;

        public TryHandleException() =>
            retrySettings = new OperationRetrySettings(fuzzy.TimeSpan(), fuzzy.TimeSpan(), fuzzy.Int32(), fuzzy.Int32());

        [Theory, MemberData(nameof(FailoverExceptions))]
        public void ReturnsNonTransientRetryResultWhenExceptionIndicatesFailover(ExceptionInformation exceptionInformation)
        {
            bool handled = sut.TryHandleException(exceptionInformation, retrySettings, out result);

            Assert.True(handled);
            var retry = (ExceptionHandlingRetryResult)result;
            Assert.False(retry.IsTransient);
            Assert.Equal(exceptionInformation.Exception.GetType().FullName, retry.ExceptionId);
            Assert.Equal(retrySettings.DefaultMaxRetryCountForNonTransientErrors, retry.MaxRetryCount);
        }

        [Theory, MemberData(nameof(TransientExceptions))]
        public void ReturnsTransientRetryResultWhenExceptionIsTransient(ExceptionInformation exceptionInformation)
        {
            bool handled = sut.TryHandleException(exceptionInformation, retrySettings, out result);

            Assert.True(handled);
            var retry = (ExceptionHandlingRetryResult)result;
            Assert.True(retry.IsTransient);
            Assert.Equal(exceptionInformation.Exception.GetType().FullName, retry.ExceptionId);
            Assert.Equal(int.MaxValue, retry.MaxRetryCount);
        }

        [Theory, MemberData(nameof(NonRetryableExceptions))]
        public void ReturnsThrowResultWhenExceptionIsNotRetryable(ExceptionInformation exceptionInformation)
        {
            bool handled = sut.TryHandleException(exceptionInformation, retrySettings, out result);

            Assert.True(handled);
            var thrown = (ExceptionHandlingThrowResult)result;
            Assert.Same(exceptionInformation.Exception, thrown.ExceptionToThrow);
        }

        [Fact]
        public void ReturnsNonTransientRetryResultWhenFaultIsRetryable()
        {
            string reason = fuzzy.String();
            FaultCode code = new(WcfRemoteExceptionInformation.FaultCodeName, new FaultCode(WcfRemoteExceptionInformation.FaultSubCodeRetryName));
            ExceptionInformation exceptionInformation = new(new FaultException(new FaultReason(reason), code));

            bool handled = sut.TryHandleException(exceptionInformation, retrySettings, out result);

            Assert.True(handled);
            var retry = (ExceptionHandlingRetryResult)result;
            Assert.False(retry.IsTransient);
            Assert.Equal(reason, retry.ExceptionId);
            Assert.Equal(retrySettings.DefaultMaxRetryCountForNonTransientErrors, retry.MaxRetryCount);
        }

        [Fact]
        public void ReturnsFalseWhenFaultCodeNameDoesNotMatch()
        {
            string faultCodeName = WcfRemoteExceptionInformation.FaultCodeName + fuzzy.String();
            FaultException exception = new(new FaultReason(fuzzy.String()), new FaultCode(faultCodeName));
            ExceptionInformation exceptionInformation = new(exception);

            bool handled = sut.TryHandleException(exceptionInformation, retrySettings, out result);

            Assert.False(handled);
            Assert.Null(result);
        }

        [Fact]
        public void ReturnsFalseWhenFaultSubCodeNameDoesNotMatch()
        {
            string subCodeName = WcfRemoteExceptionInformation.FaultSubCodeRetryName + fuzzy.String();
            FaultCode code = new(WcfRemoteExceptionInformation.FaultCodeName, new FaultCode(subCodeName));
            ExceptionInformation exceptionInformation = new(new FaultException(new FaultReason(fuzzy.String()), code));

            bool handled = sut.TryHandleException(exceptionInformation, retrySettings, out result);

            Assert.False(handled);
            Assert.Null(result);
        }

        [Fact]
        public void ReturnsFalseWhenFaultSubCodeIsMissing()
        {
            FaultCode code = new(WcfRemoteExceptionInformation.FaultCodeName);
            ExceptionInformation exceptionInformation = new(new FaultException(new FaultReason(fuzzy.String()), code));

            bool handled = sut.TryHandleException(exceptionInformation, retrySettings, out result);

            Assert.False(handled);
            Assert.Null(result);
        }

        [Fact]
        public void ReturnsNonTransientRetryResultWhenExceptionIsCommunicationException()
        {
            ExceptionInformation exceptionInformation = new(new CommunicationException(fuzzy.String()));

            bool handled = sut.TryHandleException(exceptionInformation, retrySettings, out result);

            Assert.True(handled);
            var retry = (ExceptionHandlingRetryResult)result;
            Assert.False(retry.IsTransient);
            Assert.Equal(exceptionInformation.Exception.GetType().FullName, retry.ExceptionId);
            Assert.Equal(retrySettings.DefaultMaxRetryCountForNonTransientErrors, retry.MaxRetryCount);
        }

        [Fact]
        public void ReturnsFalseWhenExceptionIsNotRecognized()
        {
            bool handled = sut.TryHandleException(new ExceptionInformation(new TestException()), retrySettings, out result);

            Assert.False(handled);
            Assert.Null(result);
        }

        [Fact(Explicit = true)] // TODO: SUT bug. Null exceptionInformation is dereferenced.
        public void ThrowsArgumentNullExceptionWhenExceptionInformationIsNull()
        {
            var actual = Assert.Throws<ArgumentNullException>(() => sut.TryHandleException(null, retrySettings, out result));
            Assert.Equal("exceptionInformation", actual.ParamName);
        }

        [Fact(Explicit = true)] // TODO: SUT bug. Null retrySettings is dereferenced for retriable exceptions.
        public void ThrowsArgumentNullExceptionWhenRetrySettingsIsNull()
        {
            ExceptionInformation exceptionInformation = new(new EndpointNotFoundException());
            var actual = Assert.Throws<ArgumentNullException>(() => sut.TryHandleException(exceptionInformation, null, out result));
            Assert.Equal(nameof(retrySettings), actual.ParamName);
        }

        public static TheoryData<ExceptionInformation> FailoverExceptions =>
        [
            new ExceptionInformation(new EndpointNotFoundException()),
            new ExceptionInformation(new CommunicationObjectAbortedException()),
            new ExceptionInformation(new CommunicationObjectFaultedException()),
            new ExceptionInformation(new ObjectDisposedException(fuzzy.String())),
            new ExceptionInformation(new ChannelTerminatedException()),
        ];

        public static TheoryData<ExceptionInformation> TransientExceptions =>
        [
            new ExceptionInformation(new TimeoutException()),
            new ExceptionInformation(new ServerTooBusyException(fuzzy.String())),
        ];

        public static TheoryData<ExceptionInformation> NonRetryableExceptions =>
        [
            new ExceptionInformation(new ActionNotSupportedException(fuzzy.String())),
            new ExceptionInformation(new AddressAccessDeniedException(fuzzy.String())),
            new ExceptionInformation(new SecurityAccessDeniedException(fuzzy.String())),
        ];

        sealed class TestException : Exception { }
    }
}
