using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace WcfServerMGR
{
  static class Program
  {
    public static string RunParam { get; set; }
    /// <summary>
    /// 应用程序的主入口点。
    /// </summary>
    [STAThread]
    static void Main(string[] args)
    {
      RunParam = string.Empty;
      if (args.Length > 0)
      {
        RunParam = args[0];
      }
      Application.EnableVisualStyles();
      Application.SetCompatibleTextRenderingDefault(false);
      Application.Run(new frm_SvcMgr());
    }
  }
}
