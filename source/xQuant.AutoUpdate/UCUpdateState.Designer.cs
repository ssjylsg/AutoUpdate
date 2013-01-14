namespace xQuant.AutoUpdate
{
    partial class UCUpdateState
    {
        /// <summary> 
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.lblName = new System.Windows.Forms.Label();
            this.panelState = new System.Windows.Forms.Panel();
            this.lblState = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lblName
            // 
            this.lblName.AutoSize = true;
            this.lblName.Location = new System.Drawing.Point(3, 9);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(29, 12);
            this.lblName.TabIndex = 0;
            this.lblName.Text = "Name";
            // 
            // panelState
            // 
            this.panelState.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.panelState.BackColor = System.Drawing.Color.Maroon;
            this.panelState.ForeColor = System.Drawing.Color.DarkRed;
            this.panelState.Location = new System.Drawing.Point(99, 0);
            this.panelState.Name = "panelState";
            this.panelState.Size = new System.Drawing.Size(139, 29);
            this.panelState.TabIndex = 1;
            // 
            // lblState
            // 
            this.lblState.AutoSize = true;
            this.lblState.Location = new System.Drawing.Point(243, 9);
            this.lblState.Name = "lblState";
            this.lblState.Size = new System.Drawing.Size(29, 12);
            this.lblState.TabIndex = 2;
            this.lblState.Text = "未知";
            // 
            // UCUpdateState
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lblState);
            this.Controls.Add(this.panelState);
            this.Controls.Add(this.lblName);
            this.Name = "UCUpdateState";
            this.Size = new System.Drawing.Size(319, 28);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.Panel panelState;
        private System.Windows.Forms.Label lblState;
    }
}
