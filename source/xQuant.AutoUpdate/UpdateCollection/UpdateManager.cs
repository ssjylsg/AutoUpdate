using System;
using System.Collections.Generic;

namespace xQuant.AutoUpdate
{
    /// <summary>
    /// ��������
    /// </summary>
    public class UpdateManager
    {
        /// <summary>
        /// �Ƿ�����ִ����
        /// </summary>
        private bool _running;
        /// <summary>
        /// ��Ϣ��ʾ
        /// </summary>
        private static ShowMessageHandler _showMessage;
        /// <summary>
        /// ��������
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
        /// ��ȡ���з���
        /// </summary>
        public static IList<IUpdateService> UpdateService
        {
            get { return new UpdateManager().CurrentService; }
        }
        /// <summary>
        /// ע����Ϣ֪ͨ�¼�
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
        /// ��ʼ��������
        /// </summary>
        public void Start()
        {
            if (_running)
            {
                return;
            }

            _running = true;

            ShowMessage(string.Format("�������������ļ���ַ{0}", BackupFolder));
            foreach (IUpdateService update in _updateService)
            {
                if (_running)
                {
                    try
                    {
                        if (update.State != UpdateState.Success && update.State != UpdateState.AfterUpdate)
                        {
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
                        return;
                    }
                    ShowMessage("������ɣ���鿴��ϸ��־");
                }
            }

            _running = false;
        }

        /// <summary>
        /// ��Ϣ��ʾ
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
        /// ֹͣ����
        /// </summary>
        public void Stop()
        {
            _running = false;
        }
        private string _updateFolderName;
        /// <summary>
        /// �������������ڵ�ַ
        /// </summary>
        public string UpdateFolderName { get { return _updateFolderName; } set { _updateFolderName = value; } }
        /// <summary>
        /// �м�����������ļ��е�ַ
        /// </summary>
        public string ProgramFolderName
        {
            get { return _programFolderName; }
            set { _programFolderName = value; }
        }
        /// <summary>
        /// �����ļ��е�ַ  ȫ·��
        /// </summary>
        public string BackupFolder
        {
            get { return _backupFolder; }
            set { _backupFolder = value; }
        }

        private string _programFolderName;


    }
}
