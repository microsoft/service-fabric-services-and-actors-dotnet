// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Tests.ExceptionConvertors
{
    using System;
    using System.Collections.Generic;
    using System.Fabric;
    using System.Threading.Tasks;
    using Microsoft.ServiceFabric.Actors.Runtime;
    using Microsoft.ServiceFabric.Services.Remoting.FabricTransport;
    using Microsoft.ServiceFabric.Services.Remoting.FabricTransport.Runtime;
    using Microsoft.ServiceFabric.Services.Remoting.V2.Messaging;
    using Xunit;

    /// <summary>
    /// FabricActorExceptionConvertor test.
    /// </summary>
    public class FabricActorExceptionTest
    {
        private static List<Services.Remoting.V2.Runtime.IExceptionConvertor> runtimeConvertors
            = new List<Services.Remoting.V2.Runtime.IExceptionConvertor>()
            {
                new Actors.Runtime.FabricActorExceptionConvertor(),
                new Services.Remoting.V2.Runtime.ExceptionConversionHandler.DefaultExceptionConvertor(),
            };

        private static Services.Remoting.V2.Runtime.ExceptionConversionHandler runtimeHandler
            = new Services.Remoting.V2.Runtime.ExceptionConversionHandler(runtimeConvertors, 
                new FabricTransportRemotingListenerSettings { RemotingExceptionDepth = 2 });

        private static List<Services.Remoting.V2.Client.IExceptionConvertor> clientConvertors
            = new List<Services.Remoting.V2.Client.IExceptionConvertor>()
            {
                new Actors.Client.FabricActorExceptionConvertor(),
            };

        private static Services.Remoting.V2.Client.ExceptionConversionHandler clientHandler
            = new Services.Remoting.V2.Client.ExceptionConversionHandler(
                clientConvertors,
                new FabricTransportRemotingSettings()
                {
                    ExceptionDeserializationTechnique = FabricTransportRemotingSettings.ExceptionDeserialization.Default,
                });

        private static List<FabricException> fabricExceptions = new List<FabricException>()
        {
            new DuplicateMessageException("DuplicateMessageException"),
            new InvalidReentrantCallException("InvalidReentrantCallException"),
            new ReminderNotFoundException("ReminderNotFoundException"),
            new ReentrancyModeDisallowedException("ReentrancyModeDisallowedException"),
            new ReentrantActorInvalidStateException("ReentrantActorInvalidStateException"),
            new ActorConcurrencyLockTimeoutException("ActorConcurrencyLockTimeoutException"),
            new ActorDeletedException("ActorDeletedException"),
            new ReminderLoadInProgressException("ReminderLoadInProgressException"),
        };

        /// <summary>
        /// Known types test.
        /// </summary>
        /// <returns>Task representing async operation.</returns>
        [Fact]
        public static async Task KnownFabricActorExceptionSerializationTest()
        {
            foreach (var exception in fabricExceptions)
            {
                var serializedData = runtimeHandler.SerializeRemoteException(exception);
                var msgStream = new SegmentedReadMemoryStream(serializedData);

                Exception resultFabricEx = null;
                try
                {
                    await clientHandler.DeserializeRemoteExceptionAndThrowAsync(msgStream);
                }
                catch (AggregateException ex)
                {
                    resultFabricEx = ex.InnerException;
                }

                Assert.True(resultFabricEx != null);
                Assert.Equal(resultFabricEx.GetType(), exception.GetType());
                Assert.Equal(resultFabricEx.Message, exception.Message);
                Assert.Equal(resultFabricEx.HResult, exception.HResult);
            }
        }
    }
}
