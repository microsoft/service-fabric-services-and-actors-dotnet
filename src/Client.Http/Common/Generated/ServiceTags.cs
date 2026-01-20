// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Wrapper for service tags - TagsRequiredToPlace and TagsRequiredToRun.
    /// </summary>
    public partial class ServiceTags
    {
        /// <summary>
        /// Initializes a new instance of the ServiceTags class.
        /// </summary>
        public ServiceTags(
            IEnumerable<string> tagsRequiredToPlace = default(IEnumerable<string>),
            IEnumerable<string> tagsRequiredToRun = default(IEnumerable<string>))
        {
            this.TagsRequiredToPlace = tagsRequiredToPlace;
            this.TagsRequiredToRun = tagsRequiredToRun;
        }

        /// <summary>
        /// </summary>
        public IEnumerable<string> TagsRequiredToPlace { get; }

        /// <summary>
        /// </summary>
        public IEnumerable<string> TagsRequiredToRun { get; }
    }
}
