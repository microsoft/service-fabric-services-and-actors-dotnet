// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabric.Client.Http
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net.Http;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.ServiceFabric.Client;
    using Microsoft.ServiceFabric.Client.Http.Serialization;
    using Microsoft.ServiceFabric.Common;
    using Newtonsoft.Json;

    /// <summary>
    /// Class containing methods for performing BackupRestoreClient operations.
    /// </summary>
    internal partial class BackupRestoreClient : IBackupRestoreClient
    {
        private readonly ServiceFabricHttpClient httpClient;

        /// <summary>
        /// Initializes a new instance of the BackupRestoreClient class.
        /// </summary>
        /// <param name="httpClient">ServiceFabricHttpClient instance.</param>
        public BackupRestoreClient(ServiceFabricHttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        /// <inheritdoc />
        public Task CreateBackupPolicyAsync(
            BackupPolicyDescription backupPolicyDescription,
            long? serverTimeout = 60,
            bool? validateConnection = false,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            backupPolicyDescription.ThrowIfNull(nameof(backupPolicyDescription));
            serverTimeout?.ThrowIfOutOfInclusiveRange("serverTimeout", 1, 4294967295);
            var requestId = Guid.NewGuid().ToString();
            var url = "BackupRestore/BackupPolicies/$/Create";
            var queryParams = new List<string>();
            
            // Append to queryParams if not null.
            serverTimeout?.AddToQueryParameters(queryParams, $"timeout={serverTimeout}");
            validateConnection?.AddToQueryParameters(queryParams, $"ValidateConnection={validateConnection}");
            queryParams.Add("api-version=6.4");
            url += "?" + string.Join("&", queryParams);
            
            string content;
            using (var sw = new StringWriter())
            {
                BackupPolicyDescriptionConverter.Serialize(new JsonTextWriter(sw), backupPolicyDescription);
                content = sw.ToString();
            }

            HttpRequestMessage RequestFunc()
            {
                var request = new HttpRequestMessage()
                {
                    Method = HttpMethod.Post,
                    Content = new StringContent(content, Encoding.UTF8),
                };
                request.Content.Headers.ContentType = System.Net.Http.Headers.MediaTypeHeaderValue.Parse("application/json; charset=utf-8");
                return request;
            }

            return this.httpClient.SendAsync(RequestFunc, url, requestId, cancellationToken);
        }

        /// <inheritdoc />
        public Task DeleteBackupPolicyAsync(
            string backupPolicyName,
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            backupPolicyName.ThrowIfNull(nameof(backupPolicyName));
            serverTimeout?.ThrowIfOutOfInclusiveRange("serverTimeout", 1, 4294967295);
            var requestId = Guid.NewGuid().ToString();
            var url = "BackupRestore/BackupPolicies/{backupPolicyName}/$/Delete";
            url = url.Replace("{backupPolicyName}", Uri.EscapeDataString(backupPolicyName));
            var queryParams = new List<string>();
            
            // Append to queryParams if not null.
            serverTimeout?.AddToQueryParameters(queryParams, $"timeout={serverTimeout}");
            queryParams.Add("api-version=6.4");
            url += "?" + string.Join("&", queryParams);
            
            HttpRequestMessage RequestFunc()
            {
                var request = new HttpRequestMessage()
                {
                    Method = HttpMethod.Post,
                };
                return request;
            }

            return this.httpClient.SendAsync(RequestFunc, url, requestId, cancellationToken);
        }

        /// <inheritdoc />
        public Task<PagedData<BackupPolicyDescription>> GetBackupPolicyListAsync(
            ContinuationToken continuationToken = default(ContinuationToken),
            long? maxResults = 0,
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            maxResults?.ThrowIfLessThan("maxResults", 0);
            serverTimeout?.ThrowIfOutOfInclusiveRange("serverTimeout", 1, 4294967295);
            var requestId = Guid.NewGuid().ToString();
            var url = "BackupRestore/BackupPolicies";
            var queryParams = new List<string>();
            
            // Append to queryParams if not null.
            continuationToken?.AddToQueryParameters(queryParams, $"ContinuationToken={continuationToken.ToString()}");
            maxResults?.AddToQueryParameters(queryParams, $"MaxResults={maxResults}");
            serverTimeout?.AddToQueryParameters(queryParams, $"timeout={serverTimeout}");
            queryParams.Add("api-version=6.4");
            url += "?" + string.Join("&", queryParams);
            
            HttpRequestMessage RequestFunc()
            {
                var request = new HttpRequestMessage()
                {
                    Method = HttpMethod.Get,
                };
                return request;
            }

            return this.httpClient.SendAsyncGetResponseAsPagedData(RequestFunc, url, BackupPolicyDescriptionConverter.Deserialize, requestId, cancellationToken);
        }

        /// <inheritdoc />
        public Task<BackupPolicyDescription> GetBackupPolicyByNameAsync(
            string backupPolicyName,
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            backupPolicyName.ThrowIfNull(nameof(backupPolicyName));
            serverTimeout?.ThrowIfOutOfInclusiveRange("serverTimeout", 1, 4294967295);
            var requestId = Guid.NewGuid().ToString();
            var url = "BackupRestore/BackupPolicies/{backupPolicyName}";
            url = url.Replace("{backupPolicyName}", Uri.EscapeDataString(backupPolicyName));
            var queryParams = new List<string>();
            
            // Append to queryParams if not null.
            serverTimeout?.AddToQueryParameters(queryParams, $"timeout={serverTimeout}");
            queryParams.Add("api-version=6.4");
            url += "?" + string.Join("&", queryParams);
            
            HttpRequestMessage RequestFunc()
            {
                var request = new HttpRequestMessage()
                {
                    Method = HttpMethod.Get,
                };
                return request;
            }

            return this.httpClient.SendAsyncGetResponse(RequestFunc, url, BackupPolicyDescriptionConverter.Deserialize, requestId, cancellationToken);
        }

        /// <inheritdoc />
        public Task<PagedData<BackupEntity>> GetAllEntitiesBackedUpByPolicyAsync(
            string backupPolicyName,
            ContinuationToken continuationToken = default(ContinuationToken),
            long? maxResults = 0,
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            backupPolicyName.ThrowIfNull(nameof(backupPolicyName));
            maxResults?.ThrowIfLessThan("maxResults", 0);
            serverTimeout?.ThrowIfOutOfInclusiveRange("serverTimeout", 1, 4294967295);
            var requestId = Guid.NewGuid().ToString();
            var url = "BackupRestore/BackupPolicies/{backupPolicyName}/$/GetBackupEnabledEntities";
            url = url.Replace("{backupPolicyName}", Uri.EscapeDataString(backupPolicyName));
            var queryParams = new List<string>();
            
            // Append to queryParams if not null.
            continuationToken?.AddToQueryParameters(queryParams, $"ContinuationToken={continuationToken.ToString()}");
            maxResults?.AddToQueryParameters(queryParams, $"MaxResults={maxResults}");
            serverTimeout?.AddToQueryParameters(queryParams, $"timeout={serverTimeout}");
            queryParams.Add("api-version=6.4");
            url += "?" + string.Join("&", queryParams);
            
            HttpRequestMessage RequestFunc()
            {
                var request = new HttpRequestMessage()
                {
                    Method = HttpMethod.Get,
                };
                return request;
            }

            return this.httpClient.SendAsyncGetResponseAsPagedData(RequestFunc, url, BackupEntityConverter.Deserialize, requestId, cancellationToken);
        }

        /// <inheritdoc />
        public Task UpdateBackupPolicyAsync(
            BackupPolicyDescription backupPolicyDescription,
            string backupPolicyName,
            long? serverTimeout = 60,
            bool? validateConnection = false,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            backupPolicyDescription.ThrowIfNull(nameof(backupPolicyDescription));
            backupPolicyName.ThrowIfNull(nameof(backupPolicyName));
            serverTimeout?.ThrowIfOutOfInclusiveRange("serverTimeout", 1, 4294967295);
            var requestId = Guid.NewGuid().ToString();
            var url = "BackupRestore/BackupPolicies/{backupPolicyName}/$/Update";
            url = url.Replace("{backupPolicyName}", Uri.EscapeDataString(backupPolicyName));
            var queryParams = new List<string>();
            
            // Append to queryParams if not null.
            serverTimeout?.AddToQueryParameters(queryParams, $"timeout={serverTimeout}");
            validateConnection?.AddToQueryParameters(queryParams, $"ValidateConnection={validateConnection}");
            queryParams.Add("api-version=6.4");
            url += "?" + string.Join("&", queryParams);
            
            string content;
            using (var sw = new StringWriter())
            {
                BackupPolicyDescriptionConverter.Serialize(new JsonTextWriter(sw), backupPolicyDescription);
                content = sw.ToString();
            }

            HttpRequestMessage RequestFunc()
            {
                var request = new HttpRequestMessage()
                {
                    Method = HttpMethod.Post,
                    Content = new StringContent(content, Encoding.UTF8),
                };
                request.Content.Headers.ContentType = System.Net.Http.Headers.MediaTypeHeaderValue.Parse("application/json; charset=utf-8");
                return request;
            }

            return this.httpClient.SendAsync(RequestFunc, url, requestId, cancellationToken);
        }

        /// <inheritdoc />
        public Task EnableApplicationBackupAsync(
            string applicationId,
            EnableBackupDescription enableBackupDescription,
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            applicationId.ThrowIfNull(nameof(applicationId));
            enableBackupDescription.ThrowIfNull(nameof(enableBackupDescription));
            serverTimeout?.ThrowIfOutOfInclusiveRange("serverTimeout", 1, 4294967295);
            var requestId = Guid.NewGuid().ToString();
            var url = "Applications/{applicationId}/$/EnableBackup";
            url = url.Replace("{applicationId}", applicationId);
            var queryParams = new List<string>();
            
            // Append to queryParams if not null.
            serverTimeout?.AddToQueryParameters(queryParams, $"timeout={serverTimeout}");
            queryParams.Add("api-version=6.4");
            url += "?" + string.Join("&", queryParams);
            
            string content;
            using (var sw = new StringWriter())
            {
                EnableBackupDescriptionConverter.Serialize(new JsonTextWriter(sw), enableBackupDescription);
                content = sw.ToString();
            }

            HttpRequestMessage RequestFunc()
            {
                var request = new HttpRequestMessage()
                {
                    Method = HttpMethod.Post,
                    Content = new StringContent(content, Encoding.UTF8),
                };
                request.Content.Headers.ContentType = System.Net.Http.Headers.MediaTypeHeaderValue.Parse("application/json; charset=utf-8");
                return request;
            }

            return this.httpClient.SendAsync(RequestFunc, url, requestId, cancellationToken);
        }

        /// <inheritdoc />
        public Task DisableApplicationBackupAsync(
            string applicationId,
            long? serverTimeout = 60,
            DisableBackupDescription disableBackupDescription = default(DisableBackupDescription),
            CancellationToken cancellationToken = default(CancellationToken))
        {
            applicationId.ThrowIfNull(nameof(applicationId));
            serverTimeout?.ThrowIfOutOfInclusiveRange("serverTimeout", 1, 4294967295);
            var requestId = Guid.NewGuid().ToString();
            var url = "Applications/{applicationId}/$/DisableBackup";
            url = url.Replace("{applicationId}", applicationId);
            var queryParams = new List<string>();
            
            // Append to queryParams if not null.
            serverTimeout?.AddToQueryParameters(queryParams, $"timeout={serverTimeout}");
            queryParams.Add("api-version=6.4");
            url += "?" + string.Join("&", queryParams);
            
            string content;
            using (var sw = new StringWriter())
            {
                if (disableBackupDescription != default(DisableBackupDescription))
                {
                    DisableBackupDescriptionConverter.Serialize(new JsonTextWriter(sw), disableBackupDescription);
                }

                content = sw.ToString();
            }

            HttpRequestMessage RequestFunc()
            {
                var request = new HttpRequestMessage()
                {
                    Method = HttpMethod.Post,
                    Content = new StringContent(content, Encoding.UTF8),
                };
                request.Content.Headers.ContentType = System.Net.Http.Headers.MediaTypeHeaderValue.Parse("application/json; charset=utf-8");
                return request;
            }

            return this.httpClient.SendAsync(RequestFunc, url, requestId, cancellationToken);
        }

        /// <inheritdoc />
        public Task<PagedData<BackupConfigurationInfo>> GetApplicationBackupConfigurationInfoAsync(
            string applicationId,
            ContinuationToken continuationToken = default(ContinuationToken),
            long? maxResults = 0,
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            applicationId.ThrowIfNull(nameof(applicationId));
            maxResults?.ThrowIfLessThan("maxResults", 0);
            serverTimeout?.ThrowIfOutOfInclusiveRange("serverTimeout", 1, 4294967295);
            var requestId = Guid.NewGuid().ToString();
            var url = "Applications/{applicationId}/$/GetBackupConfigurationInfo";
            url = url.Replace("{applicationId}", applicationId);
            var queryParams = new List<string>();
            
            // Append to queryParams if not null.
            continuationToken?.AddToQueryParameters(queryParams, $"ContinuationToken={continuationToken.ToString()}");
            maxResults?.AddToQueryParameters(queryParams, $"MaxResults={maxResults}");
            serverTimeout?.AddToQueryParameters(queryParams, $"timeout={serverTimeout}");
            queryParams.Add("api-version=6.4");
            url += "?" + string.Join("&", queryParams);
            
            HttpRequestMessage RequestFunc()
            {
                var request = new HttpRequestMessage()
                {
                    Method = HttpMethod.Get,
                };
                return request;
            }

            return this.httpClient.SendAsyncGetResponseAsPagedData(RequestFunc, url, BackupConfigurationInfoConverter.Deserialize, requestId, cancellationToken);
        }

        /// <inheritdoc />
        public Task<PagedData<BackupInfo>> GetApplicationBackupListAsync(
            string applicationId,
            long? serverTimeout = 60,
            bool? latest = false,
            DateTime? startDateTimeFilter = default(DateTime?),
            DateTime? endDateTimeFilter = default(DateTime?),
            ContinuationToken continuationToken = default(ContinuationToken),
            long? maxResults = 0,
            bool? metadataFromBlob = false,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            applicationId.ThrowIfNull(nameof(applicationId));
            serverTimeout?.ThrowIfOutOfInclusiveRange("serverTimeout", 1, 4294967295);
            maxResults?.ThrowIfLessThan("maxResults", 0);
            var requestId = Guid.NewGuid().ToString();
            var url = "Applications/{applicationId}/$/GetBackups";
            url = url.Replace("{applicationId}", applicationId);
            var queryParams = new List<string>();
            
            // Append to queryParams if not null.
            serverTimeout?.AddToQueryParameters(queryParams, $"timeout={serverTimeout}");
            latest?.AddToQueryParameters(queryParams, $"Latest={latest}");
            startDateTimeFilter?.AddToQueryParameters(queryParams, $"StartDateTimeFilter={System.Xml.XmlConvert.ToString(startDateTimeFilter.Value, System.Xml.XmlDateTimeSerializationMode.Utc).ToString()}");
            endDateTimeFilter?.AddToQueryParameters(queryParams, $"EndDateTimeFilter={System.Xml.XmlConvert.ToString(endDateTimeFilter.Value, System.Xml.XmlDateTimeSerializationMode.Utc).ToString()}");
            continuationToken?.AddToQueryParameters(queryParams, $"ContinuationToken={continuationToken.ToString()}");
            maxResults?.AddToQueryParameters(queryParams, $"MaxResults={maxResults}");
            metadataFromBlob?.AddToQueryParameters(queryParams, $"MetadataFromBlob={metadataFromBlob}");
            queryParams.Add("api-version=6.4");
            url += "?" + string.Join("&", queryParams);
            
            HttpRequestMessage RequestFunc()
            {
                var request = new HttpRequestMessage()
                {
                    Method = HttpMethod.Get,
                };
                return request;
            }

            return this.httpClient.SendAsyncGetResponseAsPagedData(RequestFunc, url, BackupInfoConverter.Deserialize, requestId, cancellationToken);
        }

        /// <inheritdoc />
        public Task SuspendApplicationBackupAsync(
            string applicationId,
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            applicationId.ThrowIfNull(nameof(applicationId));
            serverTimeout?.ThrowIfOutOfInclusiveRange("serverTimeout", 1, 4294967295);
            var requestId = Guid.NewGuid().ToString();
            var url = "Applications/{applicationId}/$/SuspendBackup";
            url = url.Replace("{applicationId}", applicationId);
            var queryParams = new List<string>();
            
            // Append to queryParams if not null.
            serverTimeout?.AddToQueryParameters(queryParams, $"timeout={serverTimeout}");
            queryParams.Add("api-version=6.4");
            url += "?" + string.Join("&", queryParams);
            
            HttpRequestMessage RequestFunc()
            {
                var request = new HttpRequestMessage()
                {
                    Method = HttpMethod.Post,
                };
                return request;
            }

            return this.httpClient.SendAsync(RequestFunc, url, requestId, cancellationToken);
        }

        /// <inheritdoc />
        public Task ResumeApplicationBackupAsync(
            string applicationId,
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            applicationId.ThrowIfNull(nameof(applicationId));
            serverTimeout?.ThrowIfOutOfInclusiveRange("serverTimeout", 1, 4294967295);
            var requestId = Guid.NewGuid().ToString();
            var url = "Applications/{applicationId}/$/ResumeBackup";
            url = url.Replace("{applicationId}", applicationId);
            var queryParams = new List<string>();
            
            // Append to queryParams if not null.
            serverTimeout?.AddToQueryParameters(queryParams, $"timeout={serverTimeout}");
            queryParams.Add("api-version=6.4");
            url += "?" + string.Join("&", queryParams);
            
            HttpRequestMessage RequestFunc()
            {
                var request = new HttpRequestMessage()
                {
                    Method = HttpMethod.Post,
                };
                return request;
            }

            return this.httpClient.SendAsync(RequestFunc, url, requestId, cancellationToken);
        }

        /// <inheritdoc />
        public Task EnableServiceBackupAsync(
            string serviceId,
            EnableBackupDescription enableBackupDescription,
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            serviceId.ThrowIfNull(nameof(serviceId));
            enableBackupDescription.ThrowIfNull(nameof(enableBackupDescription));
            serverTimeout?.ThrowIfOutOfInclusiveRange("serverTimeout", 1, 4294967295);
            var requestId = Guid.NewGuid().ToString();
            var url = "Services/{serviceId}/$/EnableBackup";
            url = url.Replace("{serviceId}", serviceId);
            var queryParams = new List<string>();
            
            // Append to queryParams if not null.
            serverTimeout?.AddToQueryParameters(queryParams, $"timeout={serverTimeout}");
            queryParams.Add("api-version=6.4");
            url += "?" + string.Join("&", queryParams);
            
            string content;
            using (var sw = new StringWriter())
            {
                EnableBackupDescriptionConverter.Serialize(new JsonTextWriter(sw), enableBackupDescription);
                content = sw.ToString();
            }

            HttpRequestMessage RequestFunc()
            {
                var request = new HttpRequestMessage()
                {
                    Method = HttpMethod.Post,
                    Content = new StringContent(content, Encoding.UTF8),
                };
                request.Content.Headers.ContentType = System.Net.Http.Headers.MediaTypeHeaderValue.Parse("application/json; charset=utf-8");
                return request;
            }

            return this.httpClient.SendAsync(RequestFunc, url, requestId, cancellationToken);
        }

        /// <inheritdoc />
        public Task DisableServiceBackupAsync(
            string serviceId,
            DisableBackupDescription disableBackupDescription = default(DisableBackupDescription),
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            serviceId.ThrowIfNull(nameof(serviceId));
            serverTimeout?.ThrowIfOutOfInclusiveRange("serverTimeout", 1, 4294967295);
            var requestId = Guid.NewGuid().ToString();
            var url = "Services/{serviceId}/$/DisableBackup";
            url = url.Replace("{serviceId}", serviceId);
            var queryParams = new List<string>();
            
            // Append to queryParams if not null.
            serverTimeout?.AddToQueryParameters(queryParams, $"timeout={serverTimeout}");
            queryParams.Add("api-version=6.4");
            url += "?" + string.Join("&", queryParams);
            
            string content;
            using (var sw = new StringWriter())
            {
                if (disableBackupDescription != default(DisableBackupDescription))
                {
                    DisableBackupDescriptionConverter.Serialize(new JsonTextWriter(sw), disableBackupDescription);
                }

                content = sw.ToString();
            }

            HttpRequestMessage RequestFunc()
            {
                var request = new HttpRequestMessage()
                {
                    Method = HttpMethod.Post,
                    Content = new StringContent(content, Encoding.UTF8),
                };
                request.Content.Headers.ContentType = System.Net.Http.Headers.MediaTypeHeaderValue.Parse("application/json; charset=utf-8");
                return request;
            }

            return this.httpClient.SendAsync(RequestFunc, url, requestId, cancellationToken);
        }

        /// <inheritdoc />
        public Task<PagedData<BackupConfigurationInfo>> GetServiceBackupConfigurationInfoAsync(
            string serviceId,
            ContinuationToken continuationToken = default(ContinuationToken),
            long? maxResults = 0,
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            serviceId.ThrowIfNull(nameof(serviceId));
            maxResults?.ThrowIfLessThan("maxResults", 0);
            serverTimeout?.ThrowIfOutOfInclusiveRange("serverTimeout", 1, 4294967295);
            var requestId = Guid.NewGuid().ToString();
            var url = "Services/{serviceId}/$/GetBackupConfigurationInfo";
            url = url.Replace("{serviceId}", serviceId);
            var queryParams = new List<string>();
            
            // Append to queryParams if not null.
            continuationToken?.AddToQueryParameters(queryParams, $"ContinuationToken={continuationToken.ToString()}");
            maxResults?.AddToQueryParameters(queryParams, $"MaxResults={maxResults}");
            serverTimeout?.AddToQueryParameters(queryParams, $"timeout={serverTimeout}");
            queryParams.Add("api-version=6.4");
            url += "?" + string.Join("&", queryParams);
            
            HttpRequestMessage RequestFunc()
            {
                var request = new HttpRequestMessage()
                {
                    Method = HttpMethod.Get,
                };
                return request;
            }

            return this.httpClient.SendAsyncGetResponseAsPagedData(RequestFunc, url, BackupConfigurationInfoConverter.Deserialize, requestId, cancellationToken);
        }

        /// <inheritdoc />
        public Task<PagedData<BackupInfo>> GetServiceBackupListAsync(
            string serviceId,
            long? serverTimeout = 60,
            bool? latest = false,
            DateTime? startDateTimeFilter = default(DateTime?),
            DateTime? endDateTimeFilter = default(DateTime?),
            ContinuationToken continuationToken = default(ContinuationToken),
            long? maxResults = 0,
            bool? metadataFromBlob = false,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            serviceId.ThrowIfNull(nameof(serviceId));
            serverTimeout?.ThrowIfOutOfInclusiveRange("serverTimeout", 1, 4294967295);
            maxResults?.ThrowIfLessThan("maxResults", 0);
            var requestId = Guid.NewGuid().ToString();
            var url = "Services/{serviceId}/$/GetBackups";
            url = url.Replace("{serviceId}", serviceId);
            var queryParams = new List<string>();
            
            // Append to queryParams if not null.
            serverTimeout?.AddToQueryParameters(queryParams, $"timeout={serverTimeout}");
            latest?.AddToQueryParameters(queryParams, $"Latest={latest}");
            startDateTimeFilter?.AddToQueryParameters(queryParams, $"StartDateTimeFilter={System.Xml.XmlConvert.ToString(startDateTimeFilter.Value, System.Xml.XmlDateTimeSerializationMode.Utc).ToString()}");
            endDateTimeFilter?.AddToQueryParameters(queryParams, $"EndDateTimeFilter={System.Xml.XmlConvert.ToString(endDateTimeFilter.Value, System.Xml.XmlDateTimeSerializationMode.Utc).ToString()}");
            continuationToken?.AddToQueryParameters(queryParams, $"ContinuationToken={continuationToken.ToString()}");
            maxResults?.AddToQueryParameters(queryParams, $"MaxResults={maxResults}");
            metadataFromBlob?.AddToQueryParameters(queryParams, $"MetadataFromBlob={metadataFromBlob}");
            queryParams.Add("api-version=6.4");
            url += "?" + string.Join("&", queryParams);
            
            HttpRequestMessage RequestFunc()
            {
                var request = new HttpRequestMessage()
                {
                    Method = HttpMethod.Get,
                };
                return request;
            }

            return this.httpClient.SendAsyncGetResponseAsPagedData(RequestFunc, url, BackupInfoConverter.Deserialize, requestId, cancellationToken);
        }

        /// <inheritdoc />
        public Task SuspendServiceBackupAsync(
            string serviceId,
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            serviceId.ThrowIfNull(nameof(serviceId));
            serverTimeout?.ThrowIfOutOfInclusiveRange("serverTimeout", 1, 4294967295);
            var requestId = Guid.NewGuid().ToString();
            var url = "Services/{serviceId}/$/SuspendBackup";
            url = url.Replace("{serviceId}", serviceId);
            var queryParams = new List<string>();
            
            // Append to queryParams if not null.
            serverTimeout?.AddToQueryParameters(queryParams, $"timeout={serverTimeout}");
            queryParams.Add("api-version=6.4");
            url += "?" + string.Join("&", queryParams);
            
            HttpRequestMessage RequestFunc()
            {
                var request = new HttpRequestMessage()
                {
                    Method = HttpMethod.Post,
                };
                return request;
            }

            return this.httpClient.SendAsync(RequestFunc, url, requestId, cancellationToken);
        }

        /// <inheritdoc />
        public Task ResumeServiceBackupAsync(
            string serviceId,
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            serviceId.ThrowIfNull(nameof(serviceId));
            serverTimeout?.ThrowIfOutOfInclusiveRange("serverTimeout", 1, 4294967295);
            var requestId = Guid.NewGuid().ToString();
            var url = "Services/{serviceId}/$/ResumeBackup";
            url = url.Replace("{serviceId}", serviceId);
            var queryParams = new List<string>();
            
            // Append to queryParams if not null.
            serverTimeout?.AddToQueryParameters(queryParams, $"timeout={serverTimeout}");
            queryParams.Add("api-version=6.4");
            url += "?" + string.Join("&", queryParams);
            
            HttpRequestMessage RequestFunc()
            {
                var request = new HttpRequestMessage()
                {
                    Method = HttpMethod.Post,
                };
                return request;
            }

            return this.httpClient.SendAsync(RequestFunc, url, requestId, cancellationToken);
        }

        /// <inheritdoc />
        public Task EnablePartitionBackupAsync(
            PartitionId partitionId,
            EnableBackupDescription enableBackupDescription,
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            partitionId.ThrowIfNull(nameof(partitionId));
            enableBackupDescription.ThrowIfNull(nameof(enableBackupDescription));
            serverTimeout?.ThrowIfOutOfInclusiveRange("serverTimeout", 1, 4294967295);
            var requestId = Guid.NewGuid().ToString();
            var url = "Partitions/{partitionId}/$/EnableBackup";
            url = url.Replace("{partitionId}", partitionId.ToString());
            var queryParams = new List<string>();
            
            // Append to queryParams if not null.
            serverTimeout?.AddToQueryParameters(queryParams, $"timeout={serverTimeout}");
            queryParams.Add("api-version=6.4");
            url += "?" + string.Join("&", queryParams);
            
            string content;
            using (var sw = new StringWriter())
            {
                EnableBackupDescriptionConverter.Serialize(new JsonTextWriter(sw), enableBackupDescription);
                content = sw.ToString();
            }

            HttpRequestMessage RequestFunc()
            {
                var request = new HttpRequestMessage()
                {
                    Method = HttpMethod.Post,
                    Content = new StringContent(content, Encoding.UTF8),
                };
                request.Content.Headers.ContentType = System.Net.Http.Headers.MediaTypeHeaderValue.Parse("application/json; charset=utf-8");
                return request;
            }

            return this.httpClient.SendAsync(RequestFunc, url, requestId, cancellationToken);
        }

        /// <inheritdoc />
        public Task DisablePartitionBackupAsync(
            PartitionId partitionId,
            DisableBackupDescription disableBackupDescription = default(DisableBackupDescription),
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            partitionId.ThrowIfNull(nameof(partitionId));
            serverTimeout?.ThrowIfOutOfInclusiveRange("serverTimeout", 1, 4294967295);
            var requestId = Guid.NewGuid().ToString();
            var url = "Partitions/{partitionId}/$/DisableBackup";
            url = url.Replace("{partitionId}", partitionId.ToString());
            var queryParams = new List<string>();
            
            // Append to queryParams if not null.
            serverTimeout?.AddToQueryParameters(queryParams, $"timeout={serverTimeout}");
            queryParams.Add("api-version=6.4");
            url += "?" + string.Join("&", queryParams);
            
            string content;
            using (var sw = new StringWriter())
            {
                if (disableBackupDescription != default(DisableBackupDescription))
                {
                    DisableBackupDescriptionConverter.Serialize(new JsonTextWriter(sw), disableBackupDescription);
                }

                content = sw.ToString();
            }

            HttpRequestMessage RequestFunc()
            {
                var request = new HttpRequestMessage()
                {
                    Method = HttpMethod.Post,
                    Content = new StringContent(content, Encoding.UTF8),
                };
                request.Content.Headers.ContentType = System.Net.Http.Headers.MediaTypeHeaderValue.Parse("application/json; charset=utf-8");
                return request;
            }

            return this.httpClient.SendAsync(RequestFunc, url, requestId, cancellationToken);
        }

        /// <inheritdoc />
        public Task<PartitionBackupConfigurationInfo> GetPartitionBackupConfigurationInfoAsync(
            PartitionId partitionId,
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            partitionId.ThrowIfNull(nameof(partitionId));
            serverTimeout?.ThrowIfOutOfInclusiveRange("serverTimeout", 1, 4294967295);
            var requestId = Guid.NewGuid().ToString();
            var url = "Partitions/{partitionId}/$/GetBackupConfigurationInfo";
            url = url.Replace("{partitionId}", partitionId.ToString());
            var queryParams = new List<string>();
            
            // Append to queryParams if not null.
            serverTimeout?.AddToQueryParameters(queryParams, $"timeout={serverTimeout}");
            queryParams.Add("api-version=6.4");
            url += "?" + string.Join("&", queryParams);
            
            HttpRequestMessage RequestFunc()
            {
                var request = new HttpRequestMessage()
                {
                    Method = HttpMethod.Get,
                };
                return request;
            }

            return this.httpClient.SendAsyncGetResponse(RequestFunc, url, PartitionBackupConfigurationInfoConverter.Deserialize, requestId, cancellationToken);
        }

        /// <inheritdoc />
        public Task<PagedData<BackupInfo>> GetPartitionBackupListAsync(
            PartitionId partitionId,
            long? serverTimeout = 60,
            bool? latest = false,
            DateTime? startDateTimeFilter = default(DateTime?),
            DateTime? endDateTimeFilter = default(DateTime?),
            bool? metadataFromBlob = false,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            partitionId.ThrowIfNull(nameof(partitionId));
            serverTimeout?.ThrowIfOutOfInclusiveRange("serverTimeout", 1, 4294967295);
            var requestId = Guid.NewGuid().ToString();
            var url = "Partitions/{partitionId}/$/GetBackups";
            url = url.Replace("{partitionId}", partitionId.ToString());
            var queryParams = new List<string>();
            
            // Append to queryParams if not null.
            serverTimeout?.AddToQueryParameters(queryParams, $"timeout={serverTimeout}");
            latest?.AddToQueryParameters(queryParams, $"Latest={latest}");
            startDateTimeFilter?.AddToQueryParameters(queryParams, $"StartDateTimeFilter={System.Xml.XmlConvert.ToString(startDateTimeFilter.Value, System.Xml.XmlDateTimeSerializationMode.Utc).ToString()}");
            endDateTimeFilter?.AddToQueryParameters(queryParams, $"EndDateTimeFilter={System.Xml.XmlConvert.ToString(endDateTimeFilter.Value, System.Xml.XmlDateTimeSerializationMode.Utc).ToString()}");
            metadataFromBlob?.AddToQueryParameters(queryParams, $"MetadataFromBlob={metadataFromBlob}");
            queryParams.Add("api-version=6.4");
            url += "?" + string.Join("&", queryParams);
            
            HttpRequestMessage RequestFunc()
            {
                var request = new HttpRequestMessage()
                {
                    Method = HttpMethod.Get,
                };
                return request;
            }

            return this.httpClient.SendAsyncGetResponseAsPagedData(RequestFunc, url, BackupInfoConverter.Deserialize, requestId, cancellationToken);
        }

        /// <inheritdoc />
        public Task SuspendPartitionBackupAsync(
            PartitionId partitionId,
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            partitionId.ThrowIfNull(nameof(partitionId));
            serverTimeout?.ThrowIfOutOfInclusiveRange("serverTimeout", 1, 4294967295);
            var requestId = Guid.NewGuid().ToString();
            var url = "Partitions/{partitionId}/$/SuspendBackup";
            url = url.Replace("{partitionId}", partitionId.ToString());
            var queryParams = new List<string>();
            
            // Append to queryParams if not null.
            serverTimeout?.AddToQueryParameters(queryParams, $"timeout={serverTimeout}");
            queryParams.Add("api-version=6.4");
            url += "?" + string.Join("&", queryParams);
            
            HttpRequestMessage RequestFunc()
            {
                var request = new HttpRequestMessage()
                {
                    Method = HttpMethod.Post,
                };
                return request;
            }

            return this.httpClient.SendAsync(RequestFunc, url, requestId, cancellationToken);
        }

        /// <inheritdoc />
        public Task ResumePartitionBackupAsync(
            PartitionId partitionId,
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            partitionId.ThrowIfNull(nameof(partitionId));
            serverTimeout?.ThrowIfOutOfInclusiveRange("serverTimeout", 1, 4294967295);
            var requestId = Guid.NewGuid().ToString();
            var url = "Partitions/{partitionId}/$/ResumeBackup";
            url = url.Replace("{partitionId}", partitionId.ToString());
            var queryParams = new List<string>();
            
            // Append to queryParams if not null.
            serverTimeout?.AddToQueryParameters(queryParams, $"timeout={serverTimeout}");
            queryParams.Add("api-version=6.4");
            url += "?" + string.Join("&", queryParams);
            
            HttpRequestMessage RequestFunc()
            {
                var request = new HttpRequestMessage()
                {
                    Method = HttpMethod.Post,
                };
                return request;
            }

            return this.httpClient.SendAsync(RequestFunc, url, requestId, cancellationToken);
        }

        /// <inheritdoc />
        public Task BackupPartitionAsync(
            PartitionId partitionId,
            BackupPartitionDescription backupPartitionDescription = default(BackupPartitionDescription),
            int? backupTimeout = 10,
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            partitionId.ThrowIfNull(nameof(partitionId));
            serverTimeout?.ThrowIfOutOfInclusiveRange("serverTimeout", 1, 4294967295);
            var requestId = Guid.NewGuid().ToString();
            var url = "Partitions/{partitionId}/$/Backup";
            url = url.Replace("{partitionId}", partitionId.ToString());
            var queryParams = new List<string>();
            
            // Append to queryParams if not null.
            backupTimeout?.AddToQueryParameters(queryParams, $"BackupTimeout={backupTimeout}");
            serverTimeout?.AddToQueryParameters(queryParams, $"timeout={serverTimeout}");
            queryParams.Add("api-version=6.4");
            url += "?" + string.Join("&", queryParams);
            
            string content;
            using (var sw = new StringWriter())
            {
                if (backupPartitionDescription != default(BackupPartitionDescription))
                {
                    BackupPartitionDescriptionConverter.Serialize(new JsonTextWriter(sw), backupPartitionDescription);
                }

                content = sw.ToString();
            }

            HttpRequestMessage RequestFunc()
            {
                var request = new HttpRequestMessage()
                {
                    Method = HttpMethod.Post,
                    Content = new StringContent(content, Encoding.UTF8),
                };
                request.Content.Headers.ContentType = System.Net.Http.Headers.MediaTypeHeaderValue.Parse("application/json; charset=utf-8");
                return request;
            }

            return this.httpClient.SendAsync(RequestFunc, url, requestId, cancellationToken);
        }

        /// <inheritdoc />
        public Task<BackupProgressInfo> GetPartitionBackupProgressAsync(
            PartitionId partitionId,
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            partitionId.ThrowIfNull(nameof(partitionId));
            serverTimeout?.ThrowIfOutOfInclusiveRange("serverTimeout", 1, 4294967295);
            var requestId = Guid.NewGuid().ToString();
            var url = "Partitions/{partitionId}/$/GetBackupProgress";
            url = url.Replace("{partitionId}", partitionId.ToString());
            var queryParams = new List<string>();
            
            // Append to queryParams if not null.
            serverTimeout?.AddToQueryParameters(queryParams, $"timeout={serverTimeout}");
            queryParams.Add("api-version=6.4");
            url += "?" + string.Join("&", queryParams);
            
            HttpRequestMessage RequestFunc()
            {
                var request = new HttpRequestMessage()
                {
                    Method = HttpMethod.Get,
                };
                return request;
            }

            return this.httpClient.SendAsyncGetResponse(RequestFunc, url, BackupProgressInfoConverter.Deserialize, requestId, cancellationToken);
        }

        /// <inheritdoc />
        public Task RestorePartitionAsync(
            PartitionId partitionId,
            RestorePartitionDescription restorePartitionDescription = default(RestorePartitionDescription),
            bool? latest = false,
            int? restoreTimeout = 10,
            long? serverTimeout = 60,
            bool? metadataFromBlob = false,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            partitionId.ThrowIfNull(nameof(partitionId));
            serverTimeout?.ThrowIfOutOfInclusiveRange("serverTimeout", 1, 4294967295);
            var requestId = Guid.NewGuid().ToString();
            var url = "Partitions/{partitionId}/$/Restore";
            url = url.Replace("{partitionId}", partitionId.ToString());
            var queryParams = new List<string>();
            
            // Append to queryParams if not null.
            latest?.AddToQueryParameters(queryParams, $"Latest={latest}");
            restoreTimeout?.AddToQueryParameters(queryParams, $"RestoreTimeout={restoreTimeout}");
            serverTimeout?.AddToQueryParameters(queryParams, $"timeout={serverTimeout}");
            metadataFromBlob?.AddToQueryParameters(queryParams, $"MetadataFromBlob={metadataFromBlob}");
            queryParams.Add("api-version=6.4");
            url += "?" + string.Join("&", queryParams);
            
            string content;
            using (var sw = new StringWriter())
            {
                if (restorePartitionDescription != default(RestorePartitionDescription))
                {
                    RestorePartitionDescriptionConverter.Serialize(new JsonTextWriter(sw), restorePartitionDescription);
                }

                content = sw.ToString();
            }

            HttpRequestMessage RequestFunc()
            {
                var request = new HttpRequestMessage()
                {
                    Method = HttpMethod.Post,
                    Content = new StringContent(content, Encoding.UTF8),
                };
                request.Content.Headers.ContentType = System.Net.Http.Headers.MediaTypeHeaderValue.Parse("application/json; charset=utf-8");
                return request;
            }

            return this.httpClient.SendAsync(RequestFunc, url, requestId, cancellationToken);
        }

        /// <inheritdoc />
        public Task SkipRestorePartitionAsync(
            PartitionId partitionId,
            int? restoreTimeout = 10,
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            partitionId.ThrowIfNull(nameof(partitionId));
            serverTimeout?.ThrowIfOutOfInclusiveRange("serverTimeout", 1, 4294967295);
            var requestId = Guid.NewGuid().ToString();
            var url = "Partitions/{partitionId}/$/SkipAutoRestore";
            url = url.Replace("{partitionId}", partitionId.ToString());
            var queryParams = new List<string>();
            
            // Append to queryParams if not null.
            restoreTimeout?.AddToQueryParameters(queryParams, $"RestoreTimeout={restoreTimeout}");
            serverTimeout?.AddToQueryParameters(queryParams, $"timeout={serverTimeout}");
            queryParams.Add("api-version=9.1");
            url += "?" + string.Join("&", queryParams);
            
            HttpRequestMessage RequestFunc()
            {
                var request = new HttpRequestMessage()
                {
                    Method = HttpMethod.Post,
                };
                return request;
            }

            return this.httpClient.SendAsync(RequestFunc, url, requestId, cancellationToken);
        }

        /// <inheritdoc />
        public Task<RestoreProgressInfo> GetPartitionRestoreProgressAsync(
            PartitionId partitionId,
            long? serverTimeout = 60,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            partitionId.ThrowIfNull(nameof(partitionId));
            serverTimeout?.ThrowIfOutOfInclusiveRange("serverTimeout", 1, 4294967295);
            var requestId = Guid.NewGuid().ToString();
            var url = "Partitions/{partitionId}/$/GetRestoreProgress";
            url = url.Replace("{partitionId}", partitionId.ToString());
            var queryParams = new List<string>();
            
            // Append to queryParams if not null.
            serverTimeout?.AddToQueryParameters(queryParams, $"timeout={serverTimeout}");
            queryParams.Add("api-version=6.4");
            url += "?" + string.Join("&", queryParams);
            
            HttpRequestMessage RequestFunc()
            {
                var request = new HttpRequestMessage()
                {
                    Method = HttpMethod.Get,
                };
                return request;
            }

            return this.httpClient.SendAsyncGetResponse(RequestFunc, url, RestoreProgressInfoConverter.Deserialize, requestId, cancellationToken);
        }

        /// <inheritdoc />
        public Task<PagedData<BackupInfo>> GetBackupsFromBackupLocationAsync(
            GetBackupByStorageQueryDescription getBackupByStorageQueryDescription,
            long? serverTimeout = 60,
            ContinuationToken continuationToken = default(ContinuationToken),
            long? maxResults = 0,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            getBackupByStorageQueryDescription.ThrowIfNull(nameof(getBackupByStorageQueryDescription));
            serverTimeout?.ThrowIfOutOfInclusiveRange("serverTimeout", 1, 4294967295);
            maxResults?.ThrowIfLessThan("maxResults", 0);
            var requestId = Guid.NewGuid().ToString();
            var url = "BackupRestore/$/GetBackups";
            var queryParams = new List<string>();
            
            // Append to queryParams if not null.
            serverTimeout?.AddToQueryParameters(queryParams, $"timeout={serverTimeout}");
            continuationToken?.AddToQueryParameters(queryParams, $"ContinuationToken={continuationToken.ToString()}");
            maxResults?.AddToQueryParameters(queryParams, $"MaxResults={maxResults}");
            queryParams.Add("api-version=6.4");
            url += "?" + string.Join("&", queryParams);
            
            string content;
            using (var sw = new StringWriter())
            {
                GetBackupByStorageQueryDescriptionConverter.Serialize(new JsonTextWriter(sw), getBackupByStorageQueryDescription);
                content = sw.ToString();
            }

            HttpRequestMessage RequestFunc()
            {
                var request = new HttpRequestMessage()
                {
                    Method = HttpMethod.Post,
                    Content = new StringContent(content, Encoding.UTF8),
                };
                request.Content.Headers.ContentType = System.Net.Http.Headers.MediaTypeHeaderValue.Parse("application/json; charset=utf-8");
                return request;
            }

            return this.httpClient.SendAsyncGetResponseAsPagedData(RequestFunc, url, BackupInfoConverter.Deserialize, requestId, cancellationToken);
        }
    }
}
