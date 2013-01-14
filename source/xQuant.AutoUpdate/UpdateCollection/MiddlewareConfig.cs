using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Xml;

namespace xQuant.AutoUpdate
{
    /// <summary>
    /// �м��Config����
    /// </summary>
    class MiddlewareConfig : BaseUpdateService
    {
        public override string Title
        {
            get { return "�м�����ø���"; }
        }

        public override string DirectoryName
        {
            get { return "�м������"; }
        }
        public override void BeforeUpdate()
        {
            // �жϵ�ǰ�м�������Ƿ�������
            if (Process.GetProcessesByName(Settings.AutoUpdateSetting.Default.ServerProgramName).Length > 0)
            {
                throw new Exception(string.Format("�м���������У����ȹر�"));
            }
            // �жϵ�ǰconfig �ļ��Ƿ����
            string configFile = Path.Combine(this.TargetPath, Settings.AutoUpdateSetting.Default.ConfigName);
            if (!File.Exists(configFile))
            {
                throw new Exception(string.Format("{0} �����ļ�δ�ҵ�",Settings.AutoUpdateSetting.Default.ConfigName));
            }

            // ����.config �ļ�
            new FileInfo(configFile).CopyTo(Path.Combine(this.BackUpFullFolerName, Settings.AutoUpdateSetting.Default.ConfigName), true);
            this.OnShowMessage(string.Format("�ɹ�����{0}�ļ�", Settings.AutoUpdateSetting.Default.ConfigName));

        }

        public override void AfterUpdate()
        {

        }

        public override void ExecuteUpdate()
        {
            string configFile = Path.Combine(this.TargetPath, Settings.AutoUpdateSetting.Default.ConfigName);
            string[] configNote = new string[]
                {
                    "service",
                    "corecontrol" ,
                    "appSettings" 
                };
            foreach (string note in configNote)
            {
                string filePath = Path.Combine(this.CurrentDirectory, string.Format("{0}.txt", note));
                if (File.Exists(filePath))
                {
                    string[] content = ReadCommands(filePath);
                    if (content.Length > 0)
                    {
                        switch (note)
                        {
                            case "service":
                                this.RemotingUpdate(configFile, content);
                                break;
                            case "corecontrol":
                                this.CorecontrolUpdate(configFile, content);
                                break;
                            case "appSettings":
                                this.AppSettingUpdate(configFile, content);
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
        }
        /// <summary>
        /// �ж����������Ƿ���KeyName
        /// </summary>
        /// <param name="keyName"></param>
        /// <param name="noteList"></param>
        /// <param name="findedNote"></param>
        /// <returns></returns>
        private bool IsExistsNote(string keyName, XmlNodeList noteList, out XmlNode findedNote, string keyValue)
        {
            for (int i = 0; i < noteList.Count; i++)
            {
                if (noteList[i].GetType() != typeof(XmlComment) && noteList[i].Name != "activated")  // ����XMLע������  ��Remoting ����������
                {
                    XmlAttribute attribute = noteList[i].Attributes[keyName];
                    if (attribute != null && attribute.Value.Equals(keyValue, StringComparison.CurrentCultureIgnoreCase))
                    {
                        findedNote = noteList[i];
                        return true;
                    }
                }
            }
            findedNote = null;
            return false;
        }
        /// <summary>
        /// ���� Corecontrol ���ý�
        /// </summary>
        /// <param name="configFile"></param>
        /// <param name="contents"></param>
        private void CorecontrolUpdate(string configFile, string[] contents)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(configFile);

            XmlNode control = doc.GetElementsByTagName("corecontrol")[0];

            foreach (string command in contents)
            {
                string keyValue = this.GetXmlAttributeValue("type", command);
                string run = this.GetXmlAttributeValue("run", command);
                XmlNode note;
                if (IsExistsNote("type", control.ChildNodes, out note, keyValue))
                {
                    note.Attributes["run"].Value = run;
                    this.OnShowMessage(string.Format("�޸����ý�:{0}", command));
                }
                else
                {
                    XmlElement xmlDoc = doc.CreateElement("start");
                    xmlDoc.SetAttribute("type", keyValue);
                    xmlDoc.SetAttribute("run", run);
                    control.AppendChild(xmlDoc);
                    this.OnShowMessage(string.Format("�������ý�:{0}", command));
                }
            }

            doc.Save(configFile);
        }
        /// <summary>
        /// ����AppSetting���ý�
        /// </summary>
        /// <param name="configFile"></param>
        /// <param name="content"></param>
        public void AppSettingUpdate(string configFile, string[] content)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(configFile);
            XmlNode setting = doc.GetElementsByTagName("appSettings")[0];
            foreach (string command in content)
            {
                string key = this.GetXmlAttributeValue("key", command);
                string value = this.GetXmlAttributeValue("value", command);
                XmlNode note;
                if (IsExistsNote("key", setting.ChildNodes, out note, key))
                {
                    note.Attributes["value"].Value = value;
                    this.OnShowMessage(string.Format("�޸����ý�:{0}", command));
                }
                else
                {
                    XmlElement xmlDoc = doc.CreateElement("start");
                    xmlDoc.SetAttribute("key", key);
                    xmlDoc.SetAttribute("value", value);
                    setting.AppendChild(xmlDoc);
                    this.OnShowMessage(string.Format("�������ý�:{0}", command));
                }
            }
            doc.Save(configFile);
        }

        /// <summary>
        /// ����service���ý�
        /// </summary>
        /// <param name="configFile"></param>
        /// <param name="content"></param>
        public void RemotingUpdate(string configFile, string[] content)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(configFile);
            XmlNode setting = doc.GetElementsByTagName("system.runtime.remoting")[0].ChildNodes[0].ChildNodes[1];

            foreach (string command in content)
            {
                string type = this.GetXmlAttributeValue("type", command);
                string mode = this.GetXmlAttributeValue("mode", command);
                string objectUri = this.GetXmlAttributeValue("objectUri", command);
                XmlNode note;
                if (IsExistsNote("type", setting.ChildNodes, out note, type))
                {
                    note.Attributes["type"].Value = type;
                    note.Attributes["objectUri"].Value = objectUri;
                    note.Attributes["mode"].Value = mode;
                    this.OnShowMessage(string.Format("�޸����ý�:{0}", command));
                }
                else
                {
                    XmlElement docXml = doc.CreateElement("wellknown");

                    docXml.SetAttribute("mode", mode);
                    docXml.SetAttribute("type", type);
                    docXml.SetAttribute("objectUri", objectUri);
                    setting.AppendChild(docXml);
                    this.OnShowMessage(string.Format("�������ý�:{0}", command));
                }
            }
            doc.Save(configFile);
        }
        /// <summary>
        /// ��ȡ�����ļ��е������� ��  <start type="xQuant.Model.XPO.Common.XPSaverCore, xQuant.Model" run="false" />
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns> 
        private string[] ReadCommands(string filePath)
        {
            List<string> command = new List<string>();

            using (StreamReader reader = new StreamReader(filePath, Encoding.Default))
            {
                string content;
                while (!reader.EndOfStream)
                {
                    content = (reader.ReadLine() ?? string.Empty).Trim();
                    if (!string.IsNullOrEmpty(content))
                    {
                        command.Add(content);
                    }
                }
            }
            return command.ToArray();
        }
        /// <summary>
        /// ��ȡXML Attribute ����
        /// </summary>
        /// <param name="attribute"></param>
        /// <param name="xml"></param>
        /// <returns></returns>
        private string GetXmlAttributeValue(string attribute, string xml)
        {
            int index = xml.IndexOf(attribute);
            xml = xml.Substring(index + attribute.Length);
            index = xml.IndexOf("\"");
            xml = xml.Substring(index + 1);
            return xml.Substring(0, xml.IndexOf("\""));
        }
    }
}
