using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;

namespace xQuant.AutoUpdate
{
    /// <summary>
    /// SqlServer ����
    /// </summary>
    class SqlDataBaseUpdateService : BaseUpdateService
    {
        public override string Title
        {
            get { return "���ݿ���������"; }
        }

        public override string DirectoryName
        {
            get { return "���ݿ�����"; }
        }

        private Dictionary<string, IList<UpdateSqlCommand>> _sqlBatCommand;
        /// <summary>
        /// 
        /// </summary>
        private void LoadDataFormFiles()
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            dictionary.Add("APP", Settings.AutoUpdateSetting.Default.APP);
            dictionary.Add("MD", Settings.AutoUpdateSetting.Default.Md);
            dictionary.Add("TRD", Settings.AutoUpdateSetting.Default.Trd);
            dictionary.Add("EXH", Settings.AutoUpdateSetting.Default.Exh);

            ExeConfigurationFileMap configMap = new ExeConfigurationFileMap();
            configMap.ExeConfigFilename = Path.Combine(this.TargetPath, Settings.AutoUpdateSetting.Default.ConfigName);
            Configuration config = ConfigurationManager.OpenMappedExeConfiguration(configMap, ConfigurationUserLevel.None);

            foreach (KeyValuePair<string, string> keyValuePair in dictionary)
            {
                string dbDir = Path.Combine(Path.Combine(this.UpdateFolderName, this.DirectoryName), keyValuePair.Key);
                if (!Directory.Exists(dbDir) || Directory.GetFiles(dbDir).Length == 0)
                {
                    continue;
                }
                IList<UpdateSqlCommand> updateSqlCommands = new List<UpdateSqlCommand>();
                foreach (string file in Directory.GetFiles(dbDir))
                {
                    UpdateSqlCommand sqlCommand = new UpdateSqlCommand(file, Util.ReadFile(file), "");
                    ConnectionStringSettings connectionString = config.ConnectionStrings.ConnectionStrings[keyValuePair.Value];
                    sqlCommand.SqlConnectionString = connectionString.ConnectionString;
                    sqlCommand.ProviderName = connectionString.ProviderName;
                    updateSqlCommands.Add(sqlCommand);
                }
                _sqlBatCommand[keyValuePair.Key] = updateSqlCommands;
            }
        }
        public override void BeforeUpdate()
        {
            if (_sqlBatCommand == null)     // ��һ��ִ��
            {
                _sqlBatCommand = new Dictionary<string, IList<UpdateSqlCommand>>();
                LoadDataFormFiles();
            }
            else
            {
                // ʧ�ܺ��ٴ�ִ��ʱ��sqlCommand ��δִ�У�����ִ��ʧ��ʱ���ٴζ�ȡsql �ļ�
                foreach (KeyValuePair<string, IList<UpdateSqlCommand>> keyValuePair in _sqlBatCommand)
                {
                    foreach (UpdateSqlCommand updateSqlCommand in keyValuePair.Value)
                    {
                        if (!updateSqlCommand.Success)
                        {
                            updateSqlCommand.ReLoadCommand(); //���¼���sql�ļ�
                        }
                    }
                }
            }
        }

        public override void AfterUpdate()
        {

        }

        public override void ExecuteUpdate()
        {
            foreach (KeyValuePair<string, IList<UpdateSqlCommand>> keyValuePair in _sqlBatCommand)
            {
                OnShowMessage(string.Format("����ִ�С�{0}�������������", keyValuePair.Key));
                foreach (UpdateSqlCommand updateCommand in keyValuePair.Value)
                {
                    if (!updateCommand.Success)
                    {
                        string errorInfo;
                        OnShowMessage(string.Format("����ִ�С�{0}���ļ�", updateCommand.SqlFileName));
                        if (!updateCommand.ExeCommand(out errorInfo))
                        {
                            throw new Exception(string.Format(" ִ��ʧ�ܣ�������Ϣ:{0}", errorInfo));
                        }
                    }
                }
            }
        }
    }
}
