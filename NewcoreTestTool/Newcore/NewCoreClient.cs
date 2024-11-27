using Newtonsoft.Json;
using NewcoreTestTool.CorePass;
using NewcoreTestTool.Dto;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using static NewcoreTestTool.CorePass.SaveOrUpdateParameter;
using NewcoreTestTool.Common;

namespace NewcoreTestTool
{
    internal class NewCoreClient
    {
        private readonly NewCoreKey _newCoreKey;
        private readonly string _baseUrl;
        private readonly int _normalRefreshMilliseconds = 90 * 60 * 1000;
        private readonly int _errorRefreshMilliseconds = 3 * 60 * 1000;
        private readonly int _defaultConnectionLimit = 30;
        private readonly int _timeout = 15000;

        private XHttpClient _client;

        public NewCoreClient(string baseUrl, NewCoreKey apiKey)
        {
            _newCoreKey = apiKey;
            _baseUrl = baseUrl;
            System.Net.ServicePointManager.DefaultConnectionLimit = _defaultConnectionLimit;
            _client = new XHttpClient()
            {
                _baseUrl = baseUrl,
            };
        }

        //public RestResponse PushTemplateRecord(string url, NewCoreRecord data)
        //{
        //    var request = new RestRequest(url).AddStringBody(TemplateSaveOrUpdateRquest.MakeRequest(data), ContentType.Json);
        //    return _client.PostAsync(request).Result;
        //}

        //public bool PushTemplateRecord(NewCoreRecord record)
        //{
        //    var request = new RestRequest(TemplateUrl.SaveOrUpdate).AddStringBody(TemplateSaveOrUpdateRquest.MakeRequest(record), ContentType.Json);
        //    var response = _client.PostAsync(request).Result;
        //    if (!response.IsSuccessful)
        //    {
        //        _logger.LogError($"打印历史记录推送失败：{JsonConvert.SerializeObject(record)}，原因：{response.ErrorMessage}");
        //        return false;
        //    }

        //    var resp = JsonConvert.DeserializeObject<NewCoreResponse>(response?.Content);
        //    if (resp == null)
        //    {
        //        _logger.LogError($"打印历史记录返回值解析失败：{response?.Content}");
        //        return false;
        //    }

        //    if (resp.code != (int)ReturnCode.Success)
        //    {
        //        _logger.LogError($"历史记录推送失败：{resp.message}");
        //        return false;
        //    }

        //    return true;
        //}
        //public CpsResponse GetCpsRecords(string type)
        //{
        //    var resp = GetCpsResponse(type);
        //    if (null != resp && resp.code == 401)
        //    {
        //        RefreshToken();
        //    }
        //    return GetCpsResponse(type);
        //}
        //public CpsResponse GetCpsResponse(string type)
        //{
        //    CpsParam param = CpsParam.MakeTestCpsParam(type);
        //    string url = "/api/metadata-app/v2/data/query/search";
        //    string content = Newtonsoft.Json.JsonConvert.SerializeObject(param);
        //    var request = _client.CreateRequest(url, XHttpClient.Method.Post, content, true);
        //    XHttpResponseBase response = _client.Execute(request);
        //    if (response.IsSuccess())
        //    {
        //        var respData = Newtonsoft.Json.JsonConvert.DeserializeObject<CpsResponse>(response.Content);
        //        return respData;
        //    }
        //    return null;
        //}

        public bool DownLoadFile(string url, string fileName)
        {
            //string url = "https://img.xinheyun.com/c4b62870-3cde-47f2-830f-a5f07801790b_20569fd93f37d534e4692dde402f9c51?attname=loginfo-T1.txt";
            string filePath = Path.GetFullPath(fileName); // 保存的文件名
            if (!Directory.Exists(Path.GetDirectoryName(filePath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(filePath));
            }

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    using (Stream responseStream = response.GetResponseStream())
                    using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        byte[] buffer = new byte[4096];
                        int bytesRead;
                        while ((bytesRead = responseStream.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            fileStream.Write(buffer, 0, bytesRead);
                        }

                        return true;
                    }
                }
            }
            catch (WebException e)
            {
                if (e.Response != null)
                {
                    using (Stream responseStream = e.Response.GetResponseStream())
                    using (StreamReader reader = new StreamReader(responseStream))
                    {
                        string errorResponse = reader.ReadToEnd();
                        //Console.WriteLine("Error response: " + errorResponse);
                    }
                }
                else
                {
                    //Console.WriteLine("Request failed: " + e.Message);
                }
            }
            return false;
        }

        public bool RefreshToken()
        {
            try
            {
                string content = Newtonsoft.Json.JsonConvert.SerializeObject(_newCoreKey);
                XHttpResponseBase response = _client.Post(NewCoreUrl.UpdateToken, content, false);
                if (response.IsSuccess())
                {
                    var respData = Newtonsoft.Json.JsonConvert.DeserializeObject<TokenResponse<NewCoreToken>>(response.Content);
                    if (respData == null || respData.ret == false || respData.data == null)
                    {
                        //_logger.LogError($"获取Token失败，{resp.errorMessage}");
                        return false;
                    }

                    _client.SetBearerAuthorization(respData.data.accessToken);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public MeiGaoTestFile QueryMeiGaoTestFile(string sn)
        {
            XHttpResponseBase response = _client.Get(NewCoreUrl.MeiGaoTest, new string[] { sn });
            if (response.IsSuccess())
            {
                var result = Newtonsoft.Json.JsonConvert.DeserializeObject<NewCoreResponse<Entity<MeiGaoTestFile>>>(response.Content);
                if (result.IsSuccess() && result.data != null)
                {
                    return result.data.entity;
                }
            }

            return null;
        }

        public List<HardwareSerialNumber> QueryMeiGaoImeiParam(string sn)
        {
            var cpsSearchImeiParam = CpsSearchImeiParam.MakeTestCpsParam(sn);
            var content = JsonConvert.SerializeObject(cpsSearchImeiParam);
            XHttpResponseBase response = _client.Post(NewCoreUrl.CpsSearch, content);
            if (response.IsSuccess())
            {
                var result = JsonConvert.DeserializeObject<NewCoreResponse<ListEntity<Dictionary<string, object>>>>(response.Content);
                if (result.IsSuccess() && result.data != null)
                {
                    var data = result.data.list;
                    List<HardwareSerialNumber> hardwareSerialNumbers = data.Select(item => HardwareSerialNumber.valueOf(item)).ToList();
                    return hardwareSerialNumbers;
                }
            }

            return null;
        }
        public NewCoreResponse<Entity<List<CpsTestFileRespEntity>>> PushTestFileResult(SaveOrUpdateParameter parameter)
        {
            string content = parameter.ConvertToString();
            XHttpResponseBase response = _client.Post(NewCoreUrl.SaveOrUpdate, content);
            if (response.IsSuccess())
            {
                var respData = Newtonsoft.Json.JsonConvert.DeserializeObject<NewCoreResponse<Entity<List<CpsTestFileRespEntity>>>>(response.Content);
                return respData;
            }
            return null;
        }

        public FileUploadInfo GetFileUploadInfo()
        {
            XHttpResponseBase response = _client.Get(NewCoreUrl.FileUpload, new string[] { });
            if (response.IsSuccess())
            {
                var respData = Newtonsoft.Json.JsonConvert.DeserializeObject<TokenResponse<FileUploadInfo>>(response.Content);
                if (respData.ret == true)
                {
                    return respData.data;
                }
                return null;
            }
            return null;
        }

        public FileResponse UpLoadFile(FileUploadInfo info, string file)
        {
            var response = _client.UpLoadFile(info, file);
            if (response.IsSuccess())
            {
                var respData = Newtonsoft.Json.JsonConvert.DeserializeObject<FileResponse>(response.Content);
                return respData;
            }
            return null;
        }
        public CpsFileInfo UpLoadFileAttachments(string file)
        {
            string fullFilePath = Path.GetFullPath(file);
            if (!File.Exists(fullFilePath))
            {
                return null;
            }
            CpsFileInfo cpsFileInfo = new CpsFileInfo();
            var fileUploadInfo = GetFileUploadInfo();
            cpsFileInfo.name = Path.GetFileName(fullFilePath);
            cpsFileInfo.url = fileUploadInfo.url + fileUploadInfo.name + "?attname=" + cpsFileInfo.name;
            cpsFileInfo.type = FileType.TXT;
            cpsFileInfo.size = new FileInfo(fullFilePath).Length;
            var resp = UpLoadFile(fileUploadInfo, file);
            return cpsFileInfo;
        }

        public NewCoreResponse<P> PushCPS<P, T>(T body, string webHookId)
        {
            try
            {
                string jsonStr = JsonConvert.SerializeObject(body);
                XHttpResponseBase response = _client.Post(webHookId, jsonStr);
                if (response.IsSuccess())
                {
                    var resp = JsonConvert.DeserializeObject<NewCoreResponse<P>>(response.Content);
                    if (resp.IsSuccess() && resp.data != null)
                    {
                        return resp;
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                return null;
            }

        }
    }
}
