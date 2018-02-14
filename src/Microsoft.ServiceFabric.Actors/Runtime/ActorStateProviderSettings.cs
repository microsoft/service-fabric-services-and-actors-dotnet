// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Runtime
{
    using System;
    using System.Text;
    using System.Fabric;
    using System.Fabric.Description;

    internal abstract class ActorStateProviderSettings
    {
        internal const string TransientErrorRetryDelayParameterName = "TransientErrorRetryDelayInSeconds";
        internal const string OperationTimeoutParameterName = "OperationTimeoutInSeconds";

        public ActorStateProviderSettings()
        {
            this.TransientErrorRetryDelay = TimeSpan.FromSeconds(1);
            this.OperationTimeout = TimeSpan.FromMinutes(5);
        }

        protected virtual void LoadFromSection(ConfigurationSection section)
        {
            this.TransientErrorRetryDelay = ActorStateProviderHelper.GetTimeConfigInSecondsAsTimeSpan(
                section,
                TransientErrorRetryDelayParameterName,
                this.TransientErrorRetryDelay);

            this.OperationTimeout = ActorStateProviderHelper.GetTimeConfigInSecondsAsTimeSpan(
                section,
                OperationTimeoutParameterName,
                this.OperationTimeout);
        }

        protected void LoadFromSettings(
            ICodePackageActivationContext activationContext,
            string configPackageName,
            string sectionName)
        {
            if (ActorStateProviderHelper.TryGetConfigSection(activationContext, configPackageName, sectionName, out var section))
            {
                this.LoadFromSection(section);
            }
        }

        public TimeSpan TransientErrorRetryDelay { get; set; }

        public TimeSpan OperationTimeout { get; set; }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.Append($"TransientErrorRetryDelay: {this.TransientErrorRetryDelay.TotalSeconds}, ");
            sb.Append($"OperationTimeout: {this.OperationTimeout.TotalSeconds}");

            return sb.ToString();
        }
    }
}
