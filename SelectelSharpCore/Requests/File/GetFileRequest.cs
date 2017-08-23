using System.Collections.Generic;
using SelectelSharpCore.Headers;
using SelectelSharpCore.Models;
using SelectelSharpCore.Models.File;
using System.Collections.Specialized;
using System.Net;
using System.Net.Http.Headers;

namespace SelectelSharpCore.Requests.File
{
    public class GetFileRequest : FileRequest<GetFileResult>
    {
        private bool allowAnonymously;

        public GetFileRequest(string containerName, string fileName, ConditionalHeaders conditionalHeaders = null, bool allowAnonymously = false)
            : base(containerName, fileName)
        {            
            this.allowAnonymously = allowAnonymously;

            SetConditionalHeaders(conditionalHeaders);
        }

        public override bool AllowAnonymously
        {
            get
            {
                return allowAnonymously;
            }
        }

        public override bool DownloadData
        {
            get
            {
                return true;
            }
        }

        internal override void Parse(HttpResponseHeaders headers, object data, HttpStatusCode status)
        {
            if (status == HttpStatusCode.OK)
            {
                this.Result = new GetFileResult((byte[])data, this.FileName, headers);
            }
            else
            {
                ParseError(null, status);
            }
        }
    }
}
