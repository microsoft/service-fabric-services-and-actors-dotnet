// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Remoting.Tests.V2.ExceptionConvertors
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.ServiceFabric.Services.Communication;
    using Microsoft.ServiceFabric.Services.Remoting.FabricTransport.Runtime;
    using Microsoft.ServiceFabric.Services.Remoting.V2.Messaging;
    using Xunit;

    /// <summary>
    /// Custom Convertor test
    /// </summary>
    public class CustomConvertorTest
    {
        private static Remoting.V2.Runtime.ExceptionConversionHandler runtimeHandler
            = new Remoting.V2.Runtime.ExceptionConversionHandler(
                new List<Remoting.V2.Runtime.IExceptionConvertor>()
                {
                    new CustomConvertorRuntime(),
                },
                new FabricTransportRemotingListenerSettings { RemotingExceptionDepth = 3 });

        private static Remoting.V2.Client.ExceptionConversionHandler clientHandler
            = new Remoting.V2.Client.ExceptionConversionHandler(
                new List<Remoting.V2.Client.IExceptionConvertor>()
                {
                    new CustomConvertorClient(),
                },
                new FabricTransport.FabricTransportRemotingSettings());

        /// <summary>
        /// Custom types test.
        /// </summary>
        /// <returns>Task representing async operation.</returns>
        [Fact]
        public static async Task KnownCustomExceptionSerializationTest()
        {
            CustomException exception = null;
            try
            {
                Throw();
            }
            catch (CustomException ex)
            {
                exception = ex;
            }

            var serializedData = runtimeHandler.SerializeRemoteException(exception);
            var msgStream = new SegmentedReadMemoryStream(serializedData);

            Exception resultEx = null;
            try
            {
                await clientHandler.DeserializeRemoteExceptionAndThrowAsync(msgStream);
            }
            catch (AggregateException ex)
            {
                resultEx = ex.InnerException;
            }

            Assert.True(resultEx != null);
            Assert.Equal(resultEx.GetType(), exception.GetType());
            Assert.Equal(resultEx.Message, exception.Message);
            Assert.Equal(resultEx.HResult, exception.HResult);

            var customResultEx = resultEx as CustomException;
            Assert.Equal(customResultEx.Field1, exception.Field1);
            Assert.Equal(customResultEx.Field2, exception.Field2);
        }

        private static void Throw()
        {
            throw new CustomException("CustomEx", "CutomField1", "CustomField2");
        }

        internal class CustomConvertorClient : Remoting.V2.Client.IExceptionConvertor
        {
            public bool TryConvertFromServiceException(ServiceException serviceException, out Exception actualException)
            {
                actualException = null;
                if (serviceException.ActualExceptionType == typeof(CustomException).FullName)
                {
                    actualException = new CustomException(
                        serviceException.Message,
                        serviceException.ActualExceptionData["Field1"],
                        serviceException.ActualExceptionData["Field2"]);

                    return true;
                }

                return false;
            }

            public bool TryConvertFromServiceException(ServiceException serviceException, Exception innerException, out Exception actualException)
            {
                throw new NotImplementedException();
            }

            public bool TryConvertFromServiceException(ServiceException serviceException, Exception[] innerExceptions, out Exception actualException)
            {
                throw new NotImplementedException();
            }
        }

        internal class CustomConvertorRuntime : Remoting.V2.Runtime.ExceptionConvertorBase
        {
            public override bool TryConvertToServiceException(Exception originalException, out ServiceException serviceException)
            {
                serviceException = null;
                if (originalException is CustomException customEx)
                {
                    serviceException = new ServiceException(customEx.GetType().FullName, customEx.Message);
                    serviceException.ActualExceptionStackTrace = originalException.StackTrace;
                    serviceException.ActualExceptionData = new Dictionary<string, string>()
                    {
                        { "Field1", customEx.Field1 },
                        { "Field2", customEx.Field2 },
                    };

                    return true;
                }

                return false;
            }
        }

        /// <summary>
        /// Custom Exception
        /// </summary>
        internal class CustomException : Exception
        {
            public CustomException(string message, string field1, string field2)
                : base(message)
            {
                this.Field1 = field1;
                this.Field2 = field2;
            }

            public string Field1 { get; set; }

            public string Field2 { get; set; }
        }
    }
}
