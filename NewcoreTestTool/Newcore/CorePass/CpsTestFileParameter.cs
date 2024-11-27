using NewcoreTestTool.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Web;
using static NewcoreTestTool.CorePass.SaveOrUpdateParameter;

namespace NewcoreTestTool.CorePass
{
    public class TestItem
    {
        public string inspectItem { get; set; }
        public string qualified { get; set; }
    }
    public static class TestFileConstant
    {
        public static string TESTFILE_TEMPLATEAPINAME = "MeiGao_sn_Inspect_jbk";
        public static string TESTFILE_CHILD_TEMPLATEAPINAME = "inspectCard";

        public static string STATUS_PROCESS_PASS = "1";
        public static string STATUS_PROCESS_NO_PASS = "2";
        public static string STATUS_FILE_PASS = "1";
        public static string STATUS_FILE_NO_PASS = "0";

        public static string SN = "sn";
        public static string PROCEDURE = "procedure";
        public static string STATUS = "status";
        public static string ATTACHMENTS = "field_attachments__c";

        public static string TEST_ITEM_SCRIPT = "脚本检测";
        public static string TEST_ITEM_MAC = "MACS";
    }

    public class CpsTestFileParameter
    {
        public static string _templateApiName = "MeiGao_sn_Inspect_jbk";

        public static SaveOrUpdateParameter CreateSaveParameter(string sn, string processName, string status, List<Dictionary<string, object>> dicts, List<CpsFileInfo> attachments)
        {
            SaveOrUpdateParameter saveOrUpdateParameter = new SaveOrUpdateParameter();
            var parentInfo = new ParentInfo() { templateApiName = TestFileConstant.TESTFILE_TEMPLATEAPINAME, id = null };

            Content content = new Content();
            SaveContent saveContent = new SaveContent();
            saveContent.AddValue(new KeyValue()
                .Add(TestFileConstant.SN, sn)
                .Add(TestFileConstant.PROCEDURE, processName)
                .Add(TestFileConstant.STATUS, status)
                .AddArray(TestFileConstant.ATTACHMENTS, attachments))
                .AddChildInfo(dicts.Select(item => new ChildInfo()
                {
                    templateApiName = TestFileConstant.TESTFILE_CHILD_TEMPLATEAPINAME,
                    parent = parentInfo,
                    value = new KeyValue().Add(item)
                }).ToList()
                );
            content.AddSaveContent(saveContent);

            return MakeBaseRequest(TestFileConstant.TESTFILE_TEMPLATEAPINAME).AddContent(content);
        }

        public static SaveOrUpdateParameter CreateUpdateParameter(string recordId, string status, List<Dictionary<string, object>> dicts)
        {
            SaveOrUpdateParameter saveOrUpdateParameter = new SaveOrUpdateParameter();

            Content content = new Content();
            UpdateContent updateContent = new UpdateContent();
            var parentInfo = new ParentInfo() { templateApiName = TestFileConstant.TESTFILE_TEMPLATEAPINAME, id = recordId };
            updateContent.AddValue(new KeyValue().Add(TestFileConstant.STATUS, status))
                .AddId(recordId)
                .AddChildInfo(dicts.Select(item => new ChildInfo()
                {
                    templateApiName = TestFileConstant.TESTFILE_CHILD_TEMPLATEAPINAME,
                    parent = parentInfo,
                    value = new KeyValue().Add(item)
                }).ToList()
                );

            content.AddUpdateContent(updateContent);
            return MakeBaseRequest(TestFileConstant.TESTFILE_TEMPLATEAPINAME).AddContent(content);
        }
    }
}
