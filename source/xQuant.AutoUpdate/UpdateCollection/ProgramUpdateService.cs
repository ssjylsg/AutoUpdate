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
            get { return "中间件程序更新"; }
        }

        public override string DirectoryName
        {
            get { return "中间件程序更新"; }
        }

        public override void BeforeUpdate()
        {
            if (Process.GetProcessesByName(Settings.AutoUpdateSetting.Default.ServerProgramName).Length > 0)
            {
                throw new Exception(string.Format("中间件还在运行，请先关闭"));
            }

            // 文件拷贝备份
            string[] files = Directory.GetFiles(this.CurrentDirectory);
            foreach (string file in files)
            {
                FileInfo fileInfo = new FileInfo(file);
                // 判断升级文件在原中间件执行文件夹中是否存在，如果存在，则需要备份
                if (new FileInfo(Path.Combine(this.TargetPath, fileInfo.Name)).Exists)
                {
                    fileInfo.CopyTo(Path.Combine(this.BackUpFullFolerName, fileInfo.Name));
                    this.OnShowMessage(string.Format("成功备份【{0}】文件", fileInfo.Name));
                }
            }
        }

        public override void AfterUpdate()
        {

        }

        public override void ExecuteUpdate()
        {
            // 执行文件拷贝
            string[] files = Directory.GetFiles(this.CurrentDirectory);
            foreach (string file in files)
            {
                FileInfo fileInfo = new FileInfo(file);
                fileInfo.CopyTo(Path.Combine(this.TargetPath, fileInfo.Name), true);
                this.OnShowMessage(string.Format("成功拷贝【{0}】文件", fileInfo.Name));
            }
        }
    }
}
