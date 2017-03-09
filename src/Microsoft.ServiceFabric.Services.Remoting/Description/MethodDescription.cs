// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------
namespace Microsoft.ServiceFabric.Services.Remoting.Description
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Reflection;
    using System.Threading;
    using Microsoft.ServiceFabric.Services.Common;

    internal class MethodDescription
    {
        private readonly MethodInfo methodInfo;
        private readonly int methodId;
        private readonly MethodArgumentDescription[] arguments;
        private readonly bool hasCancellationToken;

        private MethodDescription(
            MethodInfo methodInfo,
            MethodArgumentDescription[] arguments,
            bool hasCancellationToken)
        {
            this.methodInfo = methodInfo;
            this.methodId = IdUtil.ComputeId(methodInfo);
            this.arguments = arguments;
            this.hasCancellationToken = hasCancellationToken;
        }

        public int Id
        {
            get { return this.methodId; }
        }

        public string Name
        {
            get { return this.methodInfo.Name; }
        }

        public Type ReturnType
        {
            get { return this.methodInfo.ReturnType; }
        }

        public bool HasCancellationToken
        {
            get { return this.hasCancellationToken;  }
        }

        public MethodArgumentDescription[] Arguments
        {
            get { return this.arguments; }
        }

        public MethodInfo MethodInfo
        {
            get { return this.methodInfo; }
        }
     
        internal static MethodDescription Create(string remotedInterfaceKindName, MethodInfo methodInfo)
        {
            var argumentList = new List<MethodArgumentDescription>();
            var hasCancellationToken = false;

            foreach (var param in methodInfo.GetParameters())
            {
                if (hasCancellationToken)
                {
                    //
                    // If the method has a cancellation token, then it must be the last argument.
                    //
                    throw new ArgumentException(
                        string.Format(
                            CultureInfo.CurrentCulture,
                            SR.ErrorRemotedMethodCancellationTokenOutOfOrder,
                            remotedInterfaceKindName,
                            methodInfo.Name,
                            methodInfo.DeclaringType.FullName,
                            param.Name,
                            typeof(CancellationToken)),
                        remotedInterfaceKindName + "InterfaceType");
                }

                if (param.ParameterType == typeof(CancellationToken))
                {
                    hasCancellationToken = true;
                }
                else
                {
                    argumentList.Add(MethodArgumentDescription.Create(remotedInterfaceKindName, methodInfo, param));
                }
            }

            return new MethodDescription(
                methodInfo, 
                argumentList.ToArray(),
                hasCancellationToken);
        }
    }
}