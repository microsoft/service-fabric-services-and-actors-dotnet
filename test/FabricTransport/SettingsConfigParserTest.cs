// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.

using System;
using System.Fabric.Management.ServiceModel;
using System.IO;
using Fuzzy;
using Xunit;

namespace Microsoft.ServiceFabric.FabricTransport;

[WindowsOnly("Can't load libFabricCommon.so on Linux.")]
public abstract class SettingsConfigParserTest
{
    readonly IFabricServiceConfigParser sut = new SettingsConfigParser();

    static readonly IFuzz fuzzy = new RandomFuzz(Environment.TickCount);

    public sealed class Parse : SettingsConfigParserTest, IDisposable
    {
        // Method parameters
        readonly string fileName;

        readonly string dir = Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"))).FullName;
        readonly string section = fuzzy.String().LettersOrDigits();

        public Parse()
        {
            fileName = Path.Combine(dir, "Settings.xml");

            File.WriteAllText(fileName,
                $"""
                <?xml version="1.0" encoding="utf-8"?>
                <Settings xmlns="http://schemas.microsoft.com/2011/01/fabric">
                  <Section Name="{section}" />
                </Settings>
                """);
        }

        void IDisposable.Dispose() => Directory.Delete(dir, recursive: true);

        [Fact]
        public void ReturnsSettingsTypeParsedFromGivenFile()
        {
            SettingsType actual = sut.Parse(fileName);

            SettingsTypeSection actualSection = Assert.Single(actual.Section);
            Assert.Equal(section, actualSection.Name);
        }
    }
}
