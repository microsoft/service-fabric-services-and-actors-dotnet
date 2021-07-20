// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric
{
    /// <summary>
    /// Specifies the type of serializer for remoting exceptions.
    /// </summary>
    public enum ExceptionSerializerType
    {
        /// <summary>
        /// Represents data contract serializer for remoting exceptions.
        /// </summary>
        DataContractSerializer,

        /// <summary>
        /// Represents binary formatter serializer for remoting exceptions.
        /// </summary>
        BinaryFormatter,
    }
}
