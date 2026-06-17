// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.

using System;
using Fuzzy;
using Xunit;

namespace Microsoft.ServiceFabric.Data.Collections;

public abstract class QueueFullExceptionTest
{
    // Constructor parameters
    readonly string msg = fuzzy.String(); // mirrors SUT parameter name
    readonly Exception innerException = new();

    static readonly IFuzz fuzzy = new RandomFuzz(Environment.TickCount);

    public sealed class Constructor_String : QueueFullExceptionTest
    {
        [Fact]
        public void SetsMessage() =>
            Assert.Same(msg, new QueueFullException(msg).Message);
    }

    public sealed class Constructor_String_Exception : QueueFullExceptionTest
    {
        [Fact]
        public void SetsMessageAndInnerException()
        {
            var sut = new QueueFullException(msg, innerException);
            Assert.Same(msg, sut.Message);
            Assert.Same(innerException, sut.InnerException);
        }
    }
}
