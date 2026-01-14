// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Common;
using Microsoft.ServiceFabric.Common.Exceptions;

namespace Microsoft.ServiceFabric.Client.Http
{
    sealed partial class PropertyManagementClient : IPropertyManagementClient
    {
        async Task<bool> IPropertyManagementClient.NameExistsAsync(string nameId, long? serverTimeout, CancellationToken cancellationToken)
        {
            nameId.ThrowIfNull(nameof(nameId));
            serverTimeout?.ThrowIfOutOfInclusiveRange("serverTimeout", 1, 4294967295);

            try
            {
                await GetNameExistsInfoAsync(nameId, serverTimeout, cancellationToken);
            }
            catch (ServiceFabricException ex)
            {
                if (ex.ErrorCode.Equals(FabricErrorCodes.FABRIC_E_DOES_NOT_EXIST))
                    return false;
                throw;
            }

            return true;
        }
    }
}
