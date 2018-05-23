// ------------------------------------------------------------
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT License (MIT).See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Remoting.Builder
{
    internal class ActorEventCodeBuilderNames
        : Microsoft.ServiceFabric.Services.Remoting.Builder.CodeBuilderNames
    {
        public ActorEventCodeBuilderNames()
            : base("actorEvent")
        {
        }

        public ActorEventCodeBuilderNames(string prefix)
            : base("actorEvent" + prefix)
        {
        }

        public override string GetDataContractNamespace()
        {
            return Constants.Namespace;
        }
    }
}
