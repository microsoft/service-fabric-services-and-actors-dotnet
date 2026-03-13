// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

// On .NET 7+, GeneratedComInterfaceAttribute is provided by the runtime in System.Runtime.InteropServices.Marshalling.
// On .NET Framework, MarshalUsingAttribute and UniqueComInterfaceMarshaller<T> are shimmed in System.Fabric.dll.
// GeneratedComInterfaceAttribute is defined here as a shim for .NET Framework to avoid conditional compilation.
#if NETFRAMEWORK
namespace System.Runtime.InteropServices.Marshalling
{
    using System;

    [AttributeUsage(AttributeTargets.Interface, AllowMultiple = false, Inherited = false)]
    internal sealed class GeneratedComInterfaceAttribute : Attribute
    {
    }
}
#endif
