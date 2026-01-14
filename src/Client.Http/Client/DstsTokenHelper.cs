// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Common;

namespace Microsoft.ServiceFabric.Client
{
    /// <summary>
    /// Class to get Dsts Token. For internal use only by Service Fabric tooling.
    /// </summary>
    public class DstsTokenHelper
    {
        const string DstsClientLibraryName = "Microsoft.ServiceFabric.Client.DSTS.dll";
        const string DstsHelperClassName = "Microsoft.ServiceFabric.Client.DSTS.DSTSHelper";
        const string GetSecurityTokenMethodName = "GetAuthorizationHeader";

        /// <summary>
        /// Gets Access token from Dsts secure token service. For internal use only by Service Fabric tooling.
        /// </summary>
        /// <param name="metadata">Token Service metadata used for secured connection to cluster.</param>
        /// <param name="interactive">Flag to indicate interactive logon.</param>
        /// <param name="cancellationToken">Cancellation Token to cancel the operation.</param>
        /// <returns>Access Token from DSTS.</returns>
        public static Task<string> GetAccessTokenFromDstsAsync(TokenServiceMetadata metadata, bool interactive, CancellationToken cancellationToken = default)
        {
            if (cancellationToken.IsCancellationRequested)
                throw new OperationCanceledException();

            Assembly module;
            string assembly = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), DstsClientLibraryName);

            try
            {
                module = Assembly.LoadFrom(assembly);
            }
            catch (FileNotFoundException e)
            {
                throw new InvalidOperationException(SR.ErrorDstsNotSupported, e);
            }

            Type dstsHelper = module.GetType(DstsHelperClassName, false);
            if (dstsHelper == null)
                throw new InvalidOperationException(SR.ErrorDstsNotSupported);

            MethodInfo getAuthorizationHeaderMetod = dstsHelper.GetMethod(GetSecurityTokenMethodName, BindingFlags.Static | BindingFlags.Public);
            if (getAuthorizationHeaderMetod == null)
            {
                throw new InvalidOperationException(SR.ErrorDstsNotSupported);
            }

            object authHeader = getAuthorizationHeaderMetod.Invoke(null, new object[] { metadata.ServiceName, metadata.ServiceDnsName, metadata.Metadata, interactive });
            return Task.FromResult((string)authHeader);
        }
    }
}
