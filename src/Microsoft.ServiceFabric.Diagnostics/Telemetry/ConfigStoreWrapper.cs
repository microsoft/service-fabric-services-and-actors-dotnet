// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Fabric.Common;

namespace Microsoft.ServiceFabric.Diagnostics.Telemetry
{
    internal class ConfigStoreWrapper
    {
        readonly IConfigStore2 configStore;

        protected ConfigStoreWrapper()
        {
        }

        public ConfigStoreWrapper(IConfigStore2 configStore)
        {
            this.configStore = configStore ?? throw new ArgumentNullException(nameof(configStore), "Config store cannot be null.");
        }

        public virtual string ReadConfig(string sectionName, string key)
        {
            if (string.IsNullOrEmpty(sectionName))
            {
                throw new ArgumentException("Section name cannot be null or empty.", nameof(sectionName));
            }
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException("Key cannot be null or empty.", nameof(key));
            }

            return this.configStore.ReadUnencryptedString(sectionName, key);
        }
    }
}
