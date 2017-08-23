using System.Collections.Generic;
using SelectelSharpCore.Models.File;
using System.Collections.Specialized;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace SelectelSharpCore.Requests.File
{
    public class DeleteFileRequest : FileRequest<DeleteFileResult>
    {
        public DeleteFileRequest(string containerName, string fileName) 
            : base(containerName, fileName)
        {
        }

        internal override HttpMethod Method
        {
            get
            {
                return HttpMethod.Delete;;
            }
        }

        internal override void Parse(HttpResponseHeaders headers, object data, HttpStatusCode status)
        {
            if (status == HttpStatusCode.NoContent)
            {
                this.Result = DeleteFileResult.Deleted;
            }
            else
            {
                base.ParseError(null, status);
            }
        }

        internal override void ParseError(HttpRequestException ex, HttpStatusCode status)
        {
            if (status == HttpStatusCode.NotFound)
            {
                this.Result = DeleteFileResult.NotFound;
            }
            else
            {
                base.ParseError(ex, status);
            }
        }
    }
}
