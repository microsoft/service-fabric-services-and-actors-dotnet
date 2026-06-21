// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Fabric;
using Fuzzy;
using Microsoft.ServiceFabric.Services.Communication;
using Microsoft.ServiceFabric.Services.Remoting.V2.Runtime;
using Xunit;

namespace Microsoft.ServiceFabric.Actors.Runtime;

public abstract class FabricActorExceptionConvertorTest
{
    readonly ExceptionConvertorBase sut = new FabricActorExceptionConvertor();

    static readonly IFuzz fuzzy = new RandomFuzz(Environment.TickCount);

    public sealed class GetInnerExceptions : FabricActorExceptionConvertorTest
    {
        readonly string message = fuzzy.String();

        [Fact]
        public void ReturnsArrayContainingInnerExceptionWhenPresent()
        {
            var inner = new InvalidOperationException(fuzzy.String());
            var original = new DuplicateMessageException(message, inner);

            Exception[] result = sut.GetInnerExceptions(original);

            Exception actual = Assert.Single(result);
            Assert.Same(inner, actual);
        }

        [Fact]
        public void ReturnsNullWhenInnerExceptionIsAbsent()
        {
            var original = new DuplicateMessageException(message);

            Exception[] result = sut.GetInnerExceptions(original);

            Assert.Null(result);
        }
    }

    public sealed class TryConvertToServiceException : FabricActorExceptionConvertorTest
    {
        readonly string message = fuzzy.String();

        [Theory]
        [InlineData(typeof(DuplicateMessageException))]
        [InlineData(typeof(InvalidReentrantCallException))]
        [InlineData(typeof(ReminderNotFoundException))]
        [InlineData(typeof(ReentrancyModeDisallowedException))]
        [InlineData(typeof(ReentrantActorInvalidStateException))]
        [InlineData(typeof(ActorConcurrencyLockTimeoutException))]
        [InlineData(typeof(ActorDeletedException))]
        [InlineData(typeof(ReminderLoadInProgressException))]
        public void ReturnsTrueAndProducesServiceExceptionForKnownFabricException(Type knownType)
        {
            var original = (FabricException)Activator.CreateInstance(knownType, message);

            bool result = sut.TryConvertToServiceException(original, out ServiceException converted);

            Assert.True(result);
            Assert.Equal(knownType.FullName, converted.ActualExceptionType);
            Assert.Equal(message, converted.Message);
        }

        [Fact]
        public void ReturnsFalseWhenFabricExceptionTypeIsUnknown()
        {
            var original = new UnknownFabricException(message);

            bool result = sut.TryConvertToServiceException(original, out ServiceException converted);

            Assert.False(result);
            Assert.Null(converted);
        }

        [Fact]
        public void ReturnsFalseWhenExceptionIsNotFabricException()
        {
            var original = new InvalidOperationException(message);

            bool result = sut.TryConvertToServiceException(original, out ServiceException converted);

            Assert.False(result);
            Assert.Null(converted);
        }

        sealed class UnknownFabricException(string message) : FabricException(message);
    }
}
