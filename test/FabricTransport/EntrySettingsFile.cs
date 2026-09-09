// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.

using System.IO;
using System.Reflection;
using Xunit;

namespace Microsoft.ServiceFabric.FabricTransport;

// Helper for tests that exercise FabricServiceConfig/FabricServiceConfigSection fallback paths which probe
// {EntryAssemblyName}.Settings.xml next to the entry assembly. The product singletons offer no testability
// seam, so tests share the entry assembly's output directory. AssertAbsent converts cross-test contamination
// into an explicit, actionable failure instead of a silent regression.
static class EntrySettingsFile
{
    public static string Path { get; } = ComputePath();

    public static void AssertAbsent() =>
        Assert.False(File.Exists(Path), $"Pre-existing {Path} would conflict with this test.");

    static string ComputePath()
    {
        var entry = Assembly.GetEntryAssembly();
        return System.IO.Path.Combine(
            System.IO.Path.GetDirectoryName(entry.Location),
            entry.GetName().Name + ".Settings.xml");
    }
}
