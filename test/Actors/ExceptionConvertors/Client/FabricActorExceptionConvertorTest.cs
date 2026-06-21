// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Fabric;
using Fuzzy;
using Microsoft.ServiceFabric.Actors.Runtime;
using Microsoft.ServiceFabric.Services.Communication;
using Microsoft.ServiceFabric.Services.Remoting.V2.Client;
using Xunit;

namespace Microsoft.ServiceFabric.Actors.Client;

public abstract class FabricActorExceptionConvertorTest
{
    readonly IExceptionConvertor sut = new FabricActorExceptionConvertor();

    static readonly IFuzz fuzzy = new RandomFuzz(Environment.TickCount);

    public sealed class TryConvertFromServiceException_ServiceException_Exception : FabricActorExceptionConvertorTest
    {
        // Method parameters
        readonly ServiceException serviceException;

        readonly string message = fuzzy.String();

        public TryConvertFromServiceException_ServiceException_Exception() =>
            serviceException = ServiceExceptionFor(typeof(DuplicateMessageException), message);

        [Fact]
        public void ProducesKnownFabricExceptionWithoutInnerException()
        {
            bool result = sut.TryConvertFromServiceException(serviceException, out Exception actual);

            Assert.True(result);
            Assert.IsType<DuplicateMessageException>(actual);
            Assert.Equal(message, actual.Message);
            Assert.Null(actual.InnerException);
            AssertRemoteMetadataCopied(serviceException, actual);
        }

        [Fact]
        public void ReturnsFalseWhenActualExceptionTypeIsUnknown()
        {
            var unknownServiceException = new ServiceException(fuzzy.String(), message);

            bool result = sut.TryConvertFromServiceException(unknownServiceException, out Exception actual);

            Assert.False(result);
            Assert.Null(actual);
        }

        [Fact(Explicit = true)] // TODO: SUT bug. TryConvertFromServiceException doesn't validate serviceException.
        public void ThrowsArgumentNullExceptionWhenServiceExceptionIsNull()
        {
            // TryConvertFromServiceException dereferences serviceException.ActualExceptionType without validation,
            // surfacing the defect as a NullReferenceException.
            var exception = Assert.Throws<ArgumentNullException>(() => sut.TryConvertFromServiceException(null, out Exception _));
            Assert.Equal(nameof(serviceException), exception.ParamName);
        }
    }

    public sealed class TryConvertFromServiceException_ServiceException_ExceptionArray_Exception : FabricActorExceptionConvertorTest
    {
        // Method parameters
        readonly ServiceException serviceException;
        readonly Exception[] innerExceptions = null;

        readonly string message = fuzzy.String();

        public TryConvertFromServiceException_ServiceException_ExceptionArray_Exception() =>
            serviceException = ServiceExceptionFor(typeof(DuplicateMessageException), message);

        [Theory]
        [InlineData(typeof(DuplicateMessageException))]
        [InlineData(typeof(InvalidReentrantCallException))]
        [InlineData(typeof(ReminderNotFoundException))]
        [InlineData(typeof(ReentrancyModeDisallowedException))]
        [InlineData(typeof(ReentrantActorInvalidStateException))]
        [InlineData(typeof(ActorConcurrencyLockTimeoutException))]
        [InlineData(typeof(ActorDeletedException))]
        [InlineData(typeof(ReminderLoadInProgressException))]
        public void ReturnsTrueAndProducesKnownFabricExceptionWithPreservedMessage(Type knownType)
        {
            ServiceException knownServiceException = ServiceExceptionFor(knownType, message);

            bool result = sut.TryConvertFromServiceException(knownServiceException, innerExceptions, out Exception actual);

            Assert.True(result);
            Assert.IsType(knownType, actual);
            Assert.Equal(message, actual.Message);
            Assert.Null(actual.InnerException);
            AssertRemoteMetadataCopied(knownServiceException, actual);
        }

        [Fact]
        public void PassesFirstInnerExceptionToProducedException()
        {
            var innerException = new InvalidOperationException(fuzzy.String());
            var orderedInnerExceptions = new[] { innerException, new Exception(fuzzy.String()) };

            bool result = sut.TryConvertFromServiceException(serviceException, orderedInnerExceptions, out Exception actual);

            Assert.True(result);
            Assert.IsType<DuplicateMessageException>(actual);
            Assert.Equal(message, actual.Message);
            Assert.Same(innerException, actual.InnerException);
            AssertRemoteMetadataCopied(serviceException, actual);
        }

        [Fact]
        public void ProducesExceptionWithoutInnerExceptionWhenArrayIsEmpty()
        {
            bool result = sut.TryConvertFromServiceException(serviceException, Array.Empty<Exception>(), out Exception actual);

            Assert.True(result);
            Assert.IsType<DuplicateMessageException>(actual);
            Assert.Equal(message, actual.Message);
            Assert.Null(actual.InnerException);
            AssertRemoteMetadataCopied(serviceException, actual);
        }

        [Fact]
        public void ReturnsFalseWhenActualExceptionTypeIsUnknown()
        {
            var unknownServiceException = new ServiceException(fuzzy.String(), message);

            bool result = sut.TryConvertFromServiceException(unknownServiceException, innerExceptions, out Exception actual);

            Assert.False(result);
            Assert.Null(actual);
        }

        [Fact(Explicit = true)] // TODO: SUT bug. TryConvertFromServiceException doesn't validate serviceException.
        public void ThrowsArgumentNullExceptionWhenServiceExceptionIsNull()
        {
            // TryConvertFromServiceException dereferences serviceException.ActualExceptionType without validation,
            // surfacing the defect as a NullReferenceException.
            var exception = Assert.Throws<ArgumentNullException>(() => sut.TryConvertFromServiceException(null, innerExceptions, out Exception _));
            Assert.Equal(nameof(serviceException), exception.ParamName);
        }
    }

    public sealed class TryConvertFromServiceException_ServiceException_Exception_Exception : FabricActorExceptionConvertorTest
    {
        // Method parameters
        readonly ServiceException serviceException;
        readonly Exception innerException = new InvalidOperationException(fuzzy.String());

        readonly string message = fuzzy.String();

        public TryConvertFromServiceException_ServiceException_Exception_Exception() =>
            serviceException = ServiceExceptionFor(typeof(DuplicateMessageException), message);

        [Fact]
        public void PassesInnerExceptionToProducedException()
        {
            bool result = sut.TryConvertFromServiceException(serviceException, innerException, out Exception actual);

            Assert.True(result);
            Assert.IsType<DuplicateMessageException>(actual);
            Assert.Equal(message, actual.Message);
            Assert.Same(innerException, actual.InnerException);
            AssertRemoteMetadataCopied(serviceException, actual);
        }

        [Fact]
        public void ReturnsFalseWhenActualExceptionTypeIsUnknown()
        {
            var unknownServiceException = new ServiceException(fuzzy.String(), message);

            bool result = sut.TryConvertFromServiceException(unknownServiceException, innerException, out Exception actual);

            Assert.False(result);
            Assert.Null(actual);
        }

        [Fact(Explicit = true)] // TODO: SUT bug. TryConvertFromServiceException doesn't validate serviceException.
        public void ThrowsArgumentNullExceptionWhenServiceExceptionIsNull()
        {
            // TryConvertFromServiceException dereferences serviceException.ActualExceptionType without validation,
            // surfacing the defect as a NullReferenceException.
            var exception = Assert.Throws<ArgumentNullException>(() => sut.TryConvertFromServiceException(null, innerException, out Exception _));
            Assert.Equal(nameof(serviceException), exception.ParamName);
        }
    }

    static ServiceException ServiceExceptionFor(Type knownType, string message) =>
        new(knownType.FullName, message)
        {
            ActualExceptionData = new Dictionary<string, string> { { nameof(Exception.HResult), fuzzy.Int32().ToString() } },
            ActualExceptionStackTrace = fuzzy.String(),
        };

    // Documents current SUT behavior: FromServiceException copies remote metadata into Data. Note that
    // RemoteFabricErrorCode is derived from ActualExceptionData["HResult"] rather than "FabricErrorCode";
    // this assertion captures the existing behavior, not the intended one.
    static void AssertRemoteMetadataCopied(ServiceException serviceException, Exception actual)
    {
        string hresult = serviceException.ActualExceptionData[nameof(Exception.HResult)];
        Assert.Equal(hresult, actual.Data["RemoteHResult"]);
        Assert.Equal((FabricErrorCode)long.Parse(hresult), actual.Data["RemoteFabricErrorCode"]);
        Assert.Equal(serviceException.ActualExceptionStackTrace, actual.Data["RemoteStackTrace"]);
    }
}
