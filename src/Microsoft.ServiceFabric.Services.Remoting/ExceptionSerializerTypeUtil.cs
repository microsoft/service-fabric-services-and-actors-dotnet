// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.Serialization;
    using System.Text;
    using System.Xml;

    /// <summary>
    /// Utility class for exception serializer type enum
    /// </summary>
    public static class ExceptionSerializerTypeUtil
    {
        private static readonly DataContractSerializer ServiceExceptionDataSerializer = new DataContractSerializer(typeof(ExceptionSerializerType));

        /// <summary>
        /// Gets the serialized exception serializer type
        /// </summary>
        /// <param name="exceptionSerializerType">exception serializer type to be serialized.</param>
        /// <returns>serialized exception serializer type</returns>
        public static byte[] GetByteArray(ExceptionSerializerType exceptionSerializerType)
        {
            return Encoding.UTF8.GetBytes(exceptionSerializerType.ToString());
        }

        /// <summary>
        /// Gets the exception serializer type
        /// </summary>
        /// <param name="stream">The stream that contains the serialized exception serializer type.</param>
        /// <returns>deserialized exception serializer type</returns>
        public static ExceptionSerializerType GetSerializerType(byte[] stream)
        {
            Enum.TryParse(Encoding.UTF8.GetString(stream), out ExceptionSerializerType exceptionSerializer);
            return exceptionSerializer;
        }
    }
}
