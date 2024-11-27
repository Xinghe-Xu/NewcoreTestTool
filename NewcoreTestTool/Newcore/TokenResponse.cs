namespace NewcoreTestTool
{
    internal class TokenResponse<T>
    {
        public bool ret { get; set; }
        public T data { get; set; }
        public int errorCode { get; set; }
        public string errorMsg { get; set; }
        public int recordsTotal { get; set; }
    }
}
