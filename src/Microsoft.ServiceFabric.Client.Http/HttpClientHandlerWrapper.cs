// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Client.Http
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.ServiceFabric.Client.Http.Resources;
    using Microsoft.ServiceFabric.Common.Security;

    /// <summary>
    /// Wrapper class to configure and refresh security settings for HttpClientHandler
    /// </summary>
    internal class HttpClientHandlerWrapper
    {
        private readonly HttpClientHandler httpClientHandler;
        private ServerCertificateValidatorHttpWrapper serverCertValidator;
        private SecurityType securityType;
        private bool securityConfigured;

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpClientHandlerWrapper"/> class.
        /// </summary>
        /// <param name="httpClientHandler">HttpClientHandler instance to configure security settings for.</param>
        public HttpClientHandlerWrapper(HttpClientHandler httpClientHandler)
        {
            this.httpClientHandler = httpClientHandler;
            this.securityConfigured = false;
        }

        /// <summary>
        /// Configures security for <see cref="System.Net.Http.HttpClientHandler"/> to connect to secured cluster.
        /// </summary>
        /// <param name="settings">Credential for connecting to cluster.</param>
        public void ConfigureSecuritySettings(SecuritySettings settings)
        {
            if (this.securityConfigured)
            {
                throw new InvalidOperationException(SR.ErrorCannotConfigureSecurityAgain);
            }

            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            this.securityType = settings.SecurityType;

            if (settings.SecurityType == SecurityType.Claims)
            {
                var claimsSettings = settings as ClaimsSecuritySettings;
                this.serverCertValidator = new ServerCertificateValidatorHttpWrapper(claimsSettings.RemoteX509SecuritySettings);

                // If remote server cert validation fails, HttpClient throws HttpRequestException which has different exception information on full dotnet framework
                // and dotnet core, so ServerCertificateValidatorHttpWrapper.ValidateServerCertificate throws AuthenticationException which allows ServiceFabricHttpClient to detect it and make decisions.
                this.httpClientHandler.ServerCertificateCustomValidationCallback = this.serverCertValidator.ValidateServerCertificate;
            }
            else if (settings.SecurityType == SecurityType.X509)
            {
                var x509Settings = settings as X509SecuritySettings;
                if (!this.httpClientHandler.ClientCertificates.Contains(x509Settings.ClientCertificate))
                {
                    this.httpClientHandler.ClientCertificates.Add(x509Settings.ClientCertificate);
                }

                this.serverCertValidator = new ServerCertificateValidatorHttpWrapper(x509Settings.RemoteX509SecuritySettings);

                // If remote server cert validation fails, HttpClient throws HttpRequestException which has different exception information on full dotnet framework
                // and dotnet core, so ServerCertificateValidatorHttpWrapper.ValidateServerCertificate throws AuthenticationException which allows ServiceFabricHttpClient to detect it and make decisions.
                this.httpClientHandler.ServerCertificateCustomValidationCallback = this.serverCertValidator.ValidateServerCertificate;
            }
            else if (settings.SecurityType == SecurityType.Windows)
            {
                this.httpClientHandler.UseDefaultCredentials = true;
            }

            this.securityConfigured = true;
        }

        /// <summary>
        /// Refreshes security for <see cref="System.Net.Http.HttpClientHandler"/> to connect to secured cluster.
        /// This method is invoked to refresh security settings when client request fails because of Authentication errors.
        /// </summary>
        /// <param name="settings">Credential for connecting to cluster.</param>
        public void RefreshSecuritySettings(SecuritySettings settings)
        {
            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            if (!this.securityConfigured)
            {
                throw new InvalidOperationException(SR.ErrorCannotCallRefreshSecuritySettingsBeforeConfiguring);    
            }

            if (this.securityType != settings.SecurityType)
            {
                throw new InvalidOperationException(SR.ErrorCannotChangeSecurityType);
            }

            if (settings.SecurityType == SecurityType.Claims)
            {
                // Update RemoteX509SecuritySettings for cert validator.
                var claimsSettings = settings as ClaimsSecuritySettings;
                this.serverCertValidator.UpdateSecuritySettings(claimsSettings.RemoteX509SecuritySettings);
            }
            else if (settings.SecurityType == SecurityType.X509)
            {
                // Add new client cert and update RemoteX509SecuritySettings for cert validator.
                var x509Settings = settings as X509SecuritySettings;
                this.httpClientHandler.ClientCertificates.Clear();
                if (!this.httpClientHandler.ClientCertificates.Contains(x509Settings.ClientCertificate))
                {
                    this.httpClientHandler.ClientCertificates.Add(x509Settings.ClientCertificate);
                }

                this.serverCertValidator.UpdateSecuritySettings(x509Settings.RemoteX509SecuritySettings);
            }
        }
    }
}
