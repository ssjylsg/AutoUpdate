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
    /// 更新状态发生变化
    /// </summary>
    /// <param name="updateServiceService"></param>
    public delegate void StateChangeHandler(IUpdateService updateServiceService);
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
        /// 是否成功
        /// </summary>
        //bool Success { get; set; }

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
