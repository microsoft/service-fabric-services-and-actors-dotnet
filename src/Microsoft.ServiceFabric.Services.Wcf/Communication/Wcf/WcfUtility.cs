// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------
namespace Microsoft.ServiceFabric.Services.Communication.Wcf
{
    using System;
    using System.ServiceModel;
    using System.ServiceModel.Channels;

    /// <summary>
    /// Utility class for creating default bindings for WCF communication.
    /// </summary>
    public static class WcfUtility
    {
        private const long DefaultMaxReceivedMessageSize = 4 * 1024 * 1024;
        private const long DefaultOpenCloseTimeoutInSeconds = 5;

        internal static readonly Binding DefaultTcpClientBinding;
        internal static readonly Binding DefaultTcpListenerBinding;

        static WcfUtility()
        {
            DefaultTcpClientBinding = CreateTcpClientBinding();
            DefaultTcpListenerBinding = CreateTcpListenerBinding();
        }

        /// <summary>
        ///     Creates a TCP listener binding with no security for WCF communication.
        /// </summary>
        /// <param name="maxMessageSize">
        ///     Maximum size of the message in bytes. 
        ///     If the value is not specified or it is less than or equals to zero,
        ///     a default value of 4,194,304 bytes (4 MB) is used.
        /// </param>
        /// <param name="openTimeout">
        ///     Timeout for opening the connection. 
        ///     If the value is not specified, the default value of 5 seconds is used.
        /// </param>
        /// <param name="closeTimeout">
        ///     Time to wait for messages to drain on the connections before aborting the connection. 
        ///     If the value is not specified, the default value of 5 seconds is used.
        /// </param>
        /// <returns>A <see cref="System.ServiceModel.Channels.Binding"/> to use with <see cref="Runtime.WcfCommunicationListener{TServiceContract}"/>.</returns>
        public static Binding CreateTcpListenerBinding(
            long maxMessageSize = DefaultMaxReceivedMessageSize,
            TimeSpan openTimeout = default(TimeSpan),
            TimeSpan closeTimeout = default(TimeSpan))
        {
            var binding = new NetTcpBinding(SecurityMode.None)
            {
                SendTimeout = TimeSpan.MaxValue,
                ReceiveTimeout = TimeSpan.MaxValue,
                MaxConnections = int.MaxValue,
            };

            if (openTimeout > TimeSpan.Zero)
            {
                binding.OpenTimeout = openTimeout;
            }
            else
            {
                binding.OpenTimeout = TimeSpan.FromSeconds(DefaultOpenCloseTimeoutInSeconds);
            }

            if (closeTimeout > TimeSpan.Zero)
            {
                binding.CloseTimeout = closeTimeout;
            }
            else
            {
                binding.CloseTimeout = TimeSpan.FromSeconds(DefaultOpenCloseTimeoutInSeconds);
            }

            if (maxMessageSize <= 0)
            {
                maxMessageSize = DefaultMaxReceivedMessageSize;
            }
            binding.MaxReceivedMessageSize = maxMessageSize;
            binding.MaxBufferSize = (int)binding.MaxReceivedMessageSize;
            binding.MaxBufferPoolSize = Environment.ProcessorCount * binding.MaxReceivedMessageSize;

            return binding;
        }

        /// <summary>
        ///     Creates a TCP client binding with no security for WCF communication.
        /// </summary>
        /// <param name="maxMessageSize">
        ///     Maximum size of the message in bytes. 
        ///     If the value is not specified or it is less than or equals to zero,
        ///     a default value of 4,194,304 bytes (4 MB) is used.
        /// </param>
        /// <param name="openTimeout">
        ///     Timeout for opening the connection. 
        ///     If the value is not specified, the default value of 5 seconds is used.
        /// </param>
        /// <param name="closeTimeout">
        ///     Time to wait for messages to drain on the connections before aborting the connection. 
        ///     If the value is not specified, the default value of 5 seconds is used.
        /// </param>
        /// <returns>A <see cref="System.ServiceModel.Channels.Binding"/> to use with <see cref="Client.WcfCommunicationClientFactory{TChannel}"/>.</returns>
        public static Binding CreateTcpClientBinding(
            long maxMessageSize = DefaultMaxReceivedMessageSize,
            TimeSpan openTimeout = default(TimeSpan),
            TimeSpan closeTimeout = default(TimeSpan))
        {
            var binding = new NetTcpBinding(SecurityMode.None)
            {
                SendTimeout = TimeSpan.MaxValue,
                ReceiveTimeout = TimeSpan.MaxValue,
            };

            if (openTimeout > TimeSpan.Zero)
            {
                binding.OpenTimeout = openTimeout;
            }

            if (closeTimeout > TimeSpan.Zero)
            {
                binding.CloseTimeout = closeTimeout;
            }

            if (maxMessageSize <= 0)
            {
                maxMessageSize = DefaultMaxReceivedMessageSize;
            }

            binding.MaxReceivedMessageSize = maxMessageSize;
            binding.MaxBufferSize = (int)binding.MaxReceivedMessageSize;
            binding.MaxBufferPoolSize = Environment.ProcessorCount * binding.MaxReceivedMessageSize;

            return binding;
        }
    }
}