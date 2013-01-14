using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;

namespace xQuant.AutoUpdate
{
    /// <summary>
    /// 帮助类
    /// </summary>
    class Util
    {
        /// <summary>
        /// 执行CMD命令
        /// </summary>
        /// <param name="commandText"></param>
        /// <returns></returns>
        public static string ExeCommand(string commandText)
        {
            Process p = new Process();
            p.StartInfo.FileName = "CMD.EXE";
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardInput = true;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.CreateNoWindow = true;

            p.Start();
            p.StandardInput.WriteLine(commandText);
            p.StandardInput.WriteLine("EXIT;");

            string strOutput = p.StandardOutput.ReadToEnd();
            p.WaitForExit();
            p.Close();

            return strOutput;
        }
        /// <summary>
        /// 获取枚举DescriptionAttribute 值
        /// </summary>
        /// <param name="eEnum"></param>
        /// <returns></returns>
        public static string GetDescription(Enum eEnum)
        {
            Type enumType = eEnum.GetType();
            FieldInfo fi = enumType.GetField(Enum.GetName(enumType, eEnum));
            DescriptionAttribute dna = (DescriptionAttribute)Attribute.GetCustomAttribute(fi, typeof(DescriptionAttribute));
            return dna == null ? string.Empty : dna.Description;
        }
        /// <summary>
        /// Url 编码
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string UrlEncode(string url)
        {
            return System.Web.HttpUtility.UrlEncode(url);
        }
        /// <summary>
        /// Url 解码
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string UrlDecode(string url)
        {
            return System.Web.HttpUtility.UrlDecode(url);
        }
        /// <summary>
        /// 文件读取
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string ReadFile(string path)
        {
            using (TextReader reader = new StreamReader(path, Encoding.Default))
            {
                return reader.ReadToEnd();
            }
        }
    }
}
