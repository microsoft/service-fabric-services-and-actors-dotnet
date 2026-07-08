// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.

using System;
using System.ServiceModel;
using System.ServiceModel.Security;
using Fuzzy;
using Microsoft.ServiceFabric.Services.Communication.Client;
using Microsoft.ServiceFabric.Services.Communication.Wcf;
using Microsoft.ServiceFabric.Services.Communication.Wcf.Client;
using Xunit;

namespace Microsoft.ServiceFabric.Services.Wcf;

public abstract class WcfExceptionHandlerTest
{
    readonly IExceptionHandler sut = new WcfExceptionHandler();

    static readonly IFuzz fuzzy = new RandomFuzz(Environment.TickCount);

    public sealed class TryHandleException : WcfExceptionHandlerTest
    {
        // Method parameters
        readonly OperationRetrySettings retrySettings;
        ExceptionHandlingResult result;

        readonly int nonTransientRetryCount = fuzzy.Int32();

        public TryHandleException() =>
            retrySettings = new OperationRetrySettings(
                fuzzy.TimeSpan(), fuzzy.TimeSpan(), fuzzy.Int32(), nonTransientRetryCount);

        [Theory, MemberData(nameof(FailoverExceptions))]
        public void ReturnsNonTransientRetryResultWhenExceptionIndicatesFailover(Exception exception)
        {
            Assert.True(Handle(exception));

            var retry = Assert.IsType<ExceptionHandlingRetryResult>(result);
            Assert.False(retry.IsTransient);
            Assert.Equal(exception.GetType().FullName, retry.ExceptionId);
            Assert.Equal(nonTransientRetryCount, retry.MaxRetryCount);
        }

        [Theory, MemberData(nameof(TransientExceptions))]
        public void ReturnsTransientRetryResultWhenExceptionIsTransient(Exception exception)
        {
            Assert.True(Handle(exception));

            var retry = Assert.IsType<ExceptionHandlingRetryResult>(result);
            Assert.True(retry.IsTransient);
            Assert.Equal(exception.GetType().FullName, retry.ExceptionId);
            Assert.Equal(int.MaxValue, retry.MaxRetryCount);
        }

        [Theory, MemberData(nameof(NonRetriableExceptions))]
        public void ReturnsThrowResultWhenExceptionIsNotRetriable(Exception exception)
        {
            Assert.True(Handle(exception));

            var thrown = Assert.IsType<ExceptionHandlingThrowResult>(result);
            Assert.Same(exception, thrown.ExceptionToThrow);
        }

        [Fact]
        public void ReturnsNonTransientRetryResultWhenFaultIsRetriable()
        {
            string reason = fuzzy.String();
            var code = new FaultCode(
                WcfRemoteExceptionInformation.FaultCodeName,
                new FaultCode(WcfRemoteExceptionInformation.FaultSubCodeRetryName));

            Assert.True(Handle(new FaultException(new FaultReason(reason), code)));

            var retry = Assert.IsType<ExceptionHandlingRetryResult>(result);
            Assert.False(retry.IsTransient);
            Assert.Equal(reason, retry.ExceptionId);
            Assert.Equal(nonTransientRetryCount, retry.MaxRetryCount);
        }

        [Fact]
        public void ReturnsFalseWhenFaultCodeNameDoesNotMatch()
        {
            Assert.False(Handle(new FaultException(fuzzy.String())));
            Assert.Null(result);
        }

        [Fact]
        public void ReturnsFalseWhenFaultSubCodeNameDoesNotMatch()
        {
            var code = new FaultCode(WcfRemoteExceptionInformation.FaultCodeName, new FaultCode(fuzzy.String()));
            Assert.False(Handle(new FaultException(new FaultReason(fuzzy.String()), code)));
            Assert.Null(result);
        }

        [Fact]
        public void ReturnsNonTransientRetryResultWhenExceptionIsCommunicationException()
        {
            var exception = new CommunicationException(fuzzy.String());

            Assert.True(Handle(exception));

            var retry = Assert.IsType<ExceptionHandlingRetryResult>(result);
            Assert.False(retry.IsTransient);
            Assert.Equal(exception.GetType().FullName, retry.ExceptionId);
            Assert.Equal(nonTransientRetryCount, retry.MaxRetryCount);
        }

        [Fact]
        public void ReturnsFalseWhenExceptionIsNotRecognized()
        {
            Assert.False(Handle(new TestException()));
            Assert.Null(result);
        }

        [Fact(Explicit = true)] // TODO: SUT bug. Null exceptionInformation is dereferenced.
        public void ThrowsArgumentNullExceptionWhenExceptionInformationIsNull()
        {
            var actual = Assert.Throws<ArgumentNullException>(
                () => sut.TryHandleException(null, retrySettings, out result));
            Assert.Equal("exceptionInformation", actual.ParamName);
        }

        [Fact(Explicit = true)] // TODO: SUT bug. Null retrySettings is dereferenced for retriable exceptions.
        public void ThrowsArgumentNullExceptionWhenRetrySettingsIsNull()
        {
            var actual = Assert.Throws<ArgumentNullException>(
                () => sut.TryHandleException(new ExceptionInformation(new EndpointNotFoundException()), null, out result));
            Assert.Equal(nameof(retrySettings), actual.ParamName);
        }

        public static TheoryData<Exception> FailoverExceptions => new()
        {
            new EndpointNotFoundException(),
            new CommunicationObjectAbortedException(),
            new CommunicationObjectFaultedException(),
            new ObjectDisposedException(fuzzy.String()),
            new ChannelTerminatedException(),
        };

        public static TheoryData<Exception> TransientExceptions => new()
        {
            new TimeoutException(),
            new ServerTooBusyException(fuzzy.String()),
        };

        public static TheoryData<Exception> NonRetriableExceptions => new()
        {
            new ActionNotSupportedException(fuzzy.String()),
            new AddressAccessDeniedException(fuzzy.String()),
            new SecurityAccessDeniedException(fuzzy.String()),
        };

        bool Handle(Exception exception) =>
            sut.TryHandleException(new ExceptionInformation(exception), retrySettings, out result);

        sealed class TestException : Exception
        {
        }
    }
}
