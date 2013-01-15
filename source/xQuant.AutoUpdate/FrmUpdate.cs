using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using DevExpress.XtraEditors;

namespace xQuant.AutoUpdate
{
    public partial class FrmUpdate : Form
    {
        #region 私有变量
        /// <summary>
        /// 当前版本
        /// </summary>
        private UpdateVersion _currentVersion;
        /// <summary>
        /// 升级队列
        /// </summary>
        private Queue<UpdateVersion> _updateVersionsQueue;
        /// <summary>
        /// UC控件
        /// </summary>
        private IList<UCUpdateState> _ucUpdateStates;
        #endregion

        #region 构造函数
        public FrmUpdate()
        {
            InitializeComponent();

            AddUserControl();

#if DEBUG
            this.txtUpdate.Text = @"C:\Users\Administrator\Desktop\升级包";
            this.txtPro.Text = @"C:\Users\Administrator\Desktop\中间件程序";
#endif
        }

        /// <summary>
        /// 动态加载用户控件
        /// </summary>
        private void AddUserControl()
        {
            int count = 0;
            _ucUpdateStates = new List<UCUpdateState>();
            foreach (IUpdateService update in UpdateManager.UpdateService)
            {
                UCUpdateState ucUpdateState = new UCUpdateState(update);
                ucUpdateState.Location = new Point(3, count * 40 + 3);
                panel2.Controls.Add(ucUpdateState);
                update.StateChange += ucUpdateState.UpdateState;
                count++;
                _ucUpdateStates.Add(ucUpdateState);
            }
        }
        #endregion

        #region  开始执行升级
        /// <summary>
        /// 开始执行升级
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnStart_Click(object sender, EventArgs e)
        {
            ShowMessage("开始执行");
            _updateVersionsQueue = new Queue<UpdateVersion>();
            foreach (UpdateVersion updateVersion in GetUpdateVersions(this.txtUpdate.Text))
            {
                _updateVersionsQueue.Enqueue(updateVersion);
            }
            if (_updateVersionsQueue.Count == 0)
            {
                MessageBox.Show("当前升级包为空，或者升级包不符合命名规范", "错误信息", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            // 进度条信息
            this.progressBarUpdate.Properties.Maximum = _updateVersionsQueue.Count * UpdateManager.UpdateService.Count * 10;
            this.progressBarUpdate.Properties.Minimum = 0;
            this.progressBarUpdate.Properties.Step = 10;

            StartUpdate(_updateVersionsQueue.Dequeue());
            this.btnStart.Enabled = false;
        }

        /// <summary>
        /// 开始升级
        /// </summary>
        /// <param name="updateVersion"></param>  
        private void StartUpdate(UpdateVersion updateVersion)
        {
            this._currentVersion = updateVersion;

            // 重新绑定升级时升级状态的变化事件
            foreach (UCUpdateState ucUpdateState in _ucUpdateStates)
            {
                IUpdateService updateServiceService = ((List<IUpdateService>)updateVersion.CurrentService).Find(delegate(IUpdateService update)
                    { return update.Title == ucUpdateState.Title; });
                if (updateServiceService.State != UpdateState.Success && updateServiceService.State != UpdateState.AfterUpdate)
                {
                    ucUpdateState.ResetPanelColor(); // 重置Panel 的色彩
                }
                // 注册升级状态发生变化事件 控件颜色变化
                updateServiceService.StateChange -= ucUpdateState.UpdateState;
                updateServiceService.StateChange += ucUpdateState.UpdateState;

                // 进度条信息变化
                updateServiceService.StateChange -= UpdateServiceServiceStateChange;
                updateServiceService.StateChange += UpdateServiceServiceStateChange;
            }
            updateVersion.RegisterMessage(ShowMessage);  // 注册消息事件
            this.ShowMessage(string.Format("本次升级版本为：{0}", updateVersion.Version));
            // 异步升级开始
            new MethodInvoker(delegate() { updateVersion.BeginUpdate(); }).BeginInvoke(UpdateCallBack, updateVersion);
        }
        /// <summary>
        /// UpdateService 状态发生变化
        /// </summary>
        /// <param name="updateServiceService"></param>
        void UpdateServiceServiceStateChange(IUpdateService updateServiceService)
        {
            if (this.InvokeRequired)
            {
                progressBarUpdate.Invoke(new MethodInvoker(delegate()
                    { UpdateServiceServiceStateChange(updateServiceService); }));
            }
            else
            {
                lblTotalState.Text = string.Format("{0}{1}{2}", _currentVersion, updateServiceService.Title,
                                                   Util.GetDescription(updateServiceService.State));
                if (updateServiceService.State == UpdateState.Success)
                {
                    this.progressBarUpdate.PerformStep();
                }
            }
        }
        /// <summary>
        /// 一个版本升级完成后执行事件
        /// </summary>
        /// <param name="result"></param>
        private void UpdateCallBack(IAsyncResult result)
        {
            UpdateVersion update = result.AsyncState as UpdateVersion;
            if (update != null)
            {
                if (update.UpdateServiceError != null)   // 升级失败处理
                {
                    // 将继续升级按钮置为可用
                    this.btnResetState.Invoke(new MethodInvoker(delegate() { btnResetState.Enabled = true; }));

                    string errorMessage = string.Format("【{0}】版本【{1}】升级失败，\r\n请修改升级文件,修改完后，请点击继续升级", update.Version,
                                                        update.UpdateServiceError.Title);

                    MessageBox.Show(errorMessage, "错误提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    lblTotalState.Invoke(new MethodInvoker(delegate() { lblTotalState.Text = string.Format("{0}{1}", _currentVersion, "升级完成"); }));

                    if (_updateVersionsQueue.Count > 0) // 执行下一版本升级
                    {
                        UpdateVersion nextUpdate = _updateVersionsQueue.Dequeue();
                        this._currentVersion = nextUpdate;

                        if (MessageBox.Show(string.Format("{0}升级完成，请查看详细日志，是否继续升级？", update.Version),
                            "提示", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {

                            // 继续升级下个
                            StartUpdate(nextUpdate);
                        }
                        else
                        {
                            // 暂停跟新下个版本  将继续升级按钮置为可用
                            this.btnResetState.Invoke(new MethodInvoker(delegate() { btnResetState.Enabled = true; }));
                        }
                    }
                    else // 全部版本升级完成
                    {
                        MessageBox.Show(string.Format("{0}升级完成，请查看详细日志", update.Version));
                    }
                }
            }
        }
        /// <summary>
        /// 获取升级版本
        /// </summary>
        /// <param name="folderName"></param>
        /// <returns></returns>
        private List<UpdateVersion> GetUpdateVersions(string folderName)
        {
            List<UpdateVersion> updateVersions = new List<UpdateVersion>();
            if (Regex.IsMatch(folderName, "_+"))   //选中的文件是单个升级包 报名称符合 产品名称_版本号
            {
                updateVersions.Add(new UpdateVersion(folderName, this.txtPro.Text));
            }
            else
            {
                string[] directories = Directory.GetDirectories(folderName);
                if (directories.Length > 0)
                {
                    foreach (string directory in directories)
                    {
                        updateVersions.Add(new UpdateVersion(directory, this.txtPro.Text));
                    }
                }
            }
            updateVersions.Sort(new UpdateVersion());  // 根据版本号排序
            return updateVersions;
        }
        #endregion

        #region 消息展现
        /// <summary>
        /// 消息展现
        /// </summary>
        /// <param name="message"></param>
        private void ShowMessage(string message)
        {
            message = string.Format("{0}:{1}\r\n", DateTime.Now, message);
            if (this.txbInfo.InvokeRequired)
            {
                txbInfo.Invoke(new MethodInvoker(delegate { txbInfo.AppendText(message); }));
            }
            else
            {
                txbInfo.AppendText(message);
            }
        }
        #endregion

        #region 界面控件事件
        /// <summary>
        /// 日志清空
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClear_Click(object sender, EventArgs e)
        {
            this.txbInfo.Clear();
        }
        /// <summary>
        ///  打开升级包地址
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOpenUpdate_Click(object sender, EventArgs e)
        {
            GetDirectoryName(this.txtUpdate);
        }
        /// <summary>
        /// 获取文件夹地址并给TextEdit赋值
        /// </summary>
        /// <param name="textEdit"></param>
        private void GetDirectoryName(TextEdit textEdit)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                textEdit.Text = dialog.SelectedPath;
            }
        }
        /// <summary>
        /// 打开中间件文件夹
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnProOpen_Click(object sender, EventArgs e)
        {
            GetDirectoryName(this.txtPro);
        }

        /// <summary>
        /// 打开日志文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txbInfo_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            string fileUrl = e.LinkText;
            if (fileUrl.StartsWith("file", StringComparison.CurrentCultureIgnoreCase))
            {
                fileUrl = fileUrl.Substring("file".Length);
            }
            Util.ExeCommand(Util.UrlDecode(fileUrl.TrimStart(new char[] { '/', ':' })));
        }
        /// <summary>
        /// 保存日志
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSaveLog_Click(object sender, EventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "文本文件(*.txt)|*.txt|日志文件(*.log)|*.log|所有文件(*.*)|*.*";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                this.txbInfo.SaveFile(dialog.FileName, RichTextBoxStreamType.PlainText);
            }
        }
        /// <summary>
        /// 重置升级标志
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnResetState_Click(object sender, EventArgs e)
        {
            this.StartUpdate(this._currentVersion);
            this.btnResetState.Enabled = false;
        }
        #endregion
    }

}