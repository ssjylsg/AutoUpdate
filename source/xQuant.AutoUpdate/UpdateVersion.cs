using System;
using System.Collections.Generic;
using System.Text;

namespace xQuant.AutoUpdate
{
    /// <summary>
    /// 版本升级
    /// </summary>
    public class UpdateVersion : IComparer<UpdateVersion>
    {
        #region 私有变量
        private UpdateManager _updateManager;
        /// <summary>
        /// 产品名称
        /// </summary>
        private string _productName;
        /// <summary>
        /// 版本号
        /// </summary>
        private string _version;
        /// <summary>
        /// 本次升级文件夹全面
        /// </summary>
        private string _folderName;
        #endregion

        #region 构造函数
        public UpdateVersion()
        {

        }
        /// <summary>
        /// 版本升级
        /// </summary>
        /// <param name="updateFolderName"></param>
        /// <param name="programFolderName"></param>
        public UpdateVersion(string updateFolderName, string programFolderName)
        {
            this.FolderName = updateFolderName.Trim('\\');
            int index = this.FolderName.LastIndexOf('\\');
            string[] temp = this.FolderName.Substring(index + 1).Split(new char[] { '_' }, StringSplitOptions.RemoveEmptyEntries);
            this.Version = temp[1];
            this.ProductName = temp[0];

            _updateManager = new UpdateManager();
            this._updateManager.UpdateFolderName = updateFolderName;
            this._updateManager.ProgramFolderName = programFolderName;
            this._updateManager.BackupFolder =
                string.Format("{0}_{1}_备份_{2}", this.ProductName, this.Version, DateTime.Now.ToString("yyyy_MM_dd_HH_mm"));
        }
        #endregion

        #region 属性
        /// <summary>
        /// 获取升级失败的Update
        /// </summary>
        public IUpdateService UpdateServiceError
        {
            get
            {
                return ((List<IUpdateService>)this._updateManager.CurrentService).Find(delegate(IUpdateService update)
                    { return update.State == UpdateState.Fail; });
            }
        }
        /// <summary>
        /// 版本号
        /// </summary>
        public string Version
        {
            get { return _version; }
            private set { _version = value; }
        }

        /// <summary>
        /// 本次升级文件夹全面
        /// </summary>
        public string FolderName
        {
            get { return _folderName; }
            private set { _folderName = value; }
        }

        /// <summary>
        /// 产品名称
        /// </summary>
        public string ProductName
        {
            get { return _productName; }
            private set { _productName = value; }
        }
        /// <summary>
        /// 获取当前版本升级的Service
        /// </summary>
        public IList<IUpdateService> CurrentService
        {
            get { return this._updateManager.CurrentService; }
        }
        #endregion

        #region 重载方法
        public int Compare(UpdateVersion x, UpdateVersion y)
        {
            long version0 = long.Parse(x.Version.Replace(".", string.Empty));
            long version1 = long.Parse(y.Version.Replace(".", string.Empty));
            return version0.CompareTo(version1);
        }
        public override string ToString()
        {
            return string.Format("{0}版本正在升级：", this.Version);
        }
        #endregion

        #region 公共函数
        /// <summary>
        /// 开始升级
        /// </summary>
        public void BeginUpdate()
        {
            _updateManager.Start();
        }
        /// <summary>
        /// 停止升级
        /// </summary>
        internal void Stop()
        {
            this._updateManager.Stop();
        }
        /// <summary>
        /// 注册消息通知事件
        /// </summary>
        /// <param name="showMessage"></param>
        public void RegisterMessage(ShowMessageHandler showMessage)
        {
            this._updateManager.RegisterMessage(showMessage);
        }
        #endregion
    }
}
