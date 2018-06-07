// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Remoting.Builder
{
    using System;

    /// <summary>
    /// Represents an interface for generating the code to support remoting.
    /// </summary>
    internal interface ICodeBuilder
    {
        /// <summary>
        /// Gets the interface for getting the names of the generated code (types, interfaces, methods etc.)
        /// </summary>
        ICodeBuilderNames Names { get; }

        /// <summary>
        /// Gets or builds a type that can the remoting messages to the object implementing the specified interface.
        /// </summary>
        /// <param name="interfaceType">Interface for which to generate the method dispatcher.</param>
        /// <returns>A <see cref="MethodDispatcherBuildResult"/> containing the dispatcher to dispatch the messages destined the specified interfaces.</returns>
        MethodDispatcherBuildResult GetOrBuilderMethodDispatcher(Type interfaceType);

        /// <summary>
        /// Gets or builds a remoting messaage body types that can store the method arguments of the specified interface.
        /// </summary>
        /// <param name="interfaceType">Interface for which to generate the method body types.</param>
        /// <returns>A <see cref="MethodBodyTypesBuildResult"/> containing the method body types for each of the methods of the specified interface.</returns>
        MethodBodyTypesBuildResult GetOrBuildMethodBodyTypes(Type interfaceType);

        /// <summary>
        /// Gets or builds a factory object that can generate remoting proxy for the specified interface.
        /// </summary>
        /// <param name="interfaceType">Interface for which to generate the proxy factory object.</param>
        /// <returns>A <see cref="ProxyGeneratorBuildResult"/> containing the generator for remoting proxy for the speficifed interface.</returns>
        ProxyGeneratorBuildResult GetOrBuildProxyGenerator(Type interfaceType);
    }
}
