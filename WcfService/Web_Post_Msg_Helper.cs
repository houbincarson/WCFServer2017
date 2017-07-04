using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

namespace WcfService
{
  public class Web_Post_Msg_Helper
  {
    public static void Send_MSG_By_QA_DB(string[][] msgs)
    {
      for (int _s = 0, _sCnt = msgs.Length; _s < _sCnt; _s++)
      {
        if (msgs[_s].Length < 2)
        {
          continue;
        }
        SendMsg(
                        msgs[_s][0],
                        msgs[_s][1],
                        System.DateTime.Now.ToLocalTime().ToString());

      }
    }



    public static string SendMsg(string Phone, string Text, string sendtime)
    {
      string CORPID = "yt88731038";
      string CPPW = "yt88731038";
      for (int i = 0; i < (Text.Length / 140) + 1; i++)
      {
        string webInfo = string.Empty;
        System.Text.StringBuilder sburl = new System.Text.StringBuilder();
        sburl.Append("http://")
          //.Append(IP)  暂时写死，如需要传值，注释下列，并且把这行注释去掉
            .Append("60.209.7.15:8080/smsServer/submit?CORPID=")//修改 BY Grart
            .Append(CORPID)
            .Append("&CPPW=")
            .Append(MD5Encrypt(CPPW))
            .Append("&PHONE=")
            .Append(Phone)
            .Append("&SENDTIME=")
            .Append(sendtime)
            .Append("&CONTENT=")
            .Append(URLEncoding(Text.Substring(i * 140, i == (Text.Length / 140) ? (Text.Length - i * 140) : 140)));

        Submit(sburl.ToString(), ref webInfo);
      }
      return "SUCCESS";
    }
    public static string MD5Encrypt(string strText)
    {
      return System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(strText, "MD5").ToLower();
    }
    public static string URLEncoding(string str)
    {
      return System.Web.HttpUtility.UrlEncode(str, System.Text.Encoding.GetEncoding("UTF-8"));
    }
    public static bool Submit(string Url, ref string webInfo)
    {
      bool flag = true;
      try
      {
        System.Net.WebRequest request = System.Net.WebRequest.Create(Url);
        request.Method = "POST";

        request.GetRequestStream().Close();
        System.Net.WebResponse response = request.GetResponse();
        System.IO.StreamReader sr = new System.IO.StreamReader(response.GetResponseStream(), System.Text.Encoding.UTF8);
        webInfo = sr.ReadToEnd();
        sr.Close();
      }
      catch
      {
        flag = false;
      }
      return flag;
    }
  }
}
