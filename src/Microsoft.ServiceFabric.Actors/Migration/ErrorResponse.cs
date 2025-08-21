// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Migration
{
    using System.Fabric;
    using System.Runtime.Serialization;

    /// <summary>
    /// Migration custom http error reponse.
    /// </summary>
    [DataContract]
    [KnownType(typeof(FabricErrorCode))]
    public class ErrorResponse
    {
        /// <summary>
        /// Gets or sets the migration error message.
        /// </summary>
        [DataMember]
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the migration error code.
        /// </summary>
        [DataMember]
        public FabricErrorCode ErrorCode { get; set; }

        /// <summary>
        /// Gets or sets the exception type
        /// </summary>
        [DataMember]
        public string ExceptionType { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the error is Fabric related.
        /// </summary>
        [DataMember]
        public bool IsFabricError { get; set; }
    }
}
