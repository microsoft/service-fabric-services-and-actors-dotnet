// ------------------------------------------------------------
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT License (MIT).See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Runtime
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.ServiceFabric.Services.Remoting;
    using Microsoft.ServiceFabric.Services.Remoting.Builder;
    using Microsoft.ServiceFabric.Services.Remoting.V2;

    internal class DummyServiceRemoingRequestMessageBody : IServiceRemotingRequestMessageBody
    {
        public void SetParameter(int position, string parameName, object parameter)
        {
            // no-op
        }

        public object GetParameter(int position, string parameName, Type paramType)
        {
            throw new NotImplementedException();
        }
    }
}
