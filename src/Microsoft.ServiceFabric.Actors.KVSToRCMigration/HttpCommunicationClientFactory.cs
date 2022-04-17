// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.KVSToRCMigration
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.ServiceFabric.Services.Client;
    using Microsoft.ServiceFabric.Services.Communication.Client;

    internal class HttpCommunicationClientFactory : CommunicationClientFactoryBase<HttpCommunicationClient>
    {
        private MigrationSecuritySettings clientSecuritySettings;

        public HttpCommunicationClientFactory(
            IServicePartitionResolver servicePartitionResolver = null,
            IEnumerable<IExceptionHandler> exceptionHandlers = null,
            string traceId = null,
            MigrationSecuritySettings clientSecuritySettings = null)
            : base(servicePartitionResolver, exceptionHandlers, traceId)
        {
            this.clientSecuritySettings = clientSecuritySettings;
        }

        protected override void AbortClient(HttpCommunicationClient client)
        {
            // Do nothing
        }

        protected override Task<HttpCommunicationClient> CreateClientAsync(string endpoint, CancellationToken cancellationToken)
        {
            return Task.FromResult(new HttpCommunicationClient(endpoint, this.clientSecuritySettings));
        }

        protected override bool ValidateClient(HttpCommunicationClient client)
        {
            // Do nothing
            return true;
        }

        protected override bool ValidateClient(string endpoint, HttpCommunicationClient client)
        {
            // Do nothing
            return true;
        }
    }
}
