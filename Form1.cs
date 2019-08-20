using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SafeEducation
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }


        private void BtnComplete_Click(object sender, EventArgs e)
        {
            string courseId = GetId(txtUrl.Text);//获取id
            if (courseId == "")
            {
                MessageBox.Show("链接错误！");
                return;
            }
            //MessageBox.Show(courseId);
            //执行访问并返回结果
            bool result = false;
            try
            {
                result = WebRequestByGet("https://weiban.mycourse.cn/pharos/usercourse/finish.do?callback=jQuery&userCourseId=" + courseId + "&tenantCode=35110001&type=1");
            }catch(Exception ex)
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

        //正则方法
        private string GetBetween(string text,string before,string after)
        {
            try
            {
                Regex rg = new Regex("(?<=(" + before + "))[.\\s\\S]*?(?=(" + after + "))", RegexOptions.Multiline | RegexOptions.Singleline);
                return rg.Match(text).Value;
            }catch(Exception ex)
            {
                throw ex;
            }
            
        }

        //采用正则方式获取id
        private string GetId(string urlText)
        {
            return GetBetween(urlText, "userCourseId=", "&tenantCode");
        }

        //Get访问Url
        private bool WebRequestByGet(string url)
        {
            try
            {
                WebRequest request = WebRequest.Create(url);
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
            }catch(Exception ex)
            {
                throw ex;
            }
            
        }
    }
}
