// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Remoting.Client
{
    using System;
    using System.Fabric;
    using Microsoft.ServiceFabric.Services.Communication.Client;

    /// <summary>
    /// Provides handling of exceptions encountered in communicating with
    /// a service fabric service over remoted interfaces.
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
    ///         <item><description><see cref="FabricNotPrimaryException"/>, when the target replica is <see cref="TargetReplicaSelector.PrimaryReplica"/>.</description></item>
    ///         <item><description><see cref="FabricNotReadableException"/></description> </item>
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
    ///         <item><description><see cref="FabricTransientException"/></description> </item>
    ///     </list>
    /// </description>
    /// </item>
    /// </list>
    /// </remarks>
    public class ServiceRemotingExceptionHandler : IExceptionHandler
    {
        private static readonly string TraceType = "ServiceRemotingExceptionHandler";
        private readonly string traceId;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceRemotingExceptionHandler"/> class with a default trace id.
        /// </summary>
        public ServiceRemotingExceptionHandler()
            : this(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceRemotingExceptionHandler"/> class with a specified trace Id.
        /// </summary>
        /// <param name="traceId">
        ///     The ID to use in diagnostics traces from this component.
        /// </param>
        public ServiceRemotingExceptionHandler(string traceId)
        {
            this.traceId = traceId ?? Guid.NewGuid().ToString();
        }

        /// <summary>
        /// Examines the exception and determines how that exception can be handled.
        /// </summary>
        /// <param name="exceptionInformation">The information about the exception.</param>
        /// <param name="retrySettings">The operation retry preferences.</param>
        /// <param name="result">The result of the exception handling.</param>
        /// <returns>true if the exception is handled; otherwise, false.</returns>
        bool IExceptionHandler.TryHandleException(
            ExceptionInformation exceptionInformation,
            OperationRetrySettings retrySettings,
            out ExceptionHandlingResult result)
        {
            if (exceptionInformation.Exception is FabricNotPrimaryException)
            {
                if (exceptionInformation.TargetReplica == TargetReplicaSelector.PrimaryReplica)
                {
                    result = new ExceptionHandlingRetryResult(
                        exceptionInformation.Exception,
                        false,
                        retrySettings,
                        retrySettings.DefaultMaxRetryCountForNonTransientErrors);

                    return true;
                }

                ServiceTrace.Source.WriteInfo(
                    TraceType,
                    "{0} Got exception {1} which does not match the replica target : {2}",
                    this.traceId,
                    exceptionInformation.Exception,
                    exceptionInformation.TargetReplica);

                result = null;
                return false;
            }

            if (exceptionInformation.Exception is FabricNotReadableException)
            {
                result = new ExceptionHandlingRetryResult(
                        exceptionInformation.Exception,
                        false,
                        retrySettings,
                        retrySettings.DefaultMaxRetryCountForNonTransientErrors);

                return true;
            }

            // Note: This code handles retries for FabricTransientException even from Actors, eg. ActorDeletedException.
            if (exceptionInformation.Exception is FabricTransientException)
            {
                result = new ExceptionHandlingRetryResult(
                    exceptionInformation.Exception,
                    true,
                    retrySettings,
                    retrySettings.DefaultMaxRetryCountForTransientErrors);

                return true;
            }

            result = null;
            return false;
        }
    }
}
