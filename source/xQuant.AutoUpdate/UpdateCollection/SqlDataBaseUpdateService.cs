using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;

namespace xQuant.AutoUpdate
{
    /// <summary>
    /// SqlServer 升级
    /// </summary>
    class SqlDataBaseUpdateService : BaseUpdateService
    {
        public override string Title
        {
            get { return "数据库配置升级"; }
        }

        public override string DirectoryName
        {
            get { return "数据库配置"; }
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
            if (_sqlBatCommand == null)     // 第一次执行
            {
                _sqlBatCommand = new Dictionary<string, IList<UpdateSqlCommand>>();
                LoadDataFormFiles();
            }
            else
            {
                // 失败后再次执行时，sqlCommand 还未执行，或者执行失败时，再次读取sql 文件
                foreach (KeyValuePair<string, IList<UpdateSqlCommand>> keyValuePair in _sqlBatCommand)
                {
                    foreach (UpdateSqlCommand updateSqlCommand in keyValuePair.Value)
                    {
                        if (!updateSqlCommand.Success)
                        {
                            updateSqlCommand.ReLoadCommand(); //重新加载sql文件
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
                OnShowMessage(string.Format("正在执行【{0}】库的数据升级", keyValuePair.Key));
                foreach (UpdateSqlCommand updateCommand in keyValuePair.Value)
                {
                    if (!updateCommand.Success)
                    {
                        string errorInfo;
                        OnShowMessage(string.Format("正在执行【{0}】文件", updateCommand.SqlFileName));
                        if (!updateCommand.ExeCommand(out errorInfo))
                        {
                            throw new Exception(string.Format(" 执行失败，错误信息:{0}", errorInfo));
                        }
                    }
                }
            }
        }
    }
}
