using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NewcoreTestTool.Dto
{
    public class FileUploadInfo
    {
        public string name { get; set; }
        public string host { get; set; }
        public string url { get; set; }
        public string token { get; set; }

    }

    public class FileResponse
    {
        public string hash { get; set; }
        public string key { get; set; }
    }

    public class CpsFileInfo
    {
        public string name { get; set; }
        public string url { get; set; }
        public string type { get; set; }
        public long size { get; set; }
        public string uid { get; set; }
        public string linkProps { get; set; }

    }
    public static class FileType
    {
        public static string TXT = "text/plain";
    }
}
