// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Client
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.ServiceFabric.Client.Exceptions;
    using Microsoft.ServiceFabric.Common;
    using Microsoft.ServiceFabric.Common.Exceptions;

    /// <summary>
    /// Interface containing methods for performing ChaosClient operations.
    /// </summary>
    public partial interface IChaosClient
    {
        /// <summary>
        /// Get the status of Chaos.
        /// </summary>
        /// <remarks>
        /// Get the status of Chaos indicating whether or not Chaos is running, the Chaos parameters used for running Chaos and
        /// the status of the Chaos Schedule.
        /// </remarks>
        /// <param name ="serverTimeout">The server timeout for performing the operation in seconds. This timeout specifies the
        /// time duration that the client is willing to wait for the requested operation to complete. The default value for
        /// this parameter is 60 seconds.</param>
        /// <param name ="cancellationToken">Cancels the client-side operation.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        /// <exception cref="InvalidCredentialsException">Thrown when invalid credentials are used while making request to cluster.</exception>
        /// <exception cref="ServiceFabricRequestException">Thrown when request to Service Fabric cluster failed due to an underlying issue such as network connectivity, DNS failure or timeout.</exception>
        /// <exception cref="ServiceFabricException">Thrown when the requested operation failed at server. Exception contains Error code <see cref="FabricError.ErrorCode"/>, message indicating the failure. It also contains a flag wether the exception is transient or not, client operations can be retried if its transient.</exception>
        /// <exception cref="OperationCanceledException">Thrown when cancellation is requested for the cancellation token.</exception>
        Task<Chaos> GetChaosAsync(
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Starts Chaos in the cluster.
        /// </summary>
        /// <remarks>
        /// If Chaos is not already running in the cluster, it starts Chaos with the passed in Chaos parameters.
        /// If Chaos is already running when this call is made, the call fails with the error code
        /// FABRIC_E_CHAOS_ALREADY_RUNNING.
        /// Refer to the article [Induce controlled Chaos in Service Fabric
        /// clusters](https://docs.microsoft.com/azure/service-fabric/service-fabric-controlled-chaos) for more details.
        /// </remarks>
        /// <param name ="chaosParameters">Describes all the parameters to configure a Chaos run.</param>
        /// <param name ="serverTimeout">The server timeout for performing the operation in seconds. This timeout specifies the
        /// time duration that the client is willing to wait for the requested operation to complete. The default value for
        /// this parameter is 60 seconds.</param>
        /// <param name ="cancellationToken">Cancels the client-side operation.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        /// <exception cref="InvalidCredentialsException">Thrown when invalid credentials are used while making request to cluster.</exception>
        /// <exception cref="ServiceFabricRequestException">Thrown when request to Service Fabric cluster failed due to an underlying issue such as network connectivity, DNS failure or timeout.</exception>
        /// <exception cref="ServiceFabricException">Thrown when the requested operation failed at server. Exception contains Error code <see cref="FabricError.ErrorCode"/>, message indicating the failure. It also contains a flag wether the exception is transient or not, client operations can be retried if its transient.</exception>
        /// <exception cref="OperationCanceledException">Thrown when cancellation is requested for the cancellation token.</exception>
        Task StartChaosAsync(
            ChaosParameters chaosParameters,
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Stops Chaos if it is running in the cluster and put the Chaos Schedule in a stopped state.
        /// </summary>
        /// <remarks>
        /// Stops Chaos from executing new faults. In-flight faults will continue to execute until they are complete. The
        /// current Chaos Schedule is put into a stopped state.
        /// Once a schedule is stopped, it will stay in the stopped state and not be used to Chaos Schedule new runs of Chaos.
        /// A new Chaos Schedule must be set in order to resume scheduling.
        /// </remarks>
        /// <param name ="serverTimeout">The server timeout for performing the operation in seconds. This timeout specifies the
        /// time duration that the client is willing to wait for the requested operation to complete. The default value for
        /// this parameter is 60 seconds.</param>
        /// <param name ="cancellationToken">Cancels the client-side operation.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        /// <exception cref="InvalidCredentialsException">Thrown when invalid credentials are used while making request to cluster.</exception>
        /// <exception cref="ServiceFabricRequestException">Thrown when request to Service Fabric cluster failed due to an underlying issue such as network connectivity, DNS failure or timeout.</exception>
        /// <exception cref="ServiceFabricException">Thrown when the requested operation failed at server. Exception contains Error code <see cref="FabricError.ErrorCode"/>, message indicating the failure. It also contains a flag wether the exception is transient or not, client operations can be retried if its transient.</exception>
        /// <exception cref="OperationCanceledException">Thrown when cancellation is requested for the cancellation token.</exception>
        Task StopChaosAsync(
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Gets the next segment of the Chaos events based on the continuation token or the time range.
        /// </summary>
        /// <remarks>
        /// To get the next segment of the Chaos events, you can specify the ContinuationToken. To get the start of a new
        /// segment of Chaos events, you can specify the time range
        /// through StartTimeUtc and EndTimeUtc. You cannot specify both the ContinuationToken and the time range in the same
        /// call.
        /// When there are more than 100 Chaos events, the Chaos events are returned in multiple segments where a segment
        /// contains no more than 100 Chaos events and to get the next segment you make a call to this API with the
        /// continuation token.
        /// </remarks>
        /// <param name ="continuationToken">The continuation token to obtain next set of results</param>
        /// <param name ="startTimeUtc">The Windows file time representing the start time of the time range for which a Chaos
        /// report is to be generated. Consult [DateTime.ToFileTimeUtc
        /// Method](https://msdn.microsoft.com/library/system.datetime.tofiletimeutc(v=vs.110).aspx) for details.</param>
        /// <param name ="endTimeUtc">The Windows file time representing the end time of the time range for which a Chaos
        /// report is to be generated. Consult [DateTime.ToFileTimeUtc
        /// Method](https://msdn.microsoft.com/library/system.datetime.tofiletimeutc(v=vs.110).aspx) for details.</param>
        /// <param name ="maxResults">The maximum number of results to be returned as part of the paged queries. This parameter
        /// defines the upper bound on the number of results returned. The results returned can be less than the specified
        /// maximum results if they do not fit in the message as per the max message size restrictions defined in the
        /// configuration. If this parameter is zero or not specified, the paged query includes as many results as possible
        /// that fit in the return message.</param>
        /// <param name ="serverTimeout">The server timeout for performing the operation in seconds. This timeout specifies the
        /// time duration that the client is willing to wait for the requested operation to complete. The default value for
        /// this parameter is 60 seconds.</param>
        /// <param name ="cancellationToken">Cancels the client-side operation.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        /// <exception cref="InvalidCredentialsException">Thrown when invalid credentials are used while making request to cluster.</exception>
        /// <exception cref="ServiceFabricRequestException">Thrown when request to Service Fabric cluster failed due to an underlying issue such as network connectivity, DNS failure or timeout.</exception>
        /// <exception cref="ServiceFabricException">Thrown when the requested operation failed at server. Exception contains Error code <see cref="FabricError.ErrorCode"/>, message indicating the failure. It also contains a flag wether the exception is transient or not, client operations can be retried if its transient.</exception>
        /// <exception cref="OperationCanceledException">Thrown when cancellation is requested for the cancellation token.</exception>
        Task<PagedData<ChaosEventWrapper>> GetChaosEventsAsync(
            ContinuationToken continuationToken = default(ContinuationToken),
            string startTimeUtc = default(string),
            string endTimeUtc = default(string),
            long? maxResults = 0,
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Get the Chaos Schedule defining when and how to run Chaos.
        /// </summary>
        /// <remarks>
        /// Gets the version of the Chaos Schedule in use and the Chaos Schedule that defines when and how to run Chaos.
        /// </remarks>
        /// <param name ="serverTimeout">The server timeout for performing the operation in seconds. This timeout specifies the
        /// time duration that the client is willing to wait for the requested operation to complete. The default value for
        /// this parameter is 60 seconds.</param>
        /// <param name ="cancellationToken">Cancels the client-side operation.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        /// <exception cref="InvalidCredentialsException">Thrown when invalid credentials are used while making request to cluster.</exception>
        /// <exception cref="ServiceFabricRequestException">Thrown when request to Service Fabric cluster failed due to an underlying issue such as network connectivity, DNS failure or timeout.</exception>
        /// <exception cref="ServiceFabricException">Thrown when the requested operation failed at server. Exception contains Error code <see cref="FabricError.ErrorCode"/>, message indicating the failure. It also contains a flag wether the exception is transient or not, client operations can be retried if its transient.</exception>
        /// <exception cref="OperationCanceledException">Thrown when cancellation is requested for the cancellation token.</exception>
        Task<ChaosScheduleDescription> GetChaosScheduleAsync(
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Set the schedule used by Chaos.
        /// </summary>
        /// <remarks>
        /// Chaos will automatically schedule runs based on the Chaos Schedule.
        /// The Chaos Schedule will be updated if the provided version matches the version on the server.
        /// When updating the Chaos Schedule, the version on the server is incremented by 1.
        /// The version on the server will wrap back to 0 after reaching a large number.
        /// If Chaos is running when this call is made, the call will fail.
        /// </remarks>
        /// <param name ="chaosSchedule">Describes the schedule used by Chaos.</param>
        /// <param name ="serverTimeout">The server timeout for performing the operation in seconds. This timeout specifies the
        /// time duration that the client is willing to wait for the requested operation to complete. The default value for
        /// this parameter is 60 seconds.</param>
        /// <param name ="cancellationToken">Cancels the client-side operation.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        /// <exception cref="InvalidCredentialsException">Thrown when invalid credentials are used while making request to cluster.</exception>
        /// <exception cref="ServiceFabricRequestException">Thrown when request to Service Fabric cluster failed due to an underlying issue such as network connectivity, DNS failure or timeout.</exception>
        /// <exception cref="ServiceFabricException">Thrown when the requested operation failed at server. Exception contains Error code <see cref="FabricError.ErrorCode"/>, message indicating the failure. It also contains a flag wether the exception is transient or not, client operations can be retried if its transient.</exception>
        /// <exception cref="OperationCanceledException">Thrown when cancellation is requested for the cancellation token.</exception>
        Task PostChaosScheduleAsync(
            ChaosScheduleDescription chaosSchedule,
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken));
    }
}
