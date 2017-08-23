using SelectelSharpCore.Headers;
using SelectelSharpCore.Models.File;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;

namespace SelectelSharpCore.Requests.File
{
    public class UploadFileRequest : FileRequest<UploadFileResult>
    {
        private string ETag { get; set; }

        public UploadFileRequest(
            string containerName,
            string fileName,
            byte[] file,
            bool validateChecksum = false,
            string contentDisposition = null,
            string contentType = null,            
            long? deleteAt = null,
            long? deleteAfter = null,
            IDictionary<string, object> customHeaders = null) : base(containerName, fileName)
        {
            TryAddHeader(HeaderKeys.ContentDisposition, contentDisposition);

            if (deleteAfter.HasValue)
            {
                TryAddHeader(HeaderKeys.XDeleteAfter, deleteAfter.Value);
            }

            if (deleteAt.HasValue)
            {
                TryAddHeader(HeaderKeys.XDeleteAt, deleteAt.Value);
            }

            if (string.IsNullOrEmpty(contentType) == false)
            {
                TryAddHeader(HeaderKeys.ContentType, contentType);
            }

            if (string.IsNullOrEmpty(contentDisposition) == false)
            {
                TryAddHeader(HeaderKeys.ContentDisposition, contentDisposition);
            }

            SetCustomHeaders(customHeaders);

            if (validateChecksum)
            {
                this.ETag = GetETag(file);
                TryAddHeader(HeaderKeys.ETag, this.ETag);
            }

            this.File = file;
        }

        internal override HttpMethod Method
        {
            get
            {
                return HttpMethod.Put;
            }
        }

        internal override void Parse(HttpResponseHeaders headers, object data, HttpStatusCode status)
        {
            if (status == HttpStatusCode.Created)
            {
                if (this.ETag != null)
                {
                    // idk why Selectel's ETag check not working, so check the result once again on client.
                    if (headers.GetValues(HeaderKeys.ETag).FirstOrDefault().Equals(this.ETag, StringComparison.OrdinalIgnoreCase) == false)
                    {
                        this.Result = UploadFileResult.CheckSumValidationFailed;
                        return;
                    }
                }

                this.Result = UploadFileResult.Created;
            }
            else if ((int)status == 422)
            {
                this.Result = UploadFileResult.CheckSumValidationFailed;
            }
            else
            {
                ParseError(null, status);
            }
        }

        private string GetETag(byte[] file)
        {
            using (var md5 = MD5.Create())
            {
                var hash = md5.ComputeHash(file);
                return BitConverter.ToString(hash).Replace("-", string.Empty).ToLower();
                //return Encoding.Default.GetString();
            }
        }
    }
}
