using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Web;
using DOL.Core;
using System.Net;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

namespace DOL.UTest
{
    [TestClass]
    public class WebUnitTest2
    {
        [TestMethod]
        public void TestMethod1()
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://218.85.65.43:28000/FJWeb/Web/Studytimeselect.aspx");
            request.Method = "GET";

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream stream = response.GetResponseStream();
            StreamReader reader = new StreamReader(stream);
            string resultPage = reader.ReadToEnd();
            string cookie = response.Headers.Get("Set-Cookie");
            string seesionId = cookie.Replace("ASP.NET_SessionId=", "").Split(';')[0];

            //var cookie = new CookieContainer();
            
            CookieContainer cookies = new CookieContainer();
            string postStr = "";
           // var data=new string[6] { }
            //foreach (string key in form.Keys)
            //{
            //    postStr += key + "=" + form[key] + "&";
            //}
            postStr += "drop_cx=C1&txt_xybh_search=&btn_search=查询&txt_sfzh_search=&txt_xyxm_search=刘城熙";
            byte[] postData = Encoding.ASCII.GetBytes(postStr.Substring(0, postStr.Length - 1));
             request = (HttpWebRequest)WebRequest.Create("http://218.85.65.43:28000/FJWeb/Web/Studytimeselect.aspx");
            request.Method = "POST";
            request.AllowAutoRedirect = false;
            request.ContentType = "application/x-www-form-urlencoded";
            request.CookieContainer = new CookieContainer();
            request.Connection = "keep-alive";
            request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
            request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/58.0.3029.96 Safari/537.36";
            request.ContentLength = postData.Length;
            request.CookieContainer.Add(new Cookie("ASP.NET_SessionId", seesionId,"/", "218.85.65.43"));
            Stream requestStream = request.GetRequestStream();
            requestStream.Write(postData, 0, postData.Length);
            requestStream.Close();

             response = (HttpWebResponse)request.GetResponse();
            Stream responseStream = response.GetResponseStream();
            reader = new StreamReader(responseStream);

            resultPage = reader.ReadToEnd();
            reader.Close();
            responseStream.Close();
          

        }
        private string GetCookieValue(string cookie)
        {
            Regex regex = new Regex("=.*?;");
            Match value = regex.Match(cookie);
            string cookieValue = value.Groups[0].Value;
            return cookieValue.Substring(1, cookieValue.Length - 2);
        }

        private string GetCookieName(string cookie)
        {
            Regex regex = new Regex("ASP.NET_SessionId.*?");
            Match value = regex.Match(cookie);
            return value.Groups[0].Value;
        }

        private string getHtml(string name, string value)
        {
            CookieCollection cookies = new CookieCollection();
            cookies.Add(new Cookie(name, value));
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://218.85.65.43:28000/FJWeb/Web/Studytimeselect.aspx");
            request.Method = "GET";
            request.Headers.Add("Cookie", name + "=" + value);

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream stream = response.GetResponseStream();
            StreamReader reader = new StreamReader(stream, Encoding.UTF8);
            return reader.ReadToEnd();
        }
    }
}
