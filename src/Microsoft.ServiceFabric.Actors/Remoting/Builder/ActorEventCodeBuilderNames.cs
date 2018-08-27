// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Remoting.Builder
{
    using Microsoft.ServiceFabric.Services.Remoting.Base.Builder;

    internal class ActorEventCodeBuilderNames
        : CodeBuilderNames
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
