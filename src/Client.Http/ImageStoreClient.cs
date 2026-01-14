// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Client.Exceptions;
using Microsoft.ServiceFabric.Common;
using Microsoft.ServiceFabric.Common.Utilities;

namespace Microsoft.ServiceFabric.Client.Http
{
    sealed partial class ImageStoreClient : IImageStoreClient
    {
        const int UploadLimitSizeInBytes = 25 * 1024 * 1024;
        const int MaxConcurrentUpload = 10;
        const int MaxUploadTry = 2;
        const string ZipExtension = "zip";

        string imageStorePath;
        bool isLocalStore = false;

        Task IImageStoreClient.UploadFileAsync(byte[] fileContentsToUpload, string pathInImageStore, long? serverTimeout, CancellationToken cancellationToken) =>
            UploadFileAsync(fileContentsToUpload, pathInImageStore, null, serverTimeout, cancellationToken);

        Task IImageStoreClient.UploadFileChunkAsync(byte[] fileChunkToUpload, string pathInImageStore, Guid? sessionId, long startBytePosition, long endBytePosition, long length, long? serverTimeout, CancellationToken cancellationToken) =>
            UploadFileChunkAsync(fileChunkToUpload, pathInImageStore, sessionId, startBytePosition, endBytePosition, length, null, serverTimeout, cancellationToken);

        async Task IImageStoreClient.UploadApplicationPackageAsync(string applicationPackagePath, bool compressPackage, string applicationPackagePathInImageStore, long? serverTimeout, CancellationToken cancellationToken)
        {
            applicationPackagePath.ThrowIfNull(nameof(applicationPackagePath));

            if (!Directory.Exists(applicationPackagePath))
                throw new InvalidOperationException($"Application package path {applicationPackagePath} not found.");

            string absPkgPath = FileUtilities.GetAbsolutePath(applicationPackagePath);

            if (compressPackage)
                await CompressApplicationPackage(absPkgPath);

            string pkgPathInImageStore = applicationPackagePathInImageStore;
            if (string.IsNullOrEmpty(pkgPathInImageStore))
                pkgPathInImageStore = applicationPackagePath.Replace(Path.GetDirectoryName(applicationPackagePath), string.Empty);

            pkgPathInImageStore = pkgPathInImageStore.Trim('\\', '/');

            await LoadImageStoreConnectionString();
            if (isLocalStore)
            {
                IEnumerable<System.IO.FileInfo> files = Directory.EnumerateFiles(absPkgPath, "*", SearchOption.AllDirectories)
                    .Select(file => new System.IO.FileInfo(file));

                foreach (System.IO.FileInfo file in files)
                {
                    string targetPath = Path.Combine(imageStorePath, Path.Combine(pkgPathInImageStore, file.FullName.Substring(absPkgPath.Length + 1)));
                    if (!Directory.Exists(Path.GetDirectoryName(targetPath)))
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(targetPath));
                    }

                    File.Copy(file.FullName, targetPath, true);
                }
            }
            else
            {
                var requestId = Guid.NewGuid();
                ServiceFabricHttpClientEventSource.Current.InfoMessage(
                    $"{httpClient.ClientId}:{requestId}",
                    "Processing call for ApplicationClient.DeleteApplicationAsync");

                // list of Files to upload.
                IEnumerable<System.IO.FileInfo> files = Directory.EnumerateFiles(absPkgPath, "*", SearchOption.AllDirectories)
                    .Select(file => new System.IO.FileInfo(file));

                // List of dirs to determine where to upload _.dir fileInfo.
                var dirPathsInImageStore = new List<string> { GetPathInImageStore(absPkgPath, pkgPathInImageStore, absPkgPath) };

                dirPathsInImageStore.AddRange(
                    Directory.EnumerateDirectories(absPkgPath, "*", SearchOption.AllDirectories)
                        .Select(dir => GetPathInImageStore(absPkgPath, pkgPathInImageStore, dir)));

                // Upload small files in single upload. Upload bigger fileInfo using chunked upload.
                // Get info to upload single files.
                IEnumerable<FileUploadInfo> singleFileUploadInfos = files.Where(file => file.Length <= UploadLimitSizeInBytes)
                    .Select(file => new FileUploadInfo(
                        file,
                        GetPathInImageStore(absPkgPath, pkgPathInImageStore, file.FullName)));

                // Get info to upload chunks for files.
                var chunkInfos = new List<ChunkInfo>();
                foreach (var file in files.Where(file => file.Length > UploadLimitSizeInBytes))
                {
                    string pathInImageStore = GetPathInImageStore(absPkgPath, pkgPathInImageStore, file.FullName);
                    chunkInfos.AddRange(GetChunksInfoForFile(file, pathInImageStore));
                }

                // upload single files with up to MaxConcurrentUpload in parallel. 
                await UploadAllSingleFiles(singleFileUploadInfos, requestId, serverTimeout, cancellationToken);

                // upload chunks with up to MaxConcurrentUpload in parallel. 
                await UploadAllChunksAsync(chunkInfos, requestId, serverTimeout, cancellationToken);

                // upload _.dirs with up to MaxConcurrentUpload in parallel. 
                await UploadDirectoryCompletionMarkerFiles(dirPathsInImageStore, requestId, serverTimeout, cancellationToken);
            }
        }

        async Task UploadFileAsync(byte[] fileContentsToUpload, string pathInImageStore, string requestId, long? serverTimeout, CancellationToken cancellationToken)
        {
            await LoadImageStoreConnectionString();
            if (isLocalStore)
            {
                File.WriteAllBytes(Path.Combine(imageStorePath, pathInImageStore), fileContentsToUpload);
            }
            else
            {
                pathInImageStore.ThrowIfNull(nameof(fileContentsToUpload));
                pathInImageStore.ThrowIfNull(nameof(pathInImageStore));
                serverTimeout?.ThrowIfOutOfInclusiveRange("serverTimeout", 1, 4294967295);
                string url = "ImageStore/{pathInImageStore}";
                url = url.Replace("{pathInImageStore}", Uri.EscapeDataString(pathInImageStore.ToString()));
                requestId = requestId ?? Guid.NewGuid().ToString();
                var queryParams = new List<string>();

                // Append to queryParams if not null.
                serverTimeout?.AddToQueryParameters(queryParams, $"timeout={serverTimeout}");
                queryParams.Add("api-version=6.0");
                url += "?" + string.Join("&", queryParams);

                HttpRequestMessage RequestFunc()
                {
                    var request = new HttpRequestMessage()
                    {
                        Method = HttpMethod.Put,
                        Content = new ByteArrayContent(fileContentsToUpload),
                    };

                    request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/octet-stream");
                    return request;
                }

                await httpClient.SendAsync(RequestFunc, url, requestId, cancellationToken);
            }
        }

        Task UploadFileChunkAsync(byte[] fileChunkToUpload, string pathInImageStore, Guid? sessionId, long startBytePosition, long endBytePosition, long length, string requestId, long? serverTimeout, CancellationToken cancellationToken)
        {
            fileChunkToUpload.ThrowIfNull(nameof(fileChunkToUpload));
            pathInImageStore.ThrowIfNull(nameof(pathInImageStore));
            sessionId.ThrowIfNull(nameof(sessionId));
            startBytePosition.ThrowIfNull(nameof(startBytePosition));
            endBytePosition.ThrowIfNull(nameof(endBytePosition));
            length.ThrowIfNull(nameof(length));

            serverTimeout?.ThrowIfOutOfInclusiveRange("serverTimeout", 1, 4294967295);
            requestId = requestId ?? Guid.NewGuid().ToString();
            string url = "ImageStore/{pathInImageStore}/$/UploadChunk";
            url = url.Replace("{pathInImageStore}", Uri.EscapeDataString(pathInImageStore.ToString()));
            var queryParams = new List<string>();

            // Append to queryParams if not null.
            sessionId?.AddToQueryParameters(queryParams, $"session-id={sessionId}");
            serverTimeout?.AddToQueryParameters(queryParams, $"timeout={serverTimeout}");
            queryParams.Add("api-version=6.0");
            url += "?" + string.Join("&", queryParams);

            HttpRequestMessage RequestFunc()
            {
                var request = new HttpRequestMessage()
                {
                    Method = HttpMethod.Put,
                    Content = new ByteArrayContent(fileChunkToUpload),
                };

                request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/octet-stream");
                request.Content.Headers.ContentRange = new ContentRangeHeaderValue(startBytePosition, endBytePosition, length);
                return request;
            }

            return httpClient.SendAsync(RequestFunc, url, requestId, cancellationToken);
        }

        static IEnumerable<ChunkInfo> GetChunksInfoForFile(System.IO.FileInfo fileInfo, string filePathInImageStore)
        {
            var chunkInfos = new List<ChunkInfo>();
            var uploadSessionId = Guid.NewGuid();
            var fileUploadInfo = new FileUploadInfo(fileInfo, filePathInImageStore);

            long fileSize = fileInfo.Length;
            long chunks = fileSize / UploadLimitSizeInBytes;
            if (fileSize % UploadLimitSizeInBytes > 0)
            {
                chunks++;
            }

            long startPosition = 0;
            for (long chunk = 1; chunk <= chunks; chunk++)
            {
                long endPosition = (UploadLimitSizeInBytes * chunk) - 1;

                if (endPosition >= fileSize)
                    endPosition = fileSize - 1;

                chunkInfos.Add(
                    new ChunkInfo
                    {
                        StartPosition = startPosition,
                        EndPosition = endPosition,
                        SessionId = uploadSessionId,
                        FileUploadInfo = fileUploadInfo,
                    });

                startPosition = endPosition + 1;
            }

            return chunkInfos;
        }

        static Task CompressApplicationPackage(string appPkgPath)
        {
            var dirsToCompress = new List<string>();

            // Get the service packages in application package
            foreach (DirectoryInfo servicePackage in new DirectoryInfo(appPkgPath).GetDirectories())
                // Get Code/Config/Data packages for each service package.
                dirsToCompress.AddRange(servicePackage.GetDirectories().Select(package => package.FullName));

            return Task.WhenAll(dirsToCompress.Select(dir => Task.Run(() => CompressDirectory(dir, $"{dir}.{ZipExtension}", true))).ToArray());
        }

        static void CompressDirectory(string sourceDirToCompress, string destCompressedFile, bool deleteSourceDirAfterCompression)
        {
            if (File.Exists(destCompressedFile))
                File.Delete(destCompressedFile);

            ZipFile.CreateFromDirectory(sourceDirToCompress, destCompressedFile);

            if (deleteSourceDirAfterCompression)
                Directory.Delete(sourceDirToCompress, true);
        }

        static string GetPathInImageStore(string compressedPkgPath, string pkgPathInImageStore, string pathInCompressedPackage)
        {
            string relativePath = pathInCompressedPackage.Replace(compressedPkgPath, string.Empty).Trim('\\', '/');

            return relativePath.Equals(string.Empty)
                ? pkgPathInImageStore
                : $"{pkgPathInImageStore}{Path.DirectorySeparatorChar}{relativePath}";
        }

        async Task UploadAllChunksAsync(ICollection<ChunkInfo> chunkInfos, Guid requestId, long? serverTimeout, CancellationToken cancellationToken)
        {
            if (chunkInfos.Count > 0)
            {
                var chunkInfosBag = new ConcurrentBag<ChunkInfo>(chunkInfos);
                var concurrentOperationsRunner = new ConcurrentOperationsRunner<ChunkInfo>(
                    chunkInfo =>
                        UploadChunkAsync(
                            chunkInfo,
                            requestId.ToString(),
                            serverTimeout,
                            cancellationToken),
                    chunkInfosBag.TryTake,
                    MaxConcurrentUpload);

                await concurrentOperationsRunner.RunAll();

                var sessiodIds = new ConcurrentBag<Guid>();
                foreach (var sessionId in chunkInfos.Select(x => x.SessionId).Distinct())
                    sessiodIds.Add(sessionId);

                // TODO: Before commiting check for missing chunlks and upload them again.

                // commit all chunkuploads with up to MaxConcurrentUpload in parallel. 
                var sessionIdCommits = new ConcurrentOperationsRunner<Guid>(
                    sessionId =>
                        httpClient.ImageStore.CommitImageStoreUploadSessionAsync(
                            sessionId,
                            serverTimeout: serverTimeout,
                            cancellationToken: cancellationToken),
                    sessiodIds.TryTake,
                    MaxConcurrentUpload);

                await sessionIdCommits.RunAll();
            }
        }

        async Task UploadChunkAsync(ChunkInfo chunkInfo, string parentRequestId, long? serverTimeout, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            // Get the chunk
            long length = chunkInfo.EndPosition - chunkInfo.StartPosition + 1;
            var chunk = new byte[length];

            using (var streamSource = new FileStream(chunkInfo.FileUploadInfo.FileInfo.FullName, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                streamSource.Seek(chunkInfo.StartPosition, SeekOrigin.Begin);
                await streamSource.ReadAsync(chunk, 0, (int)length, cancellationToken);
            }

            // retry once on network, timeout issues
            int tryCount = 1;

            try
            {
                await UploadFileChunkAsync(
                    chunk,
                    chunkInfo.FileUploadInfo.FilePathInImageStore,
                    chunkInfo.SessionId,
                    chunkInfo.StartPosition,
                    chunkInfo.EndPosition,
                    chunkInfo.FileUploadInfo.FileInfo.Length,
                    $"{parentRequestId}:{Guid.NewGuid()}",
                    serverTimeout,
                    cancellationToken);
            }
            catch (ServiceFabricRequestException)
            {
                if (tryCount >= MaxUploadTry)
                    throw;
                tryCount++;
            }
        }

        Task UploadAllSingleFiles(IEnumerable<FileUploadInfo> singleFileUploadInfos, Guid requestId, long? serverTimeout, CancellationToken cancellationToken)
        {
            var singleFileUploadInfosBag = new ConcurrentBag<FileUploadInfo>(singleFileUploadInfos);
            var concurrentOperationsRunner = new ConcurrentOperationsRunner<FileUploadInfo>(
                uploadInfo =>
                    UploadSingleFile(
                        File.ReadAllBytes(uploadInfo.FileInfo.FullName),
                        uploadInfo.FilePathInImageStore,
                        requestId.ToString(),
                        serverTimeout,
                        cancellationToken),
                singleFileUploadInfosBag.TryTake,
                MaxConcurrentUpload);

            return concurrentOperationsRunner.RunAll();
        }

        Task UploadDirectoryCompletionMarkerFiles(IEnumerable<string> dirPathsInImageStore, Guid requestId, long? serverTimeout, CancellationToken cancellationToken)
        {
            var dirPaths = new ConcurrentBag<string>(dirPathsInImageStore);
            var concurrentOperationsRunner = new ConcurrentOperationsRunner<string>(
                dirPathInImageStore =>
                    UploadSingleFile(
                        new byte[0],
                        $"{dirPathInImageStore}{Path.DirectorySeparatorChar}_.dir",
                        requestId.ToString(),
                        serverTimeout,
                        cancellationToken),
                dirPaths.TryTake,
                MaxConcurrentUpload);

            return concurrentOperationsRunner.RunAll();
        }

        async Task UploadSingleFile(byte[] fileContent, string filePathInImageStore, string parentRequestId, long? serverTimeout, CancellationToken cancellationToken)
        {
            // retry once on network, timeout issues
            var tryCount = 1;

            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                await UploadFileAsync(
                    fileContent,
                    filePathInImageStore,
                    $"{parentRequestId}:{Guid.NewGuid()}",
                    serverTimeout,
                    cancellationToken);
            }
            catch (ServiceFabricRequestException)
            {
                if (tryCount >= MaxUploadTry)
                    throw;
                tryCount++;
            }
        }

        async Task LoadImageStoreConnectionString()
        {
            if (imageStorePath == null)
            {
                imageStorePath = await httpClient.Cluster.GetImageStoreConnectionStringAsync();
                if (imageStorePath != "fabric:ImageStore" && !imageStorePath.StartsWith("xstore"))
                {
                    imageStorePath = new Uri(imageStorePath).LocalPath;
                    isLocalStore = true;
                }
            }
        }

        class ChunkInfo
        {
            internal long StartPosition { get; set; }
            internal long EndPosition { get; set; }
            internal Guid SessionId { get; set; }
            internal FileUploadInfo FileUploadInfo { get; set; }
        }

        class FileUploadInfo
        {
            internal FileUploadInfo(System.IO.FileInfo fileInfo, string filePathInImageStore)
            {
                FileInfo = fileInfo;
                FilePathInImageStore = filePathInImageStore;
            }

            internal System.IO.FileInfo FileInfo { get; }
            internal string FilePathInImageStore { get; }
        }
    }
}
