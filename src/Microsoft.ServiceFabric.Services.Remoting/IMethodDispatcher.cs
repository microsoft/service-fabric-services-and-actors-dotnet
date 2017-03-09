// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------
namespace Microsoft.ServiceFabric.Services.Remoting
{
    using System.Threading;
    using System.Threading.Tasks;

    internal interface IMethodDispatcher
    {
        int InterfaceId { get; }

        Task<object> DispatchAsync(
            object objectImplementation,
            int methodId,
            object requestBody,
            CancellationToken cancellationToken);

        void Dispatch(
           object objectImplementation,
           int methodId,
           object messageBody);

        string GetMethodName(int methodId);
    }
}
