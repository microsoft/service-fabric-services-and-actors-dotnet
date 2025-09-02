// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Fuzzy;
using Inspector;
using Moq;
using Xunit;

namespace Microsoft.ServiceFabric.Diagnostics.Metrics.Implementation
{
    public abstract class Int64Meter1DTest
    {
        readonly IMeter1D<long> sut;

        // Constructor parameters
        readonly IFabricMeter fabricMeter = Mock.Of<IFabricMeter>();
        readonly List<string> systemDimensions = fuzzy.List(() => fuzzy.String());

        // Test fixture
        static readonly IFuzz fuzzy = new RandomFuzz(Environment.TickCount);

        public Int64Meter1DTest() => sut = new Int64Meter1D(fabricMeter, systemDimensions);

        public class Class : Int64Meter1DTest
        {
            [Fact]
            public void InvokesBaseWithGivenArguments()
            {
                var meter = (Meter)sut;
                Assert.Same(fabricMeter, meter.Field<IFabricMeter>().Value);
                Assert.Equal(systemDimensions, meter.Field<string[]>().Value);
            }
        }

        public class Record : Int64Meter1DTest
        {
            readonly long value = fuzzy.Int64();
            readonly string customDimension1 = fuzzy.String();

            [Fact]
            public void CallsFabricMeterRecordWithMultipleSystemDimensions()
            {
                var expectedArray = systemDimensions.Concat(new[] { customDimension1 }).ToArray();

                sut.Record(value, customDimension1);

                Mock.Get(fabricMeter).Verify(m => m.Record(value, (uint)expectedArray.Length, It.Is<string[]>(arr => arr.SequenceEqual(expectedArray))), Times.Once);
            }
        }
    }
}
