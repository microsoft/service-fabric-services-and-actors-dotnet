// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Communication.Wcf
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Runtime.Serialization;
    using System.ServiceModel;
    using System.Text;
    using System.Xml;

    internal class WcfRemoteExceptionInformation
    {
        public static readonly string FaultCodeName = "WcfRemoteExceptionInformation";
        public static readonly string FaultSubCodeRetryName = "Retry";
        public static readonly string FaultSubCodeThrowName = "Throw";

        public static readonly FaultCode FaultCodeRetry = new FaultCode(FaultCodeName,
            new FaultCode(FaultSubCodeRetryName));

        public static readonly FaultCode FaultCodeThrow = new FaultCode(FaultCodeName,
            new FaultCode(FaultSubCodeThrowName));

        private static readonly DataContractSerializer serializer =
            new DataContractSerializer(typeof(ServiceExceptionData));

        public static string ToString(Exception exception)
        {
            try
            {
                var serializer = new NetDataContractSerializer();

                var stringWriter = new StringWriter();

                using (var textStream = XmlWriter.Create(stringWriter))
                {
                    serializer.WriteObject(textStream, exception);
                    textStream.Flush();

                    return stringWriter.ToString();
                }
            }
            catch (Exception e)
            {
                var exceptionStringBuilder = new StringBuilder();

                exceptionStringBuilder.AppendFormat(
                    CultureInfo.CurrentCulture,
                    Microsoft.ServiceFabric.Services.Wcf.SR.ErrorExceptionSerializationFailed1,
                    exception.GetType().FullName);

                exceptionStringBuilder.AppendLine();

                exceptionStringBuilder.AppendFormat(
                    CultureInfo.CurrentCulture,
                    Microsoft.ServiceFabric.Services.Wcf.SR.ErrorExceptionSerializationFailed2,
                    exception);

                var exceptionData = new ServiceExceptionData(exception.GetType().FullName,
                    exceptionStringBuilder.ToString());
                if (TrySerializeExceptionData(exceptionData, out var result))
                {
                    return result;
                }
                throw e;
            }
        }


        public static Exception ToException(string exceptionString)
        {
            try
            {
                var serializer = new NetDataContractSerializer();
                var stringReader = new StringReader(exceptionString);

                // disabling DTD processing on XML streams that are not over the network.
                var settings = new XmlReaderSettings
                {
                    DtdProcessing = DtdProcessing.Prohibit,
                    XmlResolver = null
                };
                using (var textStream = XmlReader.Create(stringReader, settings))
                {
                    return (Exception)serializer.ReadObject(textStream);
                }
            }
            catch (Exception ex)
            {
                // add the message as service exception
                if (TryDeserializeExceptionData(exceptionString, out var exceptionData))
                {
                    return new ServiceException(exceptionData.Type, exceptionData.Message);
                }
                throw ex;
            }
        }

        private static bool TrySerializeExceptionData(ServiceExceptionData serviceExceptionData, out string result)
        {
            try
            {
                var stringWriter = new StringWriter();

                using (var textStream = XmlWriter.Create(stringWriter))
                {
                    serializer.WriteObject(textStream, serviceExceptionData);
                    textStream.Flush();

                    result = stringWriter.ToString();
                    return true;
                }
            }
            catch (Exception)
            {
                // no-op
            }
            result = null;
            return false;
        }

        private static bool TryDeserializeExceptionData(string exceptionString, out ServiceExceptionData result)
        {
            try
            {
                var stringReader = new StringReader(exceptionString);

                // disabling DTD processing on XML streams that are not over the network.
                var settings = new XmlReaderSettings
                {
                    DtdProcessing = DtdProcessing.Prohibit,
                    XmlResolver = null
                };
                using (var textStream = XmlReader.Create(stringReader, settings))
                {
                    result = (ServiceExceptionData)serializer.ReadObject(textStream);
                    return true;
                }
            }
            catch (Exception)
            {
                // no-op
            }

            result = null;
            return false;
        }
    }
}
