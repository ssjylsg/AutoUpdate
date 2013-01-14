using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Text;

namespace xQuant.AutoUpdate
{
    /// <summary>
    /// ���ݿ����
    /// </summary>
    class OracleDataBaseUpdateService : BaseUpdateService
    {
        public override string Title
        {
            get { return "���ݿ����ø���"; }
        }

        public override string DirectoryName
        {
            get { return "���ݿ�����"; }
        }
        /// <summary>
        /// sql ������ Key ֵΪ������Value Ϊÿ���ļ����ɵ�SQLPLUS����
        /// </summary>
        private Dictionary<string, IList<UpdateSqlCommand>> _sqlBatCommand;
        public override void BeforeUpdate()
        {
            if (_sqlBatCommand != null)
            {
                return;
            }

            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            dictionary.Add("APP", Settings.AutoUpdateSetting.Default.APP);
            dictionary.Add("MD", Settings.AutoUpdateSetting.Default.Md);
            dictionary.Add("TRD", Settings.AutoUpdateSetting.Default.Trd);
            dictionary.Add("EXH", Settings.AutoUpdateSetting.Default.Exh);

            _sqlBatCommand = new Dictionary<string, IList<UpdateSqlCommand>>();
            foreach (KeyValuePair<string, string> keyValuePair in dictionary)
            {
                string dbDir = Path.Combine(Path.Combine(this.UpdateFolderName, this.DirectoryName), keyValuePair.Key);
                string logDir = Path.Combine(dbDir, "log");
                string sqlDir = Path.Combine(dbDir, "batsql");
                if (!Directory.Exists(dbDir) || Directory.GetFiles(dbDir).Length == 0)
                {
                    continue;
                }

                #region �ļ��д���
                // ������־�ļ���
                if (!Directory.Exists(logDir))
                {
                    Directory.CreateDirectory(logDir);
                }
                // ����sql�������ļ���
                if (!Directory.Exists(sqlDir))
                {
                    Directory.CreateDirectory(sqlDir);
                }
                #endregion

                #region ��ȡ���ݿ�����
                string[] dbConnection = GetConnectionStrings(keyValuePair.Value);
                if (dbConnection == null)
                {
                    throw new Exception(string.Format("��ȡ{0}���ݿ������ַ���ʧ��", keyValuePair.Value));
                }
                #endregion

                #region ����SQL�ļ�

                string[] files = Directory.GetFiles(dbDir);

                IList<UpdateSqlCommand> updateCommands = new List<UpdateSqlCommand>();
                for (int i = 0; i < files.Length; i++)
                {
                    string fileName = new FileInfo(files[0]).Name;
                    fileName = fileName.Substring(0, fileName.IndexOf('.')); // ȥ����չ��

                    // ������ִ�е�SQL�ļ�
                    string sqlFile = Path.Combine(sqlDir, string.Format("{0}.sql", fileName));
                    // log ��־�ļ�
                    string logFile = Path.Combine(logDir, string.Format("log_{0}.txt", fileName));
                    StringBuilder stringBuilder = new StringBuilder();
                    stringBuilder.AppendFormat("SPOOL  {0}", logFile).AppendLine();
                    stringBuilder.AppendFormat("@{0}", files[0]);
                    stringBuilder.Append(@" 
                                    SPOOL OFF
                                    EXIT
                                    EOF
                                    ");

                    WriteFile(sqlFile, stringBuilder.ToString());

                    #region ����ִ������

                    UpdateSqlCommand sqlCommand = new UpdateSqlCommand(files[0],
                                                                       string.Format("SQLPLUS {0}/{1}@{2}  @{3}",
                                                                                     dbConnection[1],
                                                                                     dbConnection[2],
                                                                                     dbConnection[0], sqlFile), logFile);
                    sqlCommand.SqlConnectionString = dbConnection[3];
                    sqlCommand.ProviderName = "Oracle.DataAccess.Client";
                    updateCommands.Add(sqlCommand);


                    #endregion
                }
                #endregion

                _sqlBatCommand.Add(keyValuePair.Key, updateCommands);
            }
        }
        /// <summary>
        /// д���ļ�
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="content"></param>
        private void WriteFile(string filePath, string content)
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
            using (FileStream fileStream = File.Create(filePath))
            {
                StreamWriter writer = new StreamWriter(fileStream, Encoding.Default);
                writer.WriteLine(content);
                writer.Close();
            }
        }
        /// <summary>
        ///  SERVICE_NAME,User ID,Password,connectionString
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private string[] GetConnectionStrings(string name)
        {
            string[] result = new string[] { string.Empty, string.Empty, string.Empty };
            if (string.IsNullOrEmpty(this.TargetPath) || !Directory.Exists(this.TargetPath))
            {
                this.OnShowMessage(string.Format("δ�ҵ����ݿ�����"));
                return result;
            }
            string fileName = Path.Combine(this.TargetPath, Settings.AutoUpdateSetting.Default.ConfigName);
            if (!File.Exists(fileName))
            {
                this.OnShowMessage(string.Format("δ��{0}�ļ�", Settings.AutoUpdateSetting.Default.ConfigName));
                return result;
            }

            ExeConfigurationFileMap configMap = new ExeConfigurationFileMap();
            configMap.ExeConfigFilename = fileName;
            Configuration config = ConfigurationManager.OpenMappedExeConfiguration(configMap, ConfigurationUserLevel.None);
            ConnectionStringSettings settingsCollection = config.ConnectionStrings.ConnectionStrings[name];
            if (settingsCollection == null)
            {
                this.OnShowMessage(string.Format("δ��{0}���ý�", name));
                return result;
            }

            string connectionString = settingsCollection.ConnectionString;
            result = new string[]
                {
                    this.GetNoteValue(connectionString, "SERVICE_NAME",")"),
                    this.GetNoteValue(connectionString, "User ID",";"),
                    this.GetNoteValue(connectionString, "Password",";"),
                    connectionString
                };
            return result;
        }
        private string GetNoteValue(string connectionString, string note, string split)
        {
            int index = connectionString.IndexOf(note, StringComparison.CurrentCultureIgnoreCase);
            if (index > 0)
            {
                string source = connectionString.Substring(index + note.Length);
                source = source.Substring(0, source.IndexOf(split, StringComparison.CurrentCultureIgnoreCase));
                return source.Trim(new char[] { '=', ' ', ')' });
            }
            throw new Exception(string.Format("{0}��{1} ��δ�ҵ�", note, Settings.AutoUpdateSetting.Default.ConfigName));
        }
        public override void AfterUpdate()
        {

        }

        public override void ExecuteUpdate()
        {
            foreach (KeyValuePair<string, IList<UpdateSqlCommand>> keyValuePair in _sqlBatCommand)
            {
                OnShowMessage(string.Format("����ִ�С�{0}��������ݸ���", keyValuePair.Key));
                foreach (UpdateSqlCommand updateCommand in keyValuePair.Value)
                {
                    if (!updateCommand.Success)
                    {
                        string errorInfo;
                        OnShowMessage(string.Format("����ִ�С�{0}���ļ�", updateCommand.SqlFileName));
                        if (!updateCommand.ExeCommand(out errorInfo))
                        {
                            throw new Exception(string.Format("{0} ִ��ʧ�ܣ���鿴��ϸ��־:{1}", errorInfo, FormatFileUrl(updateCommand.LogFile)));
                        }
                    }
                }
            }
        }
        /// <summary>
        /// ��ȡ�ļ���ַ
        /// </summary>
        /// <param name="fileUrl"></param>
        /// <returns></returns>
        private string FormatFileUrl(string fileUrl)
        {
            return string.Format("file:///{0}", Util.UrlEncode(fileUrl));
        }
    }

}
