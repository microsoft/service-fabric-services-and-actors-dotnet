// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using Microsoft.ServiceFabric.Common.Security;

namespace Microsoft.ServiceFabric.Client.Http
{
    sealed class ServerCertificateValidatorHttpWrapper : ServerCertificateValidator
    {
        internal ServerCertificateValidatorHttpWrapper(RemoteX509SecuritySettings remoteX509SecuritySettings)
            : base(remoteX509SecuritySettings)
        {
        }

        internal bool ValidateServerCertificate(object sender, X509Certificate2 cert, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            bool result = ValidateCertificate(sender, cert, chain, sslPolicyErrors);

            // If remote server cert validation fails, HttpClient throws HttpRequestException which has different exception information on full dotnet framework
            // and dotnet core, so throw AuthenticationException which allows ServiceFabricHttpClient to detect it and make decisions.
            if (!result)
                throw new AuthenticationException("Server Cert validation failed");

            return result;
        }
    }
}
