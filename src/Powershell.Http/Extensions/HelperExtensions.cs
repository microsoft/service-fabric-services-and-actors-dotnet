// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Powershell.Http
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Class containing helper extension methods.
    /// </summary>
    internal static class HelperExtensions
    {
        public static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(this Hashtable table)
        {
            return table
              .Cast<DictionaryEntry>()
              .ToDictionary(entry => (TKey)entry.Key, entry => (TValue)entry.Value);
        }
    }
}
