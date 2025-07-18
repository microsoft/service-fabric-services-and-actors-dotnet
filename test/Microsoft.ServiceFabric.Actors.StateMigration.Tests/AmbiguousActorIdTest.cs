// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.StateMigration.Tests
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.ServiceFabric.Actors.KVSToRCMigration;
    using Microsoft.ServiceFabric.Actors.Migration.Exceptions;
    using Microsoft.ServiceFabric.Actors.Runtime.Migration;
    using Microsoft.ServiceFabric.Actors.StateMigration.Tests.MockTypes;
    using Xunit;
    using static Microsoft.ServiceFabric.Actors.KVSToRCMigration.IAmbiguousActorIdHandler;

    /// <summary>
    /// Ambiguous actor id tests.
    /// </summary>
    public class AmbiguousActorIdTest
    {
        /// <summary>
        /// Test non ambiguous actors scenario.
        /// </summary>
        /// <returns>Task for async operation.</returns>
        [Fact]
        public async Task NonAmbiguousActorIds()
        {
            var sp = new MockReliableCollectionsStateProvider();
            var presenceDict = sp.GetActorPresenceDictionary();
            await presenceDict.AddAsync(null, "String_MyActor1_", new byte[0]);
            await presenceDict.AddAsync(null, "String_MyActor2_", new byte[0]);
            await presenceDict.AddAsync(null, "String_MyActor3_MyEx1_", new byte[0]);
            await presenceDict.AddAsync(null, "String_MyActor4_MyEx1_MyEx2_", new byte[0]);

            var handler = new RCAmbiguousActorIdHandler(sp);
            Assert.Equal(await handler.ResolveActorIdAsync("String_MyActor1_MyState1", null, CancellationToken.None), new ActorId("MyActor1"));
            Assert.Equal(await handler.ResolveActorIdAsync("String_MyActor3_MyEx1_MyState1", null, CancellationToken.None), new ActorId("MyActor3_MyEx1"));
            Assert.Equal(await handler.ResolveActorIdAsync("Long_1234_MyState", null, CancellationToken.None), new ActorId(1234));
        }

        /// <summary>
        /// Test ambiguous actors scenario.
        /// </summary>
        /// <returns>Task for async operation.</returns>
        [Fact]
        public async Task AmbiguousActorIds()
        {
            var sp = new MockReliableCollectionsStateProvider();
            var presenceDict = sp.GetActorPresenceDictionary();
            await presenceDict.AddAsync(null, "String_MyActor1_", new byte[0]);
            await presenceDict.AddAsync(null, "String_MyActor1_MyEx1_", new byte[0]);

            var handler = new RCAmbiguousActorIdHandler(sp);
            await Assert.ThrowsAsync<AmbiguousActorIdDetectedException>(() => handler.ResolveActorIdAsync("String_MyActor1_MyEx1_MyState1", null, CancellationToken.None));
        }

        /// <summary>
        /// Test ambiguous actors scenario with resolvers
        /// </summary>
        /// <returns>Task for async operation.</returns>
       // [Fact]
        internal async Task AmbiguousActorIdsWithResolvers()
        {
            var sp = new MockReliableCollectionsStateProvider();
            var presenceDict = sp.GetActorPresenceDictionary();
            await presenceDict.AddAsync(null, "String_MyActor1_", new byte[0]);
            await presenceDict.AddAsync(null, "String_MyActor1_MyEx1_", new byte[0]);

            var handler = new RCAmbiguousActorIdHandler(sp);
            Assert.Equal(await handler.ResolveActorIdAsync("String_MyActor1_MyEx1_MyState1", null, CancellationToken.None), new ActorId("MyActor1_MyEx1"));
        }

        /// <summary>
        /// Actor Id resolver
        /// </summary>
        [AmbiguousActorIdResolverAttribute]
        public class ActorIdResolver : IAmbiguousActorIdHandler
        {
            /// <inheritdoc/>
            public Task<IAmbiguousActorIdHandler.ConditionalValue> TryResolveActorIdAsync(string stateStorageKey, CancellationToken cancellationToken)
            {
                if (stateStorageKey == "MyActor1_MyEx1_MyState1")
                {
                    return Task.FromResult(new ConditionalValue
                    {
                        HasValue = true,
                        Value = "MyActor1_MyEx1",
                    });
                }

                return Task.FromResult(new ConditionalValue
                {
                    HasValue = false,
                    Value = null,
                });
            }
        }
    }
}
