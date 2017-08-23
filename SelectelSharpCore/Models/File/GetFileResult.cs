using System.Collections.Generic;
using SelectelSharpCore.Headers;
using System.Collections.Specialized;
using System.Net.Http.Headers;

namespace SelectelSharpCore.Models.File
{
    public class GetFileResult : FileInfo
    {                
        public byte[] File { get; set; }

        public GetFileResult(byte[] file, string name, HttpResponseHeaders headers)
        {
            HeaderParsers.ParseHeaders(this, headers);
            this.File = file;
            this.Name = name;
            this.Bytes = file.Length;
        }
    }
}
