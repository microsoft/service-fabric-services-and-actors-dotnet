// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Powershell.Http
{
    using System;
    using System.Security.Cryptography.X509Certificates;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Identity.Client;
    using Microsoft.ServiceFabric.Client;
    using Microsoft.ServiceFabric.Common;

    /// <summary>
    /// Contains methods for getting credentials from AAD and loading X509 certificates.
    /// </summary>
    internal class CredentialsUtil
    {
        public static async Task<string> GetAccessTokenAsync(AadMetadata aad, CancellationToken cancellationToken)
        {
            var pca = PublicClientApplicationBuilder.Create(aad.Client).WithAuthority(aad.Authority).Build();
            var account = await pca.GetAccountAsync(aad.Client);
            var scopes = new string[] { $"{aad.Cluster}/.default" };
            try
            {
                var silentAuthResult = await pca.AcquireTokenSilent(scopes, account).ExecuteAsync();
                return silentAuthResult.AccessToken;
            }
            catch (MsalUiRequiredException)
            {
                try
                {
                    var interactiveAuthResult = await pca.AcquireTokenInteractive(scopes).WithAccount(account).ExecuteAsync();
                    return interactiveAuthResult.AccessToken;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(Resource.ErrorAAD);
                    Console.WriteLine("Message: " + ex.Message + "\n");
                    Console.WriteLine("Attempting Device Code Login");
                    try
                    {
                        var deviceCodeAuthResult = await pca.AcquireTokenWithDeviceCode(scopes, deviceCodeResult =>
                        {
                            // This will print the message on the console which tells the user where to go sign-in using
                            // a separate browser and the code to enter once they sign in.
                            // The AcquireTokenWithDeviceCode() method will poll the server after firing this
                            // device code callback to look for the successful login of the user via that browser.
                            // This background polling (whose interval and timeout data is also provided as fields in the
                            // deviceCodeCallback class) will occur until:
                            // * The user has successfully logged in via browser and entered the proper code
                            // * The timeout specified by the server for the lifetime of this code (typically ~15 minutes) has been reached
                            // * The developing application calls the Cancel() method on a CancellationToken sent into the method.
                            //   If this occurs, an OperationCanceledException will be thrown (see catch below for more details).
                            Console.WriteLine(deviceCodeResult.Message);
                            return Task.FromResult(0);
                        }).ExecuteAsync();

                        return deviceCodeAuthResult.AccessToken;
                    }
                    catch (Exception deviceCodeError)
                    {
                        Console.WriteLine("Message: " + deviceCodeError.Message + "\n");
                        throw;
                    }
                }
            }
        }

        public static X509Certificate2 GetCertificate(StoreLocation storeLocation, string storeName, object findValue, X509FindType findType)
        {
            var store = new X509Store(storeName, storeLocation);
            store.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);
            var collection = store.Certificates;
            var ret = collection.Find(findType, findValue, false);

            if (ret.Count > 0)
            {
                return ret[0];
            }

            return null;
        }

        public static Task<string> GetAccessTokenDstsAsync(TokenServiceMetadata metadata, CancellationToken cancellationToken)
        {
            return DstsTokenHelper.GetAccessTokenFromDstsAsync(metadata, true, cancellationToken);
        }
    }
}
