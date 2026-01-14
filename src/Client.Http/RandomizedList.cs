// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Collections.Generic;
using Microsoft.ServiceFabric.Client.Http.Resources;

namespace Microsoft.ServiceFabric.Client.Http
{
    /// <summary>
    /// A list wrapper that can randomly return elements or round-robin through a list
    /// </summary>
    sealed class RandomizedList<T>
    {
        readonly object lockObject;
        readonly IReadOnlyList<T> data;
        readonly int length;
        readonly Random random;
        readonly Func<int> getNext;

        int next;

        internal RandomizedList(IReadOnlyList<T> elements, Random random = null)
        {
            if (elements == null)
                throw new ArgumentNullException(nameof(elements));

            if (elements.Count < 1)
                throw new ArgumentOutOfRangeException(nameof(elements), SR.ErrorCollectionCannotBeEmpty);

            length = elements.Count;
            data = elements;
            lockObject = new object();
            next = 0;

            if (length == 1)
                getNext = () => 0;
            else
            {
                if (random != null)
                {
                    this.random = random;
                    getNext = () => this.random.Next(0, length);
                    next = getNext();
                }
                else
                    getNext = () => (next + 1) % length;
            }
        }

        internal int Count => length;

        internal T GetElement()
        {
            if (length == 1)
                return data[0];

            lock (lockObject)
            {
                var element = data[next];
                next = getNext();
                return element;
            }
        }
    }
}
