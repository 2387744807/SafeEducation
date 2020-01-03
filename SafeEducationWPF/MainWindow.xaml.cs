using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SafeEducationWPF
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            
        }

        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            //https://mcwk.mycourse.cn/course/A25003/A25003.html?userCourseId=d3847128-057b-4561-9ee9-649d01c45731&tenantCode=35110001&type=1
            string[] strCourseId = CoreFun.GetCourses(txtUrl.Text);
            for(int i = 0; i < strCourseId.Length; i++)
            {
                //if (i >= 10) { MessageBox.Show("hi");return; }
                CoreFun core = new CoreFun();
                
                //执行访问并返回结果
                bool result = false;
                try
                {

                    int type = rBtn1.IsChecked == true ? 1 : 2;
                    core.SetSubmitUrl(type,strCourseId[i]);
                    result = core.WebRequestByGet();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("发生错误：\n" + ex.Message);
                }

                if (!result)
                {
                    MessageBox.Show("完成失败！请检查输入是否正确");
                    return;
                }
            }
            MessageBox.Show("完成成功！");
        }
    }
    public class CoreFun
    {
        //public string url;
        private string SubmitUrl;

        public void SetSubmitUrl(int type,string courseId)
        {
            if (type == 1)
            {
                SubmitUrl = "https://weiban.mycourse.cn/pharos/usercourse/finish.do?callback=jQuery&userCourseId="
                    + courseId + "&tenantCode=35110001&type=1";
            }
            else
            {
                
                SubmitUrl = "https://weiban.mycourse.cn/pharos/usercourse/finish.do?callback=jQuery16406168984105384079_1578025156430&userCourseId="
                    + courseId + "&tenantCode=35110001&type=1&_="+ ConvertDateTimeToInt(DateTime.Now);
            }
        //https://weiban.mycourse.cn/pharos/usercourse/finish.do?callback=jQuery1640307857843812388_1578023347609&userCourseId=f320d36f-c180-420b-89f2-178aa85ef7ae&tenantCode=35110001&_=1578023555806
        //https://weiban.mycourse.cn/pharos/usercourse/finish.do?callback=jQuery1640307857843812388_1578023347610&userCourseId=f320d36f-c180-420b-89f2-178aa85ef7ae&tenantCode=35110001&_=1578023632644
        }
        public static long ConvertDateTimeToInt(DateTime time)
        {
            DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1, 0, 0, 0, 0));
            long t = (time.Ticks - startTime.Ticks) / 10000;   //除10000调整为13位      
            return t;
        }

        //正则方法
        private static string GetBetween(string text, string before, string after)
        {
            try
            {
                Regex rg = new Regex("(?<=(" + before + "))[.\\s\\S]*?(?=(" + after + "))", RegexOptions.Multiline | RegexOptions.Singleline);
                return rg.Match(text).Value;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        
        //Get访问Url
        public bool WebRequestByGet()
        {
            try
            {
                WebRequest request = WebRequest.Create(SubmitUrl);
                WebResponse response = request.GetResponse();
                Stream s = response.GetResponseStream();
                StreamReader sr = new StreamReader(s, Encoding.GetEncoding("utf-8"));
                string result = sr.ReadToEnd();

                //释放资源
                sr.Dispose();
                sr.Close();
                s.Dispose();
                s.Close();
                //获取msg
                if (AnalysisJson(GetBetween(result, "\\(", "\\)")) == "ok")
                {
                    return true;
                }
                else { return false; }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        //json解析
        private string AnalysisJson(string json)
        {
            try
            {
                JObject jobj = (JObject)JsonConvert.DeserializeObject(json);
                return jobj["msg"].ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public static string[] GetCourses(string submitInfo)
        {
            List<string> strResult = new List<string>(); 
            try
            {
                WebRequest request = WebRequest.Create("https://weiban.mycourse.cn/pharos/usercourse/listCourse.do?timestamp=1578017803"
                    + "&" + submitInfo);
                WebResponse response = request.GetResponse();
                Stream s = response.GetResponseStream();
                StreamReader sr = new StreamReader(s, Encoding.GetEncoding("utf-8"));
                string result = sr.ReadToEnd();

                //释放资源
                sr.Dispose();
                sr.Close();
                s.Dispose();
                s.Close();
                //获取msg
                JObject jobj = (JObject)JsonConvert.DeserializeObject(result);
                int categoryNum = jobj["data"].Count();
                for(int i = 0; i < categoryNum; i++)
                {
                    int courseNum = int.Parse(jobj["data"][i]["totalNum"].ToString());
                    for(int j = 0; j < courseNum; j++)
                    {
                        strResult.Add(jobj["data"][i]["courseList"][j]["userCourseId"].ToString());
                        string strStudyUrl = "https://weiban.mycourse.cn/pharos/usercourse/study.do?timestamp=1578026724" +
                            "&userProjectId=" + StrongString.Between(submitInfo, "userProjectId=", "&") +
                            "&courseId=" + jobj["data"][i]["courseList"][j]["resourceId"] +
                            "&tenantCode=35110001" +
                            "&userId=" + StrongString.Between(submitInfo, "userId=", "&") +
                            "&token=" + StrongString.GetRight(submitInfo, "token=");
                        WebRequest request2 = WebRequest.Create(strStudyUrl);
                        WebResponse response2 = request2.GetResponse();
                        
                        Console.WriteLine(strStudyUrl);
                        Console.WriteLine(new StreamReader(response2.GetResponseStream(), Encoding.GetEncoding("utf-8")).ReadToEnd());
                    }
                }

                return strResult.ToArray();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }

    class StrongString
    {
        public static string GetRight(string str, string s, bool ignoreCase = true)
        {
            try
            {
                int index;
                if (ignoreCase == true)
                {
                    index = str.ToLower().IndexOf(s.ToLower());
                }
                else
                {
                    index = str.IndexOf(s);
                }

                if (index == -1)
                {
                    return "";
                }
                string temp = str.Substring(index + s.Length, str.Length - s.Length - index);
                return temp;
            }
            catch
            {
                return "";
            }

        }

        /// <summary>
        /// 取文本中间内容
        /// </summary>
        /// <param name="str">原文本</param>
        /// <param name="leftstr">左边文本</param>
        /// <param name="rightstr">右边文本</param>
        /// <returns>返回中间文本内容</returns>
        public static string Between(string str, string leftstr, string rightstr)
        {
            try
            {
                int i = str.IndexOf(leftstr) + leftstr.Length;
                string temp = str.Substring(i, str.IndexOf(rightstr, i) - i);
                return temp;
            }
            catch
            {
                return "";
            }

        }
    }
}
