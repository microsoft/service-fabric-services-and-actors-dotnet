// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Collections.Generic;
using Fuzzy;
using Inspector;
using Moq;
using Xunit;

namespace Microsoft.ServiceFabric.Diagnostics.Metrics.Implementation
{
    public abstract class MeterTest
    {
        readonly IFabricMeter fabricMeter = Mock.Of<IFabricMeter>();
        static readonly IFuzz fuzzy = new RandomFuzz(Environment.TickCount);

        public class Constructor : MeterTest
        {
            [Fact]
            public void ThrowsArgumentNullExceptionWhenMeterNameIsNull()
            {
                Assert.Throws<ArgumentNullException>(() => new MeterImplementation(null, new List<string>()));
            }

            [Fact]
            public void SetsEmptySystemDimensionValuesWhenNullProvided()
            {
                Meter meter = new MeterImplementation(fabricMeter, null);

                Assert.NotNull(meter.Field<IEnumerable<string>>().Value);
                Assert.Empty(meter.Field<IEnumerable<string>>().Value);
            }

            [Fact]
            public void SetsSystemDimensionAndMeterValuesWhenProvided()
            {
                var expectedValues = fuzzy.List(() => fuzzy.String());

                Meter meter = new MeterImplementation(fabricMeter, expectedValues);

                Assert.Same(fabricMeter, meter.Field<IFabricMeter>().Value);
                Assert.Same(expectedValues, meter.Field<IEnumerable<string>>().Value);
            }
        }

        class MeterImplementation : Meter
        {
            public MeterImplementation(IFabricMeter fabricMeter, IEnumerable<string> systemDimensionValues) : base(fabricMeter, systemDimensionValues) { }
        }
    }
}
