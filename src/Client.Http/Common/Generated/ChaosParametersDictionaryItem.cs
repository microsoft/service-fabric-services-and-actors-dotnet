// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Defines an item in ChaosParametersDictionary of the Chaos Schedule.
    /// </summary>
    public partial class ChaosParametersDictionaryItem
    {
        /// <summary>
        /// Initializes a new instance of the ChaosParametersDictionaryItem class.
        /// </summary>
        /// <param name="key">The key identifying the Chaos Parameter in the dictionary. This key is referenced by Chaos
        /// Schedule Jobs.</param>
        /// <param name="value">Defines all the parameters to configure a Chaos run.
        /// </param>
        public ChaosParametersDictionaryItem(
            string key,
            ChaosParameters value)
        {
            key.ThrowIfNull(nameof(key));
            value.ThrowIfNull(nameof(value));
            this.Key = key;
            this.Value = value;
        }

        /// <summary>
        /// Gets the key identifying the Chaos Parameter in the dictionary. This key is referenced by Chaos Schedule Jobs.
        /// </summary>
        public string Key { get; }

        /// <summary>
        /// Gets defines all the parameters to configure a Chaos run.
        /// </summary>
        public ChaosParameters Value { get; }
    }
}
