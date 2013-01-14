using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text;

namespace xQuant.AutoUpdate
{
    /// <summary>
    /// SqlServer 更新
    /// </summary>
    class SqlDataBaseUpdateService : BaseUpdateService
    {
        public override string Title
        {
            get { return "数据库配置更新"; }
        }

        public override string DirectoryName
        {
            get { return "数据库配置"; }
        }

        private Dictionary<string, IList<UpdateSqlCommand>> _sqlBatCommand;
        public override void BeforeUpdate()
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            dictionary.Add("APP", Settings.AutoUpdateSetting.Default.APP);
            dictionary.Add("MD", Settings.AutoUpdateSetting.Default.Md);
            dictionary.Add("TRD", Settings.AutoUpdateSetting.Default.Trd);
            dictionary.Add("EXH", Settings.AutoUpdateSetting.Default.Exh);

            ExeConfigurationFileMap configMap = new ExeConfigurationFileMap();
            configMap.ExeConfigFilename = Path.Combine(this.TargetPath, Settings.AutoUpdateSetting.Default.ConfigName);
            Configuration config = ConfigurationManager.OpenMappedExeConfiguration(configMap, ConfigurationUserLevel.None);

            _sqlBatCommand = new Dictionary<string, IList<UpdateSqlCommand>>();
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

        public override void AfterUpdate()
        {

        }

        public override void ExecuteUpdate()
        {
            foreach (KeyValuePair<string, IList<UpdateSqlCommand>> keyValuePair in _sqlBatCommand)
            {
                OnShowMessage(string.Format("正在执行【{0}】库的数据更新", keyValuePair.Key));
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
