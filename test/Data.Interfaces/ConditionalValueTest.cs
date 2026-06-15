// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.

using System;
using Fuzzy;
using Xunit;

namespace Microsoft.ServiceFabric.Data;

public abstract class ConditionalValueTest
{
    // Constructor parameters
    readonly string value = fuzzy.String();

    static readonly IFuzz fuzzy = new RandomFuzz(Environment.TickCount);

    public sealed class HasValue : ConditionalValueTest
    {
        [Theory, InlineData(true), InlineData(false)]
        public void ReturnsHasValueConstructorArgument(bool expected) =>
            Assert.Equal(expected, new ConditionalValue<string>(hasValue: expected, value).HasValue);
    }

    public sealed class Value : ConditionalValueTest
    {
        [Fact]
        public void ReturnsValueConstructorArgumentWhenHasValueIsTrue() =>
            Assert.Same(value, new ConditionalValue<string>(hasValue: true, value).Value);

        [Fact(Explicit = true)] // TODO: SUT bug. Value returns stored value when HasValue is false.
        public void IsDefaultWhenHasValueIsFalse() =>
            // XML doc on ConditionalValue<TValue>.Value says it returns default(TValue)
            // when HasValue is false; actual impl returns the stored value.
            Assert.Null(new ConditionalValue<string>(hasValue: false, value).Value);
    }
}
