// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Describes a container and its runtime properties.
    /// </summary>
    public partial class ContainerCodePackageProperties
    {
        /// <summary>
        /// Initializes a new instance of the ContainerCodePackageProperties class.
        /// </summary>
        /// <param name="name">The name of the code package.</param>
        /// <param name="image">The Container image to use.</param>
        /// <param name="resources">The resources required by this container.</param>
        /// <param name="imageRegistryCredential">Image registry credential.</param>
        /// <param name="entryPoint">Override for the default entry point in the container.</param>
        /// <param name="commands">Command array to execute within the container in exec form.</param>
        /// <param name="environmentVariables">The environment variables to set in this container</param>
        /// <param name="settings">The settings to set in this container. The setting file path can be fetched from environment
        /// variable "Fabric_SettingPath". The path for Windows container is "C:\\secrets". The path for Linux container is
        /// "/var/secrets".</param>
        /// <param name="labels">The labels to set in this container.</param>
        /// <param name="endpoints">The endpoints exposed by this container.</param>
        /// <param name="volumeRefs">Volumes to be attached to the container. The lifetime of these volumes is independent of
        /// the application's lifetime.</param>
        /// <param name="volumes">Volumes to be attached to the container. The lifetime of these volumes is scoped to the
        /// application's lifetime.</param>
        /// <param name="diagnostics">Reference to sinks in DiagnosticsDescription.</param>
        /// <param name="reliableCollectionsRefs">A list of ReliableCollection resources used by this particular code package.
        /// Please refer to ReliableCollectionsRef for more details.</param>
        /// <param name="livenessProbe">An array of liveness probes for a code package. It determines when to restart a code
        /// package.</param>
        /// <param name="readinessProbe">An array of readiness probes for a code package. It determines when to unpublish an
        /// endpoint.</param>
        public ContainerCodePackageProperties(
            string name,
            string image,
            ResourceRequirements resources,
            ImageRegistryCredential imageRegistryCredential = default(ImageRegistryCredential),
            string entryPoint = default(string),
            IEnumerable<string> commands = default(IEnumerable<string>),
            IEnumerable<EnvironmentVariable> environmentVariables = default(IEnumerable<EnvironmentVariable>),
            IEnumerable<Setting> settings = default(IEnumerable<Setting>),
            IEnumerable<ContainerLabel> labels = default(IEnumerable<ContainerLabel>),
            IEnumerable<EndpointProperties> endpoints = default(IEnumerable<EndpointProperties>),
            IEnumerable<VolumeReference> volumeRefs = default(IEnumerable<VolumeReference>),
            IEnumerable<ApplicationScopedVolume> volumes = default(IEnumerable<ApplicationScopedVolume>),
            DiagnosticsRef diagnostics = default(DiagnosticsRef),
            IEnumerable<ReliableCollectionsRef> reliableCollectionsRefs = default(IEnumerable<ReliableCollectionsRef>),
            IEnumerable<Probe> livenessProbe = default(IEnumerable<Probe>),
            IEnumerable<Probe> readinessProbe = default(IEnumerable<Probe>))
        {
            name.ThrowIfNull(nameof(name));
            image.ThrowIfNull(nameof(image));
            resources.ThrowIfNull(nameof(resources));
            this.Name = name;
            this.Image = image;
            this.Resources = resources;
            this.ImageRegistryCredential = imageRegistryCredential;
            this.EntryPoint = entryPoint;
            this.Commands = commands;
            this.EnvironmentVariables = environmentVariables;
            this.Settings = settings;
            this.Labels = labels;
            this.Endpoints = endpoints;
            this.VolumeRefs = volumeRefs;
            this.Volumes = volumes;
            this.Diagnostics = diagnostics;
            this.ReliableCollectionsRefs = reliableCollectionsRefs;
            this.LivenessProbe = livenessProbe;
            this.ReadinessProbe = readinessProbe;
        }

        /// <summary>
        /// Gets the name of the code package.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the Container image to use.
        /// </summary>
        public string Image { get; }

        /// <summary>
        /// Gets image registry credential.
        /// </summary>
        public ImageRegistryCredential ImageRegistryCredential { get; }

        /// <summary>
        /// Gets override for the default entry point in the container.
        /// </summary>
        public string EntryPoint { get; }

        /// <summary>
        /// Gets command array to execute within the container in exec form.
        /// </summary>
        public IEnumerable<string> Commands { get; }

        /// <summary>
        /// Gets the environment variables to set in this container
        /// </summary>
        public IEnumerable<EnvironmentVariable> EnvironmentVariables { get; }

        /// <summary>
        /// Gets the settings to set in this container. The setting file path can be fetched from environment variable
        /// "Fabric_SettingPath". The path for Windows container is "C:\\secrets". The path for Linux container is
        /// "/var/secrets".
        /// </summary>
        public IEnumerable<Setting> Settings { get; }

        /// <summary>
        /// Gets the labels to set in this container.
        /// </summary>
        public IEnumerable<ContainerLabel> Labels { get; }

        /// <summary>
        /// Gets the endpoints exposed by this container.
        /// </summary>
        public IEnumerable<EndpointProperties> Endpoints { get; }

        /// <summary>
        /// Gets the resources required by this container.
        /// </summary>
        public ResourceRequirements Resources { get; }

        /// <summary>
        /// Gets volumes to be attached to the container. The lifetime of these volumes is independent of the application's
        /// lifetime.
        /// </summary>
        public IEnumerable<VolumeReference> VolumeRefs { get; }

        /// <summary>
        /// Gets volumes to be attached to the container. The lifetime of these volumes is scoped to the application's
        /// lifetime.
        /// </summary>
        public IEnumerable<ApplicationScopedVolume> Volumes { get; }

        /// <summary>
        /// Gets reference to sinks in DiagnosticsDescription.
        /// </summary>
        public DiagnosticsRef Diagnostics { get; }

        /// <summary>
        /// Gets a list of ReliableCollection resources used by this particular code package. Please refer to
        /// ReliableCollectionsRef for more details.
        /// </summary>
        public IEnumerable<ReliableCollectionsRef> ReliableCollectionsRefs { get; }

        /// <summary>
        /// Gets runtime information of a container instance.
        /// </summary>
        public ContainerInstanceView InstanceView { get; internal set; }

        /// <summary>
        /// Gets an array of liveness probes for a code package. It determines when to restart a code package.
        /// </summary>
        public IEnumerable<Probe> LivenessProbe { get; }

        /// <summary>
        /// Gets an array of readiness probes for a code package. It determines when to unpublish an endpoint.
        /// </summary>
        public IEnumerable<Probe> ReadinessProbe { get; }
    }
}
