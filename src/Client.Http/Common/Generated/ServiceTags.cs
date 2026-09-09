// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Wrapper for service tags - TagsRequiredToPlace, TagsRequiredToRun and ServiceTags.
    /// </summary>
    public partial class ServiceTags
    {
        /// <summary>
        /// Initializes a new instance of the ServiceTags class.
        /// </summary>
        public ServiceTags(
            IEnumerable<string> tagsRequiredToPlace = default(IEnumerable<string>),
            IEnumerable<string> tagsRequiredToRun = default(IEnumerable<string>),
            IEnumerable<string> tags = default(IEnumerable<string>))
        {
            this.TagsRequiredToPlace = tagsRequiredToPlace;
            this.TagsRequiredToRun = tagsRequiredToRun;
            this.Tags = tags;
        }

        /// <summary>
        /// </summary>
        public IEnumerable<string> TagsRequiredToPlace { get; }

        /// <summary>
        /// </summary>
        public IEnumerable<string> TagsRequiredToRun { get; }

        /// <summary>
        /// </summary>
        public IEnumerable<string> Tags { get; }
    }
}
