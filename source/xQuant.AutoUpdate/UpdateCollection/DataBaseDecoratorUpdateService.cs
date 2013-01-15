using System.Configuration;

namespace xQuant.AutoUpdate
{
    /// <summary>
    /// ���ݿ�����
    /// </summary>
    class DataBaseDecoratorUpdateService : BaseUpdateService
    {
        public override string Title
        {
            get { return "���ݿ���������"; }
        }

        public override string DirectoryName
        {
            get { return "���ݿ�����"; }
        }
        private BaseUpdateService _dbUpdateService;
        public override void BeforeUpdate()
        {
            if (_dbUpdateService == null)
            {
                // ���� ProviderName �����ж���SQLSERVER ����Oracle
                ExeConfigurationFileMap configMap = new ExeConfigurationFileMap();
                configMap.ExeConfigFilename = System.IO.Path.Combine(this.TargetPath, Settings.AutoUpdateSetting.Default.ConfigName);
                Configuration config = ConfigurationManager.OpenMappedExeConfiguration(configMap, ConfigurationUserLevel.None);
                string providerName = config.ConnectionStrings.ConnectionStrings[Settings.AutoUpdateSetting.Default.APP].ProviderName;

                if (providerName == "Oracle.DataAccess.Client")
                {
                    _dbUpdateService = new OracleDataBaseUpdateService();
                }
                else
                {
                    _dbUpdateService = new SqlDataBaseUpdateService();
                }
                _dbUpdateService.ShowMessage += OnShowMessage;
                _dbUpdateService.StateChange += OnStateChange;
            }

            _dbUpdateService.UpdateFolderName = this.UpdateFolderName;
            _dbUpdateService.BackUpFoler = this.BackUpFoler;
            _dbUpdateService.TargetPath = this.TargetPath;

            _dbUpdateService.BeforeUpdate();
        }

        public override void AfterUpdate()
        {
            _dbUpdateService.AfterUpdate();

        }

        public override void ExecuteUpdate()
        {
            _dbUpdateService.ExecuteUpdate();
        }
    }
}
