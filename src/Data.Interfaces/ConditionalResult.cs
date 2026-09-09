// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Data
{
    /// <summary>
    /// Represents the result of an operation that may or may not return a value.
    /// </summary>
    public struct ConditionalValue<TValue>
    {
        private readonly bool hasValue;
        private readonly TValue value;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConditionalValue{TValue}"/> struct.
        /// </summary>
        public ConditionalValue(bool hasValue, TValue value)
        {
            this.hasValue = hasValue;
            this.value = value;
        }

        /// <summary>
        /// Gets a value that indicates whether the current <see cref="ConditionalValue{TValue}"/> object has a valid value.
        /// </summary>
        public bool HasValue
        {
            get
            {
                return this.hasValue;
            }
        }

        /// <summary>
        /// Gets the value of the current <see cref="ConditionalValue{TValue}"/> object.
        /// </summary>
        /// <value>Meaningful only when <see cref="HasValue"/> is <see langword="true"/>.</value>
        public TValue Value
        {
            get
            {
                return this.value;
            }
        }
    }
}
