// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------
namespace Microsoft.ServiceFabric.Services.Remoting
{
    using System.Collections.Generic;
    using System.Fabric;
    using System.Globalization;
    using System.IO;
    using System.Runtime.Serialization;
    using System.Xml;

    /// <summary>
    /// Specifies the headers that are sent along with a ServiceRemoting message.
    /// </summary>
    [DataContract(Name = "ServiceMessageHeaders", Namespace = Constants.ServiceCommunicationNamespace)]
    public class ServiceRemotingMessageHeaders
    {
        internal const string CancellationHeaderName = "CancellationHeader";

        [DataMember(Name = "Headers", IsRequired = true, Order = 2)] private Dictionary<string, byte[]> headers;

        /// <summary>
        /// Gets or sets the methodId of the remote method.
        /// </summary>
        /// <value>The method id.</value>
        [DataMember(Name = "MethodId", IsRequired = true, Order = 0)]
        public int MethodId { get; set; }

        /// <summary>
        /// Gets or sets the interface id of the remote interface.
        /// </summary>
        /// <value>The interface id.</value>
        [DataMember(Name = "InterfaceId", IsRequired = true, Order = 1)]
        public int InterfaceId { get; set; }

        /// <summary>
        /// Gets or sets the identifier for the remote method invocation.
        /// </summary>
        [DataMember(Name = "InvocationId", IsRequired = false, Order = 3, EmitDefaultValue = false)]
        public string InvocationId { get; set; }

        /// <summary>
        /// Initializes a new instance of the ServiceRemotingMessageHeaders class.
        /// </summary>
        public ServiceRemotingMessageHeaders()
        {
            this.headers = new Dictionary<string, byte[]>();
            this.InvocationId = null;
        }

        /// <summary>
        /// Adds a new header with the specified name and value.
        /// Throws FabricElementAlreadyExistsException if a header with the same name already exists.
        /// </summary>
        /// <param name="headerName">The header Name.</param>
        /// <param name="headerValue">The header value.</param>
        public void AddHeader(string headerName, byte[] headerValue)
        {
            if (this.headers.ContainsKey(headerName))
            {
                throw new FabricElementAlreadyExistsException(
                    string.Format(
                        CultureInfo.CurrentCulture,
                        SR.ErrorHeaderAlreadyExists,
                        headerName));
            }

            this.headers[headerName] = headerValue;
        }

        /// <summary>
        /// Gets the header with the specified name.
        /// </summary>
        /// <param name="headerName">The header Name.</param>
        /// <param name="headerValue">The header value.</param>
        /// <returns>true if a header with that name exists; otherwise, false.</returns>
        public bool TryGetHeaderValue(string headerName, out byte[] headerValue)
        {
            headerValue = null;

            if (this.headers == null)
            {
                return false;
            }

            return this.headers.TryGetValue(headerName, out headerValue);
        }

        /// <summary>
        /// Serializes the headers to a byte array.
        /// </summary>
        /// <param name="serializer">The serializer.</param>
        /// <param name="msg">The headers.</param>
        /// <returns>The serialized byte array.</returns>
        public static byte[] Serialize(DataContractSerializer serializer, ServiceRemotingMessageHeaders msg)
        {
            if (msg == null)
            {
                return null;
            }

            using (var stream = new MemoryStream())
            {
                using (var writer = XmlDictionaryWriter.CreateBinaryWriter(stream))
                {
                    serializer.WriteObject(writer, msg);
                    writer.Flush();
                    return stream.ToArray();
                }
            }
        }

        /// <summary>
        /// De-serializes the byte array to a ServiceRemotingMessageHeaders object.
        /// </summary>
        /// <param name="serializer">The deserializer.</param>
        /// <param name="buffer">The buffer.</param>
        /// <returns>De-serialized headers.</returns>
        public static ServiceRemotingMessageHeaders Deserialize(DataContractSerializer serializer, byte[] buffer)
        {
            if ((buffer == null) || (buffer.Length == 0))
            {
                return null;
            }

            using (var stream = new MemoryStream(buffer))
            {
                using (var reader = XmlDictionaryReader.CreateBinaryReader(stream, XmlDictionaryReaderQuotas.Max))
                {
                    return (ServiceRemotingMessageHeaders) serializer.ReadObject(reader);
                }
            }
        }
    }
}
