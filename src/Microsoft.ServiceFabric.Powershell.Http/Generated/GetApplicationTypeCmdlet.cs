// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Powershell.Http
{
    using System;
    using System.Collections.Generic;
    using System.Management.Automation;
    using Microsoft.ServiceFabric.Common;

    /// <summary>
    /// Gets the list of application types in the Service Fabric cluster.
    /// </summary>
    [Cmdlet(VerbsCommon.Get, "SFApplicationType", DefaultParameterSetName = "GetApplicationTypeInfoList")]
    public partial class GetApplicationTypeCmdlet : CommonCmdletBase
    {
        /// <summary>
        /// Gets or sets ApplicationTypeName. The name of the application type.
        /// </summary>
        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 0, ParameterSetName = "GetApplicationTypeInfoListByName")]
        public string ApplicationTypeName { get; set; }

        /// <summary>
        /// Gets or sets ApplicationTypeDefinitionKindFilter. Used to filter on ApplicationTypeDefinitionKind which is the
        /// mechanism used to define a Service Fabric application type.
        /// - Default - Default value, which performs the same function as selecting "All". The value is 0.
        /// - All - Filter that matches input with any ApplicationTypeDefinitionKind value. The value is 65535.
        /// - ServiceFabricApplicationPackage - Filter that matches input with ApplicationTypeDefinitionKind value
        /// ServiceFabricApplicationPackage. The value is 1.
        /// - Compose - Filter that matches input with ApplicationTypeDefinitionKind value Compose. The value is 2.
        /// </summary>
        [Parameter(Mandatory = false, Position = 1, ParameterSetName = "GetApplicationTypeInfoList")]
        public int? ApplicationTypeDefinitionKindFilter { get; set; }

        /// <summary>
        /// Gets or sets ExcludeApplicationParameters. The flag that specifies whether application parameters will be excluded
        /// from the result.
        /// </summary>
        [Parameter(Mandatory = false, Position = 2, ParameterSetName = "GetApplicationTypeInfoList")]
        [Parameter(Mandatory = false, Position = 2, ParameterSetName = "GetApplicationTypeInfoListByName")]
        public bool? ExcludeApplicationParameters { get; set; }

        /// <summary>
        /// Gets or sets MaxResults. The maximum number of results to be returned as part of the paged queries. This parameter
        /// defines the upper bound on the number of results returned. The results returned can be less than the specified
        /// maximum results if they do not fit in the message as per the max message size restrictions defined in the
        /// configuration. If this parameter is zero or not specified, the paged query includes as many results as possible
        /// that fit in the return message.
        /// </summary>
        [Parameter(Mandatory = false, Position = 3, ParameterSetName = "GetApplicationTypeInfoList")]
        [Parameter(Mandatory = false, Position = 3, ParameterSetName = "GetApplicationTypeInfoListByName")]
        public long? MaxResults { get; set; }

        /// <summary>
        /// Gets or sets ServerTimeout. The server timeout for performing the operation in seconds. This timeout specifies the
        /// time duration that the client is willing to wait for the requested operation to complete. The default value for
        /// this parameter is 60 seconds.
        /// </summary>
        [Parameter(Mandatory = false, Position = 4, ParameterSetName = "GetApplicationTypeInfoList")]
        [Parameter(Mandatory = false, Position = 4, ParameterSetName = "GetApplicationTypeInfoListByName")]
        public long? ServerTimeout { get; set; }

        /// <summary>
        /// Gets or sets ApplicationTypeVersion. The version of the application type.
        /// </summary>
        [Parameter(Mandatory = false, Position = 5, ParameterSetName = "GetApplicationTypeInfoListByName")]
        public string ApplicationTypeVersion { get; set; }

        /// <inheritdoc/>
        protected override void ProcessRecordInternal()
        {
            if (this.ParameterSetName.Equals("GetApplicationTypeInfoList"))
            {
                var continuationToken = default(ContinuationToken);
                do
                {
                    var result = this.ServiceFabricClient.ApplicationTypes.GetApplicationTypeInfoListAsync(
                        applicationTypeDefinitionKindFilter: this.ApplicationTypeDefinitionKindFilter,
                        excludeApplicationParameters: this.ExcludeApplicationParameters,
                        continuationToken: continuationToken,
                        maxResults: this.MaxResults,
                        serverTimeout: this.ServerTimeout,
                        cancellationToken: this.CancellationToken).GetAwaiter().GetResult();

                    if (result == null)
                    {
                        break;
                    }

                    var count = 0;
                    foreach (var item in result.Data)
                    {
                        count++;
                        this.WriteObject(this.FormatOutput(item));
                    }

                    continuationToken = result.ContinuationToken;
                    this.WriteDebug(string.Format(Resource.MsgCountAndContinuationToken, count, continuationToken));
                }
                while (continuationToken.Next);
            }
            else if (this.ParameterSetName.Equals("GetApplicationTypeInfoListByName"))
            {
                var continuationToken = default(ContinuationToken);
                do
                {
                    var result = this.ServiceFabricClient.ApplicationTypes.GetApplicationTypeInfoListByNameAsync(
                        applicationTypeName: this.ApplicationTypeName,
                        applicationTypeVersion: this.ApplicationTypeVersion,
                        excludeApplicationParameters: this.ExcludeApplicationParameters,
                        continuationToken: continuationToken,
                        maxResults: this.MaxResults,
                        serverTimeout: this.ServerTimeout,
                        cancellationToken: this.CancellationToken).GetAwaiter().GetResult();

                    if (result == null)
                    {
                        break;
                    }

                    var count = 0;
                    foreach (var item in result.Data)
                    {
                        count++;
                        this.WriteObject(this.FormatOutput(item));
                    }

                    continuationToken = result.ContinuationToken;
                    this.WriteDebug(string.Format(Resource.MsgCountAndContinuationToken, count, continuationToken));
                }
                while (continuationToken.Next);
            }
        }

        /// <inheritdoc/>
        protected override object FormatOutput(object output)
        {
            var outputResult = output as ApplicationTypeInfo;

            var managedKeyVaultReferenceParametersObj = new PSObject(outputResult.ManagedKeyVaultReferenceParameters);
            managedKeyVaultReferenceParametersObj.Members.Add(new PSCodeMethod("ToString", typeof(OutputFormatter).GetMethod("FormatObject")));

            var result = new PSObject(outputResult);

            result.Properties.Add(new PSNoteProperty("ApplicationTypeName", outputResult.Name));
            result.Properties.Add(new PSNoteProperty("ApplicationTypeVersion", outputResult.Version));
            result.Properties.Add(new PSNoteProperty("ApplicationTypeParameterList", outputResult.DefaultParameterList));
            result.Properties.Add(new PSNoteProperty("ApplicationTypeStatus", outputResult.Status));
            result.Properties.Add(new PSNoteProperty("ApplicationTypeDefinitionKind", outputResult.ApplicationTypeDefinitionKind));
            result.Properties.Add(new PSNoteProperty("ApplicationTypeMetadata", outputResult.ApplicationTypeMetadata));

            return result;
        }
    }
}
