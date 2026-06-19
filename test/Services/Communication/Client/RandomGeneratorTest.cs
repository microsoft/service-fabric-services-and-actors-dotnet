// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using Xunit;

namespace Microsoft.ServiceFabric.Services.Communication.Client;

public abstract class RandomGeneratorTest
{
    readonly RandomGenerator sut = new();

    public sealed class NextDouble : RandomGeneratorTest
    {
        [Fact]
        public void Completes() => sut.NextDouble();
    }
}
