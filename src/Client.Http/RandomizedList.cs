// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Client.Http
{
    using System;
    using System.Collections.Generic;
    using Microsoft.ServiceFabric.Client.Http.Resources;

    /// <summary>
    /// A list wrapper that can randomly return elements or round-robin through a list
    /// </summary>
    /// <typeparam name="T">The object type contained in the list</typeparam>
    internal class RandomizedList<T>
    {
        private readonly object lockObject;
        private readonly IReadOnlyList<T> data;
        private readonly int length;
        private readonly Random random;
        private readonly Func<int> getNext;

        private int next;

        /// <summary>
        /// Initializes a new instance of the <see cref="RandomizedList{T}"/> class.
        /// </summary>
        /// <param name="elements">The elements that should be copied into the list</param>
        /// <param name="random">
        /// The random number generator to get the next index to return. If null, the random order is just sequential.
        /// </param>
        public RandomizedList(IReadOnlyList<T> elements, Random random = null)
        {
            if (elements == null)
            {
                throw new ArgumentNullException(nameof(elements));
            }

            if (elements.Count < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(elements), SR.ErrorCollectionCannotBeEmpty);
            }

            this.length = elements.Count;
            this.data = elements;
            this.lockObject = new object();
            this.next = 0;

            if (this.length == 1)
            {
                this.getNext = () => 0;
            }
            else
            {
                if (random != null)
                {
                    this.random = random;
                    this.getNext = () => this.random.Next(0, this.length);
                    this.next = this.getNext();
                }
                else
                {
                    this.getNext = () => (this.next + 1) % this.length;
                }
            }
        }

        /// <summary>
        /// Gets the number of elements in the list
        /// </summary>
        public int Count => this.length;

        /// <summary>
        /// Gets a random element of the list and sets the next to get in a thread-safe manner.
        /// </summary>
        /// <returns>An element from the list</returns>
        public T GetElement()
        {
            if (this.length == 1)
            {
                return this.data[0];
            }

            lock (this.lockObject)
            {
                var element = this.data[this.next];
                this.next = this.getNext();
                return element;
            }
        }
    }
}
