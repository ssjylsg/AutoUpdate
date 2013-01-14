using System;
using System.Collections.Generic;

namespace xQuant.AutoUpdate
{
    /// <summary>
    /// 升级管理
    /// </summary>
    public class UpdateManager
    {
        /// <summary>
        /// 是否正在执行中
        /// </summary>
        private bool _running;
        /// <summary>
        /// 消息提示
        /// </summary>
        private static ShowMessageHandler _showMessage;
        /// <summary>
        /// 更新状态发生变化
        /// </summary>
        private static ShowMessageHandler _showUpdateStateChange;
        /// <summary>
        /// 更新服务
        /// </summary>
        private readonly IList<IUpdateService> _updateService;
        public UpdateManager()
        {
            _updateService = new List<IUpdateService>();
            _updateService.Add(new MQConfigUpdateService());
            _updateService.Add(new ComUpdateService());
            _updateService.Add(new MiddlewareConfig());
            _updateService.Add(new DataBaseDecoratorUpdateService());
            _updateService.Add(new ProgramUpdateService());
        }

        private string _backupFolder;

        public IList<IUpdateService> CurrentService
        {
            get { return _updateService; }
        }
        /// <summary>
        /// 获取所有服务
        /// </summary>
        public static IList<IUpdateService> UpdateService
        {
            get { return new UpdateManager().CurrentService; }
        }
        /// <summary>
        /// 注册消息通知事件
        /// </summary>
        /// <param name="handler"></param>
        public void RegisterMessage(ShowMessageHandler handler)
        {
            foreach (IUpdateService update in CurrentService)
            {
                update.ShowMessage -= handler;
                update.ShowMessage += handler;
            }
            _showMessage -= handler;
            _showMessage += handler;
        }
        /// <summary>
        /// 注册更新状态变化事件
        /// </summary>
        /// <param name="changeHander"></param>
        public static void RegisterStateChange(ShowMessageHandler changeHander)
        {
            _showUpdateStateChange = changeHander;
        }
        /// <summary>
        /// 开始更新服务
        /// </summary>
        public void Start()
        {
            if (_running)
            {
                return;
            }

            _running = true;

            ShowMessage(string.Format("本次更新备份文件地址{0}", BackupFolder));
            foreach (IUpdateService update in _updateService)
            {
                if (_running)
                {
                    try
                    {
                        if (update.State != UpdateState.Success && update.State != UpdateState.AfterUpdate)
                        {

                            UpdateStateChange(string.Format("正在升级【{0}】", update.Title));

                            update.UpdateFolderName = UpdateFolderName;
                            update.TargetPath = ProgramFolderName;
                            update.BackUpFoler = BackupFolder;
                            update.Update();
                        }
                    }
                    catch (Exception e)
                    {
                        update.State = UpdateState.Fail;
                        ShowMessage(e.Message);
                        this._running = false;
                        throw;
                    }
                    ShowMessage("升级完成，请查看详细日志");
                }
            }
            UpdateStateChange("升级完成");
            _running = false;
        }
        /// <summary>
        /// 更新状态发生变化
        /// </summary>
        /// <param name="changeState"></param>
        private void UpdateStateChange(string changeState)
        {
            ShowMessageHandler stateChange = _showUpdateStateChange;
            if (stateChange != null)
            {
                stateChange(changeState);
            }
        }
        /// <summary>
        /// 消息提示
        /// </summary>
        /// <param name="message"></param>
        private static void ShowMessage(string message)
        {
            if (_showMessage != null)
            {
                ShowMessageHandler handler = _showMessage;
                handler(message);
            }
        }
        /// <summary>
        /// 停止更新
        /// </summary>
        public void Stop()
        {
            _running = false;
        }
        /// <summary>
        /// 重置是否更新成功标志
        /// </summary>
        //public void ReSetUpdateState()
        //{
        //    foreach (IUpdateService update in _updateService)
        //    {
        //        update.State = UpdateState.None;
        //    }
        //}

        private string _updateFolderName;
        /// <summary>
        /// 升级包程序所在地址
        /// </summary>
        public string UpdateFolderName { get { return _updateFolderName; } set { _updateFolderName = value; } }
        /// <summary>
        /// 中间件程序所在文件夹地址
        /// </summary>
        public string ProgramFolderName
        {
            get { return _programFolderName; }
            set { _programFolderName = value; }
        }
        /// <summary>
        /// 备份文件夹地址  全路径
        /// </summary>
        public string BackupFolder
        {
            get { return _backupFolder; }
            set { _backupFolder = value; }
        }

        private string _programFolderName;


    }
}
