namespace Microsoft.ServiceFabric.Services.Remoting.V2.Builder
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using Microsoft.ServiceFabric.Services.Remoting.Description;

    class InterfaceDetailsStore
    {
        private readonly ConcurrentDictionary<int, InterfaceDetails> knownTypesMap =
            new ConcurrentDictionary<int, InterfaceDetails>();

        private readonly ConcurrentDictionary<string, int> interfaceIdMapping =
            new ConcurrentDictionary<string, int>();

        public bool TryGetKnownTypes(int interfaceId, out InterfaceDetails interfaceDetails)
        {
            return this.knownTypesMap.TryGetValue(interfaceId, out interfaceDetails);
        }

        public bool TryGetKnownTypes(string interfaceName, out InterfaceDetails interfaceDetails)
        {
            int interfaceId;
            if (!this.interfaceIdMapping.TryGetValue(interfaceName, out interfaceId))
            {
                ServiceTrace.Source.WriteInfo(TraceType,"InterfaceName {0} not found ",interfaceName);
                interfaceDetails = null;
                return false;
            }
            return this.knownTypesMap.TryGetValue(interfaceId, out interfaceDetails);
        }

        private const string TraceType = "InterfaceDetailsStore";

        public void UpdateKnownTypesDetails(IEnumerable<InterfaceDescription> interfaceDescriptions)
        {
            foreach (var interfaceDescription in interfaceDescriptions)
            {
                this.UpdateKnownTypeDetail(interfaceDescription);
            }
        }

        public void UpdateKnownTypeDetail(InterfaceDescription interfaceDescription)
        {
            var responseKnownTypes = new List<Type>();
            var requestKnownType = new List<Type>();
            foreach (var entry in interfaceDescription.Methods)
            {
                if (TypeUtility.IsTaskType(entry.ReturnType) && entry.ReturnType.GetTypeInfo().IsGenericType)
                {
                    var returnType = entry.MethodInfo.ReturnType.GetGenericArguments()[0];
                    if (!responseKnownTypes.Contains(returnType))
                    {
                        responseKnownTypes.Add(returnType);
                    }
                }
                    
                requestKnownType.AddRange(entry.MethodInfo.GetParameters()
                    .ToList()
                    .Select(p => p.ParameterType)
                    .Except(requestKnownType));
            }
            var knownType = new InterfaceDetails();
            knownType.Id = interfaceDescription.Id;
            knownType.ServiceInterfaceType = interfaceDescription.InterfaceType;
            knownType.RequestKnownTypes = requestKnownType;
            knownType.ResponseKnownTypes = responseKnownTypes;
            knownType.MethodNames = interfaceDescription.Methods.ToDictionary(item => item.Name, item => item.Id);

            this.UpdateKnownTypes(interfaceDescription.Id, interfaceDescription.InterfaceType.FullName,
                knownType);
        }

        private void UpdateKnownTypes(int interfaceId, string interfaceName,
            InterfaceDetails knownTypes)
        {
            if (this.knownTypesMap.ContainsKey(interfaceId))
            {
                ServiceTrace.Source.WriteInfo(TraceType, "InterfaceId {0} and InterfaceName {1} already existing ", interfaceId,interfaceName);
                return;
            }

            if (this.knownTypesMap.TryAdd(interfaceId, knownTypes))
            {
                this.interfaceIdMapping.TryAdd(interfaceName, interfaceId);
            }
            
        }
    }
}
