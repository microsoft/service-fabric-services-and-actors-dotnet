// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Actors.Migration
{
    using System;
    using System.Collections.Generic;

    internal class MigrationInput
    {
        public DateTime StartDateTimeUTC { get; set; }

        public DateTime EndDateTimeUTC { get; set; }

        public long StartSeqNum { get; set; }

        public long EndSeqNum { get; set; }

        public long LastAppliedSeqNum { get; set; }

        public MigrationState Status { get; set; }

        public int WorkerCount { get; set; }

        public int IterationCount { get; set; }

        public MigrationPhase Phase { get; set; }

        public List<WorkerInput> WorkerInputs { get; set; }

        public class WorkerInput
        {
            public int WorkerId { get; set; }

            public int Iteration { get; set; }

            public DateTime StartDateTimeUTC { get; set; }

            public DateTime EndDateTimeUTC { get; set; }

            public long StartSeqNum { get; set; }

            public long EndSeqNum { get; set; }

            public long LastAppliedSeqNum { get; set; }

            public MigrationPhase Phase { get; set; }

            public MigrationState Status { get; set; }
        }

    }
}
