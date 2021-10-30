using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using SelectelSharpCore.Headers;
using SelectelSharpCore.Models;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SelectelSharpCore.Requests {
    public abstract class BaseRequest<T> {
        public byte[] File { get; protected set; }
        public T Result { get; protected set; }
        public bool IsSuccesful { get; set; }

        public virtual bool AllowAnonymously => false;

        public virtual bool DownloadData => false;

        internal virtual HttpMethod Method => HttpMethod.Get;

        private readonly IDictionary<string, string> query = new Dictionary<string, string>();
        private readonly IDictionary<string, string> headers = new Dictionary<string, string>();

        protected void SetCustomHeaders(IDictionary<string, object> headers = null) {
            if (headers == null) {
                return;
            }

            foreach (var header in headers) {
                var headerKey = header.Key;
                //if (!headerKey.ToLower().StartsWith(HeaderKeys.XContainerMetaPrefix.ToLower()))
                //{
                //    headerKey = string.Concat(HeaderKeys.XContainerMetaPrefix, header.Key);
                //}

                this.TryAddHeader(headerKey, header.Value);
            }
        }

        protected void SetCORSHeaders(CORSHeaders cors = null) {
            if (cors == null) {
                return;
            }

            foreach (var header in cors.GetHeaders()) {
                this.TryAddHeader(header.Key, header.Value);
            }
        }

        protected void SetConditionalHeaders(ConditionalHeaders conditional = null) {
            if (conditional == null) {
                return;
            }

            foreach (var header in conditional.GetHeaders()) {
                this.TryAddHeader(header.Key, header.Value);
            }
        }

        protected virtual string GetUrl(string storageUrl) {
            return storageUrl;
        }

        private Uri GetUri(string storageUrl) {
            var url = GetUrl(storageUrl);

            // set query params
            if (query != null && query.Any()) {
                var queryParamsList = query
                    .Where(x => !string.IsNullOrEmpty(x.Value))
                    .Select(x => string.Concat(x.Key, "=", x.Value));

                var queryParams = string.Join("&", queryParamsList);
                if (url.Contains("?")) {
                    url = string.Concat(url, queryParams);
                } else {
                    url = string.Concat(url, "?", queryParams);
                }
            }

            return new Uri(url);
        }

        internal void TryAddQueryParam(string key, object value) {
            if (value != null) {
                query.Add(key, value.ToString());
            }
        }

        internal void TryAddHeader(string key, object value) {
            if (value != null) {
                headers.Add(key, value.ToString());
            }
        }

        internal virtual async Task Execute(string storageUrl, string authToken) {
            var uri = GetUri(storageUrl);

#if DEBUG
            Debug.WriteLine(uri.ToString());
#endif

            //var client = new HttpClient();

            var request = new HttpRequestMessage {
                RequestUri = uri,
                Method = Method,
            };

            //var request = HttpWebRequest.CreateHttp(uri);
            //request.Method = this.Method.ToString();

            if (!AllowAnonymously) {
                TryAddHeader(HeaderKeys.XAuthToken, authToken);
            }

            // set Accept header
            if (headers.ContainsKey(HeaderKeys.Accept)) {
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(headers[HeaderKeys.Accept]));
                headers.Remove(HeaderKeys.Accept);
            }

            // set Content-Type header
            MediaTypeHeaderValue contentTypeHeader = null;
            if (headers.ContainsKey(HeaderKeys.ContentType)) {
                contentTypeHeader = new MediaTypeHeaderValue(headers[HeaderKeys.ContentType]);
                headers.Remove(HeaderKeys.ContentType);
            }

            // set Content-Length header
            // todo: make SetContLength method
            if (headers.ContainsKey(HeaderKeys.ContentLength)) {
                request.Content.Headers.ContentLength = long.Parse(headers[HeaderKeys.ContentLength]);
                //request.Headers.TryAddWithoutValidation(HeaderKeys.ContentLength, File.Length.ToString());

                headers.Remove(HeaderKeys.ContentLength);
            }

            // set custom headers
            if (headers != null) {
                foreach (var kv in headers.Where(x => !string.IsNullOrEmpty(x.Value))) {
                    request.Headers.Add(kv.Key, kv.Value);
                }
            }

            if (File != null && File.Length > 0) {
                request.Content = new ByteArrayContent(File);
                if (contentTypeHeader != null)
                    request.Content.Headers.ContentType = contentTypeHeader;
            }

            HttpStatusCode status = HttpStatusCode.OK;
            try {
                using (var client = new HttpClient()) {
                    HttpResponseMessage response = await client.SendAsync(request);
                    status = response.StatusCode;
                    response.EnsureSuccessStatusCode();

                    using (var rs = await response.Content.ReadAsStreamAsync()) {
                        if (DownloadData) {
                            ParseData(response, status, rs);
                        } else {
                            ParseString(response, status, rs);
                        }
                    }

                }
            } catch (HttpRequestException ex) {
                ParseError(ex, status);
            } catch (Exception ex) {
                throw (ex);
            }
        }

        private void ParseString(HttpResponseMessage response, HttpStatusCode status, Stream rs) {
            using (var reader = new StreamReader(rs, Encoding.UTF8)) {
                string content = reader.ReadToEnd();
                this.Parse(response.Headers, content, status);
            }
        }

        private void ParseData(HttpResponseMessage response, HttpStatusCode status, Stream rs) {
            using (MemoryStream ms = new MemoryStream()) {
                rs.CopyTo(ms);
                this.Parse(response.Headers, ms.ToArray(), status);
            }
        }

        internal virtual void Parse(HttpResponseHeaders headers, object content, HttpStatusCode status) {
            if (content != null) {
                SetResult(content);
            } else {
                SetStatusResult(status);
            }
        }

        internal virtual void ParseError(HttpRequestException ex, HttpStatusCode status) {
            if (ex != null) {
                throw ex;
            }
            throw new SelectelWebException(status);
        }

        internal virtual void SetStatusResult(HttpStatusCode status, params HttpStatusCode[] successfulStatuses) {
            if (typeof(T).GetTypeInfo().IsEnum && typeof(T).Equals(status.GetType())) {
                if (successfulStatuses == null || !successfulStatuses.Any() || successfulStatuses.Contains(status)) {
                    SetResult(status);
                } else {
                    throw new SelectelWebException(status);
                }
            } else {
                throw new ArgumentException();
            }
        }

        private void SetResult(object content) {
            if (content is T) {
                this.Result = (T)content;
                IsSuccesful = true;
            } else if (content is string) {
                this.Result = JsonConvert.DeserializeObject<T>(content as string, new IsoDateTimeConverter { DateTimeFormat = "YYYY-MM-DDThh:mm:ss.sTZD" });
                IsSuccesful = true;
            } else {
                throw new ArgumentException();
            }
        }
    }
}