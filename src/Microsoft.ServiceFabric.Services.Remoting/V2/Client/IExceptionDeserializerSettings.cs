// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Remoting.V2.Runtime
{
    interface IExceptionDeserializerSettings
    {
#pragma warning disable 618
        ExceptionDeserialization ExceptionDeserializationTechnique { get; }
#pragma warning restore 618
    }
}