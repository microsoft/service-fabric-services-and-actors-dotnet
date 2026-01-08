// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Client.Http
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.ServiceFabric.Common;
    using Microsoft.ServiceFabric.Common.Exceptions;

    /// <summary>
    /// Class containing methods for performing PropertyManagementClient operations.
    /// </summary>
    internal partial class PropertyManagementClient : IPropertyManagementClient
    {
        /// <inheritdoc />
        public async Task<bool> NameExistsAsync(
            string nameId,
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            nameId.ThrowIfNull(nameof(nameId));
            serverTimeout?.ThrowIfOutOfInclusiveRange("serverTimeout", 1, 4294967295);

            try
            {
                await this.GetNameExistsInfoAsync(nameId, serverTimeout, cancellationToken);
            }
            catch (ServiceFabricException ex)
            {
                if (ex.ErrorCode.Equals(FabricErrorCodes.FABRIC_E_DOES_NOT_EXIST))
                {
                    return false;
                }

                throw;
            }

            return true;
        }
    }
}
