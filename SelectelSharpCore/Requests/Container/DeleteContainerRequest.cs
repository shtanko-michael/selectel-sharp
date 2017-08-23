using System.Collections.Generic;
using SelectelSharpCore.Models.Container;
using System.Collections.Specialized;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace SelectelSharpCore.Requests.Container
{
    /// <summary>
    /// Запрос на удаление контейнера
    /// </summary>
    public class DeleteContainerRequest : ContainerRequest<DeleteContainerResult>
    {
        public DeleteContainerRequest(string containerName)
            : base(containerName)
        {
        }

        internal override HttpMethod Method
        {
            get
            {
                return HttpMethod.Delete;
            }
        }

        internal override void Parse(HttpResponseHeaders headers, object data, HttpStatusCode status)
        {
            if (status == HttpStatusCode.NoContent)
            {
                this.Result = DeleteContainerResult.Deleted;
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
                this.Result = DeleteContainerResult.NotFound;
            }
            else if (status == HttpStatusCode.Conflict)
            {
                this.Result = DeleteContainerResult.NotEmpty;
            }
            else
            {
                base.ParseError(ex, status);
            }
        }
    }
}
