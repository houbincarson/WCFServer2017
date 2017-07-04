using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Management;

namespace WcfService
{
 public   class MachineCode
{
  /// <summary>
  /// 获取当前机器的机器码
  /// </summary>
  public static MachineCode CurrentMachineCode
  {
    get
    {
      if (_CurrentMachineCode == null)
      {
        _CurrentMachineCode = new MachineCode();
      }
      return _CurrentMachineCode;
    }
  }
  private static MachineCode _CurrentMachineCode;

  /// <summary>
  /// 静态构造
  /// </summary>
  private MachineCode()
  {
    ManagementObjectCollection collection = new ManagementClass("Win32_BaseBoard").GetInstances();

    foreach (ManagementBaseObject mo in collection)
    {
      if (mo.Properties["SerialNumber"] != null && mo.Properties["SerialNumber"].Value != null)
      {
        BaseBoardID += String.Format("{0} ", mo.Properties["SerialNumber"].Value.ToString().Trim());
      }
    }
    BaseBoardID = BaseBoardID.Trim();

    collection = new ManagementClass("Win32_Processor").GetInstances();
    foreach (ManagementBaseObject mo in collection)
    {
      if (mo.Properties["ProcessorId"] != null && mo.Properties["ProcessorId"].Value != null)
      {
        CpuID += String.Format("{0} ", mo.Properties["ProcessorId"].Value.ToString().Trim());
      }
    }
    CpuID = CpuID.Trim();
  }

  /// <summary>
  /// Cpu编号，用来表明CPU型号
  /// </summary>
  public String CpuID { get; set; }

  /// <summary>
  /// 主板编号
  /// </summary>
  public String BaseBoardID { get; set; }

  /// <summary>
  /// 根据硬件信息计算出的 哈希码，期望针对每一计算机唯一
  /// </summary>
  public String Hash
  {
    get
    {
      return String.Format("{0}-{1}", BaseBoardID, CpuID);
      //return String.Format("{0}-{1}-{2}-{3}", BaseBoardID, CpuID, BiosID, HardDiskID);//.GetHashCode().ToString();
    }
  }
}
}
