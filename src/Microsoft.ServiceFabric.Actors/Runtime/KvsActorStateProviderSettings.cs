// ------------------------------------------------------------
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT License (MIT).See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Runtime
{
    using System;
    using System.Fabric;
    using System.Fabric.Description;
    using System.Text;

    internal sealed class KvsActorStateProviderSettings : ActorStateProviderSettings
    {
        private const string BackupCallbackSlowCancellationHealthReportTimeToLiveParameterName = "BackupCallbackSlowCancellationHealthReportTimeToLiveInSeconds";
        private const string BackupCallbackExpectedCancellationTimeParameterName = "BackupCallbackExpectedCancellationTimeInSeconds";

        public KvsActorStateProviderSettings()
        {
            this.BackupCallbackSlowCancellationHealthReportTimeToLive = TimeSpan.FromMinutes(5);
            this.BackupCallbackExpectedCancellationTime = TimeSpan.FromSeconds(5);
        }

        public TimeSpan BackupCallbackSlowCancellationHealthReportTimeToLive { get; set; }

        public TimeSpan BackupCallbackExpectedCancellationTime { get; set; }

        public static KvsActorStateProviderSettings LoadFrom(
            ICodePackageActivationContext activationContext,
            string configPackageName,
            string sectionName)
        {
            var settings = new KvsActorStateProviderSettings();

            settings.LoadFromSettings(activationContext, configPackageName, sectionName);

            return settings;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.Append($"{base.ToString()}, ");
            sb.Append($"BackupCallbackSlowCancellationHealthReportTimeToLive: {this.BackupCallbackSlowCancellationHealthReportTimeToLive.TotalSeconds}, ");
            sb.Append($"BackupCallbackExpectedCancellationTime: {this.BackupCallbackExpectedCancellationTime.TotalSeconds}");

            return sb.ToString();
        }

        protected override void LoadFromSection(ConfigurationSection section)
        {
            base.LoadFromSection(section);

            this.BackupCallbackSlowCancellationHealthReportTimeToLive = ActorStateProviderHelper.GetTimeConfigInSecondsAsTimeSpan(
                section,
                BackupCallbackSlowCancellationHealthReportTimeToLiveParameterName,
                this.BackupCallbackSlowCancellationHealthReportTimeToLive);

            this.BackupCallbackExpectedCancellationTime = ActorStateProviderHelper.GetTimeConfigInSecondsAsTimeSpan(
                section,
                BackupCallbackExpectedCancellationTimeParameterName,
                this.BackupCallbackExpectedCancellationTime);
        }
    }
}
