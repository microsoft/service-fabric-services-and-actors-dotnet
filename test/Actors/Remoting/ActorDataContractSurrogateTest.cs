// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
#if NETFRAMEWORK
using System.CodeDom;
using System.Collections.ObjectModel;
using System.Reflection;
#endif
using System.Runtime.Serialization;
using Fuzzy;
using Microsoft.ServiceFabric.Actors.Client;
using Microsoft.ServiceFabric.Actors.Remoting.V2.Client;
using Microsoft.ServiceFabric.Actors.Tests;
using Moq;
using Xunit;

namespace Microsoft.ServiceFabric.Actors.Remoting;

public abstract class ActorDataContractSurrogateTest
{
#if NET
    readonly ISerializationSurrogateProvider sut = new ActorDataContractSurrogate();
#else
    readonly IDataContractSurrogate sut = new ActorDataContractSurrogate();
#endif

    static readonly IFuzz fuzzy = new RandomFuzz(Environment.TickCount);

#if NETFRAMEWORK
    public sealed class GetCustomDataToExport_MemberInfo_Type : ActorDataContractSurrogateTest
    {
        readonly MemberInfo memberInfo = typeof(object).GetMethod(nameof(object.ToString));
        readonly Type dataContractType = fuzzy.Type();

        [Fact]
        public void ThrowsNotImplementedException() =>
            _ = Assert.Throws<NotImplementedException>(() => sut.GetCustomDataToExport(memberInfo, dataContractType));
    }

    public sealed class GetCustomDataToExport_Type_Type : ActorDataContractSurrogateTest
    {
        readonly Type clrType = fuzzy.Type();
        readonly Type dataContractType = fuzzy.Type();

        [Fact]
        public void ThrowsNotImplementedException() =>
            _ = Assert.Throws<NotImplementedException>(() => sut.GetCustomDataToExport(clrType, dataContractType));
    }

    public sealed class GetDataContractType : ActorDataContractSurrogateTest
    {
        Type type;

        [Fact]
        public void ReturnsActorReferenceWhenTypeImplementsIActor()
        {
            type = typeof(IFactoryTestActor);
            Assert.Same(typeof(ActorReference), sut.GetDataContractType(type));
        }

        [Fact]
        public void ReturnsInputTypeWhenItDoesNotImplementIActor()
        {
            type = typeof(string);
            Assert.Same(typeof(string), sut.GetDataContractType(type));
        }
    }
#endif

    public sealed class GetDeserializedObject : ActorDataContractSurrogateTest
    {
        object obj;
        Type targetType;

        [Fact]
        public void ReturnsNullWhenObjIsNull()
        {
            obj = null;
            targetType = fuzzy.Type();
            Assert.Null(sut.GetDeserializedObject(obj, targetType));
        }

        [Fact]
        public void ReturnsBindResultWhenObjImplementsIActorReferenceAndTargetTypeImplementsIActor()
        {
            var bound = new object();
            var reference = new Mock<IActorReference>();
            _ = reference.Setup(_ => _.Bind(typeof(IFactoryTestActor))).Returns(bound);
            obj = reference.Object;
            targetType = typeof(IFactoryTestActor);

            object result = sut.GetDeserializedObject(obj, targetType);

            Assert.Same(bound, result);
        }

        [Fact]
        public void ReturnsObjWhenTargetTypeImplementsIActorReference()
        {
            obj = Mock.Of<IActorReference>();
            targetType = typeof(IActorAndReference);
            Assert.Same(obj, sut.GetDeserializedObject(obj, targetType));
        }

        [Fact]
        public void ReturnsObjWhenTargetTypeDoesNotImplementIActor()
        {
            obj = Mock.Of<IActorReference>();
            targetType = typeof(object);
            Assert.Same(obj, sut.GetDeserializedObject(obj, targetType));
        }

        [Fact]
        public void ReturnsObjWhenItDoesNotImplementIActorReference()
        {
            obj = new object();
            targetType = fuzzy.Type();
            Assert.Same(obj, sut.GetDeserializedObject(obj, targetType));
        }

        interface IActorAndReference : IActor, IActorReference { }
    }

#if NETFRAMEWORK
    public sealed class GetKnownCustomDataTypes : ActorDataContractSurrogateTest
    {
        // Method parameters
        readonly Collection<Type> customDataTypes;

        readonly Type expected = fuzzy.Type();

        public GetKnownCustomDataTypes() =>
            customDataTypes = new Collection<Type> { expected };

        [Fact]
        public void LeavesCollectionUnchanged()
        {
            sut.GetKnownCustomDataTypes(customDataTypes);
            Assert.Single(customDataTypes, expected);
        }
    }
#endif

    public sealed class GetObjectToSerialize : ActorDataContractSurrogateTest
    {
        object obj;
        Type targetType;

        [Fact]
        public void ReturnsNullWhenObjIsNull()
        {
            obj = null;
            targetType = fuzzy.Type();
            Assert.Null(sut.GetObjectToSerialize(obj, targetType));
        }

        [Fact]
        public void ReturnsActorReferenceWhenObjImplementsIActor()
        {
            ActorId actorId = fuzzy.ActorId();
            Uri serviceUri = fuzzy.Uri();
            string listenerName = fuzzy.String();
            var partitionClient = new Mock<IActorServicePartitionClient>();
            _ = partitionClient.SetupGet(_ => _.ServiceUri).Returns(serviceUri);
            _ = partitionClient.SetupGet(_ => _.ListenerName).Returns(listenerName);
            var actorProxy = new Mock<IActorProxy>();
            _ = actorProxy.SetupGet(_ => _.ActorId).Returns(actorId);
            _ = actorProxy.SetupGet(_ => _.ActorServicePartitionClientV2).Returns(partitionClient.Object);
            obj = actorProxy.As<IFactoryTestActor>().Object;
            targetType = fuzzy.Type();

            object result = sut.GetObjectToSerialize(obj, targetType);

            var reference = (ActorReference)result;
            Assert.Same(actorId, reference.ActorId);
            Assert.Same(serviceUri, reference.ServiceUri);
            Assert.Same(listenerName, reference.ListenerName);
        }

        [Fact]
        public void ReturnsObjUnchangedWhenItDoesNotImplementIActor()
        {
            obj = new object();
            targetType = fuzzy.Type();
            Assert.Same(obj, sut.GetObjectToSerialize(obj, targetType));
        }
    }

#if NETFRAMEWORK
    public sealed class GetReferencedTypeOnImport : ActorDataContractSurrogateTest
    {
        readonly string typeName = fuzzy.String();
        readonly string typeNamespace = fuzzy.String();
        readonly object customData = new();

        [Fact]
        public void ThrowsNotImplementedException() =>
            _ = Assert.Throws<NotImplementedException>(() => sut.GetReferencedTypeOnImport(typeName, typeNamespace, customData));
    }
#endif

#if NET
    public sealed class GetSurrogateType : ActorDataContractSurrogateTest
    {
        Type type;

        [Fact]
        public void ReturnsActorReferenceWhenTypeImplementsIActor()
        {
            type = typeof(IFactoryTestActor);
            Assert.Same(typeof(ActorReference), sut.GetSurrogateType(type));
        }

        [Fact]
        public void ReturnsInputTypeWhenItDoesNotImplementIActor()
        {
            type = typeof(string);
            Assert.Same(typeof(string), sut.GetSurrogateType(type));
        }
    }
#endif

    public sealed class Instance : ActorDataContractSurrogateTest
    {
        [Fact]
        public void IsActorDataContractSurrogate() =>
            Assert.Same(typeof(ActorDataContractSurrogate), ActorDataContractSurrogate.Instance.GetType());
    }

#if NETFRAMEWORK
    public sealed class ProcessImportedType : ActorDataContractSurrogateTest
    {
        readonly CodeTypeDeclaration typeDeclaration = new();
        readonly CodeCompileUnit compileUnit = new();

        [Fact]
        public void ThrowsNotImplementedException() =>
            _ = Assert.Throws<NotImplementedException>(() => sut.ProcessImportedType(typeDeclaration, compileUnit));
    }
#endif
}
