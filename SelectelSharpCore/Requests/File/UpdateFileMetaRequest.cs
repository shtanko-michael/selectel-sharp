using SelectelSharpCore.Headers;
using SelectelSharpCore.Models.File;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace SelectelSharpCore.Requests.File
{
    public class UpdateFileMetaRequest : FileRequest<UpdateFileResult>
    {
        internal override HttpMethod Method
        {
            get
            {
                return HttpMethod.Post;
            }
        }

        public UpdateFileMetaRequest(
            string containerName, 
            string fileName, 
            IDictionary<string, object> customHeaders = null, 
            CORSHeaders corsHeaders = null)
            : base(containerName, fileName)
        {
            SetCustomHeaders(customHeaders);
            SetCORSHeaders(corsHeaders);
        }

        internal override void Parse(HttpResponseHeaders headers, object data, HttpStatusCode status)
        {
            if (status == HttpStatusCode.NoContent)
            {
                this.Result = UpdateFileResult.Updated;
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
                this.Result = UpdateFileResult.NotFound;
            }
            else
            {
                base.ParseError(null, status);
            }
        }
    }
}
