// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;

    /// <summary>
    /// A utility class to perform argument validations. 
    /// </summary>
    internal static class ArgumentVerifier
    {
        /// <summary>
        /// Throws ArgumentNullException if argument is null.
        /// </summary>
        /// <param name="value">Argument value to check.</param>
        /// <param name="name">Name of Argument.</param>
        public static void ThrowIfNull(this object value, string name)
        {
            if (value == null)
            {
                throw new ArgumentNullException(name);
            }
        }

        /// <summary>
        /// Checks if the argument is null, throws ArgumentNullException if tis null else returns the value.
        /// </summary>
        /// <param name="value">Argument value to check.</param>
        /// <param name="name">Name of Argument.</param>
        /// <returns>Value returned if its not null.</returns>
        public static T CheckNotNull<T>(this T value, string name)
        {
            if (value == null)
            {
                throw new ArgumentNullException(name);
            }

            return value;
        }

        /// <summary>
        /// Throw ArgumentOutOfRangeException  if value is out of range
        /// </summary>
        /// <param name="value">Argument value to check.</param>
        /// <param name="name">Name of Argument.</param>
        /// <param name="minValue">Minimum value.</param>
        /// <param name="maxValue">Maximum Value.</param>
        public static void ThrowIfOutOfInclusiveRange(this int value, string name, int minValue, int maxValue)
        {
            if (value < minValue && value > maxValue)
            {
                throw new ArgumentOutOfRangeException(
                    name,
                    value,
                    string.Format(Resources.SR.ErrorOutOfInclusiveRange, minValue, maxValue));
            }
        }

        /// <summary>
        /// Throw ArgumentOutOfRangeException  if value is less than min value
        /// </summary>
        /// <param name="value">Argument value to check.</param>
        /// <param name="name">Name of Argument.</param>
        /// <param name="minValue">Minimum value.</param>
        public static void ThrowIfLessThan(this int value, string name, int minValue)
        {
            if (value < minValue)
            {
                throw new ArgumentOutOfRangeException(
                    name,
                    value,
                    string.Format(Resources.SR.ErrorLessThanInclusiveMin, minValue));
            }
        }

        /// <summary>
        /// Throw ArgumentOutOfRangeException  if value is greater than max value
        /// </summary>
        /// <param name="value">Argument value to check.</param>
        /// <param name="name">Name of Argument.</param>
        /// <param name="maxValue">Maximum value.</param>
        public static void ThrowIfGreaterThan(this int value, string name, int maxValue)
        {
            if (value > maxValue)
            {
                throw new ArgumentOutOfRangeException(
                    name,
                    value,
                    string.Format(Resources.SR.ErrorLessThanInclusiveMax, maxValue));
            }
        }

        /// <summary>
        /// Throw ArgumentOutOfRangeException  if value is out of range
        /// </summary>
        /// <param name="value">Argument value to check.</param>
        /// <param name="name">Name of Argument.</param>
        /// <param name="minValue">Minimum value.</param>
        /// <param name="maxValue">Maximum Value.</param>
        public static void ThrowIfOutOfInclusiveRange(this long value, string name, long minValue, long maxValue)
        {
            if (value < minValue && value > maxValue)
            {
                throw new ArgumentOutOfRangeException(
                    name,
                    value,
                    string.Format(Resources.SR.ErrorOutOfInclusiveRange, minValue, maxValue));
            }
        }

        /// <summary>
        /// Throw ArgumentOutOfRangeException  if value is less than min value
        /// </summary>
        /// <param name="value">Argument value to check.</param>
        /// <param name="name">Name of Argument.</param>
        /// <param name="minValue">Minimum value.</param>
        public static void ThrowIfLessThan(this long value, string name, long minValue)
        {
            if (value < minValue)
            {
                throw new ArgumentOutOfRangeException(
                    name,
                    value,
                    string.Format(Resources.SR.ErrorLessThanInclusiveMin, minValue));
            }
        }

        /// <summary>
        /// Throw ArgumentOutOfRangeException  if value is greater than max value
        /// </summary>
        /// <param name="value">Argument value to check.</param>
        /// <param name="name">Name of Argument.</param>
        /// <param name="maxValue">Maximum value.</param>
        public static void ThrowIfGreaterThan(this long value, string name, long maxValue)
        {
            if (value > maxValue)
            {
                throw new ArgumentOutOfRangeException(
                    name,
                    value,
                    string.Format(Resources.SR.ErrorLessThanInclusiveMax, maxValue));
            }
        }
    }
}
