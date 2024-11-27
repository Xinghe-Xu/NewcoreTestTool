using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewcoreTestTool.Common
{
    public class Result
    {
        public bool Success { get; set; }
        public string? Message { get; set; }

        public static Result CreateSuccess(string? msg = null)
        {
            return new Result
            {
                Success = true,
            };
        }

        public static Result CreateFailure(string msg)
        {
            return new Result
            {
                Success = false,
                Message = msg,
            };
        }

        public static Result From(Result other)
        {
            return new Result
            {
                Success = other.Success,
                Message = other.Message,
            };
        }
    }

    public class Result<T> : Result
    {
        public T? Data { get; set; }

        public static Result<T> CreateSuccess(T data, string? msg = null)
        {
            return new Result<T>
            {
                Success = true,
                Data = data,
            };
        }

        public static Result<T> CreateFailure(string msg)
        {
            return new Result<T>
            {
                Success = false,
                Message = msg,
            };
        }

    }
}
