// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Remoting.Tests.V2.ExceptionConvertors
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Resources;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Runtime.Serialization;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.ServiceFabric.Services.Remoting.FabricTransport.Runtime;
    using Microsoft.ServiceFabric.Services.Remoting.V2.Messaging;
    using Xunit;

    /// <summary>
    /// SystemExceptionConvertor test.
    /// </summary>
    public class SystemExceptionConvertorTest
    {
        private static List<Remoting.V2.Runtime.IExceptionConvertor> runtimeConvertors
           = new List<Remoting.V2.Runtime.IExceptionConvertor>()
           {
                new Remoting.V2.Runtime.FabricExceptionConvertor(),
                new Remoting.V2.Runtime.SystemExceptionConvertor(),
                new Remoting.V2.Runtime.ExceptionConversionHandler.DefaultExceptionConvertor(),
           };

        private static Remoting.V2.Runtime.ExceptionConversionHandler runtimeHandler
            = new Remoting.V2.Runtime.ExceptionConversionHandler(runtimeConvertors, new FabricTransportRemotingListenerSettings()
            {
                RemotingExceptionDepth = 3,
                ExceptionSerializationTechnique = FabricTransportRemotingListenerSettings.ExceptionSerialization.Default,
            });

        private static List<Remoting.V2.Client.IExceptionConvertor> clientConvertors
            = new List<Remoting.V2.Client.IExceptionConvertor>()
            {
                new Remoting.V2.Client.SystemExceptionConvertor(),
                new Remoting.V2.Client.FabricExceptionConvertor(),
            };

        private static Remoting.V2.Client.ExceptionConversionHandler clientHandler
            = new Remoting.V2.Client.ExceptionConversionHandler(
                clientConvertors,
                new FabricTransport.FabricTransportRemotingSettings()
                {
                    ExceptionDeserializationTechnique = FabricTransport.FabricTransportRemotingSettings.ExceptionDeserialization.Default,
                });

        private static List<SystemException> systemExceptions = new List<SystemException>()
        {
            new AccessViolationException("AccessViolationException"),
            new AppDomainUnloadedException("AppDomainUnloadedException"),
            new ArgumentException("ArgumentException"),
            new ArgumentException("ArgumentException", "MyParam1"),
            new ArithmeticException("ArithmeticException"),
            new ArrayTypeMismatchException("ArrayTypeMismatchException"),
            new BadImageFormatException("BadImageFormatException"),
            new BadImageFormatException("BadImageFormatException", "MyFile1"),
            new CannotUnloadAppDomainException("CannotUnloadAppDomainException"),
            new KeyNotFoundException("KeyNotFoundException"),
            new ContextMarshalException("ContextMarshalException"),
            new DataMisalignedException("DataMisalignedException"),
#pragma warning disable CS0618 // Type or member is obsolete
            new ExecutionEngineException("ExecutionEngineException"),
#pragma warning restore CS0618 // Type or member is obsolete
            new FormatException("FormatException"),
            new IndexOutOfRangeException("IndexOutOfRangeException"),
            new InsufficientExecutionStackException("InsufficientExecutionStackException"),
            new InvalidCastException("InvalidCastException"),
            new InvalidCastException("InvalidCastException", 0x00000000),
            new InvalidOperationException("InvalidOperationException"),
            new InvalidProgramException("InvalidProgramException"),
            new InternalBufferOverflowException("InternalBufferOverflowException"),
            new InvalidDataException("InvalidDataException"),
            new IOException("IOException"),
            new IOException("IOException", 0x00000000),
            new MemberAccessException("MemberAccessException"),
            new MulticastNotSupportedException("MulticastNotSupportedException"),
            new NotImplementedException("NotImplementedException"),
            new NotSupportedException("NotSupportedException"),
            new NullReferenceException("NullReferenceException"),
            new OperationCanceledException("OperationCanceledException"),
            new OutOfMemoryException("OutOfMemoryException"),
            new RankException("RankException"),
            new AmbiguousMatchException("AmbiguousMatchException"),
            new ReflectionTypeLoadException(null, null, "ReflectionTypeLoadException"),
            new ReflectionTypeLoadException(new Type[] { typeof(ReflectionTypeLoadException), typeof(ReflectionTypeLoadException) }, null, "ReflectionTypeLoadException"),
            new MissingManifestResourceException("MissingManifestResourceException"),
            new MissingSatelliteAssemblyException("MissingSatelliteAssemblyException"),
            new MissingSatelliteAssemblyException("MissingSatelliteAssemblyException", "en-US"),
            new ExternalException("ExternalException"),
            new ExternalException("ExternalException", 0x00000000),
            new InvalidComObjectException("InvalidComObjectException"),
            new InvalidOleVariantTypeException("InvalidOleVariantTypeException"),
            new MarshalDirectiveException("MarshalDirectiveException"),
            new SafeArrayRankMismatchException("SafeArrayRankMismatchException"),
            new SafeArrayTypeMismatchException("SafeArrayTypeMismatchException"),
            new SerializationException("SerializationException"),
            new StackOverflowException("StackOverflowException"),
            new AbandonedMutexException("AbandonedMutexException"),
            new SemaphoreFullException("SemaphoreFullException"),
            new SynchronizationLockException("SynchronizationLockException"),
            new ThreadInterruptedException("ThreadInterruptedException"),
            new ThreadStateException("ThreadStateException"),
            new TimeoutException("TimeoutException"),
            new TypeInitializationException("TypeInitializationException", null),
            new TypeLoadException("TypeLoadException"),
            new TypeUnloadedException("TypeUnloadedException"),
            new UnauthorizedAccessException("UnauthorizedAccessException"),
            new ArgumentNullException("MyParam1"),
            new ArgumentNullException("ArgumentNullException", (Exception)null),
            new ArgumentNullException("MyParam1", "ArgumentNullException"),
            new FileNotFoundException("FileNotFoundException"),
            new FileNotFoundException("FileNotFoundException", "MyFile1"),
            new DirectoryNotFoundException("DirectoryNotFoundException"),
            new ObjectDisposedException("MyObject1"),
            new ObjectDisposedException("ObjectDisposedException", (Exception)null),
            new ObjectDisposedException("MyObject1", "ObjectDisposedException"),
        };

        /// <summary>
        /// Known types test.
        /// </summary>
        /// <returns>Task representing async operation.</returns>
        [Fact]
        public static async Task KnownSystemExceptionSerializationTest()
        {
            foreach (var exception in systemExceptions)
            {
                var serializedData = runtimeHandler.SerializeRemoteException(exception);
                var msgStream = new SegmentedReadMemoryStream(serializedData);

                Exception resultEx = null;
                try
                {
                    await clientHandler.DeserializeRemoteExceptionAndThrowAsync(msgStream);
                }
                catch (AggregateException ex)
                {
                    resultEx = ex.InnerException;
                }

                Assert.True(resultEx != null);
                Assert.Equal(resultEx.GetType(), exception.GetType());
                if (!exception.GetType().Equals(typeof(ArgumentException))
                    && !exception.GetType().Equals(typeof(ArgumentNullException))
                    && !exception.GetType().Equals(typeof(ObjectDisposedException)))
                {
                    // These exception types change the Message field. Its important to rehydrate the other fields.
                    Assert.Equal(resultEx.Message, exception.Message);
                }

                Assert.Equal(resultEx.HResult, exception.HResult);

                if (exception is ArgumentException argEx)
                {
                    if (argEx.ParamName != null)
                    {
                        Assert.Equal(((ArgumentException)resultEx).ParamName, argEx.ParamName);
                    }
                }
                else if (exception is BadImageFormatException badImage)
                {
                    Assert.Equal(((BadImageFormatException)resultEx).FileName, badImage.FileName);
                    Assert.Equal(((BadImageFormatException)resultEx).FusionLog, badImage.FusionLog);
                }
                else if (exception is ReflectionTypeLoadException typeLoad)
                {
                    if (typeLoad.Types != null)
                    {
                        Assert.True(((ReflectionTypeLoadException)resultEx).Types.SequenceEqual(typeLoad.Types));
                    }
                }
                else if (exception is MissingSatelliteAssemblyException satellite)
                {
                    Assert.Equal(((MissingSatelliteAssemblyException)resultEx).CultureName, satellite.CultureName);
                }
                else if (exception is ObjectDisposedException disposedEx)
                {
                    Assert.Equal(((ObjectDisposedException)resultEx).ObjectName, disposedEx.ObjectName);
                }
                else if (exception is FileNotFoundException fileNotFound)
                {
                    Assert.Equal(((FileNotFoundException)resultEx).FileName, fileNotFound.FileName);
                }
            }
        }

        /// <summary>
        /// Exception depth test.
        /// </summary>
        /// <returns>Task representing async operation.</returns>
        [Fact]
        public static async Task ExceptionDepthTest()
        {
            var aggregateException = new AggregateException(new List<Exception>()
           {
               NestedAggregateEx(4, 4),
               NestedReflectionEx(4, 4),
               NestedAggregateEx(4, 4),
               NestedReflectionEx(4, 4),
           });

            var serializedData = runtimeHandler.SerializeRemoteException(aggregateException);
            var msgStream = new SegmentedReadMemoryStream(serializedData);

            Exception resultEx = null;
            try
            {
                await clientHandler.DeserializeRemoteExceptionAndThrowAsync(msgStream);
            }
            catch (AggregateException ex)
            {
                resultEx = ex;
            }

            Assert.True(resultEx != null);
            Assert.True(((AggregateException)resultEx).InnerExceptions.Count == 3);
            Assert.True(((AggregateException)((AggregateException)resultEx).InnerExceptions[0]).InnerExceptions.Count == 3);
            Assert.True(((AggregateException)((AggregateException)((AggregateException)resultEx).InnerExceptions[0]).InnerExceptions[0]).InnerExceptions.Count == 0);
            Assert.True(((ReflectionTypeLoadException)((AggregateException)resultEx).InnerExceptions[1]).LoaderExceptions.Length == 3);
            Assert.True(((ReflectionTypeLoadException)((ReflectionTypeLoadException)((AggregateException)resultEx).InnerExceptions[1]).LoaderExceptions[0]).LoaderExceptions == null);
        }

        private static AggregateException NestedAggregateEx(int depth, int breadth)
        {
            if (depth == 1)
            {
                var inner1 = new List<Exception>();
                for (int i = 0; i < breadth; i++)
                {
                    inner1.Add(new Exception($"Leaf{i}"));
                }

                return new AggregateException(inner1);
            }

            List<AggregateException> inner = new List<AggregateException>();
            for (int i = 0; i < breadth; i++)
            {
                inner.Add(NestedAggregateEx(depth - 1, breadth));
            }

            return new AggregateException(inner);
        }

        private static ReflectionTypeLoadException NestedReflectionEx(int depth, int breadth)
        {
            if (depth == 1)
            {
                var inner1 = new List<Exception>();
                for (int i = 0; i < breadth; i++)
                {
                    inner1.Add(new Exception($"Leaf{i}"));
                }

                return new ReflectionTypeLoadException(null, inner1.ToArray());
            }

            List<ReflectionTypeLoadException> inner = new List<ReflectionTypeLoadException>();
            for (int i = 0; i < breadth; i++)
            {
                inner.Add(NestedReflectionEx(depth - 1, breadth));
            }

            return new ReflectionTypeLoadException(null, inner.ToArray());
        }
    }
}
