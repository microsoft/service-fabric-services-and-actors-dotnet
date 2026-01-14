// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;

namespace Microsoft.ServiceFabric.Common
{
    static class ArgumentVerifier
    {
        internal static void ThrowIfNull(this object value, string name)
        {
            if (value == null)
                throw new ArgumentNullException(name);
        }

        internal static T CheckNotNull<T>(this T value, string name)
        {
            if (value == null)
                throw new ArgumentNullException(name);
            return value;
        }

        internal static void ThrowIfOutOfInclusiveRange(this int value, string name, int minValue, int maxValue)
        {
            if (value < minValue && value > maxValue)
                throw new ArgumentOutOfRangeException(name, value, string.Format(Resources.SR.ErrorOutOfInclusiveRange, minValue, maxValue));
        }

        internal static void ThrowIfLessThan(this int value, string name, int minValue)
        {
            if (value < minValue)
                throw new ArgumentOutOfRangeException(name, value, string.Format(Resources.SR.ErrorLessThanInclusiveMin, minValue));
        }

        internal static void ThrowIfOutOfInclusiveRange(this long value, string name, long minValue, long maxValue)
        {
            if (value < minValue && value > maxValue)
            {
                throw new ArgumentOutOfRangeException(name, value, string.Format(Resources.SR.ErrorOutOfInclusiveRange, minValue, maxValue));
            }
        }

        internal static void ThrowIfLessThan(this long value, string name, long minValue)
        {
            if (value < minValue)
            {
                throw new ArgumentOutOfRangeException(name, value, string.Format(Resources.SR.ErrorLessThanInclusiveMin, minValue));
            }
        }
    }
}
