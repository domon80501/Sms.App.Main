using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Sms.App.Main
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            return true;
        }

        #region sms_main

        private const String host = "http://yzx.market.alicloudapi.com";
        private const String path = "/yzx/sendSms";
        private const String method = "POST";
        private const String appcode = "d5b52c64fcbe482f8f4ffe8a9f78598c";

        public void SendSms_Main(string mobile, string vCode)
        {
            txtResponse.Text += $"Sms Start:{Environment.NewLine}";
            String querys = $"mobile={mobile}&param=code%3A{vCode}&tpl_id=TP1803033";
            String bodys = "";
            String url = host + path;
            HttpWebRequest httpRequest = null;
            HttpWebResponse httpResponse = null;

            if (0 < querys.Length)
            {
                url = url + "?" + querys;
            }

            if (host.Contains("https://"))
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
                httpRequest = (HttpWebRequest)WebRequest.CreateDefault(new Uri(url));
            }
            else
            {
                httpRequest = (HttpWebRequest)WebRequest.Create(url);
            }
            httpRequest.Method = method;
            httpRequest.Headers.Add("Authorization", "APPCODE " + appcode);
            if (0 < bodys.Length)
            {
                byte[] data = Encoding.UTF8.GetBytes(bodys);
                using (Stream stream = httpRequest.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }
            }
            try
            {
                httpResponse = (HttpWebResponse)httpRequest.GetResponse();
            }
            catch (WebException ex)
            {
                httpResponse = (HttpWebResponse)ex.Response;
            }

            Stream st = httpResponse.GetResponseStream();
            StreamReader reader = new StreamReader(st, Encoding.GetEncoding("utf-8"));

            //txtResponse.Text += $@"
            //StatusCode: {httpResponse.StatusCode}{Environment.NewLine}
            //Method : {httpResponse.StatusCode}{Environment.NewLine}
            //Headers : {httpResponse.Headers}{Environment.NewLine}
            //Method : {httpResponse.StatusCode}{Environment.NewLine}
            //ResponseContent: {reader.ReadToEnd()}{Environment.NewLine}
            //";

            JObject resContent = JsonConvert.DeserializeObject<JObject>(reader.ReadToEnd());
            txtResponse.Text += $"发送结果: {GetResultMsg_Main(resContent["return_code"].ToString())}{Environment.NewLine}";
        }

        public static string GetResultMsg_Main(string status)
        {
            switch (status)
            {
                case "10000":
                    return "参数异常";
                case "10001":
                    return "手机号格式不正确";
                case "10002":
                    return "模板不存在";
                case "10003":
                    return "模板变量不正确";
                case "10004":
                    return "变量中含有敏感词";
                case "10005":
                    return "变量名称不匹配";
                case "10006":
                    return "短信长度过长";
                case "10007":
                    return "手机号查询不到归属地";
                case "10008":
                    return "产品错误";
                case "10009":
                    return "价格错误";
                case "10010":
                    return "重复调用";
                case "99999":
                    return "系统错误";
                case "00000":
                    return "调用成功";
                default:
                    return "无法判断结果";
            }
        }
        #endregion

        #region sms_sub

        private const String host_sub = "http://smsmsgs.market.alicloudapi.com";
        private const String path_sub = "/smsmsgs";
        private const String method_sub = "GET";
        private const String appcode_sub = "d5b52c64fcbe482f8f4ffe8a9f78598c";

        public void SendSms_Sub(string mobile, string vCode)
        {
            txtResponse.Text += $"Sms Start:{Environment.NewLine}";
            String querys = $"param={vCode}&phone={mobile}&sign=40741&skin=9027";
            String bodys = "";
            String url = host_sub + path_sub;
            HttpWebRequest httpRequest = null;
            HttpWebResponse httpResponse = null;

            if (0 < querys.Length)
            {
                url = url + "?" + querys;
            }

            if (host.Contains("https://"))
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
                httpRequest = (HttpWebRequest)WebRequest.CreateDefault(new Uri(url));
            }
            else
            {
                httpRequest = (HttpWebRequest)WebRequest.Create(url);
            }
            httpRequest.Method = method_sub;
            httpRequest.Headers.Add("Authorization", "APPCODE " + appcode_sub);
            if (0 < bodys.Length)
            {
                byte[] data = Encoding.UTF8.GetBytes(bodys);
                using (Stream stream = httpRequest.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }
            }
            try
            {
                httpResponse = (HttpWebResponse)httpRequest.GetResponse();
            }
            catch (WebException ex)
            {
                httpResponse = (HttpWebResponse)ex.Response;
            }

            Stream st = httpResponse.GetResponseStream();
            StreamReader reader = new StreamReader(st, Encoding.GetEncoding("utf-8"));            

            JObject resContent = JsonConvert.DeserializeObject<JObject>(reader.ReadToEnd());
            txtResponse.Text += $"发送结果: {resContent["Message"].ToString()}{Environment.NewLine}";
        }
        
        #endregion

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            var RandomNum = new Random().Next(1, 9999).ToString().PadLeft(4, '0');
            var Mobile = txtMobile.Text.Trim();

            if (cboxApi.SelectedIndex == 0)
            {
                SendSms_Main(Mobile, RandomNum);
            }
            else if (cboxApi.SelectedIndex == 1)
            {
                SendSms_Sub(Mobile, RandomNum);
            }
        }
    }
}
