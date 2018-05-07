// ------------------------------------------------------------
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT License (MIT).See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Common
{
    using System.Threading.Tasks;

    internal static class TaskDone
    {
        private static readonly Task<bool> DoneConstant = Task.FromResult(true);

        public static Task Done
        {
            get { return DoneConstant; }
        }
    }
}
