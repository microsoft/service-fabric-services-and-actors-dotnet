// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Remoting.Client
{
    using Microsoft.ServiceFabric.Actors.Runtime;
    using Microsoft.ServiceFabric.Services.Communication.Client;

    /// <summary>
    /// This class provide handling of exceptions encountered in communicating with
    /// service fabric actors over remoted actor interfaces.
    /// </summary>
    /// <remarks>
    /// <para>
    ///     This exception handler handles exceptions related to the following scenarios.
    /// </para>
    /// <list type="list">
    /// <item>
    /// <term>
    ///     Duplicate Messages:
    /// </term>
    /// <description>
    /// <para>
    ///     Operations performed on the actor are retried from the client based on the exception handling logic.
    ///     These exceptions represent various error condition including service failover. Therefore it is possible
    ///     for the actors to receive duplicate messages. If a duplicate message is received while previous
    ///     message is being processed by the actor, runtime return an internal exception to the client.
    ///     The client then retries the operation to get the result back from the actor. From the actor's
    ///     perspective duplicate operation will be performed by the clients and it should handle it in the similar
    ///     manner as if the operation was already processed and then a duplicate message arrived.
    /// </para>
    /// <para>
    ///     Exception related to duplicate operation being processed is handled by returning <see cref="ExceptionHandlingRetryResult"/> from the
    ///     <see cref="IExceptionHandler.TryHandleException(ExceptionInformation, OperationRetrySettings, out ExceptionHandlingResult)"/> method.
    ///     The <see cref="ExceptionHandlingRetryResult.IsTransient"/> property of the <see cref="ExceptionHandlingRetryResult"/> is set to true,
    ///     the <see cref="ExceptionHandlingRetryResult.RetryDelay"/>  property is set to a random value up to <see cref="OperationRetrySettings.MaxRetryBackoffIntervalOnTransientErrors"/>
    ///     and <see cref="ExceptionHandlingRetryResult.MaxRetryCount"/> property is set to <see cref="int.MaxValue"/>.
    /// </para>
    /// </description>
    /// </item>
    /// <item>
    /// <term>
    ///     <see cref="Microsoft.ServiceFabric.Actors.ActorConcurrencyLockTimeoutException"/>:
    /// </term>
    /// <description>
    /// <para>
    ///     Operations on the actors are performed using a turn based concurrency lock (<see cref="Microsoft.ServiceFabric.Actors.Runtime.ActorConcurrencySettings"/>)
    ///     that supports logical call context based reentrancy. In case of the long running actor operations it is
    ///     possible for acquisition of this lock to time out. The acquisition of the lock can also time out in case of the deadlock
    ///     situations (actor A and actor B calling each other almost at the same time).
    /// </para>
    /// <para>
    ///     The exception related to concurrency lock timeout is handled by returning <see cref="ExceptionHandlingRetryResult"/> from the
    ///     <see cref="IExceptionHandler.TryHandleException(ExceptionInformation, OperationRetrySettings, out ExceptionHandlingResult)"/> method
    ///     if the client performing the operation is not another actor.
    ///     The <see cref="ExceptionHandlingRetryResult.IsTransient"/> property of the <see cref="ExceptionHandlingRetryResult"/> is set to true,
    ///     the <see cref="ExceptionHandlingRetryResult.RetryDelay"/>  property is set to a random value up to <see cref="OperationRetrySettings.MaxRetryBackoffIntervalOnTransientErrors"/>
    ///     and <see cref="ExceptionHandlingRetryResult.MaxRetryCount"/> property is set to <see cref="int.MaxValue"/>.
    /// </para>
    /// <para>
    ///     The exception related to concurrency lock timeout is handled by returning <see cref="ExceptionHandlingThrowResult"/> from the
    ///     <see cref="IExceptionHandler.TryHandleException(ExceptionInformation, OperationRetrySettings, out ExceptionHandlingResult)"/> method,
    ///     if the client performing the operation is another actor. In the deadlock situations this allows the call chain to unwind all the way
    ///     back to the original client and the operation is then retried from there.
    /// </para>
    /// </description>
    /// </item>
    /// </list>
    /// </remarks>
    public class ActorRemotingExceptionHandler : IExceptionHandler
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ActorRemotingExceptionHandler"/> class which can be used to handle exceptions encountered in communicating with
        /// service fabric actors over remoted actor interfaces.
        /// </summary>
        public ActorRemotingExceptionHandler()
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

            if (e is ActorConcurrencyLockTimeoutException)
            {
                if (ActorLogicalCallContext.IsPresent())
                {
                    result = new ExceptionHandlingThrowResult()
                    {
                        ExceptionToThrow = e,
                    };
                    return true;
                }

                result = new ExceptionHandlingRetryResult(
                    e,
                    true,
                    retrySettings,
                    retrySettings.DefaultMaxRetryCountForTransientErrors);
                return true;
            }

            // The messaging layer may deliver duplicate messages during the connection failures.
            // E.g when client connection is disconnected but service is still processing the message. We retry on client connection failures.
            // This results to service receiving duplicate message.
            // And Actor Reentrancy throws DuplicateMessageException exception when it sees a duplicate Message (message with same callContext).
            if (e is DuplicateMessageException)
            {
                result = new ExceptionHandlingRetryResult(
                e,
                true,
                retrySettings,
                int.MaxValue);

                return true;
            }

            result = null;
            return false;
        }
    }
}
