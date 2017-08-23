using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SelectelSharpCore.Requests.Container;
using SelectelSharpCore.Models.Link;
using System.Collections.Specialized;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace SelectelSharpCore.Requests.File
{
    public class SymlinkRequest : ContainerRequest<bool>
    {
        private string link;

        internal override HttpMethod Method
        {
            get
            {
                return HttpMethod.Put;
            }
        }

        public SymlinkRequest(string containerName, Symlink link)
            : base(containerName)
        {
            this.link = link.Link;
            SetCustomHeaders(link.GetHeaders());
        }

        internal override void Parse(HttpResponseHeaders headers, object content, HttpStatusCode status)
        {
            base.Parse(headers, content, status);
        }

        internal override void ParseError(HttpRequestException ex, HttpStatusCode status)
        {
            base.ParseError(ex, status);
        }

        protected override string GetUrl(string storageUrl)
        {
            return string.Format("{0}/{1}/{2}", storageUrl, this.ContainerName, this.link);
        }
    }
}
