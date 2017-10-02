// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------
namespace Microsoft.ServiceFabric.Services.Remoting.V1
{
    using System;
    using System.Fabric.Common;
    using System.Globalization;
    using System.IO;
    using System.Reflection;
    using System.Runtime.Serialization;
    using System.Text;
    using Microsoft.ServiceFabric.Services.Common;
    using Microsoft.ServiceFabric.Services.Communication;

    /// <summary>
    /// Represents the fault type used by Service Remoting to transfer the exception details from the Service Replica to the client.
    /// </summary>
    [DataContract(Name = "RemoteExceptionInformation", Namespace = Constants.ServiceCommunicationNamespace)]
    public class RemoteExceptionInformation
    {
        /// <summary>
        /// Serialized exception or the exception message encoded as UTF8 if the exception cannot be serialized.
        /// </summary>
        /// <value>The data in the exception.</value>
        [DataMember(Name = "Data", Order = 0)]
        public byte[] Data { get; private set; }

        private static readonly DataContractSerializer serializer =
            new DataContractSerializer(typeof(ServiceExceptionData));

        /// <summary>
        /// Instantiates the RemoteExceptionInformation object with the data.
        /// </summary>
        /// <param name="data">The data to be sent to the client.</param>
        public RemoteExceptionInformation(byte[] data)
        {
            this.Data = data;
        }


        /// <summary>
        /// Indicates a method that constructs the RemoteExceptionInformation from an exception.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <returns>Returns the RemoteExceptionInformation.</returns>
        public static RemoteExceptionInformation FromException(Exception exception)
        {
            try
            {
                var serializer = new NetDataContractSerializer();
                using (var stream = new MemoryStream())
                {
                    serializer.Serialize(stream, exception);
                    stream.Flush();
                    return new RemoteExceptionInformation(stream.ToArray());
                }
            }
            catch (Exception e)
            {
                // failed to serialize the exception, include the information about the exception in the data
                ServiceTrace.Source.WriteWarning("RemoteExceptionInformation", "Serialization failed for Exception Type {0} : Reason  {1}", exception.GetType().FullName, e);
                return FromExceptionString(exception);
            }
        }

        /// <summary>
        /// Gets the exception from the RemoteExceptionInformation
        /// </summary>
        /// <param name="remoteExceptionInformation">The RemoteExceptionInformation.</param>
        /// <param name="result">The exception from the remote side.</param>
        /// <returns>true if there was a valid exception; otherwise, false.</returns>
        public static bool ToException(RemoteExceptionInformation remoteExceptionInformation, out Exception result)
        {
            Requires.ThrowIfNull(remoteExceptionInformation, "RemoteExceptionInformation");

            // try to de-serialize the bytes in to the exception
            Exception res;
            if (TryDeserializeException(remoteExceptionInformation.Data, out res))
            {
                result = res;
                return true;
            }


            // try to de-serialize the bytes in to exception message and create service exception
            if (TryDeserializeServiceException(remoteExceptionInformation.Data, out result))
            {
                return true;
            }

            //Set Reason for Serialization failure. This can happen in case where serialization succeded 
            //but deserialization fails as type is not accessible
            result = res;
            return false;
        }

        private static bool TryDeserializeException(byte[] data, out Exception result)
        {
            var serializer = new NetDataContractSerializer();
            try
            {
                using (var stream = new MemoryStream(data))
                {
                    result = (Exception)serializer.Deserialize(stream);
                    return true;
                }
            }
            catch (Exception ex)
            {
                //return reason for serialization failure
                result = ex;
                return false;
            }
        }


        private static bool TryDeserializeServiceException(byte[] data, out Exception result)
        {
            try
            {
                ServiceExceptionData eData;
                if (TryDeserializeExceptionData(data, out eData))
                {
                    result = new ServiceException(eData.Type, eData.Message);
                    return true;
                }
            }
            catch (Exception e)
            {
                //swallowing the exception
                ServiceTrace.Source.WriteWarning("RemoteExceptionInformation", "DeSerialization failed : Reason  {0}", e);
            }

            result = null;
            return false;
        }

        private static bool TryDeserializeExceptionData(byte[] data, out ServiceExceptionData result)
        {
            try
            {
                var exceptionData = (ServiceExceptionData)SerializationUtility.Deserialize(serializer, data);
                result = exceptionData;
                return true;
            }
            catch (Exception e)
            {
                //swallowing the exception
                ServiceTrace.Source.WriteWarning("RemoteExceptionInformation", " ServiceExceptionData DeSerialization failed : Reason  {0}", e);
            }

            result = null;
            return false;
        }


        private static RemoteExceptionInformation FromExceptionString(Exception exception)
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

            var exceptionBytes = SerializationUtility.Serialize(serializer, exceptionData);

            return new RemoteExceptionInformation(exceptionBytes);
        }
    }
}
