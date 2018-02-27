// ------------------------------------------------------------
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT License (MIT).See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Common
{
    using System.Threading.Tasks;

    internal static class TaskDone<T>
    {
        private static readonly Task<T> DoneConstant = Task.FromResult(default(T));

        public static Task<T> Done
        {
            get { return DoneConstant; }
        }
    }
}
