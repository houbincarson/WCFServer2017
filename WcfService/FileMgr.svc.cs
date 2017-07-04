using System.Configuration;

namespace WcfService
{
  // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码、svc 和配置文件中的类名“FileMgr”。
  public class FileMgr : IFileMgr
  {
    private static string _ProdImgFile = ConfigurationManager.AppSettings["ProdImgFile"].ToString();
    private static string _SmallPic = ConfigurationManager.AppSettings["SmallPic"].ToString();
    private static string _ImgFile = ConfigurationManager.AppSettings["ImgFile"].ToString();
    private static string _ReadFile = ConfigurationManager.AppSettings["ReadFile"].ToString();
    private static string _RecentOrgFile = ConfigurationManager.AppSettings["RecentOrgFile"].ToString();
    private static string _RecentSmallFile = ConfigurationManager.AppSettings["RecentSmallFile"].ToString();
    private static string _RecentBigFile = ConfigurationManager.AppSettings["RecentBigFile"].ToString();
    public void DoWork()
    {
    }
  }
}
