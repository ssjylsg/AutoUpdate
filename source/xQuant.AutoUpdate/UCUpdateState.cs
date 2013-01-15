using System.Drawing;
using System.Windows.Forms;

namespace xQuant.AutoUpdate
{
    public partial class UCUpdateState : UserControl
    {
        public UCUpdateState(IUpdateService updateServiceService)
        {
            InitializeComponent();
            if (this.DesignMode)
            {
                return;
            }
            this.panelState.Height = this.Height;
            this.Title = updateServiceService.Title;
        }
        private Color _defaultColore = Color.AliceBlue;
        private string _title;
        public UCUpdateState()
        {
            InitializeComponent();
        }

        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                this.lblName.Text = _title;
            }
        }
        /// <summary>
        /// ����Panel ��ɫ��
        /// </summary>
        public void ResetPanelColor()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new MethodInvoker(delegate()
                    {
                        panelState.BackColor = _defaultColore;
                        this.lblState.Text = "δ֪";
                    }));
            }
            else
            {
                panelState.BackColor = _defaultColore;
                this.lblState.Text = "δ֪";
            }
        }

        /// <summary>
        /// ��������״̬�仯�¼�
        /// </summary>
        /// <param name="updateServiceService"></param>
        public void UpdateState(IUpdateService updateServiceService)
        {
            if (this.panelState.InvokeRequired)
            {
                this.panelState.Invoke(new MethodInvoker(delegate { UpdateState(updateServiceService); }));
            }
            else
            {
                UpdateState state = updateServiceService.State;
                panelState.BackColor = _defaultColore;

                switch (state)
                {
                    case AutoUpdate.UpdateState.Success:
                    case AutoUpdate.UpdateState.AfterUpdate:
                        panelState.BackColor = Color.Green;

                        break;
                    case AutoUpdate.UpdateState.Fail:
                        panelState.BackColor = Color.Red;
                        break;
                }
                this.lblState.Text = string.Format("{0}", Util.GetDescription(state));
            }
        }
    }
}
