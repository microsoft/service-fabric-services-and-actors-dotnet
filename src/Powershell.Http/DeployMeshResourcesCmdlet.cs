// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management.Automation;
using Microsoft.ServiceFabric.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SfSbzYamlMergeUtils;

namespace Microsoft.ServiceFabric.Powershell.Http
{
    /// <summary>
    /// Deploys mesh resources in a Service Fabric Mesh cluster.
    /// </summary>
    [Cmdlet(VerbsCommon.New, "SFMeshResourceDeployment")]
    public class DeployMeshResourcesCmdlet : CommonCmdletBase
    {
        const char FullyQualifiedResourceNameSeparator = '/';

        enum ResourceType
        {
            Application,
            Volume,
            Secret,
            SecretValue,
            Network,
            Unknown,
        }

        /// <summary>
        /// Gets or sets Resource Description Files, which is a list of yaml definitions for the resources
        /// </summary>
        [Parameter(Mandatory = true, ParameterSetName = "Default")]
        public string[] ResourceDescriptionList { get; set; }

        /// <summary>
        /// Gets or sets the path to parameter file containing parameter values to be replaced in the yaml. Values to be parameterized are specified in yaml files as "[parameters('ParamName')]".
        /// </summary>
        [Parameter(Mandatory = false, ParameterSetName = "Default")]
        public string ParameterFileName { get; set; }

        /// <summary>
        /// Gets or sets the path to secrets parameter file containing parameter values to be replaced in the yaml. Values to be parameterized are specified in yaml files as "[parameters('ParamName')]".
        /// </summary>
        [Parameter(Mandatory = false, ParameterSetName = "Default")]
        public string SecretsParameterFileName { get; set; }

        /// <summary>
        /// Gets or sets the output directory for the generated resource descriptions.
        /// </summary>
        [Parameter(Mandatory = false, ParameterSetName = "Default")]
        public string OutputDirectory { get; set; }

        /// <inheritdoc />
        protected override void ProcessRecordInternal()
        {
            IServiceFabricClient client = this.ServiceFabricClient;

            string outputDir = this.OutputDirectory;
            if (string.IsNullOrEmpty(this.OutputDirectory))
                outputDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());

            // Send the yaml list and the out dir to the util
            IEnumerable<ResourceInformation> resources = this.GetResourceInfoFromYamls(outputDir);
            foreach (ResourceInformation resource in resources)
            {
                switch (resource.Type)
                {
                    case ResourceType.Volume:
                        client.MeshVolumes.CreateOrUpdateAsync(resource.Name, resource.Description.ToString(), resource.ApiVersion, this.CancellationToken).GetAwaiter().GetResult();
                        break;

                    case ResourceType.Application:
                        client.MeshApplications.CreateOrUpdateAsync(resource.Name, resource.Description.ToString(), resource.ApiVersion, this.CancellationToken).GetAwaiter().GetResult();
                        break;

                    case ResourceType.Secret:
                        client.MeshSecrets.CreateOrUpdateAsync(resource.Name, resource.Description.ToString(), resource.ApiVersion, this.CancellationToken).GetAwaiter().GetResult();
                        break;

                    case ResourceType.SecretValue:
                        string secretResourcename = resource.FullyQualifiedResourceName.Split(FullyQualifiedResourceNameSeparator)[0];
                        client.MeshSecretValues.AddValueAsync(secretResourcename, resource.Name, resource.Description.ToString(), resource.ApiVersion, this.CancellationToken).GetAwaiter().GetResult();
                        break;

                    case ResourceType.Network:
                        client.MeshNetworks.CreateOrUpdateAsync(resource.Name, resource.Description.ToString(), resource.ApiVersion, this.CancellationToken).GetAwaiter().GetResult();
                        break;

                    default:
                        this.WriteWarning(string.Format(Resource.WarningInvalidResourceType, resource.Type.ToString()));
                        break;
                }
            }

            // Clear output dir if it wasnt specified on commandline.
            if (string.IsNullOrEmpty(this.OutputDirectory))
                Directory.Delete(outputDir, true);
        }

        IEnumerable<ResourceInformation> GetResourceInfoFromYamls(string outputRootDirectory)
        {
            // Give input to merge tool all the yamlfile list and output folder and of type:SF_SBZ_JSON
            string filePrefix = "resource_";
            SfSbzYamlMergeLib.GenerateMergedDescriptions(this.ResourceDescriptionList, outputRootDirectory, this.ParameterFileName, this.SecretsParameterFileName, OutputType.SF_SBZ_JSON, filePrefix);

            // Read ResourceInformation from all files sorted by name.
            IOrderedEnumerable<FileInfo> files = Directory.GetFiles(outputRootDirectory, $"{filePrefix}*.json", SearchOption.AllDirectories).Select(file => new FileInfo(file)).OrderBy(f => f.Name);
            return files.Select((file) => JsonConvert.DeserializeObject<ResourceInformation>(File.ReadAllText(file.FullName)));
        }

        sealed class ResourceInformation
        {
            [JsonProperty("type")]
            public ResourceType Type { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("api-version")]
            public string ApiVersion { get; set; }

            [JsonProperty("fullyQualifiedResourceName")]
            public string FullyQualifiedResourceName { get; set; }

            [JsonProperty("description")]
            public JObject Description { get; set; }
        }
    }
}
