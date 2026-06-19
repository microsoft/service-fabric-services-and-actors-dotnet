// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Collections.Generic;
using Fuzzy;
using Xunit;

namespace Microsoft.ServiceFabric.Services.Communication;

public abstract class ServiceExceptionTest
{
    readonly ServiceException sut;

    // Constructor parameters
    readonly string actualExceptionType = fuzzy.String();
    readonly string message = fuzzy.String();

    static readonly IFuzz fuzzy = new RandomFuzz(Environment.TickCount);

    ServiceExceptionTest() =>
        sut = new ServiceException(actualExceptionType, message);

    public sealed class ActualExceptionData : ServiceExceptionTest
    {
        [Fact]
        public void StoresAssignedValue()
        {
            var value = new Dictionary<string, string> { [fuzzy.String()] = fuzzy.String() };
            sut.ActualExceptionData = value;
            Assert.Same(value, sut.ActualExceptionData);
        }
    }

    public sealed class ActualExceptionStackTrace : ServiceExceptionTest
    {
        [Fact]
        public void StoresAssignedValue()
        {
            string value = fuzzy.String();
            sut.ActualExceptionStackTrace = value;
            Assert.Same(value, sut.ActualExceptionStackTrace);
        }
    }

    public sealed class ActualInnerExceptions : ServiceExceptionTest
    {
        [Fact]
        public void StoresAssignedValue()
        {
            var value = new List<ServiceException>();
            sut.ActualInnerExceptions = value;
            Assert.Same(value, sut.ActualInnerExceptions);
        }
    }

    public sealed class Constructor : ServiceExceptionTest
    {
        [Fact]
        public void InitializesPropertiesToNull()
        {
            var sut = new ServiceException();
            Assert.Null(sut.ActualExceptionType);
            Assert.Null(sut.ActualExceptionData);
            Assert.Null(sut.ActualExceptionStackTrace);
            Assert.Null(sut.ActualInnerExceptions);
        }
    }

    public sealed class Constructor_String_String : ServiceExceptionTest
    {
        [Fact]
        public void InitializesProperties()
        {
            Assert.Same(actualExceptionType, sut.ActualExceptionType);
            Assert.Same(message, sut.Message);
            Assert.Null(sut.ActualExceptionData);
            Assert.Null(sut.ActualExceptionStackTrace);
            Assert.Null(sut.ActualInnerExceptions);
        }
    }
}
