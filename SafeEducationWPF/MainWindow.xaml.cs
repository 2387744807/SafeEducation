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
            CoreFun core = new CoreFun(txtUrl.Text);
            if (core.GetId() == "")
            {
                MessageBox.Show("链接错误！");
                return;
            }

            //执行访问并返回结果
            bool result = false;
            try
            {
                int type = rBtn1.IsChecked == true ? 1 : 2;
                core.SetSubmitUrl(type);
                result = core.WebRequestByGet();
            }
            catch (Exception ex)
            {
                MessageBox.Show("发生错误：\n" + ex.Message);
            }

            if (result)
            {
                MessageBox.Show("成功完成！");
            }
            else
            {
                MessageBox.Show("完成失败！");
            }
        }
    }
    public class CoreFun
    {
        public string url;
        private string SubmitUrl;

        public void SetSubmitUrl(int type)
        {
            if (type == 1)
            {
                SubmitUrl = "https://weiban.mycourse.cn/pharos/usercourse/finish.do?callback=jQuery&userCourseId="
                    + GetId() + "&tenantCode=35110001&type=1";
            }
            else
            {
                SubmitUrl = "https://weiban.mycourse.cn/pharos/usercourse/finish.do?callback=jQuery&userCourseId="
                    + GetId() + "&tenantCode=35110001&type=1";
            }
        }

        public CoreFun(string url)
        {
            this.url = url;
        }
        //正则方法
        private string GetBetween(string text, string before, string after)
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

        //采用正则方式获取id
        public string GetId()
        {
            return GetBetween(url, "userCourseId=", "&tenantCode");
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
                if (AnalysisJson(GetBetween(result, "jQuery\\(", "\\)")) == "ok")
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
    }
}
