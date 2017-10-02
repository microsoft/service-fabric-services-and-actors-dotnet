// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------
namespace Microsoft.ServiceFabric.Services.Remoting.Builder
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.ServiceFabric.Services.Remoting.Description;

    /// <summary>
    /// This class dispatches requests from the client to the interface/method of the remoted object.
    /// This class is used by remoting code generator.
    /// </summary>
    public abstract class MethodDispatcherBase : IMethodDispatcher
    {
        private int interfaceId;
        private IReadOnlyDictionary<int, string> methodNameMap;
        /// <summary>
        ///  Interface Id is used to identify remoting Interfaces.
        /// </summary>
        public int InterfaceId
        {
            get { return this.interfaceId; }
        }

        internal void SetInterfaceId(int interfaceId)
        {
            this.interfaceId = interfaceId;
        }

        internal void SetMethodNameMap(IReadOnlyDictionary<int, string> methodNameMap)
        {
            this.methodNameMap = methodNameMap;
        }


        /// <summary>
        ///This method is used to dispatch request to the specified methodId of the 
        /// interface implemented by the remoted object.
        /// </summary>
        /// <param name="objectImplementation"></param>
        /// <param name="methodId"></param>
        /// <param name="requestBody"></param>
        /// <param name="cancellationToken"></param>
        public abstract Task<object> DispatchAsync(object objectImplementation, int methodId, object requestBody,
            CancellationToken cancellationToken);

        /// <summary>
        /// This method is used to dispatch one way messages to the specified methodId of the 
        /// interface implemented by the remoted object.
        /// </summary>
        /// <param name="objectImplementation"></param>
        /// <param name="methodId"></param>
        /// <param name="messageBody"></param>
        public abstract void Dispatch(object objectImplementation, int methodId, object messageBody);

       
        /// <summary>
        /// Internal - used by Service remoting
        /// </summary>
        /// <param name="task">continuation task</param>
        /// <returns>
        /// A <see cref="System.Threading.Tasks.Task">Task</see> that represents outstanding operation.
        /// </returns>
        protected Task<object> ContinueWith(Task task)
        {
            return task.ContinueWith<object>(
                t =>
                {
                    t.GetAwaiter().GetResult();
                    return null;
                },
                TaskContinuationOptions.ExecuteSynchronously);
        }

        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="methodId"></param>
        /// <returns></returns>
        public string GetMethodName(int methodId)
        {
            return this.methodNameMap[methodId];
        }

    

        


    }
}
