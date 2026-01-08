// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common.Exceptions
{
    using System;
    using System.Collections.Generic;
    using Microsoft.ServiceFabric.Common;

    /// <summary>
    /// Contains mapping of <see cref="FabricErrorCodes"/> to exception types.
    /// </summary>
    internal class ErrorCodeExceptionMap
    {
        static ErrorCodeExceptionMap()
        {
            ErrorCodesForTransientException = new HashSet<FabricErrorCodes>
            {
                { FabricErrorCodes.FABRIC_E_RECONFIGURATION_PENDING },
                { FabricErrorCodes.FABRIC_E_NO_WRITE_QUORUM },
                { FabricErrorCodes.FABRIC_E_SERVICE_OFFLINE },
                { FabricErrorCodes.E_ABORT },
            };
        }

        public static HashSet<FabricErrorCodes> ErrorCodesForTransientException { get; }
    }
}
