// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Remoting.V2
{
    using System;

    /// <summary>
    /// Defines the interface that must be implemented to provide Request Message Body for remoting requests .
    /// This contains all the parameters remoting method has.
    /// </summary>
    public interface IServiceRemotingRequestMessageBody
    {
        /// <summary>
        /// This Api gets called to set remoting Method Parameters before serializing/dispatching the request.
        /// </summary>
        /// <param name="position">Position of the parameter in Remoting Method.</param>
        /// <param name="parameName">Parameter Name in the Remoting Method</param>
        /// <param name="parameter">Parameter Value</param>
        void SetParameter(int position, string parameName, object parameter);

        /// <summary>
        /// This is used to retrive Parameters from request Body Before dispatching to service remoting method.
        /// </summary>
        /// <param name="position">Position of the parameter in Remoting Method.</param>
        /// <param name="parameName">Parameter Name in the Remoting Method</param>
        /// <param name="paramType">Parameter Type</param>
        /// <returns></returns>
        object GetParameter(int position, string parameName, Type paramType);
    }
}
