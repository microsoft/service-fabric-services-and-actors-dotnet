// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Client.Http
{
    using System.Net.Security;
    using System.Security.Authentication;
    using System.Security.Cryptography.X509Certificates;
    using Microsoft.ServiceFabric.Client.Http.Resources;
    using Microsoft.ServiceFabric.Common.Security;

    /// <summary>
    /// Class to verify the remote Secure Sockets Layer (SSL) certificate used for authentication.
    /// </summary>
    internal class ServerCertificateValidatorHttpWrapper : ServerCertificateValidator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ServerCertificateValidatorHttpWrapper"/> class to perform remote certificate validation using <paramref name="remoteX509SecuritySettings"/>
        /// </summary>
        /// <param name="remoteX509SecuritySettings">Settings to validate remote certificate.</param>
        public ServerCertificateValidatorHttpWrapper(RemoteX509SecuritySettings remoteX509SecuritySettings)
            : base(remoteX509SecuritySettings)
        {            
        }

        /// <summary>
        /// Callback to Verify the remote Secure Sockets Layer (SSL) certificate used for authentication.
        /// </summary>
        /// <param name="sender">An object that contains state information for this validation.</param>
        /// <param name="cert">The certificate used to authenticate the remote party.</param>
        /// <param name="chain">The chain of certificate authorities associated with the remote certificate.</param>
        /// <param name="sslPolicyErrors">One or more errors associated with the remote certificate.</param>
        /// <returns>
        /// A <see cref="bool"/> value that determines whether the specified certificate is accepted for authentication.
        /// </returns>
        public bool ValidateServerCertificate(
            object sender,
            X509Certificate2 cert,
            X509Chain chain,
            SslPolicyErrors sslPolicyErrors)
        {            
            var result = this.ValidateCertificate(
                sender,
                cert,
                chain,
                sslPolicyErrors);

            // If remote server cert validation fails, HttpClient throws HttpRequestException which has different exception information on full dotnet framework
            // and dotnet core, so throw AuthenticationException which allows ServiceFabricHttpClient to detect it and make decisions.
            if (!result)
            {
                throw new AuthenticationException("Server Cert validation failed");
            }

            return result;
        }        
    }
}
