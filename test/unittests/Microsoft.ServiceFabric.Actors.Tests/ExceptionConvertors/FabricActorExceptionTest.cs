// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Tests.ExceptionConvertors
{
    using System;
    using System.Collections.Generic;
    using System.Fabric;
    using Microsoft.ServiceFabric.Actors.Runtime;
    using Microsoft.ServiceFabric.Services.Communication;
    using Microsoft.ServiceFabric.Services.Remoting.FabricTransport;
    using Microsoft.ServiceFabric.Services.Remoting.FabricTransport.Runtime;
    using Microsoft.ServiceFabric.Services.Remoting.V2;
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
                new Services.Remoting.V2.Runtime.ExceptionConvertorHelper.DefaultExceptionConvetor(),
            };

        private static Services.Remoting.V2.Runtime.ExceptionConvertorHelper runtimeHelper
            = new Services.Remoting.V2.Runtime.ExceptionConvertorHelper(runtimeConvertors, new FabricTransportRemotingListenerSettings()
            {
                RemotingExceptionDepth = 2,
                ExceptionSerializationTechnique = FabricTransportRemotingListenerSettings.ExceptionSerialization.Default,
            });

        private static List<Services.Remoting.V2.Client.IExceptionConvertor> clientConvertors
            = new List<Services.Remoting.V2.Client.IExceptionConvertor>()
            {
                new Actors.Client.FabricActorExceptionConvertor(),
            };

        private static Services.Remoting.V2.Client.ExceptionConvertorHelper clientHelper
            = new Services.Remoting.V2.Client.ExceptionConvertorHelper(
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
        [Fact]
        public static void KnownFabricActorExceptionSerializationTest()
        {
            foreach (var exception in fabricExceptions)
            {
                var serializedData = runtimeHelper.SerializeRemoteException(exception);
                var msgStream = new SegmentedReadMemoryStream(serializedData);

                Exception resultFabricEx = null;
                try
                {
                    clientHelper.DeserializeRemoteExceptionAndThrow(msgStream);
                }
                catch (Exception ex)
                {
                    resultFabricEx = ex;
                }

                Assert.True(resultFabricEx != null);
                Assert.Equal(resultFabricEx.GetType(), exception.GetType());
                Assert.Equal(resultFabricEx.Message, exception.Message);
                Assert.Equal(resultFabricEx.HResult, exception.HResult);
            }
        }
    }
}
