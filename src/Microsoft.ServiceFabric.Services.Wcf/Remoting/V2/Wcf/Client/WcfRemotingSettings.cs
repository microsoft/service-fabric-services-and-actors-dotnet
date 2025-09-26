// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using Microsoft.ServiceFabric.Services.Remoting.V2.Runtime;

namespace Microsoft.ServiceFabric.Services.Remoting.V2.Wcf.Client
{
    /// <summary>
    /// Settings that configures the Wcf Remoting.
    /// </summary>
    public class WcfRemotingSettings : IExceptionDeserializerSettings
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WcfRemotingSettings"/> class with default values.
        /// </summary>
        public WcfRemotingSettings()
        {
#pragma warning disable 618            
            this.ExceptionDeserializationTechnique = ExceptionDeserialization.Fallback;
#pragma warning restore 618
        }

        /// <summary>
        /// Gets or sets the exception deserialization technique to use.
        /// </summary>
        [Obsolete(DeprecationMessage.RemotingV1)]
        public ExceptionDeserialization ExceptionDeserializationTechnique  { get; set; }
    }
}