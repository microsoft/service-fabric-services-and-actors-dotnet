// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------
namespace Microsoft.ServiceFabric.Services.Communication.Wcf.Runtime
{
    using System;
    using System.ServiceModel.Description;
    using System.ServiceModel.Dispatcher;

    internal class WcfGlobalErrorHandlerBehaviorAttribute : Attribute, IServiceBehavior
    {
        public void AddBindingParameters(
            ServiceDescription serviceDescription, System.ServiceModel.ServiceHostBase serviceHostBase,
            System.Collections.ObjectModel.Collection<ServiceEndpoint> endpoints, System.ServiceModel.Channels.BindingParameterCollection bindingParameters)
        {
            // Nothing to do as of now.
        }

        public void ApplyDispatchBehavior(ServiceDescription serviceDescription, System.ServiceModel.ServiceHostBase serviceHostBase)
        {
            foreach (var channelDispBase in serviceHostBase.ChannelDispatchers)
            {
                var channelDisp = channelDispBase as ChannelDispatcher;

                if (channelDisp != null)
                {
                    var wcfErrorHandler = new WcfGlobalErrorHandler(channelDisp);
                    channelDisp.ErrorHandlers.Add(wcfErrorHandler);
                }
            }
        }

        public void Validate(ServiceDescription serviceDescription, System.ServiceModel.ServiceHostBase serviceHostBase)
        {
            // Nothing to do as of now.
        }
    }
}