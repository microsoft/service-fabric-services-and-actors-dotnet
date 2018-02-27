// ------------------------------------------------------------
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT License (MIT).See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Remoting.V2
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Runtime.Serialization;
    using System.Runtime.Serialization.Formatters;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.Text;
    using System.Xml;
    using Microsoft.ServiceFabric.Services.Communication;
    /// <summary>
    /// Fault type used by Service Remoting to transfer the exception details from the Service Replica to the client.
    /// </summary>
    [DataContract(Name = "RemoteException", Namespace = Constants.ServiceCommunicationNamespace)]
    internal class RemoteException
    {
        private static BinaryFormatter BinaryFormatter;
        private static readonly DataContractSerializer ServiceExceptionDataSerializer = new DataContractSerializer(typeof(ServiceExceptionData));
        static RemoteException()
        {
            BinaryFormatter = new BinaryFormatter();
            BinaryFormatter.AssemblyFormat = FormatterAssemblyStyle.Simple;
        }

        public RemoteException(List<ArraySegment<byte>> buffers)
        {
            this.Data = buffers;
        }

        /// <summary>
        /// Serialized exception or the exception message encoded as UTF8 if the exception cannot be serialized.
        /// </summary>
        /// <value>Data in the exception</value>
        [DataMember(Name = "Data", Order = 0)]
        public List<ArraySegment<byte>> Data { get; private set; }

        /// <summary>
        /// Factory method that constructs the RemoteException from an exception.
        /// </summary>
        /// <param name="exception">Exception</param>
        /// <returns>RemoteException</returns>
        public static RemoteException FromException(Exception exception)
        {
            try
            {

                using (var stream = new MemoryStream())
                {
                    BinaryFormatter.Serialize(stream, exception);
                    stream.Flush();
                    var buffers = new List<ArraySegment<byte>>
                    {
                        new ArraySegment<byte>(stream.ToArray()),
                    };
                    return new RemoteException(buffers);
                }
            }
            catch (Exception e)
            {
                // failed to serialize the exception, include the information about the exception in the data
                ServiceTrace.Source.WriteWarning(
                    "RemoteException",
                    "Serialization failed for Exception Type {0} : Reason  {1}", exception.GetType().FullName, e);
                var buffer = FromExceptionString(exception);
                return new RemoteException(buffer);
            }
        }

        /// <summary>
        /// Gets the exception from the RemoteException
        /// </summary>
        /// <param name="messageBuffer"></param>
        /// <param name="result">Exception from the remote side</param>
        /// <returns>true if there was a valid exception, false otherwise</returns>
        public static bool ToException(Stream messageBuffer, out Exception result)
        {
            // try to de-serialize the bytes in to the exception
            if (TryDeserializeException(messageBuffer, out var res))
            {
                result = res;
                return true;
            }


            // try to de-serialize the bytes in to exception requestMessage and create service exception
            if (TryDeserializeServiceException(messageBuffer, out result))
            {
                return true;
            }

            //Set Reason for Serialization failure. This can happen in case where serialization succeded
            //but deserialization fails as type is not accessible
            result = res;
            messageBuffer.Dispose();
            return false;
        }

        private static bool TryDeserializeException(Stream data, out Exception result)
        {
            try
            {
                result = (Exception)BinaryFormatter.Deserialize(data);
                return true;
            }
            catch (Exception ex)
            {
                //return reason for serialization failure
                result = ex;
                return false;
            }
        }

        private static bool TryDeserializeServiceException(Stream data, out Exception result)
        {
            try
            {
                data.Seek(0, SeekOrigin.Begin);
                if (TryDeserializeExceptionData(data, out var eData))
                {
                    result = new ServiceException(eData.Type, eData.Message);
                    return true;
                }
            }
            catch (Exception e)
            {
                //swallowing the exception
                ServiceTrace.Source.WriteWarning("RemoteException", "DeSerialization failed : Reason  {0}", e);
            }

            result = null;
            return false;
        }

        internal static bool TryDeserializeExceptionData(Stream data, out ServiceExceptionData result)
        {
            try
            {
                var exceptionData = (ServiceExceptionData)DeserializeServiceExceptionData(data);
                result = exceptionData;
                return true;
            }
            catch (Exception e)
            {
                //swallowing the exception
                ServiceTrace.Source.WriteWarning(
                    "RemoteException",
                    " ServiceExceptionData DeSerialization failed : Reason  {0}", e);
            }

            result = null;
            return false;
        }


        internal static List<ArraySegment<byte>> FromExceptionString(Exception exception)
        {
            var exceptionStringBuilder = new StringBuilder();

            exceptionStringBuilder.AppendFormat(
                CultureInfo.CurrentCulture,
                SR.ErrorExceptionSerializationFailed1,
                exception.GetType().FullName);

            exceptionStringBuilder.AppendLine();

            exceptionStringBuilder.AppendFormat(
                CultureInfo.CurrentCulture,
                SR.ErrorExceptionSerializationFailed2,
                exception);
            var exceptionData = new ServiceExceptionData(exception.GetType().FullName, exceptionStringBuilder.ToString());

            var exceptionBytes = SerializeServiceExceptionData(exceptionData);
            var buffers = new List<ArraySegment<byte>>
            {
                new ArraySegment<byte>(exceptionBytes),
            };
            return buffers;
        }

        private static object DeserializeServiceExceptionData(Stream buffer)
        {
            if ((buffer == null) || (buffer.Length == 0))
            {
                return null;
            }


            using (var reader = XmlDictionaryReader.CreateBinaryReader(buffer, XmlDictionaryReaderQuotas.Max))
            {
                return ServiceExceptionDataSerializer.ReadObject(reader);
            }
        }

        private static byte[] SerializeServiceExceptionData(ServiceExceptionData msg)
        {
            if (msg == null)
            {
                return null;
            }

            using (var stream = new MemoryStream())
            {
                using (var writer = XmlDictionaryWriter.CreateBinaryWriter(stream))
                {
                    ServiceExceptionDataSerializer.WriteObject(writer, msg);
                    writer.Flush();
                    return stream.ToArray();
                }
            }
        }
    }
}
