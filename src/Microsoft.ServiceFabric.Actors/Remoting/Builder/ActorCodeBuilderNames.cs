// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------
namespace Microsoft.ServiceFabric.Actors.Remoting.Builder
{
    internal class ActorCodeBuilderNames
        : Microsoft.ServiceFabric.Services.Remoting.Builder.CodeBuilderNames
    {
        public ActorCodeBuilderNames()
            : base("actor")
        {
        }

        public override string GetDataContractNamespace()
        {
            return Constants.Namespace;
        }
    }
}