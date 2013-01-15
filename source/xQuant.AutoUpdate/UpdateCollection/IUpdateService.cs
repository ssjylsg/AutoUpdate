using System;
using System.Collections.Generic;
using System.Text;

namespace xQuant.AutoUpdate
{
    /// <summary>
    /// 消息提示
    /// </summary>
    /// <param name="message"></param>
    public delegate void ShowMessageHandler(string message);
    /// <summary>
    /// 升级状态发生变化 每个UpdateService 中状态的变化 如执行前，执行中，执行后，执行成功，执行失败
    /// </summary>
    /// <param name="updateServiceService"></param>
    public delegate void StateChangeHandler(IUpdateService updateServiceService);
    /// <summary>
    ///升级状态发生变化   每个updateSercie 的变化，如updateService 成功，失败，或者执行中 主要用于进度条的显示
    /// </summary>
    /// <param name="message"></param>
    /// <param name="source"></param>
    //public delegate void ProcessStateChangeHandler(string message, StateChangeSource source);
    /// <summary>
    /// 升级服务
    /// </summary>
    public interface IUpdateService
    {
        /// <summary>
        /// 消息提示事件
        /// </summary>
        event ShowMessageHandler ShowMessage;
        /// <summary>
        /// 状态变化触发事件
        /// </summary>
        event StateChangeHandler StateChange;
        /// <summary>
        /// 升级名称
        /// </summary>
        string Title { get; }
        /// <summary>
        /// 升级程序所在文件地址
        /// </summary>
        string UpdateFolderName { set; }
        /// <summary>
        /// 目标地址
        /// </summary>
        string TargetPath { set; }
        /// <summary>
        /// 开始执行升级
        /// </summary>
        void Update();
        /// <summary>
        /// 状态
        /// </summary>
        UpdateState State { get; set; }
        /// <summary>
        /// 备份文件夹地址
        /// </summary>
        string BackUpFoler { get; set; }

    }
}
