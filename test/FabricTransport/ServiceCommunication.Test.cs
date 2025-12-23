// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Fabric;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.FabricTransport.Client;
using Microsoft.ServiceFabric.FabricTransport.Runtime;
using Xunit;

namespace Microsoft.ServiceFabric.FabricTransport
{
    public class ServiceCommunicationTest
    {
        [Fact]
        public static void OpenListenerTest()
        {
            var service = new DummyService();
            var settings = FabricTransportSettings.GetDefault();
            settings.OperationTimeout = TimeSpan.FromSeconds(4);
            settings.MaxConcurrentCalls = 6;
            settings.MaxMessageSize = 43355;
            settings.MaxQueueSize = 35;
            var listenerAddress = new FabricTransportListenerAddress("localhost", 0, Guid.NewGuid().ToString());
            var remotingRemotingConnectionHandler = new FabricTransportTestConnectionHandler();
            var listener = new FabricTransportListener(settings, listenerAddress, service,
                remotingRemotingConnectionHandler);
            var opentask = listener.OpenAsync(CancellationToken.None);
            var address = opentask.Result;
            Assert.True(address.Length > 0);
            listener.CloseAsync(CancellationToken.None).Wait();
        }

        [Fact]
        public static void ReadTransportSettingsFromConfig()
        {
            var fileName = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                "ServiceCommunicationTestSettings.xml");
            var settings = FabricTransportSettings.LoadFrom("TestServiceListenerTransportSettings", fileName);
            Assert.True(settings.OperationTimeout.TotalSeconds == 2);
            Assert.True(settings.KeepAliveTimeout.TotalSeconds == 1000);
            Assert.True(settings.MaxConcurrentCalls == 5);
            Assert.True(settings.MaxQueueSize == 35);

            Assert.True(settings.SecurityCredentials.CredentialType == CredentialType.X509);
            var x509securityCredentail = (X509Credentials) settings.SecurityCredentials;
            Assert.True(x509securityCredentail.FindType == X509FindType.FindBySubjectName);
            Assert.True(x509securityCredentail.FindValue.Equals("CN=alice.server.servicefabric.azure.test"));
            Assert.True(x509securityCredentail.StoreName.Equals(X509Credentials.StoreNameDefault));
            Assert.True(x509securityCredentail.ProtectionLevel == ProtectionLevel.EncryptAndSign);
            Assert.True(x509securityCredentail.StoreLocation == StoreLocation.LocalMachine);
            var commonNames = new List<string>() {"alice.server.servicefabric.azure.test"};
            Assert.True(x509securityCredentail.RemoteCommonNames.SequenceEqual(commonNames));
        }

        [Fact]
        public static void ReadDefaultTransportSettingsFromConfigWhenSectionIsNotPresent()
        {
            var settings = FabricTransportSettings.GetDefault("Dummy");
            Assert.True(settings.OperationTimeout.TotalSeconds == TimeSpan.FromMinutes(5).TotalSeconds);
            Assert.True(settings.KeepAliveTimeout.TotalSeconds == TimeSpan.Zero.TotalSeconds);
            Assert.True(settings.MaxConcurrentCalls == 0);
            Assert.True(settings.MaxQueueSize == 10000);
            Assert.True(settings.SecurityCredentials.CredentialType == CredentialType.None);
        }

        [Fact]
        public static void TestReadSettingsUsingTryLoadFrom()
        {
            FabricTransportSettings settings = null;
            var fileName = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                "ServiceCommunicationTestSettings.xml");
            var isSucceeded = FabricTransportSettings.TryLoadFrom("TestServiceListenerTransportSettings", out settings,
                fileName);
            Assert.True(isSucceeded);
            Assert.True(settings.OperationTimeout.TotalSeconds == 2);
            Assert.True(settings.KeepAliveTimeout.TotalSeconds == 1000);
            Assert.True(settings.MaxConcurrentCalls == 5);
            Assert.True(settings.MaxQueueSize == 35);
            FabricTransportSettings settings2 = null;
            isSucceeded = FabricTransportSettings.TryLoadFrom("TestServiceListener", out settings2, fileName);
            Assert.False(isSucceeded);
            FabricTransportListenerSettings listenerSettings;
            isSucceeded = FabricTransportListenerSettings.TryLoadFrom("TestServiceListenerTransportSettings",
                out listenerSettings, "Config2");
            Assert.False(isSucceeded);
        }

        [Fact]
        public static void ReadTransportSettingsFromConfigWhenSectionOrFileIsNotPresent()
        {
            var fileName = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                "ServiceCommunicationTestSettings.xml");
            try
            {
                var settings = FabricTransportSettings.LoadFrom("TestServiceListener", fileName);
                Assert.Fail("Exception should be thrown as Section is Not Present");
            }
            catch (ArgumentException)
            {
                //Expected Exception
            }
            try
            {
                var settings = FabricTransportSettings.LoadFrom("TestServiceListener");
                Assert.Fail("Exception should be thrown as FileName is Not Present");
            }
            catch (ArgumentException)
            {
                //Expected Exception
            }
        }

        [Fact]
        public static void ReadTransportListenerSettingsFromConfigPackage()
        {
            try
            {
                var settings = FabricTransportListenerSettings.LoadFrom("TestServiceListener", "Config1");
                Assert.Fail("Exception should be thrown as ConfigPackage is Not Present");
            }
            catch (ArgumentException)
            {
                //Expected Exception
            }

            try
            {
                var settings = FabricTransportListenerSettings.LoadFrom("TestServiceListener");
                Assert.Fail("Exception should be thrown as Section is Not Present");
            }
            catch (ArgumentException)
            {
                //Expected Exception
            }
        }

        // TODO: The tests were disabled in https://msazure.visualstudio.com/One/_git/WindowsFabric/pullrequest/3920855
        // Fix and enable the test.
        // [Fact]
        public static void SecuredCommunicationTestWithThumbprints()
        {
            var fileName = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                "ServiceCommunicationTestSettings.xml");
            var settings = FabricTransportSettings.LoadFrom("TestServiceListenerTransportSettingsWithThumbprint", fileName);
            byte[] replybody = {4, 5};
            byte[] replyheader = {8};
            var header = GetRequestHeader();
            var service = new DummyService(replyheader, replybody);
            byte[] body = {4, 5};
            var listenerAddress = new FabricTransportListenerAddress("localhost", 30, Guid.NewGuid().ToString());
            var listener = new FabricTransportListener(settings, listenerAddress, service,
                new FabricTransportTestConnectionHandler());
            var task = listener.OpenAsync(CancellationToken.None);
            var address = task.Result;
            Assert.True(address.Length > 0);
            var clientsetting = FabricTransportSettings.LoadFrom("TestTransportClientTransportSettingsWithThumbprint",
                fileName);
            var client = new FabricTransportClient(clientsetting, address, null);
            var taskop = client.RequestResponseAsync(header, body, TimeSpan.FromSeconds(1000));
            var reply = taskop.Result;
            Assert.False(reply.IsException);
            Assert.True(reply.GetBody().SequenceEqual(replybody), "Client Got Reply");
            listener.CloseAsync(CancellationToken.None).Wait();
        }

        // TODO: The tests were disabled in https://msazure.visualstudio.com/One/_git/WindowsFabric/pullrequest/3920855
        // Fix and enable the test.
        // [Fact]
        public static void SecuredCommunicationTestWithSecondaryThumbprints()
        {
            var fileName = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                "ServiceCommunicationTestSettings.xml");
            var settings = FabricTransportSettings.LoadFrom("TestServiceListenerTransportSettingsWithSecondaryThumbprint", fileName);
            byte[] replybody = { 4, 5 };
            byte[] replyheader = { 8 };
            var header = GetRequestHeader();
            var service = new DummyService(replyheader, replybody);
            byte[] body = { 4, 5 };
            var listenerAddress = new FabricTransportListenerAddress("localhost", 30, Guid.NewGuid().ToString());
            var listener = new FabricTransportListener(settings, listenerAddress, service,
                new FabricTransportTestConnectionHandler());
            var task = listener.OpenAsync(CancellationToken.None);
            var address = task.Result;
            Assert.True(address.Length > 0);
            var clientSettings = FabricTransportSettings.LoadFrom("TestTransportClientTransportSettingsWithSecondaryThumbprint", fileName);

            var client = new FabricTransportClient(clientSettings, address, null);
            var taskop = client.RequestResponseAsync(header, body, TimeSpan.FromSeconds(1000));
            var reply = taskop.Result;
            Assert.False(reply.IsException);
            Assert.True(reply.GetBody().SequenceEqual(replybody), "Client Got Reply");
            listener.CloseAsync(CancellationToken.None).Wait();
        }

        [Fact]
        public static void SecuredCommunicationTestWithSubjectName()
        {
            var fileName = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                "ServiceCommunicationTestSettings.xml");
            var settings = FabricTransportSettings.LoadFrom("TestServiceListenerTransportSettingsWithSubjectName", fileName);
            byte[] replybody = { 4, 5 };
            byte[] replyheader = { 8 };
            var header = GetRequestHeader();
            var service = new DummyService(replyheader, replybody);
            byte[] body = { 4, 5 };
            var listenerAddress = new FabricTransportListenerAddress("localhost", 30, Guid.NewGuid().ToString());
            var listener = new FabricTransportListener(settings, listenerAddress, service,
                new FabricTransportTestConnectionHandler());
            var task = listener.OpenAsync(CancellationToken.None);
            var address = task.Result;
            Assert.True(address.Length > 0);
            var clientSettings = FabricTransportSettings.LoadFrom("TestTransportClientTransportSettingsWithSubjectName", fileName);
            var client = new FabricTransportClient(clientSettings, address, null);
            var taskop = client.RequestResponseAsync(header, body, TimeSpan.FromSeconds(1000));
            var reply = taskop.Result;
            Assert.False(reply.IsException);
            Assert.True(reply.GetBody().SequenceEqual(replybody), "Client Got Reply");
            listener.CloseAsync(CancellationToken.None).Wait();
        }

        // TODO: The tests were disabled in https://msazure.visualstudio.com/One/_git/WindowsFabric/pullrequest/3920855
        // Fix and enable the test.
        // [Fact]
        public static void SecuredCommunicationTestWithIssuerStore()
        {
            var fileName = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                "ServiceCommunicationTestSettings.xml");
            var settings = FabricTransportSettings.LoadFrom("TestServiceListenerTransportSettingsWithIssuerStore", fileName);
            byte[] replybody = { 4, 5 };
            byte[] replyheader = { 8 };
            var header = GetRequestHeader();
            var service = new DummyService(replyheader, replybody);
            byte[] body = { 4, 5 };
            var listenerAddress = new FabricTransportListenerAddress("localhost", 30, Guid.NewGuid().ToString());
            var listener = new FabricTransportListener(settings, listenerAddress, service,
                new FabricTransportTestConnectionHandler());
            var task = listener.OpenAsync(CancellationToken.None);
            var address = task.Result;
            Assert.True(address.Length > 0);
            var clientSettings = FabricTransportSettings.LoadFrom("TestTransportClientTransportSettingsWithIssuerStore", fileName);
            var client = new FabricTransportClient(clientSettings, address, null);
            var taskop = client.RequestResponseAsync(header, body, TimeSpan.FromSeconds(1000));
            var reply = taskop.Result;
            Assert.False(reply.IsException);
            Assert.True(reply.GetBody().SequenceEqual(replybody), "Client Got Reply");
            listener.CloseAsync(CancellationToken.None).Wait();
        }

        [Fact]
        public static void TestWhenClientTimeoutForLongRunningTask()
        {
            var settings = new FabricTransportListenerSettings();
            settings.OperationTimeout = TimeSpan.FromMinutes(1);
            byte[] replybody = {4, 5};
            byte[] replyheader = {8};
            var header = GetRequestHeader();
            //Service will drop the requestMessage and client will see timeout for these request
            var service = new DummyService(replyheader, replybody, dropMessage: true);
            byte[] body = {4, 5};
            var listenerAddress = new FabricTransportListenerAddress("localhost", 31, Guid.NewGuid().ToString());
            var listener = new FabricTransportListener(settings, listenerAddress, service,
                new FabricTransportTestConnectionHandler());
            var task = listener.OpenAsync(CancellationToken.None);
            var address = task.Result;
            Assert.True(address.Length > 0);
            var client = new FabricTransportClient(settings, address, null);
            try
            {
                var taskop = client.RequestResponseAsync(header, body, settings.OperationTimeout);
                var reply = taskop.Result;
                Assert.Fail("Request dint fail with Timeout Exception");
            }
            catch (AggregateException e)
            {
                Assert.True(e.InnerException is TimeoutException);
            }

            listener.CloseAsync(CancellationToken.None).Wait();
        }

        // TODO: The tests were disabled in https://msazure.visualstudio.com/One/_git/WindowsFabric/pullrequest/3920855
        // Fix and enable the test.
        // [Fact]
        public static void SecuredCommunicationTestWithMismatchSecurity()
        {
            var fileName = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                "ServiceCommunicationTestSettings.xml");
            var listenersettings = FabricTransportSettings.LoadFrom("TestServiceListenerTransportSettingsWithThumbprint", fileName);
            byte[] replybody = {4, 5};
            byte[] replyheader = {8};
            var header = GetRequestHeader();
            var service = new DummyService(replyheader, replybody);
            byte[] body = {4, 5};
            var listenerAddress = new FabricTransportListenerAddress("localhost", 30, Guid.NewGuid().ToString());
            var listener = new FabricTransportListener(
                listenersettings,
                listenerAddress,
                service,
                new FabricTransportTestConnectionHandler());
            var task = listener.OpenAsync(CancellationToken.None);
            var address = task.Result;
            Assert.True(address.Length > 0);
            var settings = FabricTransportSettings.LoadFrom("TestTransportClientTransportSettings");
            var client = new FabricTransportClient(settings, address, null);
            try
            {
                var taskop = client.RequestResponseAsync(header, body, TimeSpan.FromSeconds(1000));
                var reply = taskop.Result;
                Assert.Fail("FabricServerAuthenticationFailedException shuld have been thrown");
            }
            catch (AggregateException e)
            {
                Assert.True(e.InnerException is FabricServerAuthenticationFailedException , "Exception thrown " + e.Flatten().InnerException);
            }

            //Sending second Request
            try
            {
                var taskop = client.RequestResponseAsync(header, body, TimeSpan.FromSeconds(1000));
                var reply = taskop.Result;
                Assert.Fail("FabricServerAuthenticationFailedException shuld have been thrown for second request");
            }
            catch (AggregateException e)
            {
                Assert.True(e.InnerException is FabricServerAuthenticationFailedException);
            }
            listener.CloseAsync(CancellationToken.None).Wait();
        }

        [Fact]
        public static void SecuredCommunicationTestWithClientUnSecure()
        {
            var fileName = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                "ServiceCommunicationTestSettings.xml");
            var listenersettings = FabricTransportSettings.LoadFrom("TestServiceListenerTransportSettings", fileName);
            byte[] replybody = {4, 5};
            byte[] replyheader = {8};
            var header = GetRequestHeader();
            var service = new DummyService(replyheader, replybody);
            byte[] body = {4, 5};
            var listenerAddress = new FabricTransportListenerAddress("localhost", 30, Guid.NewGuid().ToString());

            //listener is opened with secure transport settings.
            var listener = new FabricTransportListener(
                listenersettings,
                listenerAddress,
                service,
                new FabricTransportTestConnectionHandler());
            var task = listener.OpenAsync(CancellationToken.None);
            var address = task.Result;
            Assert.True(address.Length > 0);
            var unSecureSettings = new FabricTransportSettings();
            //Client is created with unsecure settings
            var client = new FabricTransportClient(unSecureSettings, address, null);

            try
            {
                var taskop = client.RequestResponseAsync(header, body, TimeSpan.FromSeconds(1000));
                var reply = taskop.Result;
                Assert.Fail("FabricConnectionDeniedException should have been thrown");
            }
            catch (AggregateException e)
            {
                Assert.True(e.InnerException is FabricConnectionDeniedException);
            }
            listener.CloseAsync(CancellationToken.None).Wait();
        }

        [Fact]
        public static void OpenListenerWithDefaultSettings()
        {
            var service = new DummyService();
            var settings = FabricTransportSettings.GetDefault();
            var listenerAddress = new FabricTransportListenerAddress("localhost", 0, Guid.NewGuid().ToString());
            var remotingRemotingConnectionHandler = new FabricTransportTestConnectionHandler();
            var listener = new FabricTransportListener(settings, listenerAddress, service,
                remotingRemotingConnectionHandler);
            var task = listener.OpenAsync(CancellationToken.None);
            var address = task.Result;
            Assert.True(address.Length > 0);
            listener.CloseAsync(CancellationToken.None).Wait();
        }

        [Fact]
        public static void SimpleCommunicationTest()
        {
            byte[] replybody = {4, 5};
            byte[] replyheader = {8};
            var header = GetRequestHeader();
            var service = new DummyService(replyheader, replybody);
            byte[] body = {4, 5};
            var listenerAddress = new FabricTransportListenerAddress("localhost", 25, Guid.NewGuid().ToString());
            var settings = FabricTransportSettings.GetDefault();
            var listener = new FabricTransportListener(settings, listenerAddress, service,
                new FabricTransportTestConnectionHandler());
            var task = listener.OpenAsync(CancellationToken.None);
            var address = task.Result;
            Assert.True(address.Length > 0);
            var client = new FabricTransportClient(settings, address, null);
            var taskop = client.RequestResponseAsync(header, body, TimeSpan.FromSeconds(1000));
            var reply = taskop.Result;
            Assert.False(reply.IsException);
            Assert.True(reply.GetBody().SequenceEqual(replybody), "Client Got Reply");
           listener.CloseAsync(CancellationToken.None).Wait();

        }

        [Fact]
        public static void TestWhenServiceThrowsExceptionDuringConnect()
        {
            byte[] replybody = {4, 5};
            byte[] replyheader = {8};
            var header = GetRequestHeader();
            var service = new DummyService(replyheader, replybody);
            byte[] body = {4, 5};
            var listenerAddress = new FabricTransportListenerAddress("localhost", 25, Guid.NewGuid().ToString());
            var settings = FabricTransportSettings.GetDefault();

            var handler = new FabricTransportTestConnectionHandler(failOnConnection: true);
            var listener = new FabricTransportListener(
                settings,
                listenerAddress,
                service,
                handler);

            var task = listener.OpenAsync(CancellationToken.None);
            var address = task.Result;
            Assert.True(address.Length > 0);
            var client = new FabricTransportClient(settings, address, null);
            try
            {
                var taskop = client.RequestResponseAsync(header, body, TimeSpan.FromSeconds(1000));
                var reply = taskop.Result;
                Assert.Fail("Exception Expected As Service throws Exception during Connect");
            }
            catch (AggregateException e)
            {
                Assert.True(e.Flatten().InnerException is FabricCannotConnectException);
            }

            Assert.True(handler.count == 1, "Tried to Connect for first connect Request");
            try
            {
                var taskop = client.RequestResponseAsync(header, body, TimeSpan.FromSeconds(1000));
                var reply = taskop.Result;
                Assert.Fail("For all Request for same client should get same FabricCannotConnectException Exception ");
            }
            catch (AggregateException e)
            {
                Assert.True(e.Flatten().InnerException is FabricCannotConnectException);
            }
            Assert.True(handler.count == 1, "Not Tried to Connect, As it should take previous connect Result");
            listener.CloseAsync(CancellationToken.None).Wait();
        }

        [Fact]
        public static void TestWhenConnectRequestTimedOut()
        {
            byte[] replybody = {4, 5};
            byte[] replyheader = {8};
            var header = GetRequestHeader();
            var service = new DummyService(replyheader, replybody);
            byte[] body = {4, 5};
            var listenerAddress = new FabricTransportListenerAddress("localhost", 25, Guid.NewGuid().ToString());
            var settings = FabricTransportSettings.GetDefault();
            //handler with delay as true
            var handler = new FabricTransportTestConnectionHandler(false, true);
            var listener = new FabricTransportListener(
                settings,
                listenerAddress,
                service,
                handler);

            var task = listener.OpenAsync(CancellationToken.None);
            var address = task.Result;
            Assert.True(address.Length > 0);
            var client = new FabricTransportClient(settings, address, null);
            try
            {
                var taskop = client.RequestResponseAsync(header, body, TimeSpan.FromSeconds(15000));
                var reply = taskop.Result;
                Assert.Fail("Client Request dint throw cannotConnect for timing out connect request");
            }
            catch (AggregateException e)
            {
                Assert.True(e.Flatten().InnerException is FabricCannotConnectException);
            }

            Assert.True(handler.count == 1, "Once ConnectHandler called");
            try
            {
                var taskop = client.RequestResponseAsync(header, body, TimeSpan.FromSeconds(1000));
                var reply = taskop.Result;
                Assert.Fail("Client Request dint throw cannotConnect as connect Request failed");
            }
            catch (AggregateException e)
            {
                Assert.True(e.Flatten().InnerException is FabricCannotConnectException);
            }
            Assert.True(handler.count == 1, "client dint try to re-connect.");
            listener.CloseAsync(CancellationToken.None).Wait();
        }

        [Fact]
        public static void TestWhenSendRequestAfterClosingListener()
        {
            byte[] replybody = {4, 5};
            byte[] replyheader = {8};
            var header = GetRequestHeader();
            var service = new DummyService(replyheader, replybody);
            byte[] body = {4, 5};
            var listenerAddress = new FabricTransportListenerAddress("localhost", 121, Guid.NewGuid().ToString());
            var settings = FabricTransportSettings.GetDefault();
            var listener = new FabricTransportListener(settings, listenerAddress, service,
                new FabricTransportTestConnectionHandler());
            //Open Listener
            var task = listener.OpenAsync(CancellationToken.None);
            var address = task.Result;
            Assert.True(address.Length > 0);
            var client = new FabricTransportClient(settings, address, null);
            //Send Request
            var taskop = client.RequestResponseAsync(header, body, TimeSpan.FromSeconds(1000));
            var reply = taskop.Result;
            Assert.False(reply.IsException);
            Assert.True(reply.GetBody().SequenceEqual(replybody), "Client Got Reply");

            //Close Listener. This should disconnect client
            listener.CloseAsync(CancellationToken.None).Wait();

            //Send Request Again on Same Disconnected Client
            try
            {
                var taskop1 = client.RequestResponseAsync(header, body, TimeSpan.FromSeconds(1000));
                var reply1 = taskop1.Result;
                Assert.Fail("Request dint fail with CannotConnect Exception");
            }
            catch (AggregateException aggregateException)
            {
                var ex = aggregateException.Flatten().InnerException;
                Assert.True(ex is FabricCannotConnectException);
            }
            //Open Listener on same endpoint
            var listener1 = new FabricTransportListener(settings, listenerAddress, service,
                new FabricTransportTestConnectionHandler());
            var task1 = listener1.OpenAsync(CancellationToken.None);
            var address2 = task.Result;
            Assert.True(address2.Length > 0);
            Assert.True(address2 == address);

            //Send Request Again on disconnected client.Client won't reconnect after getting disconnected.

            try
            {
                var taskop2 = client.RequestResponseAsync(header, body, TimeSpan.FromSeconds(1000));
                var reply2 = taskop2.Result;
                Assert.Fail("Request dint fail with CannotConnect Exception");
            }
            catch (AggregateException aggregateException)
            {
                var ex = aggregateException.Flatten().InnerException;
                Assert.True(ex is FabricCannotConnectException);
            }

            listener1.CloseAsync(CancellationToken.None).Wait();
        }


        [Fact]
        public static void TestWhenSendRequestonClosedClient()
        {
            byte[] replybody = {4, 5};
            byte[] replyheader = {8};
            var header = GetRequestHeader();
            var service = new DummyService(replyheader, replybody);
            byte[] body = {4, 5};
            var listenerAddress = new FabricTransportListenerAddress("localhost", 25, Guid.NewGuid().ToString());
            var settings = FabricTransportSettings.GetDefault();
            var listener = new FabricTransportListener(settings, listenerAddress, service,
                new FabricTransportTestConnectionHandler());
            var task = listener.OpenAsync(CancellationToken.None);
            var address = task.Result;
            Assert.True(address.Length > 0);
            var client = new FabricTransportClient(settings, address, null);

            var sendRequestsTasks = new List<Task>();
            for (var i = 0; i < 100; i++)
            {
                // Some Request will fail with CannotConnect Exception as Client will be in aborted state for some request.
                sendRequestsTasks.Add(Task.Run(() => { SendRequests(client); }));
            }
            var rnd = new Random();
            var number = rnd.Next(0, 5);
            Thread.Sleep(number*100);
            //Abort Client
            client.Abort();
            Task.WaitAll(sendRequestsTasks.ToArray());
            listener.CloseAsync(CancellationToken.None).Wait();
        }

        [Fact]
        public static void SendMultipleRequestButConnectFiredOnce()
        {
            byte[] replybody = {4, 5};
            byte[] replyheader = {8};
            var header = GetRequestHeader();
            var sendRequests = new List<Task<FabricTransportReplyMessage>>();
            var service = new DummyService(replyheader, replybody);
            byte[] body = {4, 5};
            var listenerAddress = new FabricTransportListenerAddress("localhost", 25, Guid.NewGuid().ToString());
            var settings = FabricTransportSettings.GetDefault();
            var listener = new FabricTransportListener(
                settings,
                listenerAddress,
                service,
                new FabricTransportTestConnectionHandler());

            var task = listener.OpenAsync(CancellationToken.None);
            var address = task.Result;
            Assert.True(address.Length > 0);
            var eventHandler = new ClientConnectionEvent();
            //EventHandler assert to check client connect fired only once.

            var client = new FabricTransportClient(settings, address, eventHandler);

            var sendRequestsTasks = new List<Task>();
            for (var i = 0; i < 100; i++)
            {
                sendRequestsTasks.Add(
                    Task.Run(
                        () => { SendRequests(client, false); }));
                ;
            }

            Task.WaitAll(sendRequestsTasks.ToArray());
            //Connect Event shoudl have fired once.
            Assert.True(eventHandler.AsyncConnectEvent.Wait(5000));

            listener.CloseAsync(CancellationToken.None).Wait();
            Thread.Sleep(2000);
            //As Listener Close will trigger Disconnect
            Assert.True(eventHandler.AsyncDisconnectEvent.Wait(5000));
        }


        [Fact]
        public static void ServiceTooBusyExceptionnTestWhenServiceQueueIsFull()
        {
            byte[] replybody = {4, 5};
            byte[] replyheader = {8};
            var header = GetRequestHeader();
            var service = new DummyService(replyheader, replybody);
            byte[] body = {4, 5};
            var listenerAddress = new FabricTransportListenerAddress("localhost", 2, Guid.NewGuid().ToString());
            var listenerSettings = new FabricTransportListenerSettings();
            listenerSettings.MaxQueueSize = 1;
            listenerSettings.MaxConcurrentCalls = 1;
            var listener = new FabricTransportListener(
                listenerSettings,
                listenerAddress,
                service,
                new FabricTransportTestConnectionHandler());
            var task = listener.OpenAsync(CancellationToken.None);
            var address = task.Result;
            Assert.True(address.Length > 0);
            var client = new FabricTransportClient(new FabricTransportSettings(), address, null);

            var tasks = new List<Task<FabricTransportReplyMessage>>();
            for (var i = 0; i < 5; i++)
            {
                var taskop = client.RequestResponseAsync(header, body, TimeSpan.FromSeconds(1000));
                tasks.Add(taskop);
            }

            for (var i = 0; i < tasks.Count; i++)
            {
                var taskop = tasks[i];
                tasks.RemoveAt(i);
                try
                {
                    var reply = taskop.Result;
                }
                catch (AggregateException ex)
                {
                    Assert.True(ex.InnerException is FabricTransientException);
                    var transientException = (FabricTransientException) ex.InnerException;
                    Assert.True(transientException.ErrorCode.Equals(FabricErrorCode.ServiceTooBusy));
                }
            }
            listener.CloseAsync(CancellationToken.None).Wait();
        }

        // This test checks that the MaxConcurrentCalls parameter is enabled and working as expected.
        [Fact]
        public static async void TestMaxConcurrentCallsParameter()
        {
            byte[] replybody = {4, 5};
            byte[] replyheader = {8};
            var header = GetRequestHeader();
            int maxConcurrentCalls = 8;
            int numCalls = 50;
            ManualResetEventSlim mainThreadWaitEvent = new ManualResetEventSlim(false);
            var service = new DummyService(replyheader, replybody, checkConcurrentCalls: true, mainThreadWaitEvent: mainThreadWaitEvent, numThreadsToWait: maxConcurrentCalls);
            byte[] body = {4, 5};
            var listenerAddress = new FabricTransportListenerAddress("localhost", 2, Guid.NewGuid().ToString());
            var listenerSettings = new FabricTransportListenerSettings();
            listenerSettings.MaxConcurrentCalls = maxConcurrentCalls;
            var listener = new FabricTransportListener(
                listenerSettings,
                listenerAddress,
                service,
                new FabricTransportTestConnectionHandler());
            var task = listener.OpenAsync(CancellationToken.None);
            var address = task.Result;
            Assert.True(address.Length > 0);
            var client = new FabricTransportClient(new FabricTransportSettings(), address, null);

            var tasks = new List<Task<FabricTransportReplyMessage>>();
            for (var i = 0; i < numCalls; i++)
            {
                var taskop = client.RequestResponseAsync(header, body, TimeSpan.FromSeconds(1000));
                tasks.Add(taskop);
            }

            // wait for the desired number of threads to reach the semaphore
            mainThreadWaitEvent.Wait();

            service.threadWaitEvent.Set();
            await Task.WhenAll(tasks);

            Assert.True(service.maxConcurrentCallsReceived <= maxConcurrentCalls);

            listener.CloseAsync(CancellationToken.None).Wait();
        }

        // This test checks that the MaxConcurrentCalls parameter is disabled when it is set to its 
        // default value of zero. This implies all the incoming calls are processed simultaneously.
        [Fact]
        public static async void TestMaxConcurrentCallsParameterWhenZero()
        {
            byte[] replybody = {4, 5};
            byte[] replyheader = {8};
            var header = GetRequestHeader();
            int maxConcurrentCalls = 0;            
            int numCalls = 50;
            ManualResetEventSlim mainThreadWaitEvent = new ManualResetEventSlim(false);
            var service = new DummyService(replyheader, replybody, checkConcurrentCalls: true, mainThreadWaitEvent: mainThreadWaitEvent, numThreadsToWait: numCalls);
            byte[] body = {4, 5};
            var listenerAddress = new FabricTransportListenerAddress("localhost", 2, Guid.NewGuid().ToString());
            var listenerSettings = new FabricTransportListenerSettings();
            listenerSettings.MaxConcurrentCalls = maxConcurrentCalls;
            var listener = new FabricTransportListener(
                listenerSettings,
                listenerAddress,
                service,
                new FabricTransportTestConnectionHandler());
            var task = listener.OpenAsync(CancellationToken.None);
            var address = task.Result;
            Assert.True(address.Length > 0);
            var client = new FabricTransportClient(new FabricTransportSettings(), address, null);

            var tasks = new List<Task<FabricTransportReplyMessage>>();
            for (var i = 0; i < numCalls; i++)
            {
                var taskop = client.RequestResponseAsync(header, body, TimeSpan.FromSeconds(1000));
                tasks.Add(taskop);
            }

            // wait for the desired number of threads to reach the semaphore
            mainThreadWaitEvent.Wait();
            
            service.threadWaitEvent.Set();
            await Task.WhenAll(tasks);

            // The MaxConcurrentSetting is translated to the MaxThreads setting in the JobQueue.
            // If the input MaxThreads is 0, Job Queue sets it to ProcessorCount.
            // So here we are comparing maxConcurrentCallsReceived with ProcessorCount to check that
            // MaxConcurrentCalls/MaxThreads is disabled and the the JobQueue is processing all the calls simultaneously.
            Assert.True(service.maxConcurrentCallsReceived > Environment.ProcessorCount);

            listener.CloseAsync(CancellationToken.None).Wait();
        }

        [Fact]
        public static void SimpleCommunicationTestWIthInvalidAddress()
        {
            byte[] replybody = {4, 5};
            byte[] replyheader = {8};
            var header = GetRequestHeader();
            var service = new DummyService(replyheader, replybody);
            byte[] body = {4, 5};
            var listenerAddress = new FabricTransportListenerAddress("localhost", 29, Guid.NewGuid().ToString());
            var settings = FabricTransportSettings.GetDefault();
            var listener = new FabricTransportListener(settings, listenerAddress, service,
                new FabricTransportTestConnectionHandler());
            var task = listener.OpenAsync(CancellationToken.None);
            var address = task.Result;
            Assert.True(address.Length > 0);
            var invalidAdress = "net.tcp://localhost:34002/35437c01-2291-483a-ab1f-f4b8e02938e5";
            try
            {
                var client = new FabricTransportClient(settings, invalidAdress, null);
                var taskop = client.RequestResponseAsync(header, body, TimeSpan.FromSeconds(1000));
                var reply = taskop.Result;
                Assert.Fail(" Request dint fail with FabricInvalidAddressException");
            }
            catch (FabricInvalidAddressException)
            {
                //Expected this Exception
            }
            catch (Exception e)
            {
                Assert.Fail("Not the Right Type of Exception" + e.InnerException + ":" + e.StackTrace);
            }
            listener.CloseAsync(CancellationToken.None).Wait();
        }

        [Fact]
        public static void TestListenerDisconnectingClientsDuringClose()
        {
            byte[] replybody = {4, 5};
            byte[] replyheader = {8};
            var header = GetRequestHeader();
            var service = new DummyService(replyheader, replybody);
            byte[] body = {4, 5};
            var listenersettings = new FabricTransportListenerSettings();
            var settings = new FabricTransportSettings();
            var listeners = new Dictionary<string, FabricTransportListener>();
            var connectionHandler = new FabricTransportTestConnectionHandler();

            // Create Multiple listener with same transport (address and port)
            for (var i = 0; i < 15; i++)
            {
                FabricTransportListener listener;
                string address;
                var listenerAddress = new FabricTransportListenerAddress("localhost", 25, Guid.NewGuid().ToString());
                CreateAndOpenListener(service, listenerAddress, settings, connectionHandler, out listener, out address);
                listeners.Add(address, listener);
            }
            //   SendRequests to each listener
            var clients = new List<FabricTransportClient>();
            foreach (var address in listeners.Keys)
            {
                for (var i = 0; i < 50; i++)
                {
                    var client = new FabricTransportClient(settings, address, null);
                    clients.Add(client);
                    var taskop = client.RequestResponseAsync(header, body, TimeSpan.FromSeconds(1000));
                    var reply = taskop.Result;

                    Assert.True(reply.GetBody().SequenceEqual(replybody), "Client Got Reply");
                }
            }

            Assert.True(connectionHandler.count == 750, "Connected Clients should be 750");

            var tasks = new List<Task>();
            foreach (var listener in listeners.Values)
            {
                var closetask = listener.CloseAsync(CancellationToken.None);
                tasks.Add(closetask);
            }
            Task.WaitAll(tasks.ToArray());
            Assert.True(connectionHandler.count == 0, "Connected Clients should be zero");
            clients.Clear();
        }


        [Fact]
        public static void CallingConnectHandlerEvenforEndpointNotFoundScenario()
        {
            byte[] replybody = {4, 5};
            byte[] replyheader = {8};
            var header = GetRequestHeader();
            var service = new DummyService(replyheader, replybody);
            byte[] body = {4, 5};
            var listenersettings = new FabricTransportListenerSettings();
            var settings = new FabricTransportSettings();

            var connectionHandler = new FabricTransportTestConnectionHandler();

            // Create Multiple listener with same transport (address and port)

            FabricTransportListener listener1;
            string address1;
            var listenerAddress = new FabricTransportListenerAddress("localhost", 25, Guid.NewGuid().ToString());
            CreateAndOpenListener(service, listenerAddress, settings, connectionHandler, out listener1, out address1);


            FabricTransportListener listener2;
            string address2;
            var listenerAddress2 = new FabricTransportListenerAddress("localhost", 25, Guid.NewGuid().ToString());
            CreateAndOpenListener(service, listenerAddress2, settings, connectionHandler, out listener2, out address2);


            //   SendRequests to listener1
            var client1 = new FabricTransportClient(settings, address1, null);
            var taskop1 = client1.RequestResponseAsync(header, body, TimeSpan.FromSeconds(1000));
            var reply1 = taskop1.Result;

            Assert.True(reply1.GetBody().SequenceEqual(replybody), "Client Got Reply");
            // closing listener1 
            var closetask = listener1.CloseAsync(CancellationToken.None);
            closetask.GetAwaiter().GetResult();

            //Creating client2 for sending request to listener1
            var eventHandler = new ClientConnectionEvent();
            var client2 = new FabricTransportClient(settings, address1, eventHandler);

            try
            {
                var taskop2 = client2.RequestResponseAsync(header, body, TimeSpan.FromSeconds(1000));
                var reply2 = taskop2.Result;
                Assert.Fail(
                    "Request should fail with CannotConnect Exception as client is trying to connect t closed listener");
            }
            catch (AggregateException e)
            {
                Assert.True(eventHandler.AsyncConnectEvent.Wait(5000),
                    "For EndpointNotFound scenario , we call connect (This needed for backward compatibility");
                Assert.True(e.InnerException is FabricEndpointNotFoundException);
            }
            listener2.CloseAsync(CancellationToken.None).Wait();
        }


        [Fact]
        public static void SimpleCommunicationWithNoReplyHeaderTest()
        {
            byte[] replybody = {4, 5};
            var header = GetRequestHeader();
            //Service will send reply with null header.
            var service = new DummyService(header: null, body: replybody);
            byte[] body = {4, 5};
            var listenerAddress = new FabricTransportListenerAddress("localhost", 20, Guid.NewGuid().ToString());
            var settings = FabricTransportSettings.GetDefault();
            var listener = new FabricTransportListener(settings, listenerAddress, service,
                new FabricTransportTestConnectionHandler());
            var task = listener.OpenAsync(CancellationToken.None);
            var address = task.Result;
            Assert.True(address.Length > 0);
            var client = new FabricTransportClient(settings, address, null);
            var taskop = client.RequestResponseAsync(header, body, TimeSpan.FromSeconds(1000));
            var reply = taskop.Result;
            Assert.True(reply.GetBody().SequenceEqual(replybody), "Client Got Reply");
            listener.CloseAsync(CancellationToken.None).Wait();
        }

#if false //Max message size checking is only meaningful for secure mode, re-enable this in secure mode
        [Fact]
        public static void MessageTooLargeTest()
        {
            byte[] replybody = {4, 5};
            byte[] replyheader = {8};
            var header = GetRequestHeader();
            var service = new DummyService(replyheader, replybody);
            var body = new byte[500000];
            for (var i = 0; i < 500000; i++)
            {
                body[i] = (byte) i;
            }
            var listenerAddress = new FabricTransportListenerAddress("localhost", 15, Guid.NewGuid().ToString());
            var settings = FabricTransportSettings.GetDefault();
            settings.MaxMessageSize = 1;
            var listener = new FabricTransportListener(settings, listenerAddress, service,
                new FabricTransportTestConnectionHandler());
            var task = listener.OpenAsync(CancellationToken.None);
            var address = task.Result;
            Assert.True(address.Length > 0);
            var client = new FabricTransportClient(settings, address, null);
            try
            {
                var taskop = client.RequestResponseAsync(header, body, TimeSpan.FromSeconds(10));
                var result = taskop.Result;
                Assert.Fail("Request dint fail with FabricMessageTooLarge Exception ");
            }
            catch (AggregateException e)
            {
                Assert.True(e.InnerException is FabricMessageTooLargeException);
            }
            catch (Exception e)
            {
                Assert.Fail("Not the Right Type of Exception" + e.InnerException + ":" + e.StackTrace);
            }
            listener.CloseAsync(CancellationToken.None).Wait();
        }

#endif

        [Fact]
        public static void ServiceCommunicationCannotConnectTest()
        {
            byte[] replybody = {4, 5};
            byte[] replyheader = {8};
            var header = GetRequestHeader();
            var service = new DummyService(replyheader, replybody);
            byte[] body = {4, 5};
            var settings = FabricTransportSettings.GetDefault();
            var address = "localhost:62613+39c610ce-f63d-45e3-8a7b-eb4ae77dd349";
            //Creating client for  any random address.
            var client = new FabricTransportClient(settings, address, null);
            var eventHandler = new ClientConnectionEvent();
            var nativeeventHandler = new FabricTransportNativeClientConnectionEventHandler(eventHandler);
            try
            {
                var taskop = client.RequestResponseAsync(header, body, TimeSpan.FromSeconds(10));
                var result = taskop.Result;
                Assert.Fail("Request dint fail with FabricCannotConnect Exception ");
            }
            catch (AggregateException e)
            {
                Assert.False(eventHandler.AsyncConnectEvent.Wait(5000));
                Assert.True(e.InnerException is FabricCannotConnectException);
            }
            catch (Exception e)
            {
                Assert.Fail("Not the Right Type of Exception" + e.InnerException + ":" + e.StackTrace);
            }
        }

        [Fact]
        public static void ServiceCommunicationFabricEndpointNotFoundTest()
        {
            byte[] replybody = {4, 5};
            byte[] replyheader = {8};
            var header = GetRequestHeader();
            var service = new DummyService(replyheader, replybody);
            byte[] body = {4, 5};
            var settings = FabricTransportSettings.GetDefault();
            var listenerAddress = new FabricTransportListenerAddress("localhost", 19, Guid.NewGuid().ToString());
            var listener = new FabricTransportListener(settings,
                listenerAddress, service,
                new FabricTransportTestConnectionHandler());
            var task = listener.OpenAsync(CancellationToken.None);
            var address = task.Result;
            Assert.True(address.Length > 0);
            var address1 = address.Split('+')[0] + "+1234";
            //Creaing Client for same ip address as opened listener but different service path
            var client = new FabricTransportClient(settings, address1, null);

            try
            {
                var taskop = client.RequestResponseAsync(header, body, TimeSpan.FromSeconds(100));
                var result = taskop.Result;
                Assert.Fail("This should throw FabricEndpointNotFound Exception ");
            }
            catch (AggregateException e)
            {
                Assert.True(e.InnerException is FabricEndpointNotFoundException, "Exception :" + e.InnerException);
            }
            catch (Exception e)
            {
                Assert.Fail("Not the Right Type of Exception" + e.InnerException + ":" + e.StackTrace);
            }
            listener.CloseAsync(CancellationToken.None).Wait();
        }

        [Fact]
        public static void SimpleNotificationAsyncTest()
        {
            byte[] replybody = {4, 5};
            byte[] replynotification = {4, 5};
            byte[] notificationByte = {4, 5};
            byte[] replyheader = {8};
            var header = GetRequestHeader();
            var service = new DummyService(replyheader, replybody);
            var callback = new DummyCallBackImplementation(replynotification);
            byte[] body = {4, 5};
            var listenerAddress = new FabricTransportListenerAddress("localhost", 10, Guid.NewGuid().ToString());
            var settings = FabricTransportSettings.GetDefault();
            var remotingRemotingConnectionHandler = new FabricTransportTestConnectionHandler();
            //Create and Open Listener
            var listener = new FabricTransportListener(settings, listenerAddress, service,
                remotingRemotingConnectionHandler);
            var task = listener.OpenAsync(CancellationToken.None);
            var address = task.Result;
            Assert.True(address.Length > 0);
            var client = new FabricTransportClient(settings, address, null, callback);
            //Register For Notification
            var taskop = client.RequestResponseAsync(header, body, TimeSpan.FromSeconds(1000));
            var reply = taskop.Result;
            Assert.True(reply.GetBody().SequenceEqual(replybody));
            //SendNotification
            var notificationTask = service.SendNotificationAsync();
            var replymessage = notificationTask.Result;
            Assert.True(replymessage.SequenceEqual(replynotification), "Client Got Notification");
            client.Abort();
            listener.Abort();
        }

        [Fact]
        public static void SimpleNotificationOneWayTest()
        {
            byte[] replybody = {4, 5};
            byte[] replynotification = {4, 5};

            byte[] notificationByte = {4, 5};
            byte[] replyheader = {8};
            var header = GetRequestHeader();
            var service = new DummyService(replyheader, replybody);
            var callback = new DummyCallBackImplementation(replynotification);
            byte[] body = {4, 5};
            var listenerAddress = new FabricTransportListenerAddress("localhost", 15, Guid.NewGuid().ToString());
            var settings = FabricTransportSettings.GetDefault();
            var remotingRemotingConnectionHandler = new FabricTransportTestConnectionHandler();
            //Create and Open Listener
            var listener = new FabricTransportListener(settings, listenerAddress, service,
                remotingRemotingConnectionHandler);
            var task = listener.OpenAsync(CancellationToken.None);
            var address = task.Result;
            Assert.True(address.Length > 0);
            var client = new FabricTransportClient(settings, address, null, callback);
            //Register For Notification
            var taskop = client.RequestResponseAsync(header, body, TimeSpan.FromSeconds(4000));
            service.AsyncEvent.Set();
            var reply = taskop.Result;
            Assert.True(reply.GetBody().SequenceEqual(replybody));
            //SendNotification
            service.SendNotificationOneWay();

            Assert.True(callback.AsyncEvent.WaitOne(TimeSpan.FromMilliseconds(5000)));
            Assert.True(replybody.SequenceEqual(callback.GetNotificationMessage()), "Client Got Notification");
            listener.CloseAsync(CancellationToken.None).Wait();
            client.Abort();
        }

        private static byte[] GetRequestHeader()
        {
            return new byte[10];
        }

        private static void CreateAndOpenListener(
            DummyService service,
            FabricTransportListenerAddress listenerAddress,
            FabricTransportSettings settings,
            FabricTransportTestConnectionHandler connectionHandler,
            out FabricTransportListener listener,
            out string address)
        {
            listener = new FabricTransportListener(
                settings,
                listenerAddress,
                service,
                connectionHandler);
            var task = listener.OpenAsync(CancellationToken.None);
            address = task.Result;
            Assert.True(address.Length > 0);
        }

        private static void SendRequests(FabricTransportClient client, bool ExceptionExpected = true)
        {
            byte[] body = {4, 5};
            var header = GetRequestHeader();
            var sendRequests = new List<Task<FabricTransportReplyMessage>>();

            for (var i = 0; i < 100; i++)
            {
                sendRequests.Add(client.RequestResponseAsync(header, body, TimeSpan.FromSeconds(1000)));
            }
            for (var i = 0; i < sendRequests.Count; i++)
            {
                try
                {
                    var res = sendRequests[i].Result;
                }
                catch (AggregateException e)
                {
                    if (!ExceptionExpected)
                    {
                        throw;
                    }
                    var ex = e.Flatten().InnerException;
                    Assert.True(ex is FabricCannotConnectException, e.Message);
                }
            }
        }
    }


  internal class DummyCallBackImplementation : IFabricTransportCallbackMessageHandler
    {
        private readonly byte[] replybody;
        private byte[] notificationMsg;

        public DummyCallBackImplementation(byte[] reply)
        {
            this.replybody = reply;
            this.AsyncEvent = new ManualResetEvent(false);
        }

        public ManualResetEvent AsyncEvent { get; private set; }


        public byte[] GetNotificationMessage()
        {
            return this.notificationMsg;
        }

        public Task<byte[]> RequestResponseAsync(byte[] messageHeaders, byte[] requestBody)
        {
            return Task.Factory.StartNew<byte[]>(() => { return this.replybody; });
        }

        public void OneWayMessage(byte[] messageHeaders, byte[] requestBody)
        {
            this.notificationMsg = requestBody;
            this.AsyncEvent.Set();
        }
    }

    internal class DummyService : IFabricTransportMessageHandler
    {
        private FabricTransportRequestContext requestContext;
        private readonly byte[] header;
        private readonly byte[] body;

        private readonly bool dropMessage;
        private readonly Object thisLock;
        private readonly bool checkConcurrentCalls;
        private volatile int currentThreadCount;
        private readonly int numThreadsToWait;
        private readonly ManualResetEventSlim mainThreadWaitEvent;


        public DummyService(bool dropMessage = false)
        {
            this.dropMessage = dropMessage;
        }

        public DummyService(byte[] header, byte[] body, bool dropMessage = false,
                            bool checkConcurrentCalls = false, int numThreadsToWait = -1,
                            ManualResetEventSlim mainThreadWaitEvent = null)
        {
            this.AsyncEvent = new ManualResetEventSlim();
            this.AsyncEvent.Reset();
            this.header = header;
            this.body = body;
            this.dropMessage = dropMessage;
            this.thisLock = new Object();
            this.checkConcurrentCalls = checkConcurrentCalls;
            this.currentThreadCount = 0;
            // create an ManualResetEventSlim object in an unsignalled state
            this.threadWaitEvent = new ManualResetEventSlim(false);
            this.maxConcurrentCallsReceived = 0;
            this.numThreadsToWait = numThreadsToWait;
            this.mainThreadWaitEvent = mainThreadWaitEvent;
        }

        public ManualResetEventSlim AsyncEvent { get; private set; }
        public ManualResetEventSlim threadWaitEvent { get; private set; }
        public int maxConcurrentCallsReceived { get; private set; }

        public Task<byte[]> SendNotificationAsync()
        {
            return this.requestContext.GetCallbackClient().RequestResponseAsync(this.header, this.body);
        }

        public void SendNotificationOneWay()
        {
            this.requestContext.GetCallbackClient().OneWayMessage(this.header, this.body);
        }

        public Task<FabricTransportReplyMessage> RequestResponseAsync(FabricTransportRequestContext requestContext,
            byte[] messageHeaders, byte[] requestBody)
        {
            return Task.Run(
                () =>
                {
                    if (this.dropMessage)
                    {
                        Thread.Sleep(TimeSpan.FromMinutes(5));
                    }
                    if (this.checkConcurrentCalls && this.mainThreadWaitEvent != null && this.numThreadsToWait != -1)
                    {
                        Interlocked.Increment(ref this.currentThreadCount);
                        lock (this.thisLock)
                        {
                            this.maxConcurrentCallsReceived = Math.Max(this.maxConcurrentCallsReceived, this.currentThreadCount);
                        }
                        // if at least numThreadsToWait threads have entered, signal the main thread to continue.
                        if (this.currentThreadCount >= this.numThreadsToWait)
                        {
                            this.mainThreadWaitEvent.Set();
                        }

                        // the thread will wait here until it is released by the main thread
                        this.threadWaitEvent.Wait();
                        Interlocked.Decrement(ref this.currentThreadCount);
                    }
                    this.requestContext = requestContext;
                    return new FabricTransportReplyMessage(false, this.body);
                });
        }

        public void HandleOneWay(FabricTransportRequestContext requestContext, byte[] messageHeaders, byte[] requestBody)
        {
            ;
        }
    }

    public class FabricTransportTestConnectionHandler : IFabricTransportConnectionHandler
    {
        public int count = 0;
        private readonly Object thisLock = new Object();
        private readonly bool failOnConnection;
        private readonly bool isDelay;
		private FabricTransportCallbackClient callback;
		
        public FabricTransportTestConnectionHandler(bool failOnConnection = false, bool isdelay = false)
        {
            this.failOnConnection = failOnConnection;
            this.isDelay = isdelay;
        }

        Task IFabricTransportConnectionHandler.ConnectAsync(
            FabricTransportCallbackClient fabricTransportServiceRemotingCallback, TimeSpan timeout)
        {
            lock (this.thisLock)
                this.count++;
			this.callback = fabricTransportServiceRemotingCallback;
            if (this.failOnConnection)
            {
                throw new InvalidEnumArgumentException();
            }
            if (this.isDelay)
            {
//More than default connect Timeout(5 sec)
                return Task.Delay(TimeSpan.FromSeconds(7));
            }

            return Task.FromResult(true);
        }

        Task IFabricTransportConnectionHandler.DisconnectAsync(string clientId, TimeSpan timeout)
        {
            lock (this.thisLock)
                this.count--;
            return Task.FromResult(true);
        }

        FabricTransportCallbackClient IFabricTransportConnectionHandler.GetCallBack(string clientId)
        {
           return this.callback;
        }
    }

    internal class ClientConnectionEvent : IFabricTransportClientConnectionHandler
    {
        public ClientConnectionEvent()
        {
            this.AsyncConnectEvent = new ManualResetEventSlim();
            this.AsyncDisconnectEvent = new ManualResetEventSlim();
        }

        public void OnConnected()
        {
            Assert.False(this.AsyncConnectEvent.IsSet, "Connect Event not yet fired");
            this.AsyncConnectEvent.Set();
        }

        public ManualResetEventSlim AsyncConnectEvent { get; set; }
        public ManualResetEventSlim AsyncDisconnectEvent { get; set; }


        public void OnDisconnected()
        {
            Assert.True(this.AsyncConnectEvent.IsSet, "Connect Fired Once");
            Assert.False(this.AsyncDisconnectEvent.IsSet, "Disconnect Not Fired yet");
            this.AsyncDisconnectEvent.Set();
        }
    }
}
