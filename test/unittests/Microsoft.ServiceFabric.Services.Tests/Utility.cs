// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Tests
{
    using System;
    using System.Fabric.Health;

    internal class Utility
    {
        public static bool IsRunAsyncUnhandledExceptionHealthInformation(HealthInformation hInfo)
        {
            return (hInfo.RemoveWhenExpired &&
                hInfo.HealthState == HealthState.Warning &&
                hInfo.TimeToLive == TimeSpan.FromMinutes(5) &&
                hInfo.SourceId == "RunAsync" &&
                hInfo.Property == "RunAsyncUnhandledException");
        }

        public static bool IsRunAsyncSlowCancellationHealthInformation(HealthInformation hInfo)
        {
            return (hInfo.RemoveWhenExpired &&
                hInfo.HealthState == HealthState.Warning &&
                hInfo.TimeToLive == TimeSpan.FromMinutes(5) &&
                hInfo.SourceId == "RunAsync" &&
                hInfo.Property == "RunAsyncSlowCancellation");
        }
    }
}
