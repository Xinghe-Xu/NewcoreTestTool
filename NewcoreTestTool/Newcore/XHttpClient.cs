using NewcoreTestTool.Dto;
using System;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace NewcoreTestTool
{
    internal class XHttpClient
    {
        public enum Method
        {
            Get = 0, Post, Put, Delete, Head, Options,
            Patch, Merge, Copy, Search
        }

        private static string[] _methodMap = { "GET", "POST", "PUT", "DELETE", "HEAD", "OPTIONS", "PATCH", "MERGE", "COPY", "SEARCH" };

        public string _baseUrl = string.Empty;
        private string _authorization = string.Empty;

        private ConfigReader _configReader = new ConfigReader();

        public void SetBearerAuthorization(string key)
        {
            _authorization = "Bearer " + key;
        }

        public XHttpClient() { }

        public XHttpResponseBase Post(string resource, string content, bool useAuthorization = true)
        {
            var request = CreateRequest(resource, Method.Post, content, useAuthorization);
            return Execute(request);
        }

        public XHttpResponseBase Get(string resource, string[] content, bool useAuthorization = true)
        {
            string realUrl = string.Format(resource, content);
            var request = CreateRequest(realUrl, Method.Get, null, useAuthorization);
            return Execute(request);
        }
        public XHttpResponseBase UpLoadFile(FileUploadInfo info, string file)
        {
            var request = CreateUpLoadFileRequest(info, file);
            return Execute(request);
        }

        public HttpWebRequest CreateRequest(string resource, Method method, string content, bool useAuthorization = true)
        {
            string fullUrl = _baseUrl + resource;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(fullUrl);
            request.Method = _methodMap[(int)method];
            request.ContentType = "application/json";
            if (useAuthorization && !string.IsNullOrEmpty(_authorization))
            {
                request.Headers.Add("Authorization", _authorization);
            }

            if (string.IsNullOrEmpty(fullUrl))
            {
                return null;
            }

            if (fullUrl.StartsWith("https", StringComparison.OrdinalIgnoreCase))
            {
                //关于ServicePointManager.SecurityProtocol的设置是解决问题的关键。
                //ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Ssl3;
                //.Net4.0
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
                ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
            }

            if (content != null)
            {
                using (Stream requestStream = request.GetRequestStream())
                {
                    byte[] data = System.Text.Encoding.UTF8.GetBytes(content);
                    requestStream.Write(data, 0, data.Length);
                }
            }

            return request;
        }

        private static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            return true; //总是接受 
        }

        public XHttpResponseBase Execute(HttpWebRequest request)
        {
            XHttpResponseBase responseBase = new XHttpResponseBase();
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                responseBase.Code = response.StatusCode;
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    using (Stream responseStream = response.GetResponseStream())
                    {
                        using (StreamReader reader = new StreamReader(responseStream))
                        {
                            string responseData = reader.ReadToEnd();
                            responseBase.Content = responseData;
                        }
                    }
                }
            }

            return responseBase;
        }

        public HttpWebRequest CreateUpLoadFileRequest(FileUploadInfo info, string file)
        {
            string url = info.host;
            if (string.IsNullOrWhiteSpace(info.host) || !info.host.StartsWith("http"))
            {
                url = _baseUrl + info.host;
            }
            
            string filePath = Path.GetFullPath(file);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.Headers.Add("authority", "upload.qiniup.com");
            request.Headers.Add("Authorization", _authorization);

            // 添加其他头部信息...
            string boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x");
            request.ContentType = "multipart/form-data; boundary=" + boundary;

            using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            using (Stream requestStream = request.GetRequestStream())
            using (StreamWriter writer = new StreamWriter(requestStream))
            {
                string header = string.Format("--" + boundary + "\r\nContent-Disposition: form-data; name=\"key\"\r\n\r\n{0}\r\n", info.name);
                writer.Write(header);
                writer.Flush();

                header = string.Format("--" + boundary + "\r\nContent-Disposition: form-data; name=\"token\"\r\n\r\n{0}\r\n", info.token);
                writer.Write(header);
                writer.Flush();

                header = "--" + boundary + "\r\nContent-Disposition: form-data; name=\"file\"; filename=\"" + Path.GetFileName(filePath) + "\"\r\nContent-Type: application/octet-stream\r\n\r\n";
                writer.Write(header);
                writer.Flush();

                byte[] buffer = new byte[4096];
                int bytesRead;
                while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
                {
                    requestStream.Write(buffer, 0, bytesRead);
                }

                writer.Write("\r\n--" + boundary + "--");
                writer.Flush();
            }

            return request;
        }

    }
}
