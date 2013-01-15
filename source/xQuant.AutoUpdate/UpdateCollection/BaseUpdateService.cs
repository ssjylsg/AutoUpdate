using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace xQuant.AutoUpdate
{
    /// <summary>
    /// 抽象升级服务
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
        /// 当前执行文件夹名称
        /// </summary>
        public abstract string DirectoryName { get; }

        private string _updateFolderName;

        /// <summary>
        /// 当前执行升级文件夹地址
        /// </summary>
        protected string CurrentDirectory
        {
            get { return System.IO.Path.Combine(this.UpdateFolderName, this.DirectoryName); }
        }
        private string _targetPath;

        /// <summary>
        /// 升级
        /// </summary>
        public void Update()
        {
            if (!System.IO.Directory.Exists(CurrentDirectory))
            {
                this.OnShowMessage(string.Format("【{0}】文件夹不存在", CurrentDirectory));
                this.State = UpdateState.Fail;
                return;
            }
            this.OnShowMessage(string.Format("【{0}】执行升级前操作 ", this.Title));
            this.State = UpdateState.BeforeUpdate;
            string backupDir = System.IO.Path.Combine(this.TargetPath, this.BackUpFoler);
            if (!System.IO.Directory.Exists(backupDir))
            {
                System.IO.Directory.CreateDirectory(backupDir);
            }
            this.BeforeUpdate();
            this.OnShowMessage(string.Format("升级{0}中...", this.Title));
            this.State = UpdateState.Update;
            this.ExecuteUpdate();
            this.State = UpdateState.Success;
            this.OnShowMessage(string.Format("【{0}】升级{1}", this.Title, this.State == UpdateState.Success ? "成功" : "失败"));

            this.OnShowMessage(string.Format("【{0}】执行升级后操作", this.Title));
            this.State = UpdateState.AfterUpdate;
            this.AfterUpdate();

        }
        private UpdateState _state;
        /// <summary>
        /// 目标地址
        /// </summary>
        public string TargetPath
        {
            get { return _targetPath; }
            set { _targetPath = value; }
        }
        /// <summary>
        /// 升级文件夹地址 （不包括具体地址）
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
        /// 备份文件夹名称
        /// </summary>
        public string BackUpFoler
        {
            get { return _backUpFoler; }
            set { _backUpFoler = value; }
        }
        /// <summary>
        /// 备份文件夹全路径
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
        /// 升级之前 操作
        /// </summary>
        public abstract void BeforeUpdate();
        /// <summary>
        /// 升级之后操作
        /// </summary>
        public abstract void AfterUpdate();
        /// <summary>
        /// 执行升级
        /// </summary>
        public abstract void ExecuteUpdate();

    }

    public enum UpdateState
    {
        [Description("未知")]
        None,
        [Description("升级前操作")]
        BeforeUpdate,
        [Description("升级中")]
        Update,
        [Description("升级成功")]
        AfterUpdate,
        [Description("升级失败")]
        Fail,
        [Description("升级成功")]
        Success
    }
    ///// <summary>
    ///// 跟新状态发生变化来源
    ///// </summary>
    //public enum StateChangeSource
    //{
    //    /// <summary>
    //    /// 版本升级成功
    //    /// </summary>
    //    VersionUpdateSuccess,
    //    /// <summary>
    //    /// UpdateService 正在执行
    //    /// </summary>
    //    UpdateServiceRunning,
    //    /// <summary>
    //    /// 一个UpdateSuccess 升级成功
    //    /// </summary>
    //    UpdateServiceSuccess,
    //    /// <summary>
    //    /// 异常
    //    /// </summary>
    //    Error
    //}
}
