// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Diagnostics
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using Microsoft.ServiceFabric.Actors.Runtime;
    using Microsoft.ServiceFabric.Services.Remoting.Description;
    using Xunit;

    public abstract class ActorMethodInfoUtilTest
    {
        internal readonly ActorMethodFriendlyNameBuilder nameBuilder;
        internal readonly ActorTypeInformation typeInfo;
        internal readonly IReadOnlyDictionary<long, ActorMethodInfo> actorMethodInfos;

        protected ActorMethodInfoUtilTest()
        {
            typeInfo = ActorTypeInformation.Get(typeof(TestActor));
            nameBuilder = new ActorMethodFriendlyNameBuilder(typeInfo);
            actorMethodInfos = ActorMethodInfoUtil.BuildActorMethodInfo(nameBuilder, typeInfo);
        }

        public sealed class BuildActorMethodInfo : ActorMethodInfoUtilTest
        {
            [Fact]
            public void ReturnsAllInterfaceMethods()
            {
                var expectedMethodCount = typeInfo.InterfaceTypes
                    .SelectMany(interfaceType =>
                    {
                        nameBuilder.GetActorInterfaceMethodDescriptionsV2(interfaceType, out _, out var methodDescriptions);
                        return methodDescriptions;
                    })
                    .Count();

                Assert.Equal(expectedMethodCount, actorMethodInfos.Count);
                Assert.IsType<ReadOnlyDictionary<long, ActorMethodInfo>>(actorMethodInfos);
            }

            [Fact]
            public void MapsKeysToFriendlyNamesAndSignatures()
            {
                foreach (var interfaceType in typeInfo.InterfaceTypes)
                {
                    nameBuilder.GetActorInterfaceMethodDescriptionsV2(interfaceType, out var interfaceId, out var methodDescriptions);

                    foreach (MethodDescription methodDescription in methodDescriptions)
                    {
                        var key = ActorMethodInfoUtil.GetInterfaceMethodKey((uint)interfaceId, (uint)methodDescription.Id);
                        Assert.True(actorMethodInfos.ContainsKey(key));

                        var methodInfo = methodDescription.MethodInfo;
                        var actorMethodInfo = actorMethodInfos[key];

                        Assert.Equal($"{methodInfo.DeclaringType.Name}.{methodInfo.Name}", actorMethodInfo.methodName);
                        Assert.Equal(methodInfo.ToString(), actorMethodInfo.methodSignature);
                    }
                }
            }
        }
    }
}
