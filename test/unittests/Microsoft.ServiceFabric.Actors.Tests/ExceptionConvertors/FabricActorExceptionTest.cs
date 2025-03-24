// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Fabric;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.FabricTransport.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.V2.Messaging;
using Microsoft.ServiceFabric.Services.Remoting.V2.Runtime;
using Xunit;

namespace Microsoft.ServiceFabric.Actors.Tests.ExceptionConvertors
{
    public class FabricActorExceptionTest
    {
        static readonly IEnumerable<IExceptionConvertor> runtimeConvertors = new IExceptionConvertor[]
            {
                new FabricActorExceptionConvertor(),
                new DefaultExceptionConvertor(),
            };

        static readonly ExceptionSerializer serializer = new ExceptionSerializer(
            runtimeConvertors, new FabricTransportRemotingListenerSettings { RemotingExceptionDepth = 2 });

        static readonly IEnumerable<Services.Remoting.V2.Client.IExceptionConvertor> clientConvertors = new Services.Remoting.V2.Client.IExceptionConvertor[]
            {
                new Client.FabricActorExceptionConvertor(),
            };

        static readonly Services.Remoting.V2.Client.ExceptionDeserializer deserializer = 
            new Services.Remoting.V2.Client.ExceptionDeserializer(clientConvertors);

        static readonly IEnumerable<FabricException> fabricExceptions = new FabricException[]
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

        [Fact]
        public static async Task KnownFabricActorExceptionSerializationTest()
        {
            foreach (FabricException exception in fabricExceptions)
            {
                List<ArraySegment<byte>> serializedData = serializer.SerializeRemoteException(exception);
                using var stream = new SegmentedReadMemoryStream(serializedData);

                Exception actual = null;
                try
                {
                    await deserializer.DeserializeRemoteExceptionAndThrowAsync(stream);
                }
                catch (AggregateException ex)
                {
                    actual = ex.InnerException;
                }

                Assert.True(actual != null);
                Assert.Equal(actual.GetType(), exception.GetType());
                Assert.Equal(actual.Message, exception.Message);
                Assert.Equal(actual.HResult, exception.HResult);
            }
        }
    }
}
