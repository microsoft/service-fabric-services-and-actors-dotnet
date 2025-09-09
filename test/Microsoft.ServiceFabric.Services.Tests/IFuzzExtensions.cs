// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using Fuzzy;
using Moq;

namespace Microsoft.ServiceFabric.Services
{
    static class IFuzzExtensions
    {
        internal static Type Type(this IFuzz fuzzy)
        {
            var mock = new Mock<Type>();
            mock.SetupGet(_ => _.Name).Returns(fuzzy.String());
            return mock.Object;
        }
    }
}
