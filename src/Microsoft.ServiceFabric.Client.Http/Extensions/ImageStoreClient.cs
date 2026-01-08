// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Client.Http
{
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
    using Microsoft.ServiceFabric.Client;
    using Microsoft.ServiceFabric.Client.Exceptions;
    using Microsoft.ServiceFabric.Common;
    using Microsoft.ServiceFabric.Common.Utilities;

    /// <summary>
    /// Class containing methods for performing ImageStoreClient operataions.
    /// </summary>
    internal partial class ImageStoreClient : IImageStoreClient
    {
        private const int UploadLimitSizeInBytes = 25 * 1024 * 1024;        
        private const int MaxConcurrentUpload = 10;
        private const int MaxUploadTry = 2;
        private const string ZipExtension = "zip";
        private string imageStorePath;
        private bool isLocalStore = false;

        /// <inheritdoc />
        public Task UploadFileAsync(
            byte[] fileContentsToUpload,
            string pathInImageStore,
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return this.UploadFileAsync(
                fileContentsToUpload,
                pathInImageStore,
                null,
                serverTimeout,
                cancellationToken);
        }

        /// <inheritdoc />
        public Task UploadFileChunkAsync(
            byte[] fileChunkToUpload,
            string pathInImageStore,
            Guid? sessionId,
            long startBytePosition,
            long endBytePosition,
            long length,
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return this.UploadFileChunkAsync(
                fileChunkToUpload,
                pathInImageStore,
                sessionId,
                startBytePosition,
                endBytePosition,
                length,
                null,
                serverTimeout,
                cancellationToken);
        }

        /// <inheritdoc />
        public async Task UploadApplicationPackageAsync(
            string applicationPackagePath,
            bool compressPackage = false,
            string applicationPackagePathInImageStore = default(string),
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            applicationPackagePath.ThrowIfNull(nameof(applicationPackagePath));

            if (!Directory.Exists(applicationPackagePath))
            {
                throw new InvalidOperationException($"Application package path {applicationPackagePath} not found.");
            }

            var absPkgPath = FileUtilities.GetAbsolutePath(applicationPackagePath);

            if (compressPackage)
            {
                await CompressApplicationPackage(absPkgPath);
            }

            var pkgPathInImageStore = applicationPackagePathInImageStore;
            if (string.IsNullOrEmpty(pkgPathInImageStore))
            {
                pkgPathInImageStore = applicationPackagePath.Replace(Path.GetDirectoryName(applicationPackagePath), string.Empty);
            }

            pkgPathInImageStore = pkgPathInImageStore.Trim('\\', '/');

            await this.LoadImageStoreConnectionString();
            if (this.isLocalStore)
            {
                var files = Directory.EnumerateFiles(absPkgPath, "*", SearchOption.AllDirectories)
                    .Select(file => new System.IO.FileInfo(file));

                foreach (var file in files)
                {
                    var targetPath = Path.Combine(this.imageStorePath, Path.Combine(pkgPathInImageStore, file.FullName.Substring(absPkgPath.Length + 1)));
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
                    $"{this.httpClient.ClientId}:{requestId}",
                    "Processing call for ApplicationClient.DeleteApplicationAsync");

                // list of Files to upload.
                var files = Directory.EnumerateFiles(absPkgPath, "*", SearchOption.AllDirectories)
                    .Select(file => new System.IO.FileInfo(file));

                // List of dirs to determine where to upload _.dir fileInfo.
                var dirPathsInImageStore =
                    new List<string> { GetPathInImageStore(absPkgPath, pkgPathInImageStore, absPkgPath) };

                dirPathsInImageStore.AddRange(
                    Directory.EnumerateDirectories(absPkgPath, "*", SearchOption.AllDirectories)
                        .Select(dir => GetPathInImageStore(absPkgPath, pkgPathInImageStore, dir)));

                // Upload small files in single upload. Upload bigger fileInfo using chunked upload.
                // Get info to upload single files.
                var singleFileUploadInfos = files.Where(file => file.Length <= UploadLimitSizeInBytes)
                .Select(file => new FileUploadInfo(
                    file,
                    GetPathInImageStore(absPkgPath, pkgPathInImageStore, file.FullName)));

                // Get info to upload chunks for files.
                var chunkInfos = new List<ChunkInfo>();
                foreach (var file in files.Where(file => file.Length > UploadLimitSizeInBytes))
                {
                    var pathInImageStore = GetPathInImageStore(absPkgPath, pkgPathInImageStore, file.FullName);
                    chunkInfos.AddRange(GetChunksInfoForFile(file, pathInImageStore));
                }

                // upload single files with up to MaxConcurrentUpload in parallel. 
                await this.UploadAllSingleFiles(singleFileUploadInfos, requestId, serverTimeout, cancellationToken);

                // upload chunks with up to MaxConcurrentUpload in parallel. 
                await this.UploadAllChunksAsync(chunkInfos, requestId, serverTimeout, cancellationToken);

                // upload _.dirs with up to MaxConcurrentUpload in parallel. 
                await this.UploadDirectoryCompletionMarkerFiles(dirPathsInImageStore, requestId, serverTimeout, cancellationToken);
            }
        }

        /// <summary>
        /// Uploads contents of the file to the image store.
        /// Used by IApplicationClient.UploadApplicationPackageAsync to trace a request Id associated with original Uplaod call.
        /// </summary>
        /// <param name="fileContentsToUpload">File contents to upload.</param>
        /// <param name ="pathInImageStore">Relative path to file or folder in the image store from its root.</param>
        /// <param name="requestId">Request Id to use for tracing.</param>
        /// <param name ="serverTimeout">The server timeout for performing the operation in seconds. This specifies the time
        /// duration that the client is willing to wait for the requested operation to complete. The default value for this
        /// parameter is 60 seconds.</param>
        /// <param name ="cancellationToken">Cancels the client-side operation.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        internal async Task UploadFileAsync(
            byte[] fileContentsToUpload,
            string pathInImageStore,
            string requestId = null,
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            await this.LoadImageStoreConnectionString();
            if (this.isLocalStore)
            {
                File.WriteAllBytes(Path.Combine(this.imageStorePath, pathInImageStore), fileContentsToUpload);
            }
            else
            {
                pathInImageStore.ThrowIfNull(nameof(fileContentsToUpload));
                pathInImageStore.ThrowIfNull(nameof(pathInImageStore));
                serverTimeout?.ThrowIfOutOfInclusiveRange("serverTimeout", 1, 4294967295);
                var url = "ImageStore/{pathInImageStore}";
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

                await this.httpClient.SendAsync(RequestFunc, url, requestId, cancellationToken);
            }
        }

        /// <summary>
        /// Uploads a file chunk to the image store relative path. Used by IApplicationClient.UploadApplicationPackageAsync to trace a request Id associated with original Uplaod call.
        /// </summary>
        /// <param name="fileChunkToUpload">Byte array containing file chunk to upload.</param>
        /// <param name ="pathInImageStore">Relative path to file or folder in the image store from its root.</param>
        /// <param name ="sessionId">A GUID generated by the user for a file uploading. It identifies an image store upload
        /// session which keeps track of all file chunks until it is committed.</param>
        /// <param name ="startBytePosition">The start position of the chunk in byte array represnting file contents.</param>
        /// <param name ="endBytePosition">The end position of the chunk in byte array representing file contents.</param>
        /// <param name="length">The size, in bytes, of the file for which chunk is to be uploaded.</param>
        /// <param name="requestId">Request Id to use for tracing.</param>
        /// <param name ="serverTimeout">The server timeout for performing the operation in seconds. This specifies the time
        /// duration that the client is willing to wait for the requested operation to complete. The default value for this
        /// parameter is 60 seconds.</param>
        /// <param name ="cancellationToken">Cancels the client-side operation.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        /// <remarks>
        /// When uploading file chunks to the image store,  the Content Range information is sent to server.
        /// Example: if the total file length is 20,000 bytes, and chunk being uploaded is for first 5,000 bytes,
        /// value of <paramref name="startBytePosition"/> would be 0, value of <paramref name="endBytePosition"/> would be 4999 and value of
        /// <paramref name="length"/> would be 20000.
        /// </remarks>
        internal Task UploadFileChunkAsync(
            byte[] fileChunkToUpload,
            string pathInImageStore,
            Guid? sessionId,
            long startBytePosition,
            long endBytePosition,
            long length,
            string requestId = null,
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            fileChunkToUpload.ThrowIfNull(nameof(fileChunkToUpload));
            pathInImageStore.ThrowIfNull(nameof(pathInImageStore));
            sessionId.ThrowIfNull(nameof(sessionId));
            startBytePosition.ThrowIfNull(nameof(startBytePosition));
            endBytePosition.ThrowIfNull(nameof(endBytePosition));
            length.ThrowIfNull(nameof(length));

            serverTimeout?.ThrowIfOutOfInclusiveRange("serverTimeout", 1, 4294967295);
            requestId = requestId ?? Guid.NewGuid().ToString();
            var url = "ImageStore/{pathInImageStore}/$/UploadChunk";
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
                request.Content.Headers.ContentRange =
                    new ContentRangeHeaderValue(startBytePosition, endBytePosition, length);
                return request;
            }

            return this.httpClient.SendAsync(RequestFunc, url, requestId, cancellationToken);
        }

        private static IEnumerable<ChunkInfo> GetChunksInfoForFile(
            System.IO.FileInfo fileInfo,
            string filePathInImageStore)
        {
            var chunkInfos = new List<ChunkInfo>();
            var uploadSessionId = Guid.NewGuid();
            var fileUploadInfo = new FileUploadInfo(fileInfo, filePathInImageStore);

            var fileSize = fileInfo.Length;
            var chunks = fileSize / UploadLimitSizeInBytes;
            if (fileSize % UploadLimitSizeInBytes > 0)
            {
                chunks++;
            }

            long startPosition = 0;
            for (var chunk = 1; chunk <= chunks; chunk++)
            {
                long endPosition = (UploadLimitSizeInBytes * chunk) - 1;

                if (endPosition >= fileSize)
                {
                    endPosition = fileSize - 1;
                }

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

        /// <summary>
        /// Compresses Application Package. Code, Config and Data Pkg folders are compressed in each service package.
        /// </summary>
        /// <param name="appPkgPath">Absolute path to application package.</param>
        private static Task CompressApplicationPackage(string appPkgPath)
        {
            var dirsToCompress = new List<string>();

            // Get the service packages in application package
            foreach (var servicePackage in new DirectoryInfo(appPkgPath).GetDirectories())
            {
                // Get Code/Config/Data packages for each service package.
                dirsToCompress.AddRange(servicePackage.GetDirectories().Select(package => package.FullName));
            }

            return Task.WhenAll(dirsToCompress.Select(dir => Task.Run(() => CompressDirectory(dir, $"{dir}.{ZipExtension}", true))).ToArray());
        }

        private static void CompressDirectory(
            string sourceDirToCompress,
            string destCompressedFile,
            bool deleteSourceDirAfterCompression)
        {
            if (File.Exists(destCompressedFile))
            {
                File.Delete(destCompressedFile);
            }

            ZipFile.CreateFromDirectory(sourceDirToCompress, destCompressedFile);

            if (deleteSourceDirAfterCompression)
            {
                Directory.Delete(sourceDirToCompress, true);
            }
        }

        /// <summary>
        /// Gets the path to upload to in image store.
        /// </summary>
        /// <param name="compressedPkgPath">Path to local compressed package</param>
        /// <param name="pkgPathInImageStore">Package Path in image store.</param>
        /// <param name="pathInCompressedPackage">Path to fileInfo/dir in compressed package.</param>
        /// <returns>Relative PAth in image store.</returns>
        private static string GetPathInImageStore(
            string compressedPkgPath,
            string pkgPathInImageStore,
            string pathInCompressedPackage)
        {
            var relativePath = pathInCompressedPackage.Replace(compressedPkgPath, string.Empty).Trim('\\', '/');

            return relativePath.Equals(string.Empty)
                ? pkgPathInImageStore
                : $"{pkgPathInImageStore}{Path.DirectorySeparatorChar}{relativePath}";
        }

        private async Task UploadAllChunksAsync(
            ICollection<ChunkInfo> chunkInfos,
            Guid requestId,
            long? serverTimeout,
            CancellationToken cancellationToken)
        {
            if (chunkInfos.Count > 0)
            {
                var chunkInfosBag = new ConcurrentBag<ChunkInfo>(chunkInfos);
                var concurrentOperationsRunner = new ConcurrentOperationsRunner<ChunkInfo>(
                    chunkInfo =>
                        this.UploadChunkAsync(
                            chunkInfo,
                            requestId.ToString(),
                            serverTimeout,
                            cancellationToken),
                    chunkInfosBag.TryTake,
                    MaxConcurrentUpload);

                await concurrentOperationsRunner.RunAll();

                var sessiodIds = new ConcurrentBag<Guid>();
                foreach (var sessionId in chunkInfos.Select(x => x.SessionId).Distinct())
                {
                    sessiodIds.Add(sessionId);
                }

                // TODO: Before commiting check for missing chunlks and upload them again.

                // commit all chunkuploads with up to MaxConcurrentUpload in parallel. 
                var sessionIdCommits = new ConcurrentOperationsRunner<Guid>(
                    sessionId =>
                        this.httpClient.ImageStore.CommitImageStoreUploadSessionAsync(
                            sessionId,
                            serverTimeout: serverTimeout,
                            cancellationToken: cancellationToken),
                    sessiodIds.TryTake,
                    MaxConcurrentUpload);

                await sessionIdCommits.RunAll();
            }
        }

        private async Task UploadChunkAsync(
            ChunkInfo chunkInfo,
            string parentRequestId,
            long? serverTimeout,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            // Get the chunk
            var length = chunkInfo.EndPosition - chunkInfo.StartPosition + 1;
            var chunk = new byte[length];

            using (var streamSource = new FileStream(
                chunkInfo.FileUploadInfo.FileInfo.FullName,
                FileMode.Open,
                FileAccess.Read,
                FileShare.Read))
            {
                streamSource.Seek(chunkInfo.StartPosition, SeekOrigin.Begin);
                await streamSource.ReadAsync(chunk, 0, (int)length, cancellationToken);
            }

            // retry once on network, timeout issues
            var tryCount = 1;

            try
            {
                await this.UploadFileChunkAsync(
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
                {
                    throw;
                }

                tryCount++;
            }
        }

        private Task UploadAllSingleFiles(
            IEnumerable<FileUploadInfo> singleFileUploadInfos,
            Guid requestId,
            long? serverTimeout,
            CancellationToken cancellationToken)
        {
            var singleFileUploadInfosBag = new ConcurrentBag<FileUploadInfo>(singleFileUploadInfos);
            var concurrentOperationsRunner = new ConcurrentOperationsRunner<FileUploadInfo>(
                uploadInfo =>
                    this.UploadSingleFile(
                        File.ReadAllBytes(uploadInfo.FileInfo.FullName),
                        uploadInfo.FilePathInImageStore,
                        requestId.ToString(),
                        serverTimeout,
                        cancellationToken),
                singleFileUploadInfosBag.TryTake,
                MaxConcurrentUpload);

            return concurrentOperationsRunner.RunAll();
        }

        private Task UploadDirectoryCompletionMarkerFiles(
            IEnumerable<string> dirPathsInImageStore,
            Guid requestId,
            long? serverTimeout,
            CancellationToken cancellationToken)
        {
            var dirPaths = new ConcurrentBag<string>(dirPathsInImageStore);
            var concurrentOperationsRunner = new ConcurrentOperationsRunner<string>(
                dirPathInImageStore =>
                    this.UploadSingleFile(
                        new byte[0],
                        $"{dirPathInImageStore}{Path.DirectorySeparatorChar}_.dir",
                        requestId.ToString(),
                        serverTimeout,
                        cancellationToken),
                dirPaths.TryTake,
                MaxConcurrentUpload);

            return concurrentOperationsRunner.RunAll();
        }

        private async Task UploadSingleFile(
            byte[] fileContent,
            string filePathInImageStore,
            string parentRequestId,
            long? serverTimeout,
            CancellationToken cancellationToken)
        {
            // retry once on network, timeout issues
            var tryCount = 1;

            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                await this.UploadFileAsync(
                    fileContent,
                    filePathInImageStore,
                    $"{parentRequestId}:{Guid.NewGuid()}",
                    serverTimeout,
                    cancellationToken);
            }
            catch (ServiceFabricRequestException)
            {
                if (tryCount >= MaxUploadTry)
                {
                    throw;
                }

                tryCount++;
            }
        }

        private async Task LoadImageStoreConnectionString()
        {
            if (this.imageStorePath == null)
            {
                this.imageStorePath = await this.httpClient.Cluster.GetImageStoreConnectionStringAsync();
                if (this.imageStorePath != "fabric:ImageStore" && !this.imageStorePath.StartsWith("xstore"))
                {
                    this.imageStorePath = new Uri(this.imageStorePath).LocalPath;
                    this.isLocalStore = true;
                }
            }
        }

        /// <summary>
        /// Contains Information about a chunk to be uploaded.
        /// </summary>
        private class ChunkInfo
        {
            internal long StartPosition { get; set; }

            internal long EndPosition { get; set; }

            internal Guid SessionId { get; set; }

            internal FileUploadInfo FileUploadInfo { get; set; }
        }

        /// <summary>
        /// Contians Information about File to be uploaded.
        /// </summary>
        private class FileUploadInfo
        {
            internal FileUploadInfo(System.IO.FileInfo fileInfo, string filePathInImageStore)
            {
                this.FileInfo = fileInfo;
                this.FilePathInImageStore = filePathInImageStore;
            }

            internal System.IO.FileInfo FileInfo { get; }

            internal string FilePathInImageStore { get; }
        }
    }
}
