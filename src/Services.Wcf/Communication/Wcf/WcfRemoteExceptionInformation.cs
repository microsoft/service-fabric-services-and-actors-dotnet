// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.

using System;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Xml;

namespace Microsoft.ServiceFabric.Services.Communication.Wcf;

static class WcfRemoteExceptionInformation
{
    internal static readonly string FaultCodeName = "WcfRemoteExceptionInformation";
    internal static readonly string FaultSubCodeRetryName = "Retry";
    internal static readonly FaultCode FaultCodeRetry = new(FaultCodeName, new FaultCode(FaultSubCodeRetryName));
}
