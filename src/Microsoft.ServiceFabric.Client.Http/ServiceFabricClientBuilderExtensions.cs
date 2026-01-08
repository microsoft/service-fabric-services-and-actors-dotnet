// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Client.Http
{
    using System;
    using System.Linq;
    using System.Net.Http;
    using Microsoft.ServiceFabric.Client.Http.Resources;

    /// <summary>
    /// Estensions for IServiceFabricClientBuilder for configuring ServiceFabricHttpClient.
    /// </summary>
    public static class ServiceFabricClientBuilderExtensions
    {
        /// <summary>
        /// Configure IServiceFabricClientBuilder to create IServiceFabricClient implementation which communicates with Service Fabric cluster via the REST http gateway using the provided HttpClientHandler and DelegatingHandlers.
        /// </summary>
        /// <param name="builder">Builder </param>
        /// <param name="innerHandler">The inner handler which is responsible for processing the HTTP response messages. When null or not provided, <see cref="System.Net.Http.HttpClientHandler"/> will be used as last handler in channel.</param>
        /// <param name="delegateHandlers">An ordered list of <see cref="System.Net.Http.DelegatingHandler"/> instances to be invoked in HTTP message channel as message flows to and from the last handler in the channel.</param>
        /// <returns>IServiceFabricClientBuilder instance.</returns>
        public static ServiceFabricClientBuilder ConfigureHttpClientSettings(
            this ServiceFabricClientBuilder builder,
            HttpClientHandler innerHandler,
            params DelegatingHandler[] delegateHandlers)
        {
            if (innerHandler == null)
            {
                throw new ArgumentNullException(nameof(innerHandler));
            }

            if (delegateHandlers.Any(handler => handler == null))
            {
                throw new ArgumentException(SR.ErrorNullDelegateHandler);
            }

            builder.Container[typeof(HttpClientHandler)] = innerHandler;
            builder.Container[typeof(DelegatingHandler[])] = delegateHandlers;
            return builder;
        }

        internal static ServiceFabricClientBuilder AddClientTypeIdentity(
            this ServiceFabricClientBuilder builder,
            string clientType)
        {
            return builder;
        }
    }
}
