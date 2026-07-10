// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.

using System;
using System.IO;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Security;
using System.Xml;
using Fuzzy;
using Microsoft.ServiceFabric.Services.Communication.Client;
using Xunit;

namespace Microsoft.ServiceFabric.Services.Communication.Wcf.Client;

public abstract class WcfExceptionHandlerTest
{
    readonly IExceptionHandler sut = new WcfExceptionHandler();

    static readonly IFuzz fuzzy = new RandomFuzz(Environment.TickCount);

    public sealed class TryHandleException : WcfExceptionHandlerTest
    {
        readonly OperationRetrySettings retrySettings;

        public TryHandleException() =>
            retrySettings = new OperationRetrySettings(fuzzy.TimeSpan(), fuzzy.TimeSpan(), fuzzy.Int32(), fuzzy.Int32());

        [Theory, MemberData(nameof(FailoverExceptions))]
        public void ReturnsNonTransientRetryResultWhenExceptionIndicatesFailover(ExceptionInformation exceptionInformation)
        {
            bool handled = sut.TryHandleException(exceptionInformation, retrySettings, out ExceptionHandlingResult result);

            Assert.True(handled);
            var retry = (ExceptionHandlingRetryResult)result;
            Assert.False(retry.IsTransient);
            Assert.Equal(exceptionInformation.Exception.GetType().FullName, retry.ExceptionId);
            Assert.Equal(retrySettings.DefaultMaxRetryCountForNonTransientErrors, retry.MaxRetryCount);
        }

        [Theory, MemberData(nameof(TransientExceptions))]
        public void ReturnsTransientRetryResultWhenExceptionIsTransient(ExceptionInformation exceptionInformation)
        {
            bool handled = sut.TryHandleException(exceptionInformation, retrySettings, out ExceptionHandlingResult result);

            Assert.True(handled);
            var retry = (ExceptionHandlingRetryResult)result;
            Assert.True(retry.IsTransient);
            Assert.Equal(exceptionInformation.Exception.GetType().FullName, retry.ExceptionId);
            Assert.Equal(int.MaxValue, retry.MaxRetryCount);
        }

        [Theory, MemberData(nameof(NonRetryableExceptions))]
        public void ReturnsThrowResultWhenExceptionIsNotRetryable(ExceptionInformation exceptionInformation)
        {
            bool handled = sut.TryHandleException(exceptionInformation, retrySettings, out ExceptionHandlingResult result);

            Assert.True(handled);
            var thrown = (ExceptionHandlingThrowResult)result;
            Assert.Same(exceptionInformation.Exception, thrown.ExceptionToThrow);
        }

        [Theory, MemberData(nameof(SupportedFaultXmlFormats))]
        public void ReturnsNonTransientRetryResultWhenFaultIsRetryable(string exceptionId, string xml)
        {
            FaultException exception = new(new FaultReason(xml), WcfRemoteExceptionInformation.FaultCodeRetry);
            ExceptionInformation exceptionInformation = new(exception);

            bool handled = sut.TryHandleException(exceptionInformation, retrySettings, out ExceptionHandlingResult result);

            Assert.True(handled);
            var retry = (ExceptionHandlingRetryResult)result;
            Assert.False(retry.IsTransient);
            Assert.Equal(exceptionId, retry.ExceptionId);
            Assert.Equal(retrySettings.DefaultMaxRetryCountForNonTransientErrors, retry.MaxRetryCount);
        }

        [Fact]
        public void ThrowsArgumentNullExceptionWhenExceptionInformationIsNull()
        {
            ExceptionInformation exceptionInformation = null;
            var actual = Assert.Throws<ArgumentNullException>(() => sut.TryHandleException(exceptionInformation, retrySettings, out ExceptionHandlingResult result));
            Assert.Equal(nameof(exceptionInformation), actual.ParamName);
        }

        [Fact]
        public void ThrowsArgumentNullExceptionWhenRetrySettingsIsNull()
        {
            ExceptionInformation exceptionInformation = new(new EndpointNotFoundException());
            var actual = Assert.Throws<ArgumentNullException>(() => sut.TryHandleException(exceptionInformation, null, out ExceptionHandlingResult result));
            Assert.Equal(nameof(retrySettings), actual.ParamName);
        }

        [Fact]
        public void ReturnsFalseWhenFaultCodeNameDoesNotMatch()
        {
            string faultCodeName = WcfRemoteExceptionInformation.FaultCodeName + fuzzy.String();
            FaultCode code = new(faultCodeName, new FaultCode(WcfRemoteExceptionInformation.FaultSubCodeRetryName));
            (_, string xml) = DataContractSerializerException();
            ExceptionInformation exceptionInformation = new(new FaultException(new FaultReason(xml), code));

            bool handled = sut.TryHandleException(exceptionInformation, retrySettings, out ExceptionHandlingResult result);

            Assert.False(handled);
            Assert.Null(result);
        }

        [Fact]
        public void ReturnsFalseWhenFaultSubCodeNameDoesNotMatch()
        {
            string subCodeName = WcfRemoteExceptionInformation.FaultSubCodeRetryName + fuzzy.String();
            FaultCode code = new(WcfRemoteExceptionInformation.FaultCodeName, new FaultCode(subCodeName));
            (_, string xml) = DataContractSerializerException();
            ExceptionInformation exceptionInformation = new(new FaultException(new FaultReason(xml), code));

            bool handled = sut.TryHandleException(exceptionInformation, retrySettings, out ExceptionHandlingResult result);

            Assert.False(handled);
            Assert.Null(result);
        }

        [Fact]
        public void ReturnsFalseWhenFaultSubCodeIsMissing()
        {
            FaultCode code = new(WcfRemoteExceptionInformation.FaultCodeName);
            (_, string xml) = DataContractSerializerException();
            ExceptionInformation exceptionInformation = new(new FaultException(new FaultReason(xml), code));

            bool handled = sut.TryHandleException(exceptionInformation, retrySettings, out ExceptionHandlingResult result);

            Assert.False(handled);
            Assert.Null(result);
        }

        [Theory]
        [InlineData(""), InlineData("NotXml"), InlineData("<NoType />")]
        [InlineData("<ServiceExceptionData xmlns='urn:ServiceFabric.Communication'><Type></Type></ServiceExceptionData>")]
        [InlineData("<TestException xmlns:z='http://schemas.microsoft.com/2003/10/Serialization/' z:Type='   ' />")]
        public void ReturnsFalseWhenFaultReasonDoesNotContainExceptionId(string xml)
        {
            FaultException exception = new(new FaultReason(xml), WcfRemoteExceptionInformation.FaultCodeRetry);
            ExceptionInformation exceptionInformation = new(exception);

            bool handled = sut.TryHandleException(exceptionInformation, retrySettings, out ExceptionHandlingResult result);

            Assert.False(handled);
            Assert.Null(result);
        }

        [Fact]
        public void ReturnsNonTransientRetryResultWhenExceptionIsCommunicationException()
        {
            ExceptionInformation exceptionInformation = new(new CommunicationException(fuzzy.String()));

            bool handled = sut.TryHandleException(exceptionInformation, retrySettings, out ExceptionHandlingResult result);

            Assert.True(handled);
            var retry = (ExceptionHandlingRetryResult)result;
            Assert.False(retry.IsTransient);
            Assert.Equal(exceptionInformation.Exception.GetType().FullName, retry.ExceptionId);
            Assert.Equal(retrySettings.DefaultMaxRetryCountForNonTransientErrors, retry.MaxRetryCount);
        }

        [Fact]
        public void ReturnsFalseWhenExceptionIsNotRecognized()
        {
            bool handled = sut.TryHandleException(new ExceptionInformation(new TestException()), retrySettings, out ExceptionHandlingResult result);

            Assert.False(handled);
            Assert.Null(result);
        }

        public static TheoryData<ExceptionInformation> FailoverExceptions =>
        [
            new ExceptionInformation(new EndpointNotFoundException()),
            new ExceptionInformation(new CommunicationObjectAbortedException()),
            new ExceptionInformation(new CommunicationObjectFaultedException()),
            new ExceptionInformation(new ObjectDisposedException(fuzzy.String())),
            new ExceptionInformation(new ChannelTerminatedException()),
        ];

        public static TheoryData<ExceptionInformation> TransientExceptions =>
        [
            new ExceptionInformation(new TimeoutException()),
            new ExceptionInformation(new ServerTooBusyException(fuzzy.String())),
        ];

        public static TheoryData<ExceptionInformation> NonRetryableExceptions =>
        [
            new ExceptionInformation(new ActionNotSupportedException(fuzzy.String())),
            new ExceptionInformation(new AddressAccessDeniedException(fuzzy.String())),
            new ExceptionInformation(new SecurityAccessDeniedException(fuzzy.String())),
        ];

        public static TheoryData<string, string> SupportedFaultXmlFormats =>
        [
            NetDataContractSerializerException(),
            DataContractSerializerException(),
        ];

        static (string, string) NetDataContractSerializerException()
        {
            NetDataContractSerializer serializer = new();
            TestException exception = new();
            using StringWriter stringWriter = new();
            using var textStream = XmlWriter.Create(stringWriter);
            serializer.WriteObject(textStream, exception);
            textStream.Flush();
            return (exception.GetType().FullName, stringWriter.ToString());
        }

        static (string, string) DataContractSerializerException()
        {
            DataContractSerializer serializer = new(typeof(ServiceExceptionData));
            ServiceExceptionData exceptionData = new(fuzzy.String(), fuzzy.String());
            using StringWriter stringWriter = new();
            using var textStream = XmlWriter.Create(stringWriter);
            serializer.WriteObject(textStream, exceptionData);
            textStream.Flush();
            return (exceptionData.Type, stringWriter.ToString());
        }

        [Serializable] sealed class TestException : Exception { }
    }
}
