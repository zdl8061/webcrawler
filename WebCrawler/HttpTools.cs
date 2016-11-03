﻿using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace WebCrawler
{
    /// <summary>
    /// 请求方法
    /// </summary>
    public enum RequestMethod { GET, POST }

    /// <summary>
    /// HTTP请求
    /// </summary>
    public class HttpTools
    {
        public HttpTools(string url)
            : this(url, Encoding.UTF8) { }

        public HttpTools(string url, Encoding charset)
        {
            this.Url = url;
            this.Timeout = 30000;
            this.Charset = charset;
            this.Headers = new Parameters();
            this.Parameters = new Parameters();
        }

        public int Timeout { get; set; }

        public Encoding Charset = Encoding.UTF8;

        public string Url { get; set; }

        public Parameters Headers { get; private set; }

        public Parameters Parameters { get; private set; }

        #region 方法动作
        /// <summary>
        /// 建立HttpRequest实例
        /// </summary>
        /// <param name="method"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        private HttpWebRequest CreateWebRequest(RequestMethod method, string url)
        {
            var request = CreateRequest(method.ToString(), url, this.Timeout);
            foreach (var p in this.Headers.Items)
            {
                request.Headers.Add(p.Key, p.Value);
            }
            return request;
        }
        /// <summary>
        /// GET请求
        /// </summary>
        /// <returns></returns>
        public string Get(bool isencode=true)
        {
            string queryString = this.Parameters.BuildQueryString(isencode);
            string url = this.Url;
            if (!string.IsNullOrEmpty(queryString))
            {
                url = string.Concat(url, url.IndexOf('?') == -1 ? '?' : '&', queryString);
            }
            var request = this.CreateWebRequest(RequestMethod.GET, url);
            return ReadAllResponseText(request, this.Charset);
        }

        /// <summary>
        /// POST请求
        /// </summary>
        /// <returns></returns>
        public string Post(bool isencode=true)
        {
            var request = this.CreateWebRequest(RequestMethod.POST, this.Url);
            request.ContentType = "application/x-www-form-urlencoded";

            if (this.Parameters.Items.Count != 0)
            {
                string queryString = this.Parameters.BuildQueryString(isencode);
                byte[] data = this.Charset.GetBytes(queryString);
                request.ContentLength = data.Length;

                using (var stream = request.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }
            }
            return ReadAllResponseText(request, this.Charset);
        }

        /// <summary>
        /// POST请求适合单值，不包含键名
        /// </summary>
        /// <returns></returns>
        public string Post(string val)
        {
            var request = this.CreateWebRequest(RequestMethod.POST, this.Url);
            request.ContentType = "application/x-www-form-urlencoded";

            byte[] data = this.Charset.GetBytes(val);
            request.ContentLength = data.Length;

            using (var stream = request.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }

            return ReadAllResponseText(request, this.Charset);
        }

        /// <summary>
        /// 提交文件
        /// </summary>
        /// <param name="files">要提交上传的文件列表</param>
        /// <returns></returns>
        public string PostFile(Files files)
        {
            var request = this.CreateWebRequest(RequestMethod.POST, this.Url);

            string boundary = string.Concat("--", GetBoundary());
            request.ContentType = string.Concat("multipart/form-data; boundary=", boundary);
            request.KeepAlive = true;

            using (MemoryStream ms = new MemoryStream())
            {
                byte[] boundaryData = this.Charset.GetBytes("\r\n--" + boundary + "\r\n");
                if (this.Parameters.Items.Count != 0)
                {
                    //写入参数
                    string parameterData = "Content-Disposition: form-data; name=\"{0}\"\r\n\r\n{1}";
                    foreach (var p in this.Parameters.Items)
                    {
                        string item = string.Format(parameterData, p.Key, p.Value);
                        byte[] data = this.Charset.GetBytes(item);
                        ms.Write(boundaryData, 0, boundaryData.Length);
                        ms.Write(data, 0, data.Length);
                    }
                }

                if (files != null)
                {
                    //写入文件数据
                    string fileData = "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: {2}\r\n\r\n";
                    foreach (var p in files.Items)
                    {
                        if (p.Value != null)
                        {
                            string item = string.Format(fileData, p.Key, p.Value.FileName, p.Value.ContentType);
                            byte[] data = this.Charset.GetBytes(item);
                            ms.Write(boundaryData, 0, boundaryData.Length);
                            ms.Write(data, 0, data.Length);
                            p.Value.WriteTo(ms);
                        }
                    }
                }

                //写入结束线
                boundaryData = this.Charset.GetBytes("\r\n--" + boundary + "--\r\n");
                ms.Write(boundaryData, 0, boundaryData.Length);

                request.ContentLength = ms.Length;
                using (var stream = request.GetRequestStream())
                {
                    ms.WriteTo(stream);
                }
            }

            return ReadAllResponseText(request, this.Charset);
        }

        public static string GetBoundary()
        {
            string str = "abcdefghijklmnopqrstuvwxyz0123456789";
            StringBuilder builder = new StringBuilder();
            Random random = new Random();
            for (int i = 0; i < 10; i++)
            {
                int num2 = random.Next(str.Length);
                builder.Append(str[num2]);
            }
            return builder.ToString();
        }

        /// <summary>
        /// 建立请求
        /// </summary>
        /// <param name="method"></param>
        /// <param name="url"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public static HttpWebRequest CreateRequest(string method, string url, int timeout)
        {
            var uri = new Uri(url);
            if (uri.Scheme.Equals("https", StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    ServicePointManager.ServerCertificateValidationCallback = ValidateAllCertificate;
                }
                catch { }
            }
            var request = (HttpWebRequest)HttpWebRequest.Create(url);
            request.Method = method;
            request.ServicePoint.Expect100Continue = false;
            request.Timeout = timeout;

            return request;
        }

        private static bool ValidateAllCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors policyErrors)
        {
            return true;
        }

        /// <summary>
        /// 读取所有输出的文本数据
        /// </summary>
        /// <param name="request"></param>
        /// <param name="charset"></param>
        /// <returns></returns>
        public static string ReadAllResponseText(HttpWebRequest request, Encoding charset)
        {
            using (WebResponse response = request.GetResponse())
            {
                using (StreamReader reader = new StreamReader(response.GetResponseStream(), charset))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        #endregion

        #region HMACSHA1 签名

        /// <summary>
        /// HMACSHA1加密
        /// </summary>
        /// <param name="key"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string HMACSHA1(string key, string data)
        {
            HMACSHA1 hmacsha1 = new HMACSHA1();
            hmacsha1.Key = Encoding.ASCII.GetBytes(key);

            byte[] dataBuffer = System.Text.Encoding.ASCII.GetBytes(data);
            byte[] hashBytes = hmacsha1.ComputeHash(dataBuffer);

            return Convert.ToBase64String(hashBytes);
        }

        /// <summary>
        /// HMACSHA1 签名
        /// </summary>
        /// <param name="key">签名密钥</param>
        /// <param name="requestMethod">请求模式（Get\Post）</param>
        /// <param name="requestUrl">请求URL</param>
        /// <param name="parameters">请求参数</param>
        /// <returns></returns>
        public static string HMACSHA1Signature(string key, string requestMethod, string requestUrl, Parameters parameters)
        {

            StringBuilder data = new StringBuilder(100);
            data.AppendFormat("{0}&{1}&", requestMethod.ToUpper(), Uri.EscapeDataString(requestUrl));
            //处理参数
            if (parameters != null)
            {
                data.Append(parameters.BuildQueryString(true));
            }
            return HMACSHA1(key, data.ToString());
        }

        /// <summary>
        /// 克隆流
        /// </summary>
        /// <param name="inStream"></param>
        /// <returns></returns>
        public static MemoryStream CloneStream(Stream inStream)
        {
            MemoryStream ms = new MemoryStream();
            byte[] buffer = new byte[128];

            while (true)
            {
                int sz = inStream.Read(buffer, 0, 128);
                if (sz == 0) break;
                ms.Write(buffer, 0, sz);
            }
            ms.Position = 0;
            return ms;
        }

        #endregion

    }

    /// <summary>
    /// 参数
    /// </summary>
    public class Parameters
    {
        /// <summary>
        /// 
        /// </summary>
        public Parameters()
        {
            this.Items = new List<KeyValuePair<string, string>>(10);
        }

        public Parameters(string query)
        {
            this.Items = new List<KeyValuePair<string, string>>(10);
            var nv = System.Web.HttpUtility.ParseQueryString(query);
            foreach (string key in nv.Keys)
            {
                this.Add(key, nv[key]);
            }
        }

        public string this[string key]
        {
            get { return this.Items.SingleOrDefault((kv) => { return kv.Key == key; }).Value; }
        }

        /// <summary>
        /// 参数
        /// </summary>
        public List<KeyValuePair<string, string>> Items
        {
            get;
            private set;
        }
        /// <summary>
        /// 清空参数
        /// </summary>
        /// <returns></returns>
        public void Clear()
        {
            this.Items.Clear();
        }

        /// <summary>
        /// 排序
        /// </summary>
        public void Sort()
        {
            this.Items.Sort(new Comparison<KeyValuePair<string, string>>((x1, x2) =>
            {
                if (x1.Key == x2.Key)
                {
                    return string.Compare(x1.Value, x2.Value);
                }
                else
                {
                    return string.Compare(x1.Key, x2.Key);
                }
            }));
        }

        /// <summary>
        /// 添加查询参数
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void Add(string key, object value)
        {
            this.Add(key, (value == null ? string.Empty : value.ToString()));
        }
        /// <summary>
        /// 添加查询参数
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void Add(string key, string value)
        {
            this.Items.Add(new KeyValuePair<string, string>(key, value));
        }
        /// <summary>
        /// 批量添加集合数据
        /// </summary>
        /// <param name="collection"></param>
        public void AddRange(IEnumerable<KeyValuePair<string, string>> collection)
        {
            if (collection == null) return;
            this.Items.AddRange(collection);
        }

        /// <summary>
        /// 构造查询参数字符串,字符格式如: name1=value1&amp;name2=value2
        /// </summary>
        /// <param name="encodeValue">是否对值进行编码</param>
        /// <returns></returns>
        public string BuildQueryString(bool encodeValue)
        {
            StringBuilder buffer = new StringBuilder();
            foreach (var p in this.Items)
            {
                if (buffer.Length != 0) buffer.Append("&");
                buffer.AppendFormat("{0}={1}", encodeValue ? Uri.EscapeDataString(p.Key) : p.Key, encodeValue ? Uri.EscapeDataString(p.Value) : p.Value);
            }
            return buffer.ToString();
        }

        /// <summary>
        /// 构造字符串,字符串格式如: name1="value1", name2="value2"
        /// </summary>
        /// <param name="encodeValue"></param>
        /// <returns></returns>
        public string BuildString(bool encodeValue)
        {
            StringBuilder buffer = new StringBuilder();
            foreach (var p in this.Items)
            {
                if (buffer.Length != 0) buffer.Append(", ");
                buffer.AppendFormat("{0}=\"{1}\"", encodeValue ? Uri.EscapeDataString(p.Key) : p.Key, encodeValue ? Uri.EscapeDataString(p.Value) : p.Value);
            }
            return buffer.ToString();
        }
    }

    /// <summary>
    /// 文件集
    /// </summary>
    public class Files
    {
        /// <summary>
        /// 
        /// </summary>
        public Files()
        {
            this.Items = new List<KeyValuePair<string, UploadFile>>();
        }

        /// <summary>
        /// 文件集
        /// </summary>
        public List<KeyValuePair<string, UploadFile>> Items
        {
            get;
            private set;
        }
        /// <summary>
        /// 清空所有文件
        /// </summary>
        /// <returns></returns>
        public void Clear()
        {
            this.Items.Clear();
        }
        /// <summary>
        /// 添加文件
        /// </summary>
        /// <param name="name"></param>
        /// <param name="file"></param>
        public void Add(string name, UploadFile file)
        {
            this.Items.Add(new KeyValuePair<string, UploadFile>(name, file));
        }
    }

    /// <summary>
    /// 上传文件
    /// </summary>
    public class UploadFile
    {
        /// <summary>
        /// 根据本地文件地址实例化
        /// </summary>
        /// <param name="fileUri">文件地址，可以为本地文件绝对地址(c:\files\test.jpg)，也可以为网络文件绝对地址(http://www.domain.com/files/test.jpg)</param>
        public UploadFile(string fileUri)
        {
            this.SetFileUri(fileUri);
            this.ContentType = GetContentType(Path.GetExtension(this.FileName));
        }
        /// <summary>
        /// 根据本地文件地址与文件类型实例化
        /// </summary>
        /// <param name="fileUri">文件地址，可以为本地文件绝对地址(c:\files\test.jpg)，也可以为网络文件绝对地址(http://www.domain.com/files/test.jpg)</param>
        /// <param name="contentType">文件类型</param>
        public UploadFile(string fileUri, string contentType)
        {
            this.SetFileUri(fileUri);
            this.ContentType = contentType;
        }
        /// <summary>
        /// 根据文件名、文件类型与文件流实现化
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <param name="contentType">文件类型</param>
        /// <param name="stream">文件数据流</param>
        public UploadFile(string fileName, string contentType, Stream stream)
        {
            this.FileName = fileName;
            this.ContentType = contentType;
            this.FileStream = stream;
        }

        /// <summary>
        /// 文件名称
        /// </summary>
        public string FileName { get; private set; }

        /// <summary>
        /// 文件类型
        /// </summary>
        public string ContentType { get; private set; }

        /// <summary>
        /// 设置文件URI地址
        /// </summary>
        private Uri FileUri = null;
        /// <summary>
        /// 设置文件URI地址
        /// </summary>
        /// <param name="fileUri"></param>
        private void SetFileUri(string fileUri)
        {
            this.FileUri = new Uri(fileUri);
            this.FileName = Path.GetFileName(this.FileUri.AbsolutePath);
        }
        /// <summary>
        /// 文件数据流
        /// </summary>
        private Stream FileStream;

        /// <summary>
        /// 将当前的文件数据写入到某个数据流中
        /// </summary>
        /// <param name="stream"></param>
        public void WriteTo(Stream stream)
        {
            byte[] buffer = new byte[512];
            int size = 0;
            if (this.FileUri != null)
            {
                if (this.FileUri.IsFile)
                {
                    if (File.Exists(this.FileUri.LocalPath))
                    {
                        //写入本地文件流
                        using (System.IO.FileStream reader = new FileStream(this.FileUri.LocalPath, FileMode.Open, FileAccess.Read))
                        {
                            while ((size = reader.Read(buffer, 0, buffer.Length)) > 0)
                            {
                                stream.Write(buffer, 0, size);
                            }
                        }
                    }
                }
                else
                {
                    //网络流
                    using (WebClient client = new WebClient())
                    {
                        client.Headers.Add(HttpRequestHeader.Referer, this.FileUri.OriginalString);
                        try
                        {
                            var data = client.DownloadData(this.FileUri);
                            stream.Write(data, 0, data.Length);
                        }
                        catch { }
                    }
                }
            }
            if (this.FileStream != null)
            {
                //写入文件流
                //this.FileStream.Position = 0;
                while ((size = this.FileStream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    stream.Write(buffer, 0, size);
                }
            }
        }

        /// <summary>
        /// 根据文件扩展名获取文件类型
        /// </summary>
        /// <param name="fileExt">文件扩展名</param>
        /// <returns></returns>
        private string GetContentType(string fileExt)
        {
            string contentType = "application/octetstream";
            if (!string.IsNullOrEmpty(fileExt))
            {
                fileExt = fileExt.ToLower();

                //尝试从注册表中取值
                bool hasError = false;
                try
                {
                    RegistryKey regKey = Registry.ClassesRoot.OpenSubKey(fileExt);
                    if (regKey != null)
                    {
                        object v = regKey.GetValue("Content Type");
                        if (v != null) contentType = v.ToString();
                    }
                }
                catch (SecurityException)
                {
                    hasError = true;
                }
                catch (UnauthorizedAccessException)
                {
                    hasError = true;
                }

                if (hasError || string.IsNullOrEmpty(contentType))
                {
                    //处理常用图片文件的ContentType
                    contentType = GetCommonFileContentType(fileExt);
                }
            }

            return contentType;
        }
        /// <summary>
        /// 获取通用文件的文件类型
        /// </summary>
        /// <param name="fileExt">文件扩展名.如".jpg",".gif"等</param>
        /// <returns></returns>
        private string GetCommonFileContentType(string fileExt)
        {
            switch (fileExt)
            {
                case ".jpg":
                case ".jpeg":
                    return "image/jpeg";
                case ".gif":
                    return "image/gif";
                case ".bmp":
                    return "image/bmp";
                case ".png":
                    return "image/png";
                default:
                    return "application/octetstream";
            }
        }
    }
}
