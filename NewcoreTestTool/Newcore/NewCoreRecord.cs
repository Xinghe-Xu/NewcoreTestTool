namespace NewcoreTestTool
{
    public static class NewCoreRecordStatus
    {
        public static string Success = @"成功";
        public static string Failure = @"失败";
    }
    public class NewCoreRecord
    {
        public string RecordId { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }
        public int Copies { get; set; }
        public long DateTime { get; set; }
        public string OperatorID { get; set; }
    }
}
