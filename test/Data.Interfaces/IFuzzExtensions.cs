// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.

using System.Fabric;
using Fuzzy;
using BackupVersion = Microsoft.ServiceFabric.Data.BackupInfo.BackupVersion;

namespace Microsoft.ServiceFabric.Data;

static class IFuzzExtensions
{
    internal static Epoch Epoch(this IFuzz fuzzy) =>
        new(fuzzy.Int64(), fuzzy.Int64());

    internal static BackupVersion BackupVersion(this IFuzz fuzzy) =>
        new(fuzzy.Epoch(), fuzzy.Int64());
}
