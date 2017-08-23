using System.Collections.Generic;
using Newtonsoft.Json;
using SelectelSharpCore.Common;
using SelectelSharpCore.Headers;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Net.Http.Headers;

namespace SelectelSharpCore.Models.File
{
    public class UploadArchiveResult
    {
        [JsonProperty("Response Status")]
        public string ResponseStatus { get; set; }

        [JsonProperty("Response Body")]
        public string ResponseBody { get; set; }

        public ReadOnlyCollection<string> Errors { get; set; }

        [JsonProperty("Number Files Created")]
        public int NumberFilesCreated { get; set; }

        [Header(HeaderKeys.ExtractId)]
        public string ExtractId { get; set; }

        public UploadArchiveResult()
        { }

        public UploadArchiveResult(HttpResponseHeaders headers)
        {
            HeaderParsers.ParseHeaders(this, headers);
        }
    }
}
