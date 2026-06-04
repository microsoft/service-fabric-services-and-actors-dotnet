// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Fabric;
using Microsoft.ServiceFabric.Services.Communication.AspNetCore;
using Xunit;

namespace Microsoft.ServiceFabric.AspNetCore.Tests
{
    /// <summary>
    /// Test class for HttpSysCommunicationListener.
    /// </summary>
    public class HttpSysCommunicationListenerTests : AspNetCoreCommunicationListenerTests
    {
        /// <summary>
        /// Tests Url for ServiceFabricIntegrationOptions.UseUniqueServiceUrl
        /// 1. When no endpointRef is provided:
        ///   a. url given to Func to create IWebHost/IHost should be http://+:0
        ///   b. url returned from OpenAsync should be http://IPAddressOrFQDN:0/PartitionId/ReplicaId
        ///
        ///
        /// 2. When endpointRef is provided (protocol and port comes from endpoint.) :
        ///   a. url given to Func to create IWebHost/IHost should be protocol://+:port.
        ///   b. url returned from OpenAsync should be protocol://IPAddressOrFQDN:port/PartitionId/ReplicaId.
        ///
        /// </summary>
        /// <param name="hostType">The type of host used to create the listener.</param>
        [Theory]
        [MemberData(nameof(HostTypes))]
        public void VerifyWithUseUniqueServiceUrlOption(string hostType)
        {
            var context = TestMocksRepository.GetMockStatelessServiceContext();
            context.CodePackageActivationContext.GetEndpoints().Add(this.GetTestEndpoint());
            this.Listener = this.CreateListener(context, EndpointName, hostType);
            this.UseUniqueServiceUrlOptionVerifier();
        }

        /// <summary>
        /// Tests Url for ServiceFabricIntegrationOptions.None
        /// 1. When endpoint name is provided (protocol and port comes from endpoint.) :
        ///   a. url given to Func to create IWebHost/IHost should be protocol://+:port.
        ///   b. url returned from OpenAsync should be protocol://IPAddressOrFQDN:port.
        ///
        /// </summary>
        /// <param name="hostType">The type of host used to create the listener.</param>
        [Theory]
        [MemberData(nameof(HostTypes))]
        public void VerifyWithoutUseUniqueServiceUrlOption(string hostType)
        {
            var context = TestMocksRepository.GetMockStatelessServiceContext();
            context.CodePackageActivationContext.GetEndpoints().Add(this.GetTestEndpoint());
            this.Listener = this.CreateListener(context, EndpointName, hostType);
            this.WithoutUseUniqueServiceUrlOptionVerifier();
        }

        /// <summary>
        /// Verify Listener Open and Close.
        /// </summary>
        /// <param name="hostType">The type of host used to create the listener.</param>
        [Theory]
        [MemberData(nameof(HostTypes))]
        public void VerifyListenerOpenClose(string hostType)
        {
            var context = TestMocksRepository.GetMockStatelessServiceContext();
            context.CodePackageActivationContext.GetEndpoints().Add(this.GetTestEndpoint());
            this.Listener = this.CreateListener(context, EndpointName, hostType);
            this.ListenerOpenCloseVerifier();
        }

        /// <summary>
        /// InvalidOperationException is thrown when Endpoint is not found in service manifest.
        /// </summary>
        /// <param name="hostType">The type of host used to create the listener.</param>
        [Theory]
        [MemberData(nameof(HostTypes))]
        public void ExceptionForEndpointNotFound(string hostType)
        {
            this.Listener = this.CreateListener(TestMocksRepository.GetMockStatelessServiceContext(), "NoEndPoint", hostType);
            this.ExceptionForEndpointNotFoundVerifier();
        }

        /// <summary>
        /// ArgumentException is thrown when endpointName is null or empty string.
        /// </summary>
        /// <param name="hostType">The type of host used to create the listener.</param>
        [Theory]
        [MemberData(nameof(HostTypes))]
        public void VerifyExceptionForNullOrEmptyEndpointName(string hostType)
        {
            Action action =
                () =>
                    this.CreateListener(
                        TestMocksRepository.GetMockStatelessServiceContext(),
                        null,
                        hostType);
            Assert.Throws<ArgumentException>(action);

            action =
                () =>
                    this.CreateListener(
                        TestMocksRepository.GetMockStatelessServiceContext(),
                        string.Empty,
                        hostType);
            Assert.Throws<ArgumentException>(action);
        }

        private HttpSysCommunicationListener CreateListener(StatelessServiceContext context, string endpointName, string hostType)
        {
            if (hostType == "WebHost")
            {
                return new HttpSysCommunicationListener(context, endpointName, (uri, listen) => this.IWebHostBuildFunc(uri, listen));
            }
            else
            {
                return new HttpSysCommunicationListener(context, endpointName, (uri, listen) => this.IHostBuildFunc(uri, listen));
            }
        }
    }
}
