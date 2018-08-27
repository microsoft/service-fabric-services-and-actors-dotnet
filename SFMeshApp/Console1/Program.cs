using System;
using System.IO;
using System.Reflection;
using System.Threading;
using Microsoft.ServiceFabric.Services.Remoting.Mesh.Client;
using Microsoft.ServiceFabric.Services.Remoting.Mesh.FabricTransport.Runtime;

namespace Console1
{
    class Program
    {

        static void Main(string[] args)
        {
            //Things to bo done.
            /*1 Add IServicePartitioResolve Implemententation
             2. Publish ENdpoint to Dns/Naming Service
             3 Implement your own FabricTransportRemotingClientFactory Impl using new FabricTransportClientPulic withourt serviceContext*/
            var partitionId = Guid.NewGuid();

            ProcessDirectory(Directory.GetCurrentDirectory());
            var settings = new FabricTransportRemotingMeshListenerSettings();
            settings.EndpointResourceName = "Console1Listener";

            var listener =
                new Microsoft.ServiceFabric.Services.Remoting.Mesh.Runtime.FabricTransportServiceRemotingListener(
                    partitionId,
                    new Class1(),
                    settings);
            var endpoint = listener.OpenAsync(CancellationToken.None).Result;
            Console.WriteLine("Endpoint Listener {0}", endpoint);
            var endpoint2 = string.Format("{0}:20004 +{1}", "Console1", partitionId);
            var proxyfactory = new ServiceProxyFactory((c) =>
                {
                    return new Microsoft.ServiceFabric.Services.Remoting.Mesh.Client.
                        FabricTransportServiceRemotingClientFactory(endpoint);
                }
            );
            var proxy = proxyfactory.CreateServiceProxy<IMYService>(new Uri("fabric:\\MyService"));
            try
            {
                var result = proxy.GetWord().Result;
            }
            catch (Exception)
            {

            }

            Thread.Sleep(Timeout.Infinite);

        }

        // Process all files in the directory passed in, recurse on any directories 
        // that are found, and process the files they contain.
        public static void ProcessDirectory(string targetDirectory)
        {
            // Process the list of files found in the directory.
            string[] fileEntries = Directory.GetFiles(targetDirectory);
            foreach (string fileName in fileEntries)
                ProcessFile(fileName);

            // Recurse into subdirectories of this directory.
            string[] subdirectoryEntries = Directory.GetDirectories(targetDirectory);
            foreach (string subdirectory in subdirectoryEntries)
                ProcessDirectory(subdirectory);
        }

        // Insert logic for processing found files here.
        public static void ProcessFile(string path)
        {
            Console.WriteLine("Processed file '{0}'.", path);
        }

    }
}
