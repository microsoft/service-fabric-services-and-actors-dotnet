// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.

using System.ServiceModel;

namespace Microsoft.ServiceFabric.Services.Communication.Wcf;

static class WcfRemoteExceptionInformation
{
    internal static readonly string FaultCodeName = "WcfRemoteExceptionInformation";
    internal static readonly string FaultSubCodeRetryName = "Retry";
    internal static readonly FaultCode FaultCodeRetry = new(FaultCodeName, new FaultCode(FaultSubCodeRetryName));
}
