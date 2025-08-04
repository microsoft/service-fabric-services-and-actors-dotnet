// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Fabric.Common;
using System.Linq;

namespace Microsoft.ServiceFabric.Diagnostics.Telemetry
{
    public class MockConfigStore : IConfigStore2
    {
        private readonly Dictionary<string, string> store = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        public MockConfigStore(string nodeName, string runtimeVersion, bool metricsConfigEnabled)
        {
            AddKeyValue("FabricNode", "InstanceName", nodeName);
            AddKeyValue("FabricNode", "NodeVersion", runtimeVersion);
            AddKeyValue("Telemetry/Metrics", "IsEnabled", metricsConfigEnabled.ToString().ToLower());
        }

        public void AddKeyValue(string sectionName, string keyName, string value)
        {
            store.Add(sectionName + "/" + keyName, value);
        }

        public string ReadUnencryptedString(string sectionName, string keyName)
        {
            string value;
            bool success = store.TryGetValue(sectionName + "/" + keyName, out value);

            return success ? value : string.Empty;
        }

        public string ReadUnencryptedString(string sectionName, string keyName, string defaultValue)
        {
            throw new NotImplementedException();
        }

        public ICollection<string> GetAllKeys(string sectionName)
        {
            return store.Keys.Where(k => k.StartsWith(sectionName)).Select(k => k.Substring(sectionName.Length + 1)).ToList();
        }

        public ICollection<string> GetSections(string partialSectionName)
        {
            throw new NotImplementedException();
        }

        public ICollection<string> GetKeys(string sectionName, string partialKeyName)
        {
            throw new NotImplementedException();
        }

        public bool IgnoreUpdateFailures { get; set; }

        public string ReadString(string sectionName, string keyName, out bool isEncrypted)
        {
            throw new NotImplementedException();
        }
    }
}
