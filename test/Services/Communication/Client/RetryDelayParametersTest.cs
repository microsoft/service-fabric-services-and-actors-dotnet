// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using Fuzzy;
using Xunit;

namespace Microsoft.ServiceFabric.Services.Communication.Client;

public abstract class RetryDelayParametersTest
{
    static readonly IFuzz fuzzy = new RandomFuzz(Environment.TickCount);

    readonly RetryDelayParameters sut;

    // Constructor parameters
    readonly int retryAttempt = fuzzy.Int32();
    readonly bool isTransient = fuzzy.Boolean();

    RetryDelayParametersTest() =>
        sut = new RetryDelayParameters(retryAttempt, isTransient);

    public sealed class Constructor : RetryDelayParametersTest
    {
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void InitializesProperties(bool isTransient)
        {
            var sut = new RetryDelayParameters(retryAttempt, isTransient);

            Assert.Equal(isTransient, sut.IsTransient);
            Assert.Equal(retryAttempt, sut.RetryAttempt);
        }
    }
}
