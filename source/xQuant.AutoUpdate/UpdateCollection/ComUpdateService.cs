namespace xQuant.AutoUpdate
{
    /// <summary>
    /// Com ����
    /// </summary>
    public class ComUpdateService : BaseUpdateService
    {
        public override string Title
        {
            get { return "Com ����"; }
        }

        public override string DirectoryName
        {
            get { return "COM����"; }
        }

        public override void BeforeUpdate()
        {

        }

        public override void AfterUpdate()
        {

        }

        public override void ExecuteUpdate()
        {
            string[] files = System.IO.Directory.GetFiles(this.CurrentDirectory, "*.bat");
            if (files.Length > 0)
            {
                foreach (string file in files)
                {
                    this.OnShowMessage(Util.ExeCommand(file));
                }
            }
        }
    }
}
