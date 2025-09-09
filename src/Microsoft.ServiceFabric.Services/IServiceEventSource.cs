// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services
{
    interface IServiceEventSource
    {
        void InfoText(string id, string type, string message);
        void WarningText(string id, string type, string message);
        void ErrorText(string id, string type, string message);
    }
}
