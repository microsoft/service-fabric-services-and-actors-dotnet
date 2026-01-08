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
    /// Creates or updates a Service Fabric property.
    /// </summary>
    [Cmdlet(VerbsCommon.New, "SFProperty")]
    public partial class NewPropertyCmdlet : CommonCmdletBase
    {
        /// <summary>
        /// Gets or sets Binary flag
        /// </summary>
        [Parameter(Mandatory = false, Position = 0, ParameterSetName = "_Binary_")]
        public SwitchParameter Binary { get; set; }

        /// <summary>
        /// Gets or sets Int64 flag
        /// </summary>
        [Parameter(Mandatory = false, Position = 0, ParameterSetName = "_Int64_")]
        public SwitchParameter Int64 { get; set; }

        /// <summary>
        /// Gets or sets Double flag
        /// </summary>
        [Parameter(Mandatory = false, Position = 0, ParameterSetName = "_Double_")]
        public SwitchParameter Double { get; set; }

        /// <summary>
        /// Gets or sets String flag
        /// </summary>
        [Parameter(Mandatory = false, Position = 0, ParameterSetName = "_String_")]
        public SwitchParameter String { get; set; }

        /// <summary>
        /// Gets or sets Guid flag
        /// </summary>
        [Parameter(Mandatory = false, Position = 0, ParameterSetName = "_Guid_")]
        public SwitchParameter Guid { get; set; }

        /// <summary>
        /// Gets or sets NameId. The Service Fabric name, without the 'fabric:' URI scheme.
        /// </summary>
        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 1)]
        public string NameId { get; set; }

        /// <summary>
        /// Gets or sets PropertyName. The name of the Service Fabric property.
        /// </summary>
        [Parameter(Mandatory = true, Position = 2)]
        public string PropertyName { get; set; }

        /// <summary>
        /// Gets or sets Data. Array of bytes to be sent as an integer array. Each element of array is a number between 0 and
        /// 255.
        /// </summary>
        [Parameter(Mandatory = true, Position = 3, ParameterSetName = "_Binary_")]
        public byte[] BinaryData { get; set; }

        /// <summary>
        /// Gets or sets Data.
        /// </summary>
        [Parameter(Mandatory = true, Position = 3, ParameterSetName = "_Int64_")]
        [Parameter(Mandatory = true, Position = 3, ParameterSetName = "_Double_")]
        [Parameter(Mandatory = true, Position = 3, ParameterSetName = "_String_")]
        [Parameter(Mandatory = true, Position = 3, ParameterSetName = "_Guid_")]
        public string Data { get; set; }

        /// <summary>
        /// Gets or sets CustomTypeId. The property's custom type ID. Using this property, the user is able to tag the type of
        /// the value of the property.
        /// </summary>
        [Parameter(Mandatory = false, Position = 4)]
        public string CustomTypeId { get; set; }

        /// <summary>
        /// Gets or sets ServerTimeout. The server timeout for performing the operation in seconds. This timeout specifies the
        /// time duration that the client is willing to wait for the requested operation to complete. The default value for
        /// this parameter is 60 seconds.
        /// </summary>
        [Parameter(Mandatory = false, Position = 5)]
        public long? ServerTimeout { get; set; }

        /// <inheritdoc/>
        protected override void ProcessRecordInternal()
        {
            PropertyValue propertyValue = null;
            if (this.Binary.IsPresent)
            {
                propertyValue = new BinaryPropertyValue(
                    data: this.BinaryData);
            }
            else if (this.Int64.IsPresent)
            {
                propertyValue = new Int64PropertyValue(
                    data: this.Data);
            }
            else if (this.Double.IsPresent)
            {
                propertyValue = new DoublePropertyValue(
                    data: double.Parse(this.Data));
            }
            else if (this.String.IsPresent)
            {
                propertyValue = new StringPropertyValue(
                    data: this.Data);
            }
            else if (this.Guid.IsPresent)
            {
                propertyValue = new GuidPropertyValue(
                    data: new Guid(this.Data));
            }

            var propertyDescription = new PropertyDescription(
            propertyName: this.PropertyName,
            value: propertyValue,
            customTypeId: this.CustomTypeId);

            this.ServiceFabricClient.Properties.PutPropertyAsync(
                nameId: this.NameId,
                propertyDescription: propertyDescription,
                serverTimeout: this.ServerTimeout,
                cancellationToken: this.CancellationToken).GetAwaiter().GetResult();

            Console.WriteLine("Success!");
        }
    }
}
