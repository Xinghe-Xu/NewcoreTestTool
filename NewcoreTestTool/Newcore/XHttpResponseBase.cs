using System.Net;

namespace NewcoreTestTool
{
    internal class XHttpResponseBase
    {
        public XHttpResponseBase()
        {
            Code = HttpStatusCode.NotFound;
        }

        public bool IsSuccess()
        {
            return Code == HttpStatusCode.OK;
        }
        public HttpStatusCode Code { get; set; }
        public string Content { get; set; }
    }
}
