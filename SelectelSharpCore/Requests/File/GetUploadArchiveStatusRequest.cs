using System.Collections.Generic;
using SelectelSharpCore.Headers;
using System.Collections.Specialized;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace SelectelSharpCore.Requests.File
{
    public class GetUploadArchiveStatusRequest : 
        BaseRequest<string>
    {
        private string ExtractId { get; set; }

        internal override HttpMethod Method
        {
            get
            {
                return HttpMethod.Get;
            }
        }


        public GetUploadArchiveStatusRequest(string extractId)
            : base()
        {
            TryAddHeader(HeaderKeys.Accept, HeaderKeys.AcceptJson);
            this.ExtractId = extractId;
        }

        protected override string GetUrl(string storageUrl)
        {
            return string.Format("https://api.selcdn.ru/v1/extract-archive/{0}", this.ExtractId);
        }

        internal override void Parse(HttpResponseHeaders headers, object content, HttpStatusCode status)
        {
            base.Parse(headers, content, status);
        }
    }
}
