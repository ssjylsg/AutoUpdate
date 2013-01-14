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
        /// <summary>
        /// 当前版本
        /// </summary>
        private UpdateVersion _currentVersion;
        /// <summary>
        /// 升级队列
        /// </summary>
        private Queue<UpdateVersion> _updateVersionsQueue;
        public FrmUpdate()
        {
            InitializeComponent();
            UpdateManager.RegisterStateChange(UpdateStateChange);
            AddControl();
        }
        /// <summary>
        /// UC控件
        /// </summary>
        private IList<UCUpdateState> _ucUpdateStates;
        /// <summary>
        /// 动态加载用户控件
        /// </summary>
        private void AddControl()
        {
            int count = 1;
            _ucUpdateStates = new List<UCUpdateState>();
            foreach (IUpdateService update in UpdateManager.UpdateService)
            {
                UCUpdateState ucUpdateState = new UCUpdateState(update);
                ucUpdateState.Location = new Point(0, count * 40);
                panel2.Controls.Add(ucUpdateState);
                update.StateChange += ucUpdateState.UpdateState;
                count++;
                _ucUpdateStates.Add(ucUpdateState);
            }
        }
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
            StartUpdate(_updateVersionsQueue.Dequeue(), true);
            this.btnStart.Enabled = false;
        }

        /// <summary>
        /// 开始升级
        /// </summary>
        /// <param name="updateVersion"></param>
        /// <param name="isResetPanelColor"> </param>
        private void StartUpdate(UpdateVersion updateVersion, bool isResetPanelColor)
        {
            this._currentVersion = updateVersion;

            // 重新绑定更新时更新状态的变化事件
            foreach (UCUpdateState ucUpdateState in _ucUpdateStates)
            {
                IUpdateService updateServiceService = ((List<IUpdateService>)updateVersion.CurrentService).Find(delegate(IUpdateService update)
                    { return update.Title == ucUpdateState.Title; });
                if (isResetPanelColor)
                {
                    ucUpdateState.ResetPanelColor(); // 重置Panel 的色彩
                }
                // 注册更新状态发生变化事件
                updateServiceService.StateChange -= ucUpdateState.UpdateState;
                updateServiceService.StateChange += ucUpdateState.UpdateState;
            }
            updateVersion.RegisterMessage(ShowMessage);  // 注册消息事件
            this.ShowMessage(string.Format("本次升级版本为：{0}", updateVersion.Version));
            // 异步更新开始
            new MethodInvoker(delegate() { updateVersion.BeginUpdate(); }).BeginInvoke(UpdateCallBack, updateVersion);
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
                if (update.UpdateServiceError != null)   // 更新失败处理
                {
                    // 将继续更新按钮置为可用
                    this.btnResetState.Invoke(new MethodInvoker(delegate() { btnResetState.Enabled = true; }));

                    string errorMessage = string.Format("【{0}】版本【{1}】升级失败，\r\n请修改升级文件,修改完后，请点击继续更新", update.Version,
                                                        update.UpdateServiceError.Title);

                    MessageBox.Show(errorMessage, "错误提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    if (_updateVersionsQueue.Count > 0) // 执行下一版本的更新
                    {
                        if (MessageBox.Show(string.Format("{0}升级完成，请查看详细日志，是否继续升级？", update.Version),
                            "", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            UpdateVersion nextUpdate = _updateVersionsQueue.Dequeue();
                            this._currentVersion = nextUpdate;
                            StartUpdate(nextUpdate, true);
                        }
                    }
                    else // 全部版本升级完成
                    {
                        MessageBox.Show(string.Format("{0}升级完成，请查看详细日志", update.Version));
                    }
                }
            }
        }
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


            updateVersions.Sort(new UpdateVersion());
            return updateVersions;
        }
        private void btnStop_Click(object sender, EventArgs e)
        {
            _currentVersion.Stop();
        }

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

        private void btnClear_Click(object sender, EventArgs e)
        {
            this.txbInfo.Clear();
        }

        private void btnOpenUpdate_Click(object sender, EventArgs e)
        {
            GetDirectoryName(this.txtUpdate);
        }
        private void GetDirectoryName(TextEdit textEdit)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                textEdit.Text = dialog.SelectedPath;
            }
        }

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
        /// 重置更新标志
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnResetState_Click(object sender, EventArgs e)
        {
            this.StartUpdate(this._currentVersion, false);
            this.btnResetState.Enabled = false;
        }
        /// <summary>
        /// 状态变化
        /// </summary>
        /// <param name="message"></param>
        private void UpdateStateChange(string message)
        {
            message = string.Format("{1}  {0}", message, _currentVersion);
            if (this.lblTotalState.InvokeRequired)
            {
                txbInfo.Invoke(new MethodInvoker(delegate { lblTotalState.Text = message; }));
            }
            else
            {
                lblTotalState.Text = message;
            }
        }
    }

}