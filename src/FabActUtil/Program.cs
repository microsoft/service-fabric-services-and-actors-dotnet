// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace FabActUtil
{
    using System;
    using System.IO;
    using System.Reflection;
    using FabActUtil.CommandLineParser;

    internal class Program
    {
        private static string assemblyResolvePath;

        private static int Main(string[] args)
        {
            var parsedArguments = new ToolArguments();
            if (!CommandLineUtility.ParseCommandLineArguments(args, parsedArguments) || !parsedArguments.IsValid())
            {
                Console.Write(CommandLineUtility.CommandLineArgumentsUsage(typeof(ToolArguments)));
                return -1;
            }

            try
            {
                assemblyResolvePath = parsedArguments.AssemblyResolvePath;
                var currentDomain = AppDomain.CurrentDomain;
                currentDomain.AssemblyResolve += new ResolveEventHandler(ResolveHandler);

                Tool.Run(parsedArguments);
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
                return -1;
            }

            return 0;
        }

        private static Assembly ResolveHandler(object sender, ResolveEventArgs args)
        {
            // The ResolveHandler is called if the dependencies are not in same location as the executable.
            if (assemblyResolvePath != null)
            {
                if (Directory.Exists(assemblyResolvePath))
                {
                    // try to load dll and then exe
                    var assemblyName = new AssemblyName(args.Name).Name;
                    var assemblyPath = Path.Combine(assemblyResolvePath, assemblyName + ".dll");

                    if (File.Exists(assemblyPath))
                    {
                        return Assembly.LoadFrom(assemblyPath);
                    }
                    else
                    {
                        assemblyPath = Path.Combine(assemblyResolvePath, assemblyName + ".exe");
                        if (File.Exists(assemblyPath))
                        {
                            return Assembly.LoadFrom(assemblyPath);
                        }
                    }
                }
            }

            return null;
        }
    }
}
