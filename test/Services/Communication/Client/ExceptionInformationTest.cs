// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using Fuzzy;
using Xunit;

namespace Microsoft.ServiceFabric.Services.Communication.Client;

public abstract class ExceptionInformationTest
{
    // Constructor parameters
    readonly System.Exception exception = new InvalidOperationException();
    readonly TargetReplicaSelector targetReplica = fuzzy.Enum<TargetReplicaSelector>();

    static readonly IFuzz fuzzy = new RandomFuzz(Environment.TickCount);

    public sealed class Constructor_Exception : ExceptionInformationTest
    {
        [Fact]
        public void InitializesProperties()
        {
            var sut = new ExceptionInformation(exception);
            Assert.Same(exception, sut.Exception);
            Assert.Equal(TargetReplicaSelector.Default, sut.TargetReplica);
        }

        [Fact(Explicit = true)] // TODO: SUT bug. Missing argument validation for exception.
        public void ThrowsArgumentNullExceptionWhenExceptionIsNull()
        {
            var actual = Assert.Throws<ArgumentNullException>(
                () => new ExceptionInformation(null));
            Assert.Equal(nameof(exception), actual.ParamName);
        }
    }

    public sealed class Constructor_Exception_TargetReplicaSelector : ExceptionInformationTest
    {
        [Theory]
        [InlineData(TargetReplicaSelector.Default)]
        [InlineData(TargetReplicaSelector.RandomReplica)]
        [InlineData(TargetReplicaSelector.RandomSecondaryReplica)]
        public void InitializesProperties(TargetReplicaSelector targetReplica)
        {
            var sut = new ExceptionInformation(exception, targetReplica);
            Assert.Same(exception, sut.Exception);
            Assert.Equal(targetReplica, sut.TargetReplica);
        }

        [Fact(Explicit = true)] // TODO: SUT bug. Missing argument validation for exception.
        public void ThrowsArgumentNullExceptionWhenExceptionIsNull()
        {
            var actual = Assert.Throws<ArgumentNullException>(
                () => new ExceptionInformation(null, targetReplica));
            Assert.Equal(nameof(exception), actual.ParamName);
        }
    }
}
