// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Remoting.Base.V2
{
    using System;
    using System.Runtime.Serialization;

    [DataContract(Name = "WrappedMsgBody", Namespace = Constants.ServiceCommunicationNamespace)]
    internal class WrappedRemotingMessageBody : WrappedMessage, IServiceRemotingRequestMessageBody, IServiceRemotingResponseMessageBody
    {
        public void SetParameter(
            int position,
            string parameName,
            object parameter)
        {
            throw new NotImplementedException();
        }

        public object GetParameter(
            int position,
            string parameName,
            Type paramType)
        {
            throw new NotImplementedException();
        }

        public void Set(
            object response)
        {
            throw new NotImplementedException();
        }

        public object Get(
            Type paramType)
        {
            throw new NotImplementedException();
        }
    }
}
