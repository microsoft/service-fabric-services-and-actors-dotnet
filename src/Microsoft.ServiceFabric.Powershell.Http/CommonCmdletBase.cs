// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Powershell.Http
{
    using System;
    using System.Management.Automation;
    using System.Reflection;
    using System.Threading;
    using Microsoft.ServiceFabric.Client;
    using Microsoft.ServiceFabric.Client.Exceptions;

    /// <summary>
    /// Base class for Service Fabric Powershell Commandlets.
    /// </summary>
    public abstract class CommonCmdletBase : PSCmdlet
    {
        private CancellationTokenSource cancellationTokenSource;

        /// <summary>
        /// Gets the service fabric client object
        /// </summary>
        protected IServiceFabricClient ServiceFabricClient
        {
            get
            {
                var client = (IServiceFabricClient)this.SessionState.PSVariable.GetValue(Constants.ClusterConnectionVariableName);

                if (client == null)
                {
                    throw new InvalidOperationException(Resource.ErrorNotConnected);
                }

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
                if (this.cancellationTokenSource == null)
                {
                    this.cancellationTokenSource = new CancellationTokenSource();
                }

                return this.cancellationTokenSource.Token;
            }
        }

        /// <inheritdoc />
        protected override void StopProcessing()
        {
            if (this.cancellationTokenSource != null)
            {
                this.cancellationTokenSource.Cancel(true);
                this.cancellationTokenSource.Dispose();
            }
        }

        /// <inheritdoc />
        protected override void EndProcessing()
        {
            if (this.cancellationTokenSource != null)
            {
                this.cancellationTokenSource.Dispose();
            }
        }

        /// <summary>
        /// Format the output of cmdlet by adding specific property or including tables for better interpretation
        /// </summary>
        /// <param name="output"> Result returned by the PS cmdlet </param>
        /// <returns> Returns the formatted output </returns>
        protected virtual object FormatOutput(object output)
        {
            return output;
        }

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
                this.ProcessRecordInternal();
            }
            catch (Exception ex)
            {
                var className = this.GetType().Name;
                if (className.EndsWith("Cmdlet", StringComparison.OrdinalIgnoreCase))
                {
                    var index = className.LastIndexOf("Cmdlet", StringComparison.OrdinalIgnoreCase);
                    className = className.Remove(index);
                }

                var errorId = $"{className}{"ErrorId"}";

                if (ex is TargetInvocationException && ex.InnerException != null)
                {
                    this.ThrowTerminatingError(ex.InnerException, errorId, null);
                }
                else
                {
                    this.ThrowTerminatingError(ex, errorId, null);
                }
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
            var errorCategory = GetErrorCategoryForException(exception);

            this.WriteVerbose(exception.ToString());

            this.ThrowTerminatingError(new ErrorRecord(exception, errorId, errorCategory, target));
        }

        private static ErrorCategory GetErrorCategoryForException(Exception exception)
        {
            var errorCategory = ErrorCategory.NotSpecified;

            if (exception is ArgumentException)
            {
                errorCategory = ErrorCategory.InvalidArgument;
            }
            else if (exception is InvalidOperationException)
            {
                errorCategory = ErrorCategory.InvalidOperation;
            }
            else if (exception is TimeoutException)
            {
                errorCategory = ErrorCategory.OperationTimeout;
            }
            else if (exception is OperationCanceledException)
            {
                errorCategory = ErrorCategory.OperationStopped;
            }
            else if (exception is UnauthorizedAccessException
                     || exception is InvalidCredentialsException)
            {
                errorCategory = ErrorCategory.SecurityError;
            }
            else if (exception is NullReferenceException)
            {
                errorCategory = ErrorCategory.ResourceUnavailable;
            }
            else if (exception is ServiceFabricRequestException)
            {
                errorCategory = ErrorCategory.ConnectionError;
            }

            // TODO: Can be further refined by using FabricError.ErrorCode
            return errorCategory;
        }
    }
}
