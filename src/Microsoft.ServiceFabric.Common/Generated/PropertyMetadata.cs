// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The metadata associated with a property, including the property's name.
    /// </summary>
    public partial class PropertyMetadata
    {
        /// <summary>
        /// Initializes a new instance of the PropertyMetadata class.
        /// </summary>
        /// <param name="typeId">The kind of property, determined by the type of data. Following are the possible values.
        /// Possible values include: 'Invalid', 'Binary', 'Int64', 'Double', 'String', 'Guid'</param>
        /// <param name="customTypeId">The property's custom type ID.</param>
        /// <param name="parent">The name of the parent Service Fabric Name for the property. It could be thought of as the
        /// name-space/table under which the property exists.</param>
        /// <param name="sizeInBytes">The length of the serialized property value.</param>
        /// <param name="lastModifiedUtcTimestamp">Represents when the Property was last modified. Only write operations will
        /// cause this field to be updated.</param>
        /// <param name="sequenceNumber">The version of the property. Every time a property is modified, its sequence number is
        /// increased.</param>
        public PropertyMetadata(
            PropertyValueKind? typeId = default(PropertyValueKind?),
            string customTypeId = default(string),
            FabricName parent = default(FabricName),
            int? sizeInBytes = default(int?),
            DateTime? lastModifiedUtcTimestamp = default(DateTime?),
            string sequenceNumber = default(string))
        {
            this.TypeId = typeId;
            this.CustomTypeId = customTypeId;
            this.Parent = parent;
            this.SizeInBytes = sizeInBytes;
            this.LastModifiedUtcTimestamp = lastModifiedUtcTimestamp;
            this.SequenceNumber = sequenceNumber;
        }

        /// <summary>
        /// Gets the kind of property, determined by the type of data. Following are the possible values. Possible values
        /// include: 'Invalid', 'Binary', 'Int64', 'Double', 'String', 'Guid'
        /// </summary>
        public PropertyValueKind? TypeId { get; }

        /// <summary>
        /// Gets the property's custom type ID.
        /// </summary>
        public string CustomTypeId { get; }

        /// <summary>
        /// Gets the name of the parent Service Fabric Name for the property. It could be thought of as the name-space/table
        /// under which the property exists.
        /// </summary>
        public FabricName Parent { get; }

        /// <summary>
        /// Gets the length of the serialized property value.
        /// </summary>
        public int? SizeInBytes { get; }

        /// <summary>
        /// Gets represents when the Property was last modified. Only write operations will cause this field to be updated.
        /// </summary>
        public DateTime? LastModifiedUtcTimestamp { get; }

        /// <summary>
        /// Gets the version of the property. Every time a property is modified, its sequence number is increased.
        /// </summary>
        public string SequenceNumber { get; }
    }
}
