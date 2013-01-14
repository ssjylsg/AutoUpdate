using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace xQuant.AutoUpdate
{
    class ProgramUpdateService : BaseUpdateService
    {
        public override string Title
        {
            get { return "�м���������"; }
        }

        public override string DirectoryName
        {
            get { return "�м���������"; }
        }

        public override void BeforeUpdate()
        {
            if (Process.GetProcessesByName(Settings.AutoUpdateSetting.Default.ServerProgramName).Length > 0)
            {
                throw new Exception(string.Format("�м���������У����ȹر�"));
            }

            // �ļ���������
            string[] files = Directory.GetFiles(this.CurrentDirectory);
            foreach (string file in files)
            {
                FileInfo fileInfo = new FileInfo(file);
                // �ж������ļ���ԭ�м��ִ���ļ������Ƿ���ڣ�������ڣ�����Ҫ����
                if (new FileInfo(Path.Combine(this.TargetPath, fileInfo.Name)).Exists)
                {
                    fileInfo.CopyTo(Path.Combine(this.BackUpFullFolerName, fileInfo.Name));
                    this.OnShowMessage(string.Format("�ɹ����ݡ�{0}���ļ�", fileInfo.Name));
                }
            }
        }

        public override void AfterUpdate()
        {

        }

        public override void ExecuteUpdate()
        {
            // ִ���ļ�����
            string[] files = Directory.GetFiles(this.CurrentDirectory);
            foreach (string file in files)
            {
                FileInfo fileInfo = new FileInfo(file);
                fileInfo.CopyTo(Path.Combine(this.TargetPath, fileInfo.Name), true);
                this.OnShowMessage(string.Format("�ɹ�������{0}���ļ�", fileInfo.Name));
            }
        }
    }
}
