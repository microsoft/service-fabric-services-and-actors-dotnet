// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.StateMigration.Tests
{
    using System;
    using System.Globalization;
    using Microsoft.ServiceFabric.Actors;
    using Microsoft.ServiceFabric.Actors.KVSToRCMigration;
    using Microsoft.ServiceFabric.Actors.Migration.Exceptions;
    using Xunit;

    /// <summary>
    /// Migration Settings tests.
    /// </summary>
    public class MigrationSettingsTest
    {
        /// <summary>
        /// Tests two actor ids for null equality.
        /// </summary>
        [Fact]
        public void TestMigrationSettings()
        {
            var tgtSettings = new MigrationSettings
            {
                ChunksPerEnumeration = 10,
                CopyPhaseParallelism = 2,
                DowntimeThreshold = 1000,
                KeyValuePairsPerChunk = 100,
                MigrationMode = Runtime.Migration.MigrationMode.Auto,
                TargetServiceUri = new Uri("fabric:/MyApp/MySourceSvc"),
                SourceServiceUri = new Uri("fabric:/MyApp/MyTargetSvc"),
            };

            tgtSettings.Validate(isSource: false);

            tgtSettings.SourceServiceUri = null;
            Assert.Throws<InvalidMigrationConfigException>(() => tgtSettings.Validate(isSource: false));

            var sourceSettings = new MigrationSettings
            {
                TargetServiceUri = new Uri("fabric:/MyApp/MyTargetSvc"),
            };

            sourceSettings.Validate(isSource: true);
            sourceSettings.TargetServiceUri = new Uri("//blah");
            Assert.Throws<InvalidMigrationConfigException>(() => sourceSettings.Validate(isSource: true));
        }
    }
}
