// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services
{
    interface ITrace
    {
        void Error(string message);
        void Info(string message);
        void Warning(string message);
    }
}
