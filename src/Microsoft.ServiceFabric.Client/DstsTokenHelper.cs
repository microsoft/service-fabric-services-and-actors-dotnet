// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Client
{
    using System;
    using System.IO;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.ServiceFabric.Client.Resources;
    using Microsoft.ServiceFabric.Common;

    /// <summary>
    /// Class to get Dsts Token. For internal use only by Service Fabric tooling.
    /// </summary>
    public class DstsTokenHelper
    {
        private const string DstsClientLibraryName = "Microsoft.ServiceFabric.Client.DSTS.dll";
        private const string DstsHelperClassName = "Microsoft.ServiceFabric.Client.DSTS.DSTSHelper";
        private const string GetSecurityTokenMethodName = "GetAuthorizationHeader";

        /// <summary>
        /// Gets Access token from Dsts secure token service. For internal use only by Service Fabric tooling.
        /// </summary>
        /// <param name="metadata">Token Service metadata used for secured connection to cluster.</param>
        /// <param name="interactive">Flag to indicate interactive logon.</param>
        /// <param name="cancellationToken">Cancellation Token to cancel the operation.</param>
        /// <returns>Access Token from DSTS.</returns>
        public static Task<string> GetAccessTokenFromDstsAsync(TokenServiceMetadata metadata, bool interactive, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (cancellationToken.IsCancellationRequested)
            {
                throw new OperationCanceledException();
            }

            Assembly module;            
            var assembly = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), DstsClientLibraryName);

            try
            {
                module = Assembly.LoadFrom(assembly);
            }
            catch (FileNotFoundException e)
            {
                throw new InvalidOperationException(SR.ErrorDstsNotSupported, e);
            }

            var dstsHelper = module.GetType(DstsHelperClassName, false);
            if (dstsHelper == null)
            {
                throw new InvalidOperationException(SR.ErrorDstsNotSupported);
            }

            var getAuthorizationHeaderMetod = dstsHelper.GetMethod(GetSecurityTokenMethodName, BindingFlags.Static | BindingFlags.Public);
            if (getAuthorizationHeaderMetod == null)
            {
                throw new InvalidOperationException(SR.ErrorDstsNotSupported);
            }

            var authHeader = getAuthorizationHeaderMetod.Invoke(null, new object[] { metadata.ServiceName, metadata.ServiceDnsName, metadata.Metadata, interactive });
            return Task.FromResult((string)authHeader);
        }
    }
}
