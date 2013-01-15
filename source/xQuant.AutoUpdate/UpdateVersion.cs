using System;
using System.Collections.Generic;
using System.Text;

namespace xQuant.AutoUpdate
{
    /// <summary>
    /// �汾����
    /// </summary>
    public class UpdateVersion : IComparer<UpdateVersion>
    {
        #region ˽�б���
        private UpdateManager _updateManager;
        /// <summary>
        /// ��Ʒ����
        /// </summary>
        private string _productName;
        /// <summary>
        /// �汾��
        /// </summary>
        private string _version;
        /// <summary>
        /// ���������ļ���ȫ��
        /// </summary>
        private string _folderName;
        #endregion

        #region ���캯��
        public UpdateVersion()
        {

        }
        /// <summary>
        /// �汾����
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
                string.Format("{0}_{1}_����_{2}", this.ProductName, this.Version, DateTime.Now.ToString("yyyy_MM_dd_HH_mm"));
        }
        #endregion

        #region ����
        /// <summary>
        /// ��ȡ����ʧ�ܵ�Update
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
        /// �汾��
        /// </summary>
        public string Version
        {
            get { return _version; }
            private set { _version = value; }
        }

        /// <summary>
        /// ���������ļ���ȫ��
        /// </summary>
        public string FolderName
        {
            get { return _folderName; }
            private set { _folderName = value; }
        }

        /// <summary>
        /// ��Ʒ����
        /// </summary>
        public string ProductName
        {
            get { return _productName; }
            private set { _productName = value; }
        }
        /// <summary>
        /// ��ȡ��ǰ�汾������Service
        /// </summary>
        public IList<IUpdateService> CurrentService
        {
            get { return this._updateManager.CurrentService; }
        }
        #endregion

        #region ���ط���
        public int Compare(UpdateVersion x, UpdateVersion y)
        {
            long version0 = long.Parse(x.Version.Replace(".", string.Empty));
            long version1 = long.Parse(y.Version.Replace(".", string.Empty));
            return version0.CompareTo(version1);
        }
        public override string ToString()
        {
            return string.Format("{0}�汾����������", this.Version);
        }
        #endregion

        #region ��������
        /// <summary>
        /// ��ʼ����
        /// </summary>
        public void BeginUpdate()
        {
            _updateManager.Start();
        }
        /// <summary>
        /// ֹͣ����
        /// </summary>
        internal void Stop()
        {
            this._updateManager.Stop();
        }
        /// <summary>
        /// ע����Ϣ֪ͨ�¼�
        /// </summary>
        /// <param name="showMessage"></param>
        public void RegisterMessage(ShowMessageHandler showMessage)
        {
            this._updateManager.RegisterMessage(showMessage);
        }
        #endregion
    }
}
