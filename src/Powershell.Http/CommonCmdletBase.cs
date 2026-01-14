// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Management.Automation;
using System.Reflection;
using System.Threading;
using Microsoft.ServiceFabric.Client;
using Microsoft.ServiceFabric.Client.Exceptions;

namespace Microsoft.ServiceFabric.Powershell.Http
{
    /// <summary>
    /// Base class for Service Fabric Powershell Commandlets.
    /// </summary>
    public abstract class CommonCmdletBase : PSCmdlet
    {
        CancellationTokenSource cancellationTokenSource;

        /// <summary>
        /// Gets the service fabric client object
        /// </summary>
        protected IServiceFabricClient ServiceFabricClient
        {
            get
            {
                var client = (IServiceFabricClient)SessionState.PSVariable.GetValue(Constants.ClusterConnectionVariableName);
                if (client == null)
                    throw new InvalidOperationException(Resource.ErrorNotConnected);
                return client;
            }
        }

        /// <summary>
        /// Gets the cancellation Token object
        /// </summary>
        protected CancellationToken CancellationToken
        {
            get
            {
                if (cancellationTokenSource == null)
                    cancellationTokenSource = new CancellationTokenSource();
                return cancellationTokenSource.Token;
            }
        }

        /// <inheritdoc />
        protected override void StopProcessing()
        {
            if (cancellationTokenSource != null)
            {
                cancellationTokenSource.Cancel(true);
                cancellationTokenSource.Dispose();
            }
        }

        /// <inheritdoc />
        protected override void EndProcessing()
        {
            if (cancellationTokenSource != null)
                cancellationTokenSource.Dispose();
        }

        /// <summary>
        /// Format the output of cmdlet by adding specific property or including tables for better interpretation
        /// </summary>
        /// <param name="output"> Result returned by the PS cmdlet </param>
        /// <returns> Returns the formatted output </returns>
        protected virtual object FormatOutput(object output) => output;

        /// <summary>
        /// Function that defines the behavior of PS cmdlet. Contains the core logic of the PS cmdlet.
        /// </summary>
        protected abstract void ProcessRecordInternal();

        /// <summary>
        /// Wrapper function around the ProcessRecordInternal() of PS cmdlet. Used to handle errors in a standard way.
        /// </summary>
        protected override void ProcessRecord()
        {
            try
            {
                ProcessRecordInternal();
            }
            catch (Exception ex)
            {
                string className = GetType().Name;
                if (className.EndsWith("Cmdlet", StringComparison.OrdinalIgnoreCase))
                {
                    int index = className.LastIndexOf("Cmdlet", StringComparison.OrdinalIgnoreCase);
                    className = className.Remove(index);
                }

                string errorId = $"{className}{"ErrorId"}";
                if (ex is TargetInvocationException && ex.InnerException != null)
                    ThrowTerminatingError(ex.InnerException, errorId, null);
                else
                    ThrowTerminatingError(ex, errorId, null);
            }
        }

        /// <summary>
        /// Throws Terminating Error.
        /// </summary>
        /// <param name="exception">Exception which is caught by ProcessRecord..</param>
        /// <param name="errorId">Error Id.</param>
        /// <param name="target">Target.</param>
        protected void ThrowTerminatingError(Exception exception, string errorId, object target)
        {
            ErrorCategory errorCategory = GetErrorCategoryForException(exception);
            WriteVerbose(exception.ToString());
            ThrowTerminatingError(new ErrorRecord(exception, errorId, errorCategory, target));
        }

        static ErrorCategory GetErrorCategoryForException(Exception exception)
        {
            var errorCategory = ErrorCategory.NotSpecified;

            if (exception is ArgumentException)
                errorCategory = ErrorCategory.InvalidArgument;
            else if (exception is InvalidOperationException)
                errorCategory = ErrorCategory.InvalidOperation;
            else if (exception is TimeoutException)
                errorCategory = ErrorCategory.OperationTimeout;
            else if (exception is OperationCanceledException)
                errorCategory = ErrorCategory.OperationStopped;
            else if (exception is UnauthorizedAccessException || exception is InvalidCredentialsException)
                errorCategory = ErrorCategory.SecurityError;
            else if (exception is NullReferenceException)
                errorCategory = ErrorCategory.ResourceUnavailable;
            else if (exception is ServiceFabricRequestException)
                errorCategory = ErrorCategory.ConnectionError;

            // TODO: Can be further refined by using FabricError.ErrorCode
            return errorCategory;
        }
    }
}
