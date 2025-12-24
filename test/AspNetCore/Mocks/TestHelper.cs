// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Reflection;
using System.Runtime.Serialization;

namespace Microsoft.ServiceFabric.AspNetCore.Tests
{
    /// <summary>
    /// TestHelper.
    /// </summary>
    internal static class TestHelper
    {
        public static T CreateInstanced<T>()
            where T : class
        {
#pragma warning disable SYSLIB0050 // FormatterServices is obsolete
            return FormatterServices.GetSafeUninitializedObject(typeof(T)) as T;
#pragma warning restore SYSLIB0050
        }

        public static T Set<T>(this T instance, string property, object value)
            where T : class
        {
            typeof(T).GetProperty(property).SetValue(instance, value);
            return instance;
        }

        internal static T InvokeMember<T>(object instance, string memberName, params object[] args)
        {
            var type = instance.GetType();
            var result = type.InvokeMember(memberName, BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, Type.DefaultBinder, instance, args);
            return (T)result;
        }
    }
}
