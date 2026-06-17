// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Microsoft.ServiceFabric.Data.Beta
{
        
    /// <summary>
    /// Defines isolation level options for single item primary reads within a transaction
    /// </summary>
    public enum IsolationLevel
    {
        /// <summary>
        /// Always use read repeatable for single item primary reads
        /// </summary>
        ReadRepeatable = 0,

        /// <summary>
        /// Always use snapshot for single item primary reads
        /// </summary>
        Snapshot = 1
    }
    
}
