// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using Microsoft.ServiceFabric.Services.Remoting;

/// <summary>
/// Exception Deserialization option to use(applies to V2 Remoting only).
/// </summary>
[Obsolete(DeprecationMessage.RemotingV1)]
public enum ExceptionDeserialization
{
    /// <summary>
    /// Uses only DCS to deserialize the service remoting message containing exception details.
    /// </summary>
    Default,

    /// <summary>
    /// Attempts to deserialize using DCS and fallback to BinaryFormatter if DCS fails.
    /// To be used in compat scenarios. Fallback option will be deprecated in future.
    /// </summary>
    Fallback,
}