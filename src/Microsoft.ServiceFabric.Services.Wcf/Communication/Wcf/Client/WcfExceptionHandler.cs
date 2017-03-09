// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------
namespace Microsoft.ServiceFabric.Services.Communication.Wcf.Client
{
    using System;
    using System.ServiceModel;
    using System.ServiceModel.Security;
    using Microsoft.ServiceFabric.Services.Communication.Client;

    /// <summary>
    /// This class provide handling of WCF exceptions encountered in communicating with 
    /// a service fabric service that is using WCF based communication listener.
    /// </summary>
    /// <remarks>
    /// The exceptions are handled as per the description below:
    /// <list type="table">
    /// <item>
    /// <description>
    ///     The following exceptions indicate service failover. These exceptions are handled by returning <see cref="ExceptionHandlingRetryResult"/> from the 
    ///     <see cref="IExceptionHandler.TryHandleException(ExceptionInformation, OperationRetrySettings, out ExceptionHandlingResult)"/> method.
    ///     The <see cref="ExceptionHandlingRetryResult.IsTransient"/> property of the <see cref="ExceptionHandlingRetryResult"/> is set to false, 
    ///     the <see cref="ExceptionHandlingRetryResult.RetryDelay"/>  property is set to a random value up to <see cref="OperationRetrySettings.MaxRetryBackoffIntervalOnNonTransientErrors"/> 
    ///     and <see cref="ExceptionHandlingRetryResult.MaxRetryCount"/> property is set to <see cref="int.MaxValue"/>.
    ///     <list type="bullet">
    ///         <item><description><see cref="EndpointNotFoundException"/></description> </item>
    ///         <item><description><see cref="CommunicationObjectAbortedException"/></description> </item>
    ///         <item><description><see cref="CommunicationObjectFaultedException"/></description> </item>
    ///         <item><description><see cref="ObjectDisposedException"/></description> </item>
    ///         <item><description><see cref="ChannelTerminatedException"/></description> </item>
    ///     </list>
    /// </description>
    /// </item>
    /// <item>
    /// <description>
    ///     The following exceptions indicate transient error conditions and handled by returning <see cref="ExceptionHandlingRetryResult"/> from the 
    ///     <see cref="IExceptionHandler.TryHandleException(ExceptionInformation, OperationRetrySettings, out ExceptionHandlingResult)"/> method.
    ///     The <see cref="ExceptionHandlingRetryResult.IsTransient"/> property of the <see cref="ExceptionHandlingRetryResult"/> is set to true, 
    ///     the <see cref="ExceptionHandlingRetryResult.RetryDelay"/>  property is set to a random value up to <see cref="OperationRetrySettings.MaxRetryBackoffIntervalOnTransientErrors"/> 
    ///     and <see cref="ExceptionHandlingRetryResult.MaxRetryCount"/> property is set to <see cref="int.MaxValue"/>.
    ///     <list type="bullet">
    ///         <item><description><see cref="TimeoutException"/></description> </item>
    ///         <item><description><see cref="ServerTooBusyException"/></description> </item>
    ///     </list>
    /// </description>
    /// </item>
    /// <item>
    /// <description>
    ///     The following exceptions indicate mismatch in the binding or contract between the client and the service. 
    ///     These exceptions are handled by returning <see cref="ExceptionHandlingThrowResult"/> from the 
    ///     <see cref="IExceptionHandler.TryHandleException(ExceptionInformation, OperationRetrySettings, out ExceptionHandlingResult)"/> method.
    ///     <list type="bullet">
    ///         <item><description><see cref="ActionNotSupportedException"/></description> </item>
    ///         <item><description><see cref="AddressAccessDeniedException"/></description> </item>
    ///         <item><description><see cref="SecurityAccessDeniedException"/></description> </item>
    ///     </list>
    /// </description>
    /// </item>
    /// <item>
    /// <description>
    ///     The following exceptions are indicate an error from the service. 
    ///     They are handled by returning <see cref="ExceptionHandlingThrowResult"/> from the 
    ///     <see cref="IExceptionHandler.TryHandleException(ExceptionInformation, OperationRetrySettings, out ExceptionHandlingResult)"/> method.
    ///     <list type="bullet">
    ///         <item><description><see cref="FaultException"/></description> </item>
    ///     </list>
    /// </description>
    /// </item>
    /// <item>
    /// <description>
    ///     All other exceptions that are <see cref="CommunicationException"/>, but not <see cref="FaultException"/> are handled by returning 
    ///     <see cref="ExceptionHandlingRetryResult"/> from the 
    ///     <see cref="IExceptionHandler.TryHandleException(ExceptionInformation, OperationRetrySettings, out ExceptionHandlingResult)"/> method.
    ///     The <see cref="ExceptionHandlingRetryResult.IsTransient"/> property of the <see cref="ExceptionHandlingRetryResult"/> is set to true, 
    ///     the <see cref="ExceptionHandlingRetryResult.RetryDelay"/>  property is set to a random value up to <see cref="OperationRetrySettings.MaxRetryBackoffIntervalOnTransientErrors"/> 
    ///     and <see cref="ExceptionHandlingRetryResult.MaxRetryCount"/> property is set to <see cref="OperationRetrySettings.DefaultMaxRetryCount"/>.
    /// </description>
    /// </item>
    /// </list>
    /// </remarks>
    public class WcfExceptionHandler : IExceptionHandler
    {
        /// <summary>
        /// Initializes a new instance of WcfExceptionHandler class.
        /// </summary>
        public WcfExceptionHandler()
        {
            
        }
        /// <summary>
        /// Method that examines the exception and determines how that exception can be handled. 
        /// </summary>
        /// <param name="exceptionInformation">Information about the exception</param>
        /// <param name="retrySettings">The operation retry preferences.</param>
        /// <param name="result">Result of the exception handling</param>
        /// <returns>true if the exception is handled, false otherwise</returns>
        bool IExceptionHandler.TryHandleException(
            ExceptionInformation exceptionInformation,
            OperationRetrySettings retrySettings,
            out ExceptionHandlingResult result)
        {
            var e = exceptionInformation.Exception;

            // retry with resolve - these exceptions indicate a possible fail over
            if ((e is EndpointNotFoundException) ||
                (e is CommunicationObjectAbortedException) ||
                (e is CommunicationObjectFaultedException) ||
                (e is ObjectDisposedException) ||
                (e is ChannelTerminatedException))
            {
                result = new ExceptionHandlingRetryResult(
                    e,
                    false,
                    retrySettings,
                    int.MaxValue);
                return true;
            }


            // retry on timeout and service busy exceptions
            if ((e is TimeoutException) ||
                (e is ServerTooBusyException))
            {
                result = new ExceptionHandlingRetryResult(
                    e,
                    true,
                    retrySettings,
                    int.MaxValue);
                return true;
            }


            // Derived types of Communication Exception that are not retriable.
            if ((e is ActionNotSupportedException) ||
                (e is AddressAccessDeniedException))
            {
                result = new ExceptionHandlingThrowResult()
                {
                    ExceptionToThrow = e
                };
                return true;
            }

            // Security related derived types of Communication Exception that are not retriable
            if (e is SecurityAccessDeniedException)
            {
                result = new ExceptionHandlingThrowResult()
                {
                    ExceptionToThrow = e
                };
                return true;
            }


            var faultException = e as FaultException;
            if (faultException != null)
            {
                if (faultException.Code.Name == WcfRemoteExceptionInformation.FaultCodeName)
                {
                    var actualException = WcfRemoteExceptionInformation.ToException(faultException.Reason.ToString());

                    if (faultException.Code.SubCode.Name == WcfRemoteExceptionInformation.FaultSubCodeRetryName)
                    {
                        result = new ExceptionHandlingRetryResult(
                            actualException,
                            false,
                            retrySettings,
                            int.MaxValue);
                        return true;
                    }
                }
            }

            // retry on all communication exceptions, including the protocol exceptions for default max retry
            if ((faultException == null) && (e is CommunicationException))
            {
                result = new ExceptionHandlingRetryResult(
                    e,
                    false,
                    retrySettings,
                    retrySettings.DefaultMaxRetryCount);
                return true;
            }

            result = null;
            return false;
        }
    }
}