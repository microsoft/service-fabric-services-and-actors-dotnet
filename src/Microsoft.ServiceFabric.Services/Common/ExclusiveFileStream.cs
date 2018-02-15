// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Common
{
    using System;
    using System.IO;
    using System.Threading;

    internal class ExclusiveFileStream : IDisposable
    {
        private const int MaxAttempts = 60;
        private const int MaxRetryIntervalMillis = 1000;
        private const int MinRetryIntervalMillis = 100;
        private static readonly Random Rand = new Random();

        public FileStream Value { get; private set; }

        private ExclusiveFileStream(FileStream stream)
        {
            this.Value = stream;
        }

        public void Dispose()
        {
            this.Value.Dispose();
        }

        public static ExclusiveFileStream Acquire(
            string path,
            FileMode fileMode,
            FileShare fileShare,
            FileAccess fileAccess)
        {
            var numAttempts = 0;
            while (true)
            {
                numAttempts++;
                try
                {
                    var fileStream = File.Open(
                        path,
                        fileMode,
                        fileAccess,
                        fileShare);

                    return new ExclusiveFileStream(fileStream);
                }
                catch (IOException)
                {
                    Thread.Sleep(Rand.Next(MinRetryIntervalMillis, MaxRetryIntervalMillis));
                    if (numAttempts > MaxAttempts)
                    {
                        throw;
                    }
                }
            }
        }
    }
}
