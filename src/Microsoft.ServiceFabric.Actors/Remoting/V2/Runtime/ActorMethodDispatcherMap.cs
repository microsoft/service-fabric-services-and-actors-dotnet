// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------
namespace Microsoft.ServiceFabric.Actors.Remoting.V2.Runtime
{
    using System.Collections.Generic;
    using System.Globalization;
    using Microsoft.ServiceFabric.Actors.Remoting.V2.Builder;
    using Microsoft.ServiceFabric.Actors.Runtime;

    class ActorMethodDispatcherMap
    {
        private readonly IDictionary<int, ActorMethodDispatcherBase> map;

        public ActorMethodDispatcherMap(ActorTypeInformation actorTypeInformation)
        {
            this.map = new Dictionary<int, ActorMethodDispatcherBase>();

            foreach (var actorInterfaceType in actorTypeInformation.InterfaceTypes)
            {
                var methodDispatcher = ActorCodeBuilder.GetOrCreateMethodDispatcher(actorInterfaceType);
                this.map.Add(methodDispatcher.InterfaceId, methodDispatcher);
            }
        }

        public ActorMethodDispatcherBase GetDispatcher(int interfaceId, int methodId)
        {
            ActorMethodDispatcherBase methodDispatcher;
            if (!this.map.TryGetValue(interfaceId, out methodDispatcher))
            {
                throw new KeyNotFoundException(string.Format(CultureInfo.CurrentCulture,
                    SR.ErrorMethodDispatcherNotFound, interfaceId));
            }

            return methodDispatcher;
        }

    }
}
