namespace xQuant.AutoUpdate
{
    /// <summary>
    /// Com Éý¼¶
    /// </summary>
    public class ComUpdateService : BaseUpdateService
    {
        public override string Title
        {
            get { return "Com Éý¼¶"; }
        }

        public override string DirectoryName
        {
            get { return "COMÉý¼¶"; }
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
