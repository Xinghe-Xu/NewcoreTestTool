using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using static NewcoreTestTool.CpsSearchImeiParam;

namespace NewcoreTestTool
{
    public class NewCoreResponse
    {
        public int code { get; set; }
        public string message { get; set; }
        public bool IsSuccess()
        {
            return code == (int)HttpStatusCode.OK;
        }
    }

    public class NewCoreResponse<T> : NewCoreResponse
    {
        public T data { get; set; }
    }

    public class Entity<T>
    {
        public T entity { get; set; }
    }

    public class ListEntity<T>
    {
        public List<T> list { get; set; }
    }

}
