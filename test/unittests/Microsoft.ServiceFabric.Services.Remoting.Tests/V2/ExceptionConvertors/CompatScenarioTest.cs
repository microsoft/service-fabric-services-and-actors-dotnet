// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Remoting.Tests.V2.ExceptionConvertors
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.ServiceFabric.Services.Remoting.FabricTransport;
    using Microsoft.ServiceFabric.Services.Remoting.FabricTransport.Runtime;
    using Microsoft.ServiceFabric.Services.Remoting.V2;
    using Microsoft.ServiceFabric.Services.Remoting.V2.Messaging;
    using Xunit;

    /// <summary>
    /// Compat scenario tests.
    /// </summary>
    [Obsolete(DeprecationMessage.RemotingV1)]
    public class CompatScenarioTest
    {
        private static List<Remoting.V2.Runtime.IExceptionConvertor> runtimeConvertors
           = new List<Remoting.V2.Runtime.IExceptionConvertor>()
           {
                new Remoting.V2.Runtime.FabricExceptionConvertor(),
                new Remoting.V2.Runtime.SystemExceptionConvertor(),
                new Remoting.V2.Runtime.ExceptionConversionHandler.DefaultExceptionConvertor(),
           };

        private static Remoting.V2.Runtime.ExceptionConversionHandler runtimeHandler
            = new Remoting.V2.Runtime.ExceptionConversionHandler(runtimeConvertors,
                new FabricTransportRemotingListenerSettings { ExceptionSerializationTechnique = FabricTransportRemotingListenerSettings.ExceptionSerialization.BinaryFormatter });

        private static List<Remoting.V2.Client.IExceptionConvertor> clientConvertors
            = new List<Remoting.V2.Client.IExceptionConvertor>()
            {
                new Remoting.V2.Client.SystemExceptionConvertor(),
                new Remoting.V2.Client.FabricExceptionConvertor(),
            };

        private static Remoting.V2.Client.ExceptionConversionHandler clientHandler
            = new Remoting.V2.Client.ExceptionConversionHandler(
                clientConvertors,
                FabricTransportRemotingSettings.GetDefault());

        /// <summary>
        /// Old client and new service test.
        /// </summary>
        [Fact]
        public static void OldClientTest()
        {
            var exception = new ArgumentException("My arg is invalid");
            var serializedData = runtimeHandler.SerializeRemoteException(exception);
            var msgStream = new SegmentedReadMemoryStream(serializedData);

            var isDes = RemoteException.ToException(msgStream, out var resultEx);
            Assert.True(resultEx != null);
            Assert.Equal(resultEx.GetType(), exception.GetType());
            Assert.Equal(resultEx.Message, exception.Message);
            Assert.Equal(resultEx.HResult, exception.HResult);
        }

        /// <summary>
        /// New client and old service test.
        /// </summary>
        /// <returns>Task representing async operation.</returns>
        [Fact]
        public static async Task OldServerTest()
        {
            var exception = new ArgumentException("My arg is invalid");
            var serializedData = RemoteException.FromException(exception).Data;
            var msgStream = new SegmentedReadMemoryStream(serializedData);

            Exception resultEx = null;
            try
            {
                await clientHandler.DeserializeRemoteExceptionAndThrowAsync(msgStream);
            }
            catch (AggregateException ex)
            {
                resultEx = ex.InnerException;
            }

            Assert.True(resultEx != null);
            Assert.Equal(resultEx.GetType(), exception.GetType());
            Assert.Equal(resultEx.Message, exception.Message);
            Assert.Equal(resultEx.HResult, exception.HResult);
        }
    }
}
