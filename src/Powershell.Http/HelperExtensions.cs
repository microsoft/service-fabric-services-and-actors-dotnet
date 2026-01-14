// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.ServiceFabric.Powershell.Http
{
    static class HelperExtensions
    {
        internal static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(this Hashtable table) =>
            table.Cast<DictionaryEntry>().ToDictionary(entry => (TKey)entry.Key, entry => (TValue)entry.Value);
    }
}
