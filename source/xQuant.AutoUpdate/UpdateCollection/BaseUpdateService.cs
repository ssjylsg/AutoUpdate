using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace xQuant.AutoUpdate
{
    /// <summary>
    /// ������������
    /// </summary>
    public abstract class BaseUpdateService : IUpdateService
    {
        protected BaseUpdateService()
        {
            this.State = UpdateState.None;
        }
        public event ShowMessageHandler ShowMessage;
        public event StateChangeHandler StateChange;

        protected void OnStateChange(IUpdateService updateService)
        {
            StateChangeHandler handler = StateChange;
            if (handler != null)
            {
                handler(updateService);
            }
        }

        protected void OnShowMessage(string message)
        {
            ShowMessageHandler handler = ShowMessage;
            if (handler != null) handler(message);
        }

        public abstract string Title { get; }
        /// <summary>
        /// ��ǰִ���ļ�������
        /// </summary>
        public abstract string DirectoryName { get; }

        private string _updateFolderName;

        /// <summary>
        /// ��ǰִ�������ļ��е�ַ
        /// </summary>
        protected string CurrentDirectory
        {
            get { return System.IO.Path.Combine(this.UpdateFolderName, this.DirectoryName); }
        }
        private string _targetPath;

        /// <summary>
        /// ����
        /// </summary>
        public void Update()
        {
            if (!System.IO.Directory.Exists(CurrentDirectory))
            {
                this.OnShowMessage(string.Format("��{0}���ļ��в�����", CurrentDirectory));
                this.State = UpdateState.Fail;
                return;
            }
            this.OnShowMessage(string.Format("��{0}��ִ������ǰ���� ", this.Title));
            this.State = UpdateState.BeforeUpdate;
            string backupDir = System.IO.Path.Combine(this.TargetPath, this.BackUpFoler);
            if (!System.IO.Directory.Exists(backupDir))
            {
                System.IO.Directory.CreateDirectory(backupDir);
            }
            this.BeforeUpdate();
            this.OnShowMessage(string.Format("����{0}��...", this.Title));
            this.State = UpdateState.Update;
            this.ExecuteUpdate();
            this.State = UpdateState.Success;
            this.OnShowMessage(string.Format("��{0}������{1}", this.Title, this.State == UpdateState.Success ? "�ɹ�" : "ʧ��"));

            this.OnShowMessage(string.Format("��{0}��ִ�����������", this.Title));
            this.State = UpdateState.AfterUpdate;
            this.AfterUpdate();

        }
        private UpdateState _state;
        /// <summary>
        /// Ŀ���ַ
        /// </summary>
        public string TargetPath
        {
            get { return _targetPath; }
            set { _targetPath = value; }
        }
        /// <summary>
        /// �����ļ��е�ַ �������������ַ��
        /// </summary>
        public string UpdateFolderName
        {
            get { return _updateFolderName; }
            set { _updateFolderName = value; }
        }

        public UpdateState State
        {
            get { return _state; }
            set
            {
                if (value != _state)
                {
                    _state = value;
                    OnStateChange(this);
                }
            }
        }
        /// <summary>
        /// �����ļ�������
        /// </summary>
        public string BackUpFoler
        {
            get { return _backUpFoler; }
            set { _backUpFoler = value; }
        }
        /// <summary>
        /// �����ļ���ȫ·��
        /// </summary>
        public string BackUpFullFolerName
        {
            get
            {
                string backupDir = System.IO.Path.Combine(this.TargetPath, this.BackUpFoler);
                string fullName = System.IO.Path.Combine(backupDir, this.DirectoryName);
                if (!System.IO.Directory.Exists(fullName))
                {
                    System.IO.Directory.CreateDirectory(fullName);
                }
                return fullName;
            }
        }
        private string _backUpFoler;


        /// <summary>
        /// ����֮ǰ ����
        /// </summary>
        public abstract void BeforeUpdate();
        /// <summary>
        /// ����֮�����
        /// </summary>
        public abstract void AfterUpdate();
        /// <summary>
        /// ִ������
        /// </summary>
        public abstract void ExecuteUpdate();

    }

    public enum UpdateState
    {
        [Description("δ֪")]
        None,
        [Description("����ǰ����")]
        BeforeUpdate,
        [Description("������")]
        Update,
        [Description("�����ɹ�")]
        AfterUpdate,
        [Description("����ʧ��")]
        Fail,
        [Description("�����ɹ�")]
        Success
    }
    ///// <summary>
    ///// ����״̬�����仯��Դ
    ///// </summary>
    //public enum StateChangeSource
    //{
    //    /// <summary>
    //    /// �汾�����ɹ�
    //    /// </summary>
    //    VersionUpdateSuccess,
    //    /// <summary>
    //    /// UpdateService ����ִ��
    //    /// </summary>
    //    UpdateServiceRunning,
    //    /// <summary>
    //    /// һ��UpdateSuccess �����ɹ�
    //    /// </summary>
    //    UpdateServiceSuccess,
    //    /// <summary>
    //    /// �쳣
    //    /// </summary>
    //    Error
    //}
}
