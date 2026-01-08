// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Client.Http
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Class containing helper extension methods.
    /// </summary>
    internal static class HelperExtensions
    {
        /// <summary>
        /// Helper extension method to add item to query parameters.
        /// This allows to write code with null propagation operator without too much if null checks when adding to query parameters.
        /// </summary>
        /// <param name="value">object value for which this method will be called using null propagation operator.</param>
        /// <param name="queryParams">Query parameters list.</param>
        /// <param name="item">item to add to query parameters.</param>
        public static void AddToQueryParameters(this object value, List<string> queryParams, string item)
        {
            queryParams.Add(item);
        }
    }
}
