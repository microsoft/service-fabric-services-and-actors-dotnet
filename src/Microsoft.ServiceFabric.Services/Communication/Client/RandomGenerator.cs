// ------------------------------------------------------------
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT License (MIT).See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Communication.Client
{
    using System;

    internal class RandomGenerator
    {
        private readonly object randomLock;
        private Random rand = new Random();

        public RandomGenerator()
        {
            this.randomLock = new object();
        }

        public double NextDouble()
        {
            lock (this.randomLock)
            {
                return this.rand.NextDouble();
            }
        }
    }
}
