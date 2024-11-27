using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace NewcoreTestTool.CorePass
{
    public class SaveOrUpdateParameter
    {
        public JObject Data { get; set; }

        public string ConvertToString()
        {
            return Data.ToString();
        }

        public static SaveOrUpdateParameter MakeBaseRequest(string templateApiName)
        {
            JObject body = new JObject();

            // 表单的ApiName
            body["templateApiName"] = templateApiName;
            // "contents"
            body["contents"] = null;
            // 是否异步，目前该功能并未开放，默认为同步
            body["async"] = false;

            // 审核流操作类型：
            // 		SAVE 						= 保存
            // 		SUBMIT 					= 提交
            // 		SAVE_AND_SUBMIT = 保存并提交
            body["workflowInitOperation"] = "SAVE";

            // 功能特性开关：
            // 接口支持通过开关控制某些功能特性，默认情况下，请不要填写这个参数，即所有功能处于开启状态
            // 【PS】：除非你知道自己在做什么，否则不要管这个参数，让它用默认值。
            // 以下是接口支持的功能，通过参数true/false可以开启或禁用某一功能：
            // 		AUTO_CODEC 					= 自动生成编号
            // 		CONSTRAINT_VALID 		= 约束校验
            // 		FORMAT_CHECK 				= 数据格式检查
            // 		CORE_BI_SYNC 				= CoreBI数据同步
            // 		AUDIT_TRAIL 				= 操作审计（单据日志）
            // 		NOTIFICATION 				= 单据消息通知
            // 		FLOW 								= 触发工作流
            // 		WORKFLOW 						= 触发审核流
            JObject features_object = new JObject();
            features_object["WORKFLOW"] = false;
            body["features"] = features_object;

            JObject data = new JObject();
            data["body"] = body;

            return new SaveOrUpdateParameter { Data = data};
        }

        public SaveOrUpdateParameter AddContent(Content content)
        {
            Data["body"]["contents"] = content.Data;
            return this;
        }


        public class Content
        {
            public JArray Data { get; set; } = new JArray();

            public Content AddSaveContent(SaveContent saveContent)
            {
                Data.Add(saveContent.Data);
                return this;
            }

            public Content AddUpdateContent(UpdateContent updateContent)
            {
                Data.Add(updateContent.Data);
                return this;
            }

        }

        public class SaveContent
        {
            public JObject Data { get; set; } = new JObject();
            public SaveContent AddValue(KeyValue value)
            {
                Data["value"] = value.Data;
                return this;
            }

            public SaveContent AddChildInfo(ChildInfo child)
            {
                if (Data["children"] == null)
                {
                    Data["children"] = new JObject();
                    Data["children"][child.templateApiName] = new JArray();
                }

                JArray arr = (JArray)Data["children"][child.templateApiName];
                arr.Add(child.GetChildInfo());
                return this;
            }

            public SaveContent AddChildInfo(List<ChildInfo> childs)
            {
                if (Data["children"] == null)
                {
                    Data["children"] = new JObject();
                    Data["children"][childs[0].templateApiName] = new JArray();
                }

                JArray arr = (JArray)Data["children"][childs[0].templateApiName];
                foreach(var item in childs)
                {
                    arr.Add(item.GetChildInfo());
                }
                return this;
            }
        }

        public class UpdateContent
        {
            public JObject Data { get; set; } = new JObject();

            public UpdateContent AddValue(KeyValue value)
            {
                Data["value"] = value.Data;
                return this;
            }

            public UpdateContent AddId(string id)
            {
                Data["id"] = id == null ? null : id;
                return this;
            }

            public UpdateContent AddChildInfo(ChildInfo child)
            {
                if (Data["children"] == null)
                {
                    Data["children"] = new JObject();
                    Data["children"][child.templateApiName] = new JArray();
                }

                JArray arr = (JArray)Data["children"][child.templateApiName];
                arr.Add(child.GetChildInfo());
                return this;
            }

            public UpdateContent AddChildInfo(List<ChildInfo> childs)
            {
                if (Data["children"] == null)
                {
                    Data["children"] = new JObject();
                    Data["children"][childs[0].templateApiName] = new JArray();
                }

                JArray arr = (JArray)Data["children"][childs[0].templateApiName];
                foreach (var item in childs)
                {
                    arr.Add(item.GetChildInfo());
                }
                return this;
            }
        }

        public class KeyValue
        {
            public JObject Data { get; set; } = new JObject();
            public KeyValue Add(Dictionary<string, object> dict)
            {
                if (dict != null)
                {
                    foreach (var item in dict)
                    {
                        Data[item.Key] = item.Value == null ? null : (string)item.Value;
                    }
                }

                return this;
            }

            public KeyValue Add(string key, JObject value)
            {
                Data[key] = value;
                return this;
            }

            public KeyValue Add(string key, string value)
            {
                Data[key] = value;
                return this;
            }

            public KeyValue AddArray(string key, object value)
            {
                Data[key] = value == null ? null : JArray.FromObject(value);
                return this;
            }
        }
        public class ParentInfo
        {
            public string templateApiName { get; set; }
            public string id { get; set; }
        }

        public class ChildInfo
        {
            public string templateApiName { get; set; }
            public ParentInfo parent {get; set;}
            public KeyValue value { get; set; }

            public JObject GetChildInfo()
            {
                JObject data = new JObject();
                data["parent"] = parent == null ? null : JObject.FromObject(parent);
                data["value"] = value == null ? null : value.Data;
                return data;
            }
        }
    }
}