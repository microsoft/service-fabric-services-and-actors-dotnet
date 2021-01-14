// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace StartupServicesUtility
{
    using System;
    using System.Collections.Generic;
    using System.Fabric.Management.ServiceModel;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml;
    using System.Xml.Serialization;

    /// <summary>
    /// Class defining model for StartupServicesManifestType
    /// </summary>
    [XmlRoot("ApplicationManifest", Namespace = "http://schemas.microsoft.com/2011/01/fabric", IsNullable = false)]
    [XmlType(Namespace = "http://schemas.microsoft.com/2011/01/fabric")]
    public class StartupServicesManifestType
    {
        /// <summary>
        /// Gets or sets Parameters section
        /// </summary>
        [XmlArrayItem("Parameter", IsNullable = false)]
        public ApplicationManifestTypeParameter[] Parameters { get; set; }

        /// <summary>
        /// Gets or Sets DefaultServices section
        /// Entre one more comment line for testing
        /// </summary>
        public DefaultServicesType DefaultServices { get; set; }
    }
}
