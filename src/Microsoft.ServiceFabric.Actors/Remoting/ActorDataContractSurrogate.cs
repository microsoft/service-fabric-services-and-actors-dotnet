// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Remoting
{
    using System;
    using System.CodeDom;
    using System.Collections.ObjectModel;
    using System.Reflection;
    using System.Runtime.Serialization;

#if DotNetCoreClr
    internal class ActorDataContractSurrogate : ISerializationSurrogateProvider
    {
        public static readonly ISerializationSurrogateProvider Singleton = new ActorDataContractSurrogate();
        public Type GetSurrogateType(Type type)
        {
            if (typeof(IActor).IsAssignableFrom(type))
            {
                return typeof(ActorReference);
            }

            return type;
        }

        public object GetObjectToSerialize(object obj, Type targetType)
        {
            if (obj == null)
            {
                return null;
            }
            else if (obj is IActor)
            {
                return ActorReference.Get(obj);
            }

            return obj;
        }

        public object GetDeserializedObject(object obj, Type targetType)
        {
            if (obj == null)
            {
                return null;
            }
            else if (obj is IActorReference && typeof(IActor).IsAssignableFrom(targetType) &&
                     !typeof(IActorReference).IsAssignableFrom(targetType))
            {
                return ((IActorReference)obj).Bind(targetType);
            }
            return obj;
        }
    }

#else
    internal class ActorDataContractSurrogate : IDataContractSurrogate
    {
        public static readonly IDataContractSurrogate Singleton = new ActorDataContractSurrogate();

        public Type GetDataContractType(Type type)
        {
            if (typeof(IActor).IsAssignableFrom(type))
            {
                return typeof(ActorReference);
            }

            return type;
        }

        public object GetObjectToSerialize(object obj, Type targetType)
        {
            if (obj == null)
            {
                return null;
            }
            else if (obj is IActor)
            {
                return ActorReference.Get(obj);
            }

            return obj;
        }

        public object GetDeserializedObject(object obj, Type targetType)
        {
            if (obj == null)
            {
                return null;
            }
            else if (obj is IActorReference && typeof(IActor).IsAssignableFrom(targetType) &&
                     !typeof(IActorReference).IsAssignableFrom(targetType))
            {
                return ((IActorReference)obj).Bind(targetType);
            }


            return obj;
        }

        public object GetCustomDataToExport(Type clrType, Type dataContractType)
        {
            throw new NotImplementedException();
        }

        public object GetCustomDataToExport(MemberInfo memberInfo, Type dataContractType)
        {
            throw new NotImplementedException();
        }

        public void GetKnownCustomDataTypes(Collection<Type> customDataTypes)
        {
        }

        public Type GetReferencedTypeOnImport(string typeName, string typeNamespace, object customData)
        {
            throw new NotImplementedException();
        }

        public CodeTypeDeclaration ProcessImportedType(CodeTypeDeclaration typeDeclaration, CodeCompileUnit compileUnit)
        {
            throw new NotImplementedException();
        }
    }
#endif
}
