// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using Microsoft.ServiceFabric.Services.Remoting.FabricTransport.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.V2.Runtime;

namespace Microsoft.ServiceFabric.Services.Remoting.V2.Wcf.Runtime
{
    /// <summary>
    /// Settings that configures the Wcf Remoting Listener.
    /// </summary>
    public class WcfRemotingListenerSettings : IExceptionSerializerSettings
    {
        private int remotingExceptionDepth;

        /// <summary>
        /// Initializes a new instance of the <see cref="WcfRemotingListenerSettings"/> class with default values.
        /// </summary>
        public WcfRemotingListenerSettings()
        {
            this.remotingExceptionDepth = ExceptionConversionHandler.DefaultRemotingExceptionDepth;
        }

        /// <summary>
        /// Gets or sets the exception serialization technique.
        /// </summary>
        /// <remarks>Applies only to V2 Remoting.</remarks>
        [Obsolete(DeprecationMessage.RemotingV1)]
        public FabricTransportRemotingListenerSettings.ExceptionSerialization ExceptionSerializationTechnique { get; set; }


        /// <summary>
        /// Gets or sets the depth of exceptions to be sent to the client in case of remoting call failing with exception.
        /// </summary>
        /// <remarks>The allowed values are greater than or equal to 1. If the supplied value is less than 1, then the field is set to int.MaxValue.</remarks>
        public int RemotingExceptionDepth
        {
            get
            {
                return this.remotingExceptionDepth;
            }

            set
            {
                if (value <= 0)
                {
                    this.remotingExceptionDepth = int.MaxValue;
                }
                else
                {
                    this.remotingExceptionDepth = value;
                }
            }
        }
    }
}
