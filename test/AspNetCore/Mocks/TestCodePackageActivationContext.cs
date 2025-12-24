// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Fabric;
using System.Fabric.Description;
using System.Fabric.Health;
using System.Linq;
using Microsoft.Extensions.Configuration;

namespace Microsoft.ServiceFabric.AspNetCore.Tests
{
    /// <summary>
    /// Test implementation of the code package activation context.
    /// </summary>
    /// <seealso cref="System.Fabric.ICodePackageActivationContext" />
    public class TestCodePackageActivationContext : ICodePackageActivationContext
    {
        private readonly IDictionary<string, IConfiguration> configs;

        // private readonly XElement manifest = null;
        private bool disposedValue = false; // To detect redundant calls

        /// <summary>
        /// Initializes a new instance of the <see cref="TestCodePackageActivationContext"/> class.
        /// </summary>
        /// <param name="config">The configuration used to populate the activation context.</param>
        public TestCodePackageActivationContext(IConfiguration config)
        {
            this.configs = new Dictionary<string, IConfiguration>() { { "Config", config } };

            // this.manifest = XElement.Load("PackageRoot\\ServiceManifest.xml");
            this.ApplicationName = config[nameof(this.ApplicationName)];
            this.ApplicationTypeName = config[nameof(this.ApplicationTypeName)];

            // this.ServiceTypes = new TestServiceTypes(config, this.manifest);
            // this.Endpoints = new TestEndPoints(config, this.manifest);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TestCodePackageActivationContext"/> class.
        /// </summary>
        /// <param name="configs">The configuration used to populate the activation context.</param>
        public TestCodePackageActivationContext(IDictionary<string, IConfiguration> configs)
        {
            this.configs = configs;

            // this.manifest = XElement.Load("PackageRoot\\ServiceManifest.xml");
            // this.ApplicationName = config[nameof(this.ApplicationName)];
            // this.ApplicationTypeName = config[nameof(this.ApplicationTypeName)];
            // this.ServiceTypes = new TestServiceTypes(config, this.manifest);
            // this.Endpoints = new TestEndPoints(config, this.manifest);
        }

#pragma warning disable CS0067 // these events are not used yet, thus disable the warning here.
        /// <summary>
        /// Event raised when new <see cref="T:System.Fabric.CodePackage" /> is added to the service manifest.
        /// </summary>
        public event EventHandler<PackageAddedEventArgs<CodePackage>> CodePackageAddedEvent;

        /// <summary>
        /// Event raised when a <see cref="T:System.Fabric.CodePackage" /> in the service manifest is modified.
        /// </summary>
        public event EventHandler<PackageModifiedEventArgs<CodePackage>> CodePackageModifiedEvent;

        /// <summary>
        /// Event raised when a <see cref="T:System.Fabric.CodePackage" /> is removed from the service manifest.
        /// </summary>
        public event EventHandler<PackageRemovedEventArgs<CodePackage>> CodePackageRemovedEvent;

        /// <summary>
        /// Event raised when new <see cref="T:System.Fabric.ConfigurationPackage" /> is added to the service manifest.
        /// </summary>
        public event EventHandler<PackageAddedEventArgs<ConfigurationPackage>> ConfigurationPackageAddedEvent;

        /// <summary>
        /// Event raised when a <see cref="T:System.Fabric.ConfigurationPackage" /> in the service manifest is modified.
        /// </summary>
        public event EventHandler<PackageModifiedEventArgs<ConfigurationPackage>> ConfigurationPackageModifiedEvent;

        /// <summary>
        /// Event raised when a <see cref="T:System.Fabric.ConfigurationPackage" /> is removed from the service manifest.
        /// </summary>
        public event EventHandler<PackageRemovedEventArgs<ConfigurationPackage>> ConfigurationPackageRemovedEvent;

        /// <summary>
        /// Event raised when new <see cref="T:System.Fabric.DataPackage" /> is added to the service manifest.
        /// </summary>
        public event EventHandler<PackageAddedEventArgs<DataPackage>> DataPackageAddedEvent;

        /// <summary>
        /// Event raised when a <see cref="T:System.Fabric.DataPackage" /> in the service manifest is modified.
        /// </summary>
        public event EventHandler<PackageModifiedEventArgs<DataPackage>> DataPackageModifiedEvent;

        /// <summary>
        /// Event raised when a <see cref="T:System.Fabric.DataPackage" /> is removed from the service manifest.
        /// </summary>
        public event EventHandler<PackageRemovedEventArgs<DataPackage>> DataPackageRemovedEvent;
#pragma warning restore CS0067

        /// <inheritdoc/>
        public string ApplicationName { get; set; }

        /// <inheritdoc/>
        public string ApplicationTypeName { get; set; }

        /// <inheritdoc/>
        public string CodePackageName { get; set; }

        /// <inheritdoc/>
        public string CodePackageVersion { get; set; }

        /// <inheritdoc/>
        public string ContextId { get; set; }

        /// <inheritdoc/>
        public string LogDirectory { get; set; }

        /// <inheritdoc/>
        public string TempDirectory { get; set; }

        /// <inheritdoc/>
        public string WorkDirectory { get; set; }

        /// <summary>
        /// Gets or sets the service types.
        /// </summary>
        /// <value>
        /// The service types.
        /// </value>
        public KeyedCollection<string, ServiceTypeDescription> ServiceTypes { get; set; }

        /// <summary>
        /// Gets the endpoints.
        /// </summary>
        /// <value>
        /// The endpoints.
        /// </value>
        public KeyedCollection<string, EndpointResourceDescription> Endpoints { get; private set; }

        /// <summary>
        /// Gets or sets the name of the service manifet.
        /// </summary>
        /// <value>
        /// The name of the service manifet.
        /// </value>
        private string ServiceManifetName { get; set; }

        /// <summary>
        /// Gets or sets the service manifest version.
        /// </summary>
        /// <value>
        /// The service manifest version.
        /// </value>
        private string ServiceManifestVersion { get; set; }

        /// <summary>
        /// Triggers the configuration package modified event.
        /// </summary>
        /// <param name="configurationRoot">The configuration root.</param>
        /// <param name="packageName">The name of the package.</param>
        public void TriggerConfigurationPackageModifiedEvent(IConfigurationRoot configurationRoot, string packageName)
        {
            var oldPackage = this.GetConfigurationPackageObject(packageName);
            var newPackage = MockConfigurationPackage.CreateDefaultPackage(configurationRoot, packageName);
            this.ConfigurationPackageModifiedEvent(this, new PackageModifiedEventArgs<ConfigurationPackage>() { OldPackage = oldPackage, NewPackage = newPackage });
        }

        /// <summary>
        /// Retrieves the principals defined in the application manifest.
        /// </summary>
        /// <returns>
        /// The principals defined in the application manifest.
        /// </returns>
        public ApplicationPrincipalsDescription GetApplicationPrincipals()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Retrieves the list of code package names in the service manifest.
        /// </summary>
        /// <returns>
        /// The list of code package names in the service manifest.
        /// </returns>
        public IList<string> GetCodePackageNames()
        {
            return new List<string>() { this.CodePackageName };
        }

        /// <summary>
        /// Returns the <see cref="T:System.Fabric.CodePackage" /> object from Service Package that matches the desired package name.
        /// </summary>
        /// <param name="packageName">The name of the code package.</param>
        /// <returns>
        /// The <see cref="T:System.Fabric.CodePackage" /> object from Service Package that matches the desired package name.
        /// </returns>
        /// <remarks>
        /// Throws KeyNotFoundException exception if the package is not found.
        /// </remarks>
        public CodePackage GetCodePackageObject(string packageName)
        {
            // return new TestCodePackage(null);
            throw new NotImplementedException();
        }

        /// <summary>
        /// Retrieves the list of configuration package names in the service manifest.
        /// </summary>
        /// <returns>
        /// The list of configuration package names in the service manifest.
        /// </returns>
        public IList<string> GetConfigurationPackageNames()
        {
            return this.configs.Keys.ToList();
        }

        /// <summary>
        /// Returns the <see cref="T:System.Fabric.ConfigurationPackage" /> object from Service Package that matches the desired package name.
        /// </summary>
        /// <param name="packageName">The name of the configuration package.</param>
        /// <returns>
        /// The <see cref="T:System.Fabric.ConfigurationPackage" /> object from Service Package that matches the desired package name.
        /// </returns>
        /// <remarks>
        /// Throws KeyNotFoundException exception if the package is not found.
        /// </remarks>
        public ConfigurationPackage GetConfigurationPackageObject(string packageName)
        {
            var config = this.configs[packageName];
            return MockConfigurationPackage.CreateDefaultPackage(config, packageName);
        }

        /// <summary>
        /// Retrieves the list of data package names in the service manifest.
        /// </summary>
        /// <returns>
        /// The list of data package names in the service manifest.
        /// </returns>
        public IList<string> GetDataPackageNames()
        {
            return new List<string>() { "Data" };
        }

        /// <summary>
        /// Returns the <see cref="T:System.Fabric.DataPackage" /> object from Service Package that matches the desired package name.
        /// </summary>
        /// <param name="packageName">The name of the data package.</param>
        /// <returns>
        /// The <see cref="T:System.Fabric.DataPackage" /> object from Service Package that matches the desired package name.
        /// </returns>
        /// <remarks>
        /// Throws KeyNotFoundException exception if the package is not found.
        /// </remarks>
        public DataPackage GetDataPackageObject(string packageName)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Retrieves the endpoint resource with a given name from the service manifest.
        /// </summary>
        /// <param name="endpointName">The name of the endpoint.</param>
        /// <returns>
        /// The endpoint resource with the specified name.
        /// </returns>
        public EndpointResourceDescription GetEndpoint(string endpointName)
        {
            return this.Endpoints[endpointName];
        }

        /// <summary>
        /// Retrieves the endpoint resources in the service manifest.
        /// </summary>
        /// <returns>
        /// The endpoint resources in the service manifest.
        /// </returns>
        public KeyedCollection<string, EndpointResourceDescription> GetEndpoints()
        {
            return this.Endpoints;
        }

        /// <summary>
        /// Retrieves the list of Service Group types in the service manifest.
        /// </summary>
        /// <returns>
        /// The list of Service Group types in the service manifest.
        /// </returns>
        public KeyedCollection<string, ServiceGroupTypeDescription> GetServiceGroupTypes()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Retrieves the name of the service manifest.
        /// </summary>
        /// <returns>
        /// The name of the service manifest.
        /// </returns>
        public string GetServiceManifestName()
        {
            return this.ServiceManifetName;
        }

        /// <summary>
        /// Retrieves the version of the service manifest.
        /// </summary>
        /// <returns>
        /// The version of the service manifest.
        /// </returns>
        public string GetServiceManifestVersion()
        {
            return this.ServiceManifestVersion;
        }

        /// <summary>
        /// Retrieves the list of Service types in the service manifest.
        /// </summary>
        /// <returns>
        /// The list of service types in the service manifest.
        /// </returns>
        public KeyedCollection<string, ServiceTypeDescription> GetServiceTypes()
        {
            this.ThrowIfDisposed();
            return this.ServiceTypes;
        }

        /// <summary>
        /// Reports the application health.
        /// </summary>
        /// <param name="healthInformation">The health information.</param>
        public void ReportApplicationHealth(HealthInformation healthInformation)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Reports the deployed service package health.
        /// </summary>
        /// <param name="healthInformation">The health information.</param>
        public void ReportDeployedServicePackageHealth(HealthInformation healthInformation)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Reports the deployed application health.
        /// </summary>
        /// <param name="healthInformation">The health information.</param>
        public void ReportDeployedApplicationHealth(HealthInformation healthInformation)
        {
            throw new NotImplementedException();
        }

        #region IDisposable Support

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            this.Dispose(true);

            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Reports health for current application.
        /// Specifies options to control how the report is sent.
        /// </summary>
        /// <param name="healthInfo">The <see cref="T:System.Fabric.Health.HealthInformation" /> that describes the health report information.</param>
        /// <param name="sendOptions">The <see cref="T:System.Fabric.Health.HealthReportSendOptions" /> that controls how the report is sent.</param>
        public void ReportApplicationHealth(HealthInformation healthInfo, HealthReportSendOptions sendOptions)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Reports health for current deployed application.
        /// Specifies options to control how the report is sent.
        /// </summary>
        /// <param name="healthInfo">The <see cref="T:System.Fabric.Health.HealthInformation" /> that describes the health report information.</param>
        /// <param name="sendOptions">The <see cref="T:System.Fabric.Health.HealthReportSendOptions" /> that controls how the report is sent.</param>
        public void ReportDeployedApplicationHealth(HealthInformation healthInfo, HealthReportSendOptions sendOptions)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Reports health for current deployed service package.
        /// Specifies send options that control how the report is sent to the health store.
        /// </summary>
        /// <param name="healthInfo">The <see cref="T:System.Fabric.Health.HealthInformation" /> that describes the health report information.</param>
        /// <param name="sendOptions">The <see cref="T:System.Fabric.Health.HealthReportSendOptions" /> that controls how the report is sent.</param>
        public void ReportDeployedServicePackageHealth(HealthInformation healthInfo, HealthReportSendOptions sendOptions)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Throws if disposed.
        /// </summary>
        /// <exception cref="ObjectDisposedException">TestCodePackageActivationContext.</exception>
        internal void ThrowIfDisposed()
        {
            if (this.disposedValue)
            {
                throw new ObjectDisposedException(nameof(TestCodePackageActivationContext));
            }
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.
                this.disposedValue = true;
            }
        }
        #endregion
    }
}
