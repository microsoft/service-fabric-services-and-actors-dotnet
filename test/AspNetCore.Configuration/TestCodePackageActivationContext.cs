// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Fabric;
using System.Fabric.Description;
using System.Fabric.Health;
using System.Linq;
using Microsoft.Extensions.Configuration;

namespace Microsoft.ServiceFabric.AspNetCore.Configuration;

public class TestCodePackageActivationContext : ICodePackageActivationContext
{
    readonly IDictionary<string, IConfiguration> configs;

    // private readonly XElement manifest = null;
    bool disposedValue = false; // To detect redundant calls

    public TestCodePackageActivationContext(IConfiguration config)
    {
        configs = new Dictionary<string, IConfiguration>() { { "Config", config } };

        ApplicationName = config[nameof(ApplicationName)];
        ApplicationTypeName = config[nameof(ApplicationTypeName)];
    }

    public TestCodePackageActivationContext(IDictionary<string, IConfiguration> configs) => this.configs = configs;

#pragma warning disable 67 // Unused events

    public event EventHandler<PackageAddedEventArgs<CodePackage>> CodePackageAddedEvent;

    public event EventHandler<PackageModifiedEventArgs<CodePackage>> CodePackageModifiedEvent;

    public event EventHandler<PackageRemovedEventArgs<CodePackage>> CodePackageRemovedEvent;

    public event EventHandler<PackageAddedEventArgs<ConfigurationPackage>> ConfigurationPackageAddedEvent;

    public event EventHandler<PackageModifiedEventArgs<ConfigurationPackage>> ConfigurationPackageModifiedEvent;

    public event EventHandler<PackageRemovedEventArgs<ConfigurationPackage>> ConfigurationPackageRemovedEvent;

    public event EventHandler<PackageAddedEventArgs<DataPackage>> DataPackageAddedEvent;

    public event EventHandler<PackageModifiedEventArgs<DataPackage>> DataPackageModifiedEvent;

    public event EventHandler<PackageRemovedEventArgs<DataPackage>> DataPackageRemovedEvent;

#pragma warning restore 67

    public string ApplicationName { get; set; }

    public string ApplicationTypeName { get; set; }

    public string CodePackageName { get; set; }

    public string CodePackageVersion { get; set; }

    public string ContextId { get; set; }

    public string LogDirectory { get; set; }

    public string TempDirectory { get; set; }

    public string WorkDirectory { get; set; }

    public KeyedCollection<string, ServiceTypeDescription> ServiceTypes { get; set; }

    public KeyedCollection<string, EndpointResourceDescription> Endpoints { get; private set; }

    string ServiceManifetName { get; set; }

    string ServiceManifestVersion { get; set; }

    public void TriggerConfigurationPackageModifiedEvent(IConfigurationRoot configurationRoot, string packageName)
    {
        var oldPackage = GetConfigurationPackageObject(packageName);
        var newPackage = MockConfigurationPackage.CreateDefaultPackage(configurationRoot, packageName);
        ConfigurationPackageModifiedEvent(this, new PackageModifiedEventArgs<ConfigurationPackage>() { OldPackage = oldPackage, NewPackage = newPackage });
    }

    public void RaiseConfigurationPackageModifiedEvent(ConfigurationPackage newPackage) => 
        ConfigurationPackageModifiedEvent(this, new PackageModifiedEventArgs<ConfigurationPackage>() { OldPackage = null, NewPackage = newPackage });

    public void RaiseConfigurationPackageAddedEvent(ConfigurationPackage package) => 
        ConfigurationPackageAddedEvent(this, new PackageAddedEventArgs<ConfigurationPackage>() { Package = package });

    public ApplicationPrincipalsDescription GetApplicationPrincipals() => throw new NotImplementedException();

    public IList<string> GetCodePackageNames() => [CodePackageName];

    public CodePackage GetCodePackageObject(string packageName) => throw new NotImplementedException();

    public IList<string> GetConfigurationPackageNames() => [.. configs.Keys];

    public ConfigurationPackage GetConfigurationPackageObject(string packageName)
    {
        var config = configs[packageName];
        return MockConfigurationPackage.CreateDefaultPackage(config, packageName);
    }

    public IList<string> GetDataPackageNames() => ["Data"];

    public DataPackage GetDataPackageObject(string packageName) => throw new NotImplementedException();

    public EndpointResourceDescription GetEndpoint(string endpointName) => Endpoints[endpointName];

    public KeyedCollection<string, EndpointResourceDescription> GetEndpoints() => Endpoints;

    public KeyedCollection<string, ServiceGroupTypeDescription> GetServiceGroupTypes() => throw new NotImplementedException();

    public string GetServiceManifestName() => ServiceManifetName;

    public string GetServiceManifestVersion() => ServiceManifestVersion;

    public KeyedCollection<string, ServiceTypeDescription> GetServiceTypes()
    {
        ThrowIfDisposed();
        return ServiceTypes;
    }

    public void ReportApplicationHealth(HealthInformation healthInformation) => throw new NotImplementedException();

    public void ReportDeployedServicePackageHealth(HealthInformation healthInformation) => throw new NotImplementedException();

    public void ReportDeployedApplicationHealth(HealthInformation healthInformation) => throw new NotImplementedException();

    #region IDisposable Support

    public void Dispose() => Dispose(true);

    public void ReportApplicationHealth(HealthInformation healthInfo, HealthReportSendOptions sendOptions) => throw new NotImplementedException();

    public void ReportDeployedApplicationHealth(HealthInformation healthInfo, HealthReportSendOptions sendOptions) => throw new NotImplementedException();

    public void ReportDeployedServicePackageHealth(HealthInformation healthInfo, HealthReportSendOptions sendOptions) => throw new NotImplementedException();

    internal void ThrowIfDisposed()
    {
        if (disposedValue)
        {
            throw new ObjectDisposedException(nameof(TestCodePackageActivationContext));
        }
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                // TODO: dispose managed state (managed objects).
            }

            // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
            // TODO: set large fields to null.
            disposedValue = true;
        }
    }
    #endregion
}
