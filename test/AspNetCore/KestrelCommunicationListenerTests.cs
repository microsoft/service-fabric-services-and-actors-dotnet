// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Fabric;
using FluentAssertions;
using Microsoft.ServiceFabric.Services.Communication.AspNetCore;
using Xunit;

namespace Microsoft.ServiceFabric.AspNetCore.Tests
{
    /// <summary>
    /// Test class for KestrelCommunicationListener.
    /// </summary>
    public class KestrelCommunicationListenerTests : AspNetCoreCommunicationListenerTests
    {
        // 1. When no endpointRef is provided:
        //   a. url given to Func to create IHost should be http://+:0
        //   b. url returned from OpenAsync should be http://IPAddressOrFQDN:0/PartitionId/ReplicaId
        //
        //

        /// <summary>
        /// Tests Url for ServiceFabricIntegrationOptions.UseUniqueServiceUrl
        /// 1. When endpoint name is provided (protocol and port comes from endpoint.) :
        ///   a. url given to Func to create IWebHost/IHost should be protocol://+:port.
        ///   b. url returned from OpenAsync should be protocol://IPAddressOrFQDN:port/PartitionId/ReplicaId.
        ///
        /// </summary>
        [Fact]
        public void VerifyWithUseUniqueServiceUrlOption()
        {
            var context = TestMocksRepository.GetMockStatelessServiceContext();
            context.CodePackageActivationContext.GetEndpoints().Add(this.GetTestEndpoint());
            this.Listener = this.CreateListener(context, EndpointName);
            this.UseUniqueServiceUrlOptionVerifier();
        }

        /// <summary>
        /// Tests Url for ServiceFabricIntegrationOptions.None
        /// 1. When endpoint name is provided (protocol and port comes from endpoint.) :
        ///   a. url given to Func to create IWebHost/IHost should be protocol://+:port.
        ///   b. url returned from OpenAsync should be protocol://IPAddressOrFQDN:port.
        ///
        /// </summary>
        [Fact]
        public void VerifyWithoutUseUniqueServiceUrlOption()
        {
            var context = TestMocksRepository.GetMockStatelessServiceContext();
            context.CodePackageActivationContext.GetEndpoints().Add(this.GetTestEndpoint());
            this.Listener = this.CreateListener(context, EndpointName);
            this.WithoutUseUniqueServiceUrlOptionVerifier();
        }

        /// <summary>
        /// Verify Listener Open and Close.
        /// </summary>
        [Fact]
        public void VerifyListenerOpenClose()
        {
            var context = TestMocksRepository.GetMockStatelessServiceContext();
            context.CodePackageActivationContext.GetEndpoints().Add(this.GetTestEndpoint());
            this.Listener = this.CreateListener(context, EndpointName);
            this.ListenerOpenCloseVerifier();
        }

        /// <summary>
        /// InvalidOperationEXception is thrown when Endpoint is not found in service manifest.
        /// </summary>
        [Fact]
        public void ExceptionForEndpointNotFound()
        {
            this.Listener = this.CreateListener(TestMocksRepository.GetMockStatelessServiceContext(), "NoEndPoint");
            this.ExceptionForEndpointNotFoundVerifier();
        }

        /// <summary>
        /// ArgumentException is thrown when endpointName is empty string.
        /// </summary>
        [Fact]
        public void VerifyExceptionForEmptyEndpointName()
        {
            Action action =
                () =>
                    this.CreateListener(
                        TestMocksRepository.GetMockStatelessServiceContext(),
                        string.Empty);
            action.Should().Throw<ArgumentException>();
        }

        private KestrelCommunicationListener CreateListener(StatelessServiceContext context, string endpointName)
        {
            return new KestrelCommunicationListener(context, endpointName, (uri, listen) => this.IHostBuildFunc(uri, listen));
        }
    }
}
