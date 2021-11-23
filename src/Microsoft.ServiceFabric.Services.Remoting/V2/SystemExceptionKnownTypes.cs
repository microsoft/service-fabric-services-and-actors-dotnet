// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Services.Remoting.V2
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using System.Resources;
    using System.Runtime.InteropServices;
    using System.Runtime.Serialization;
    using System.Threading;
    using System.Xml;
    using Microsoft.ServiceFabric.Services.Communication;

    internal class SystemExceptionKnownTypes
    {
#pragma warning disable SA1401 // Fields should be private
        public static IDictionary<string, ConvertorFuncs> ServiceExceptionConvertors =
#pragma warning restore SA1401 // Fields should be private
            new Dictionary<string, ConvertorFuncs>()
        {
                {
                    "System.AccessViolationException", new ConvertorFuncs()
                    {
                        ToServiceExFunc = ex => ToServiceException(ex),
                        FromServiceExFunc = (svcEx, innerEx) => FromServiceException<AccessViolationException>(svcEx, innerEx),
                        InnerExFunc = ex => GetInnerExceptions(ex),
                    }
                },
                {
                    "System.AppDomainUnloadedException", new ConvertorFuncs()
                    {
                        ToServiceExFunc = ex => ToServiceException(ex),
                        FromServiceExFunc = (svcEx, innerEx) => FromServiceException<AppDomainUnloadedException>(svcEx, innerEx),
                        InnerExFunc = ex => GetInnerExceptions(ex),
                    }
                },
                {
                    "System.ArgumentException",  new ConvertorFuncs()
                    {
                        ToServiceExFunc = ex => ToServiceException((ArgumentException)ex),
                        FromServiceExFunc = (svcEx, innerEx) => FromServiceException<ArgumentException>(svcEx, innerEx),
                        InnerExFunc = ex => GetInnerExceptions(ex),
                    }
                },
                {
                    "System.ArithmeticException", new ConvertorFuncs()
                    {
                        ToServiceExFunc = ex => ToServiceException(ex),
                        FromServiceExFunc = (svcEx, innerEx) => FromServiceException<ArithmeticException>(svcEx, innerEx),
                        InnerExFunc = ex => GetInnerExceptions(ex),
                    }
                },
                {
                    "System.ArrayTypeMismatchException", new ConvertorFuncs()
                    {
                        ToServiceExFunc = ex => ToServiceException(ex),
                        FromServiceExFunc = (svcEx, innerEx) => FromServiceException<ArrayTypeMismatchException>(svcEx, innerEx),
                        InnerExFunc = ex => GetInnerExceptions(ex),
                    }
                },
                {
                    "System.BadImageFormatException", new ConvertorFuncs()
                    {
                        ToServiceExFunc = ex => ToServiceException((BadImageFormatException)ex),
                        FromServiceExFunc = (svcEx, innerEx) => FromServiceException<BadImageFormatException>(svcEx, innerEx),
                        InnerExFunc = ex => GetInnerExceptions(ex),
                    }
                },
                {
                    "System.CannotUnloadAppDomainException", new ConvertorFuncs()
                    {
                        ToServiceExFunc = ex => ToServiceException(ex),
                        FromServiceExFunc = (svcEx, innerEx) => FromServiceException<CannotUnloadAppDomainException>(svcEx, innerEx),
                        InnerExFunc = ex => GetInnerExceptions(ex),
                    }
                },
                {
                    "System.KeyNotFoundException", new ConvertorFuncs()
                    {
                        ToServiceExFunc = ex => ToServiceException(ex),
                        FromServiceExFunc = (svcEx, innerEx) => FromServiceException<KeyNotFoundException>(svcEx, innerEx),
                        InnerExFunc = ex => GetInnerExceptions(ex),
                    }
                },
                {
                    "System.ContextMarshalException", new ConvertorFuncs()
                    {
                        ToServiceExFunc = ex => ToServiceException(ex),
                        FromServiceExFunc = (svcEx, innerEx) => FromServiceException<ContextMarshalException>(svcEx, innerEx),
                        InnerExFunc = ex => GetInnerExceptions(ex),
                    }
                },
                {
                    "System.DataMisalignedException", new ConvertorFuncs()
                    {
                        ToServiceExFunc = ex => ToServiceException(ex),
                        FromServiceExFunc = (svcEx, innerEx) => FromServiceException<DataMisalignedException>(svcEx, innerEx),
                        InnerExFunc = ex => GetInnerExceptions(ex),
                    }
                },
                {
                    "System.ExecutionEngineException", new ConvertorFuncs()
                    {
                        ToServiceExFunc = ex => ToServiceException(ex),
#pragma warning disable CS0618 // Type or member is obsolete
                        FromServiceExFunc = (svcEx, innerEx) => FromServiceException<ExecutionEngineException>(svcEx, innerEx),
#pragma warning restore CS0618 // Type or member is obsolete
                        InnerExFunc = ex => GetInnerExceptions(ex),
                    }
                },
                {
                    "System.FormatException", new ConvertorFuncs()
                    {
                        ToServiceExFunc = ex => ToServiceException(ex),
                        FromServiceExFunc = (svcEx, innerEx) => FromServiceException<FormatException>(svcEx, innerEx),
                        InnerExFunc = ex => GetInnerExceptions(ex),
                    }
                },
                {
                    "System.IndexOutOfRangeException", new ConvertorFuncs()
                    {
                        ToServiceExFunc = ex => ToServiceException(ex),
                        FromServiceExFunc = (svcEx, innerEx) => FromServiceException<IndexOutOfRangeException>(svcEx, innerEx),
                        InnerExFunc = ex => GetInnerExceptions(ex),
                    }
                },
                {
                    "System.InsufficientExecutionStackException", new ConvertorFuncs()
                    {
                        ToServiceExFunc = ex => ToServiceException(ex),
                        FromServiceExFunc = (svcEx, innerEx) => FromServiceException<InsufficientExecutionStackException>(svcEx, innerEx),
                        InnerExFunc = ex => GetInnerExceptions(ex),
                    }
                },
                {
                    "System.InvalidCastException", new ConvertorFuncs()
                    {
                        ToServiceExFunc = ex => ToServiceException(ex),
                        FromServiceExFunc = (svcEx, innerEx) => FromServiceException<InvalidCastException>(svcEx, innerEx),
                        InnerExFunc = ex => GetInnerExceptions(ex),
                    }
                },
                {
                    "System.InvalidOperationException", new ConvertorFuncs()
                    {
                        ToServiceExFunc = ex => ToServiceException(ex),
                        FromServiceExFunc = (svcEx, innerEx) => FromServiceException<InvalidOperationException>(svcEx, innerEx),
                        InnerExFunc = ex => GetInnerExceptions(ex),
                    }
                },
                {
                    "System.InvalidProgramException", new ConvertorFuncs()
                    {
                        ToServiceExFunc = ex => ToServiceException(ex),
                        FromServiceExFunc = (svcEx, innerEx) => FromServiceException<InvalidProgramException>(svcEx, innerEx),
                        InnerExFunc = ex => GetInnerExceptions(ex),
                    }
                },
                {
                    "System.IO.InternalBufferOverflowException", new ConvertorFuncs()
                    {
                        ToServiceExFunc = ex => ToServiceException(ex),
                        FromServiceExFunc = (svcEx, innerEx) => FromServiceException<InternalBufferOverflowException>(svcEx, innerEx),
                        InnerExFunc = ex => GetInnerExceptions(ex),
                    }
                },
                {
                    "System.IO.InvalidDataException", new ConvertorFuncs()
                    {
                        ToServiceExFunc = ex => ToServiceException(ex),
                        FromServiceExFunc = (svcEx, innerEx) => FromServiceException<InvalidDataException>(svcEx, innerEx),
                        InnerExFunc = ex => GetInnerExceptions(ex),
                    }
                },
                {
                    "System.IO.IOException", new ConvertorFuncs()
                    {
                        ToServiceExFunc = ex => ToServiceException(ex),
                        FromServiceExFunc = (svcEx, innerEx) => FromServiceException<IOException>(svcEx, innerEx),
                        InnerExFunc = ex => GetInnerExceptions(ex),
                    }
                },
                {
                    "System.MemberAccessException", new ConvertorFuncs()
                    {
                        ToServiceExFunc = ex => ToServiceException(ex),
                        FromServiceExFunc = (svcEx, innerEx) => FromServiceException<MemberAccessException>(svcEx, innerEx),
                        InnerExFunc = ex => GetInnerExceptions(ex),
                    }
                },
                {
                    "System.MulticastNotSupportedException", new ConvertorFuncs()
                    {
                        ToServiceExFunc = ex => ToServiceException(ex),
                        FromServiceExFunc = (svcEx, innerEx) => FromServiceException<MulticastNotSupportedException>(svcEx, innerEx),
                        InnerExFunc = ex => GetInnerExceptions(ex),
                    }
                },
                {
                    "System.NotImplementedException", new ConvertorFuncs()
                    {
                        ToServiceExFunc = ex => ToServiceException(ex),
                        FromServiceExFunc = (svcEx, innerEx) => FromServiceException<NotImplementedException>(svcEx, innerEx),
                        InnerExFunc = ex => GetInnerExceptions(ex),
                    }
                },
                {
                    "System.NotSupportedException", new ConvertorFuncs()
                    {
                        ToServiceExFunc = ex => ToServiceException(ex),
                        FromServiceExFunc = (svcEx, innerEx) => FromServiceException<NotSupportedException>(svcEx, innerEx),
                        InnerExFunc = ex => GetInnerExceptions(ex),
                    }
                },
                {
                    "System.NullReferenceException", new ConvertorFuncs()
                    {
                        ToServiceExFunc = ex => ToServiceException(ex),
                        FromServiceExFunc = (svcEx, innerEx) => FromServiceException<NullReferenceException>(svcEx, innerEx),
                        InnerExFunc = ex => GetInnerExceptions(ex),
                    }
                },
                {
                    "System.OperationCanceledException", new ConvertorFuncs()
                    {
                        ToServiceExFunc = ex => ToServiceException(ex),
                        FromServiceExFunc = (svcEx, innerEx) => FromServiceException<OperationCanceledException>(svcEx, innerEx),
                        InnerExFunc = ex => GetInnerExceptions(ex),
                    }
                },
                {
                    "System.OutOfMemoryException", new ConvertorFuncs()
                    {
                        ToServiceExFunc = ex => ToServiceException(ex),
                        FromServiceExFunc = (svcEx, innerEx) => FromServiceException<OutOfMemoryException>(svcEx, innerEx),
                        InnerExFunc = ex => GetInnerExceptions(ex),
                    }
                },
                {
                    "System.RankException", new ConvertorFuncs()
                    {
                        ToServiceExFunc = ex => ToServiceException(ex),
                        FromServiceExFunc = (svcEx, innerEx) => FromServiceException<RankException>(svcEx, innerEx),
                        InnerExFunc = ex => GetInnerExceptions(ex),
                    }
                },
                {
                    "System.Reflection.AmbiguousMatchException", new ConvertorFuncs()
                    {
                        ToServiceExFunc = ex => ToServiceException(ex),
                        FromServiceExFunc = (svcEx, innerEx) => FromServiceException<AmbiguousMatchException>(svcEx, innerEx),
                        InnerExFunc = ex => GetInnerExceptions(ex),
                    }
                },
                {
                    "System.Reflection.ReflectionTypeLoadException", new ConvertorFuncs()
                    {
                        ToServiceExFunc = ex => ToServiceException((ReflectionTypeLoadException)ex),
                        FromServiceExFunc = (svcEx, innerEx) => FromServiceException<ReflectionTypeLoadException>(svcEx, innerEx),
                        InnerExFunc = ex => GetInnerExceptions((ReflectionTypeLoadException)ex),
                    }
                },
                {
                    "System.Resources.MissingManifestResourceException", new ConvertorFuncs()
                    {
                        ToServiceExFunc = ex => ToServiceException(ex),
                        FromServiceExFunc = (svcEx, innerEx) => FromServiceException<MissingManifestResourceException>(svcEx, innerEx),
                        InnerExFunc = ex => GetInnerExceptions(ex),
                    }
                },
                {
                    "System.Resources.MissingSatelliteAssemblyException", new ConvertorFuncs()
                    {
                        ToServiceExFunc = ex => ToServiceException((MissingSatelliteAssemblyException)ex),
                        FromServiceExFunc = (svcEx, innerEx) => FromServiceException<MissingSatelliteAssemblyException>(svcEx, innerEx),
                        InnerExFunc = ex => GetInnerExceptions(ex),
                    }
                },
                {
                    "System.Runtime.InteropServices.ExternalException", new ConvertorFuncs()
                    {
                        ToServiceExFunc = ex => ToServiceException((ExternalException)ex),
                        FromServiceExFunc = (svcEx, innerEx) => FromServiceException<ExternalException>(svcEx, innerEx),
                        InnerExFunc = ex => GetInnerExceptions(ex),
                    }
                },
                {
                    "System.Runtime.InteropServices.InvalidComObjectException", new ConvertorFuncs()
                    {
                        ToServiceExFunc = ex => ToServiceException(ex),
                        FromServiceExFunc = (svcEx, innerEx) => FromServiceException<InvalidComObjectException>(svcEx, innerEx),
                        InnerExFunc = ex => GetInnerExceptions(ex),
                    }
                },
                {
                    "System.Runtime.InteropServices.InvalidOleVariantTypeException", new ConvertorFuncs()
                    {
                        ToServiceExFunc = ex => ToServiceException(ex),
                        FromServiceExFunc = (svcEx, innerEx) => FromServiceException<InvalidOleVariantTypeException>(svcEx, innerEx),
                        InnerExFunc = ex => GetInnerExceptions(ex),
                    }
                },
                {
                    "System.Runtime.InteropServices.MarshalDirectiveException", new ConvertorFuncs()
                    {
                        ToServiceExFunc = ex => ToServiceException(ex),
                        FromServiceExFunc = (svcEx, innerEx) => FromServiceException<MarshalDirectiveException>(svcEx, innerEx),
                        InnerExFunc = ex => GetInnerExceptions(ex),
                    }
                },
                {
                    "System.Runtime.InteropServices.SafeArrayRankMismatchException", new ConvertorFuncs()
                    {
                        ToServiceExFunc = ex => ToServiceException(ex),
                        FromServiceExFunc = (svcEx, innerEx) => FromServiceException<SafeArrayRankMismatchException>(svcEx, innerEx),
                        InnerExFunc = ex => GetInnerExceptions(ex),
                    }
                },
                {
                    "System.Runtime.InteropServices.SafeArrayTypeMismatchException", new ConvertorFuncs()
                    {
                        ToServiceExFunc = ex => ToServiceException(ex),
                        FromServiceExFunc = (svcEx, innerEx) => FromServiceException<SafeArrayTypeMismatchException>(svcEx, innerEx),
                        InnerExFunc = ex => GetInnerExceptions(ex),
                    }
                },
                /*{
                    "System.Runtime.Remoting.RemotingException", new ConvertorFuncs()
                    {
                        ToServiceExFunc = ex => ToServiceException(ex),
                        FromServiceExFunc = (svcEx, innerEx) => FromServiceException<RemotingException>(svcEx, innerEx),
                        InnerExFunc = ex => GetInnerExceptions(ex),
                    }
                },
                {
                    "System.Runtime.Remoting.ServerException", new ConvertorFuncs()
                    {
                        ToServiceExFunc = ex => ToServiceException(ex),
                        FromServiceExFunc = (svcEx, innerEx) => FromServiceException<ServerException>(svcEx, innerEx),
                        InnerExFunc = ex => GetInnerExceptions(ex),
                    }
                },*/
                {
                    "System.Runtime.Serialization.SerializationException", new ConvertorFuncs()
                    {
                        ToServiceExFunc = ex => ToServiceException(ex),
                        FromServiceExFunc = (svcEx, innerEx) => FromServiceException<SerializationException>(svcEx, innerEx),
                        InnerExFunc = ex => GetInnerExceptions(ex),
                    }
                },
                {
                    "System.System.StackOverflowException", new ConvertorFuncs()
                    {
                        ToServiceExFunc = ex => ToServiceException(ex),
                        FromServiceExFunc = (svcEx, innerEx) => FromServiceException<StackOverflowException>(svcEx, innerEx),
                        InnerExFunc = ex => GetInnerExceptions(ex),
                    }
                },
                {
                    "System.Threading.AbandonedMutexException", new ConvertorFuncs()
                    {
                        ToServiceExFunc = ex => ToServiceException((AbandonedMutexException)ex),
                        FromServiceExFunc = (svcEx, innerEx) => FromServiceException<AbandonedMutexException>(svcEx, innerEx),
                        InnerExFunc = ex => GetInnerExceptions(ex),
                    }
                },
                {
                    "System.Threading.SemaphoreFullException", new ConvertorFuncs()
                    {
                        ToServiceExFunc = ex => ToServiceException(ex),
                        FromServiceExFunc = (svcEx, innerEx) => FromServiceException<SemaphoreFullException>(svcEx, innerEx),
                        InnerExFunc = ex => GetInnerExceptions(ex),
                    }
                },
                {
                    "System.Threading.SynchronizationLockException", new ConvertorFuncs()
                    {
                        ToServiceExFunc = ex => ToServiceException(ex),
                        FromServiceExFunc = (svcEx, innerEx) => FromServiceException<SynchronizationLockException>(svcEx, innerEx),
                        InnerExFunc = ex => GetInnerExceptions(ex),
                    }
                },
                {
                    "System.Threading.ThreadAbortException", new ConvertorFuncs()
                    {
                        ToServiceExFunc = ex => ToServiceException((ThreadAbortException)ex),
                        FromServiceExFunc = (svcEx, innerEx) => FromServiceException<ThreadAbortException>(svcEx, innerEx),
                        InnerExFunc = ex => GetInnerExceptions(ex),
                    }
                },
                {
                    "System.Threading.ThreadInterruptedException", new ConvertorFuncs()
                    {
                        ToServiceExFunc = ex => ToServiceException(ex),
                        FromServiceExFunc = (svcEx, innerEx) => FromServiceException<ThreadInterruptedException>(svcEx, innerEx),
                        InnerExFunc = ex => GetInnerExceptions(ex),
                    }
                },
                {
                    "System.Threading.ThreadStartException", new ConvertorFuncs()
                    {
                        ToServiceExFunc = ex => ToServiceException(ex),
                        FromServiceExFunc = (svcEx, innerEx) => FromServiceException<ThreadStartException>(svcEx, innerEx),
                        InnerExFunc = ex => GetInnerExceptions(ex),
                    }
                },
                {
                    "System.Threading.ThreadStateException", new ConvertorFuncs()
                    {
                        ToServiceExFunc = ex => ToServiceException(ex),
                        FromServiceExFunc = (svcEx, innerEx) => FromServiceException<ThreadStateException>(svcEx, innerEx),
                        InnerExFunc = ex => GetInnerExceptions(ex),
                    }
                },
                {
                    "System.TimeoutException", new ConvertorFuncs()
                    {
                        ToServiceExFunc = ex => ToServiceException(ex),
                        FromServiceExFunc = (svcEx, innerEx) => FromServiceException<TimeoutException>(svcEx, innerEx),
                        InnerExFunc = ex => GetInnerExceptions(ex),
                    }
                },
                {
                    "System.TypeInitializationException", new ConvertorFuncs()
                    {
                        ToServiceExFunc = ex => ToServiceException((TypeInitializationException)ex),
                        FromServiceExFunc = (svcEx, innerEx) => FromServiceException<TypeInitializationException>(svcEx, innerEx),
                        InnerExFunc = ex => GetInnerExceptions(ex),
                    }
                },
                {
                    "System.TypeLoadException", new ConvertorFuncs()
                    {
                        ToServiceExFunc = ex => ToServiceException((TypeLoadException)ex),
                        FromServiceExFunc = (svcEx, innerEx) => FromServiceException<TypeLoadException>(svcEx, innerEx),
                        InnerExFunc = ex => GetInnerExceptions(ex),
                    }
                },
                {
                    "System.TypeUnloadedException", new ConvertorFuncs()
                    {
                        ToServiceExFunc = ex => ToServiceException(ex),
                        FromServiceExFunc = (svcEx, innerEx) => FromServiceException<TypeUnloadedException>(svcEx, innerEx),
                        InnerExFunc = ex => GetInnerExceptions(ex),
                    }
                },
                {
                    "System.UnauthorizedAccessException", new ConvertorFuncs()
                    {
                        ToServiceExFunc = ex => ToServiceException(ex),
                        FromServiceExFunc = (svcEx, innerEx) => FromServiceException<UnauthorizedAccessException>(svcEx, innerEx),
                        InnerExFunc = ex => GetInnerExceptions(ex),
                    }
                },
                /*{
                    "System.UriTemplateMatchException", new ConvertorFuncs()
                    {
                        ToServiceExFunc = ex => ToServiceException(ex),
                        FromServiceExFunc = (svcEx, innerEx) => FromServiceException<UriTemplateMatchException>(svcEx, innerEx),
                        InnerExFunc = ex => GetInnerExceptions(ex),
                    }
                },*/
        };

        private static Exception[] GetInnerExceptions(Exception exception)
        {
            return exception.InnerException == null ? null : new Exception[] { exception.InnerException };
        }

        private static Exception[] GetInnerExceptions(ReflectionTypeLoadException exception)
        {
            return exception.LoaderExceptions;
        }

        private static ServiceException ToServiceException(Exception exception)
        {
            var serviceException = new ServiceException(exception.GetType().ToString(), exception.Message);
            serviceException.ActualExceptionStackTrace = exception.StackTrace;
            serviceException.ActualExceptionData = new Dictionary<object, object>()
            {
                { "HResult", exception.HResult },
            };

            return serviceException;
        }

        private static ServiceException ToServiceException(ArgumentException exception)
        {
            var serviceException = ToServiceException((Exception)exception);

            serviceException.ActualExceptionData.Add("ParamName", exception.ParamName);

            return serviceException;
        }

        private static ArgumentException ArgumentExceptionFromServiceEx(ServiceException serviceException, Exception innerException)
        {
            ArgumentException originalEx;
            var args = new List<object>();

            args.Add(serviceException.Message);
            args.Add(serviceException.ActualExceptionData.ContainsKey("ParamName"));
            args.Add(innerException);

            originalEx = (ArgumentException)Activator.CreateInstance(typeof(ArgumentException), args.ToArray());

            return originalEx;
        }

        private static ServiceException ToServiceException(BadImageFormatException exception)
        {
            var serviceException = ToServiceException((Exception)exception);

            serviceException.ActualExceptionData.Add("FileName", exception.FileName);
            serviceException.ActualExceptionData.Add("FusionLog", exception.FusionLog);

            return serviceException;
        }

        private static BadImageFormatException BadImageFormatExceptionFromServiceEx(ServiceException serviceException, Exception innerException)
        {
            BadImageFormatException originalEx;
            var args = new List<object>();

            args.Add(serviceException.Message);
            args.Add(serviceException.ActualExceptionData["FileName"]);
            args.Add(innerException);

            originalEx = (BadImageFormatException)Activator.CreateInstance(typeof(ArgumentException), args.ToArray());

            originalEx.Data.Add("FusionLog", serviceException.ActualExceptionData["FusionLog"]);

            return originalEx;
        }

        private static ServiceException ToServiceException(ReflectionTypeLoadException exception)
        {
            var serviceException = ToServiceException((Exception)exception);

            var types = exception.Types != null
                ? SerializeObject<Type[]>(exception.Types)
                : null;

            serviceException.ActualExceptionData.Add("Types", types);

            return serviceException;
        }

        private static ReflectionTypeLoadException ReflectionTypeLoadExceptionFromServiceEx(ServiceException serviceException, Exception[] innerExceptions)
        {
            ReflectionTypeLoadException originalEx;
            var args = new List<object>();

            args.Add(serviceException.ActualExceptionData["Types"] != null
                ? DeserializeObject<Type[]>((byte[])serviceException.ActualExceptionData["Types"])
                : null);
            args.Add(innerExceptions);
            args.Add(serviceException.Message);

            originalEx = (ReflectionTypeLoadException)Activator.CreateInstance(typeof(ReflectionTypeLoadException), args.ToArray());

            return originalEx;
        }

        private static ServiceException ToServiceException(MissingSatelliteAssemblyException exception)
        {
            var serviceException = ToServiceException((Exception)exception);

            serviceException.ActualExceptionData.Add("CultureName", exception.CultureName);

            return serviceException;
        }

        private static MissingSatelliteAssemblyException MissingSatelliteAssemblyExceptionFromServiceEx(ServiceException serviceException, Exception innerException)
        {
            MissingSatelliteAssemblyException originalEx;
            var args = new List<object>();

            args.Add(serviceException.Message);

            if (innerException != null)
            {
                args.Add(innerException);
            }
            else if (serviceException.ActualExceptionData["CultureName"] != null)
            {
                args.Add(serviceException.ActualExceptionData["CultureName"]);
            }

            originalEx = (MissingSatelliteAssemblyException)Activator.CreateInstance(typeof(MissingSatelliteAssemblyException), args.ToArray());

            return originalEx;
        }

        private static ServiceException ToServiceException(ExternalException exception)
        {
            var serviceException = ToServiceException((Exception)exception);
            serviceException.ActualExceptionData.Add("ErrorCode", exception.ErrorCode.ToString());

            return serviceException;
        }

        private static ExternalException ExternalExceptionFromServiceEx(ServiceException serviceException, Exception innerException)
        {
            ExternalException originalEx;
            var args = new List<object>();

            args.Add(serviceException.Message);

            if (innerException != null)
            {
                args.Add(innerException);
            }
            else if (serviceException.ActualExceptionData["ErrorCode"] != null)
            {
                args.Add(serviceException.ActualExceptionData["ErrorCode"]);
            }

            originalEx = (ExternalException)Activator.CreateInstance(typeof(ExternalException), args.ToArray());

            return originalEx;
        }

        private static ServiceException ToServiceException(AbandonedMutexException exception)
        {
            var serviceException = ToServiceException((Exception)exception);

            if (exception.Mutex != null)
            {
                var serString = SerializeObject(exception.Mutex);
                if (serString != null)
                {
                    serviceException.ActualExceptionData.Add("Mutex", serString);
                }
            }

            serviceException.ActualExceptionData.Add("MutexIndex", exception.MutexIndex.ToString());

            return serviceException;
        }

        private static AbandonedMutexException AbandonedMutexExceptionFromServiceEx(ServiceException serviceException, Exception innerException)
        {
            AbandonedMutexException originalEx;
            var args = new List<object>();

            args.Add(serviceException.Message);
            args.Add(innerException);
            args.Add(serviceException.ActualExceptionData["MutexIndex"]);
            args.Add(serviceException.ActualExceptionData.ContainsKey("Mutex")
                ? DeserializeObject<Mutex>((byte[])serviceException.ActualExceptionData["Mutex"])
                : null);

            originalEx = (AbandonedMutexException)Activator.CreateInstance(typeof(AbandonedMutexException), args.ToArray());

            return originalEx;
        }

        private static ServiceException ToServiceException(ThreadAbortException exception)
        {
            var serviceException = ToServiceException((Exception)exception);

            if (exception.ExceptionState != null)
            {
                var serString = SerializeObject(exception.ExceptionState);
                if (serString != null)
                {
                    serviceException.ActualExceptionData.Add("ExceptionState", serString);
                }
            }

            return serviceException;
        }

        private static ThreadAbortException ThreadAbortExceptionFromServiceEx(ServiceException serviceException, Exception innerException)
        {
            ThreadAbortException originalEx;
            originalEx = (ThreadAbortException)Activator.CreateInstance(typeof(ThreadAbortException));

            if (serviceException.ActualExceptionData.ContainsKey("ExceptionState"))
            {
                originalEx.Data.Add("ExceptionState", DeserializeObject<object>((byte[])serviceException.ActualExceptionData["ExceptionState"]));
            }

            return originalEx;
        }

        private static ServiceException ToServiceException(TypeInitializationException exception)
        {
            var serviceException = ToServiceException((Exception)exception);

            serviceException.ActualExceptionData.Add("TypeName", exception.TypeName);

            return serviceException;
        }

        private static TypeInitializationException TypeInitializationExceptionFromServiceEx(ServiceException serviceException, Exception innerException)
        {
            TypeInitializationException originalEx;
            var args = new List<object>();

            if (innerException != null)
            {
                args.Add(serviceException.ActualExceptionData["TypeName"]);
                args.Add(innerException);
            }
            else
            {
                args.Add(serviceException.Message);
            }

            originalEx = (TypeInitializationException)Activator.CreateInstance(typeof(TypeInitializationException), args.ToArray());

            return originalEx;
        }

        private static ServiceException ToServiceException(TypeLoadException exception)
        {
            var serviceException = ToServiceException((Exception)exception);

            serviceException.ActualExceptionData.Add("TypeName", exception.TypeName);

            return serviceException;
        }

        private static TypeLoadException TypeLoadExceptionFromServiceEx(ServiceException serviceException, Exception innerException)
        {
            TypeLoadException originalEx;
            var args = new List<object>();

            args.Add(serviceException.Message);
            args.Add(innerException);

            originalEx = (TypeLoadException)Activator.CreateInstance(typeof(TypeLoadException), args.ToArray());

            originalEx.Data.Add("TypeName", serviceException.ActualExceptionData["TypeName"]);

            return originalEx;
        }

        private static T FromServiceException<T>(ServiceException serviceException, params Exception[] innerExceptions)
            where T : Exception
        {
            Exception originalEx = null;
            var firstInnerEx = innerExceptions == null || innerExceptions.Length == 0 ? null : innerExceptions[0];

            if (typeof(T) == typeof(ArgumentException))
            {
                originalEx = ArgumentExceptionFromServiceEx(serviceException, firstInnerEx);
            }
            else if (typeof(T) == typeof(BadImageFormatException))
            {
                originalEx = BadImageFormatExceptionFromServiceEx(serviceException, firstInnerEx);
            }
            else if (typeof(T) == typeof(ReflectionTypeLoadException))
            {
                originalEx = ReflectionTypeLoadExceptionFromServiceEx(serviceException, innerExceptions);
            }
            else if (typeof(T) == typeof(MissingSatelliteAssemblyException))
            {
                originalEx = MissingSatelliteAssemblyExceptionFromServiceEx(serviceException, firstInnerEx);
            }
            else if (typeof(T) == typeof(ExternalException))
            {
                originalEx = ExternalExceptionFromServiceEx(serviceException, firstInnerEx);
            }
            else if (typeof(T) == typeof(AbandonedMutexException))
            {
                originalEx = AbandonedMutexExceptionFromServiceEx(serviceException, firstInnerEx);
            }
            else if (typeof(T) == typeof(ThreadAbortException))
            {
                originalEx = ThreadAbortExceptionFromServiceEx(serviceException, firstInnerEx);
            }
            else if (typeof(T) == typeof(TypeInitializationException))
            {
                originalEx = TypeInitializationExceptionFromServiceEx(serviceException, firstInnerEx);
            }
            else if (typeof(T) == typeof(TypeLoadException))
            {
                originalEx = TypeLoadExceptionFromServiceEx(serviceException, firstInnerEx);
            }
            else
            {
                originalEx = (Exception)Activator.CreateInstance(typeof(T), new object[] { serviceException.Message, firstInnerEx });
            }

            // HResult property setter is public only starting netcore 3.0
            originalEx.Data.Add("HResult", serviceException.ActualExceptionData["HResult"]); // Check if Data is initialized

            return (T)originalEx;
        }

        private static byte[] SerializeObject<T>(T obj)
        {
            try
            {
                using (var memStm = new MemoryStream())
                {
                    using (var writer = XmlDictionaryWriter.CreateBinaryWriter(memStm))
                    {
                        var serializer = new DataContractSerializer(typeof(T));
                        serializer.WriteObject(writer, obj);
                        writer.Flush();

                        return memStm.ToArray();
                    }
                }
            }
            catch (Exception)
            {
                // Trace
            }

            return null;
        }

        private static T DeserializeObject<T>(byte[] serObj)
        {
            try
            {
                using (var memStm = new MemoryStream(serObj))
                {
                    using (var reader = XmlDictionaryReader.CreateBinaryReader(memStm, XmlDictionaryReaderQuotas.Max))
                    {
                        var serializer = new DataContractSerializer(typeof(T));
                        return (T)serializer.ReadObject(reader);
                    }
                }
            }
            catch (Exception)
            {
                // Trace
            }

            return default(T);
        }

        internal class ConvertorFuncs
        {
            public Func<Exception, ServiceException> ToServiceExFunc { get; set; }

            public Func<ServiceException, Exception[], Exception> FromServiceExFunc { get; set; }

            public Func<Exception, Exception[]> InnerExFunc { get; set; }
        }
    }
}
