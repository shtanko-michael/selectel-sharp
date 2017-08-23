using System.Collections.Generic;
using System.Collections.Specialized;
using SelectelSharpCore.Models;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace SelectelSharpCore.Requests
{
    /// <summary>
    /// Запрос информации о хранилище
    /// </summary>
    public class StorageInfoRequest : BaseRequest<StorageInfo>
    {
        internal override HttpMethod Method => HttpMethod.Head;

        internal override void Parse(HttpResponseHeaders headers, object data, HttpStatusCode status)
        {
            this.Result = new StorageInfo(headers);
        }
    }
}
