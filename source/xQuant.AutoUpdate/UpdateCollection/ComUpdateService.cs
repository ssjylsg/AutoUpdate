namespace xQuant.AutoUpdate
{
    /// <summary>
    /// Com 更新
    /// </summary>
    public class ComUpdateService : BaseUpdateService
    {
        public override string Title
        {
            get { return "Com 更新"; }
        }

        public override string DirectoryName
        {
            get { return "COM更新"; }
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
