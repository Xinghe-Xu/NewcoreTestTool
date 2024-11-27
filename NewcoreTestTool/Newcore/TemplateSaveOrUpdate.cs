using Newtonsoft.Json.Linq;

namespace NewcoreTestTool
{
    public class TemplateSaveOrUpdateRquest
    {
        //public string BuildJson()
        //{
        //    JObject valueObject = new JObject();
        //    valueObject["field_vzCDS__c"] = 1690905600000;
        //    valueObject["field_ZfJiX__c"] = "这是一条文本";
        //    valueObject["field_esHlx__c"] = 10;
        //    valueObject["field_BNYIE__c"] = "2023/08/10 17:11";

        //    JObject parentObject = new JObject();
        //    parentObject["templateApiName"] = "template_hZxc6";
        //    parentObject["id"] = "648a56a4e2e9927aaa37efd5";

        //    JObject contentObject = new JObject();
        //    contentObject["parent"] = parentObject;
        //    contentObject["value"] = valueObject;

        //    JArray contentsArray = new JArray();
        //    contentsArray.Add(contentObject);

        //    JObject bodyObject = new JObject();
        //    bodyObject["templateApiName"] = "template_It7XC";
        //    bodyObject["contents"] = contentsArray;
        //    bodyObject["async"] = false;
        //    bodyObject["workflowInitOperation"] = "SAVE";

        //    JObject jsonObject = new JObject();
        //    jsonObject["body"] = bodyObject;

        //    return jsonObject.ToString();
        //}

        public static string MakeRequest(NewCoreRecord record)
        {
            JObject operatorObject = new JObject();
            operatorObject["id"] = record.OperatorID;

            JObject valueObject = new JObject();
            valueObject["field_vzCDS__c"] = record.Type;
            valueObject["field_ZfJiX__c"] = record.Status;
            valueObject["field_esHlx__c"] = record.Copies;
            valueObject["field_BNYIE__c"] = record.DateTime;
            valueObject["field_Lnb6Y__c"] = operatorObject;

            JObject parentObject = new JObject();
            parentObject["templateApiName"] = "template_hZxc6";
            parentObject["id"] = record.RecordId;

            JObject contentObject = new JObject();
            contentObject["parent"] = parentObject;
            contentObject["value"] = valueObject;

            JArray contentsArray = new JArray();
            contentsArray.Add(contentObject);

            JObject bodyObject = new JObject();
            bodyObject["templateApiName"] = "template_It7XC";
            bodyObject["contents"] = contentsArray;
            bodyObject["async"] = false;
            bodyObject["workflowInitOperation"] = "SAVE";

            JObject jsonObject = new JObject();
            jsonObject["body"] = bodyObject;

            return jsonObject.ToString();
        }

    }
}
