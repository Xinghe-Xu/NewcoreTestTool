namespace NewcoreTestTool
{
    internal class CpsResponse
    {
        public int code { get; set; }
        public string message { get; set; }
        public Data data { get; set; }
    }

    public class Data
    {
        public CpsRecord[] list { get; set; }
        public int start { get; set; }
        public int length { get; set; }
        public int recordsTotal { get; set; }
    }

    public class CpsRecord
    {
        public long updatedTime { get; set; }
        public string code { get; set; }
        public Updatedby updatedBy { get; set; }
        public Field_Vcacd__C[] field_vCacd__c { get; set; }
        public Auditor auditor { get; set; }
        public string field_zxUud__c { get; set; }
        public string audit_status { get; set; }
        public string field_ECkVJ__c { get; set; }
        public int version { get; set; }
        public long audit_time { get; set; }
        public Createdby createdBy { get; set; }
        public long createdTime { get; set; }
        public string id { get; set; }
        public string snowflakeId { get; set; }
    }

    public class Updatedby
    {
        public string id { get; set; }
    }

    public class Auditor
    {
        public string id { get; set; }
    }

    public class Createdby
    {
        public string id { get; set; }
    }

    public class Field_Vcacd__C
    {
        public string name { get; set; }
        public string url { get; set; }
        public string type { get; set; }
        public int size { get; set; }
        public string uid { get; set; }
        public string linkProps { get; set; }
    }
}
