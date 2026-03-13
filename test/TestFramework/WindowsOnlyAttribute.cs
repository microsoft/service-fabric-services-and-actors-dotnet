// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.

using System;
using System.Reflection;
using Xunit;
using Xunit.v3;

namespace Microsoft.ServiceFabric;

/// <summary>
/// Tests with this attribute run only on Windows.
/// </summary>
public sealed class WindowsOnlyAttribute(string reason) : BeforeAfterTestAttribute
{
    readonly string reason = reason ?? throw new ArgumentNullException(nameof(reason));

    public override void Before(MethodInfo methodUnderTest, IXunitTest test) => 
        Assert.SkipUnless(Environment.OSVersion.Platform == PlatformID.Win32NT, reason);
}
