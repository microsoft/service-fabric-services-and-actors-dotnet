// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.

using System;
using System.Collections.ObjectModel;

namespace Microsoft.ServiceFabric;

public class StubKeyedCollection<TKey, TItem>(Func<TItem, TKey> getKey) : KeyedCollection<TKey, TItem>()
{
    protected override TKey GetKeyForItem(TItem item) => getKey(item);
}
