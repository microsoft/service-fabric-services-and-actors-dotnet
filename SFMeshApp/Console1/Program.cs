using System;
using System.IO;
using System.Reflection;
using System.Threading;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using Microsoft.ServiceFabric.Services.Remoting.FabricTransport;
using Microsoft.ServiceFabric.Services.Remoting.FabricTransport.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.V2.FabricTransport.Client;

namespace Console1
{
    class Program
    {
        static Program()
        {
        //    AppDomain.CurrentDomain.AssemblyResolve += LoadFromFabricCodePath;
        }
        static void Main(string[] args)
        {
            //Things to bo done.
            /*1 Add IServicePartitioResolve Implemententation
             2. Publish ENdpoint to Dns/Naming Service
             3 Implement your own FabricTransportRemotingClientFactory Impl using new FabricTransportClientPulic withourt serviceContext*/
            var partitionId = Guid.NewGuid();

          //  ProcessDirectory(Directory.GetCurrentDirectory());
            var settings = new FabricTransportRemotingListenerSettings();
            settings.EndpointResourceName = "Console1Listener";
            var listener = new Microsoft.ServiceFabric.Services.Remoting.V2.FabricTransport.Runtime.FabricTransportServiceRemotingListener(partitionId,
                new Class1(),
                settings);
            var endpoint = listener.OpenAsync(CancellationToken.None).Result;
            Console.WriteLine("Endpoint Listener {0}",endpoint);
            var endpoint2 = string.Format("{0}:20004 +{1}", "Console1", partitionId);
            var proxyfactory = new ServiceProxyFactory((c) =>
            { return new MyRemotingClientFactory(endpoint); }
            );
            var proxy = proxyfactory.CreateServiceProxy<IMYService>(new Uri("fabric:\\MyService"));
            try
            {
                var result = proxy.GetWord().Result;
            }
            catch (Exception){

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

        private static Assembly LoadFromFabricCodePath(object sender, ResolveEventArgs args)
        {
            string assemblyName = new AssemblyName(args.Name).Name;

         
            try
            {
                string assemblyPath = Path.Combine("C:\\app\\bin\\x64\\Debug\\netcoreapp2.0", assemblyName + ".dll");
                if (File.Exists(assemblyPath))
                {
                    return Assembly.LoadFrom(assemblyPath);
                }
            }
            catch (Exception e)
            {
                // Supress any Exception so that we can continue to
                // load the assembly through other means
                Console.WriteLine("Exception in LoadFromFabricCodePath={0}", e.ToString());
            }

            return null;
        }
    }
}
