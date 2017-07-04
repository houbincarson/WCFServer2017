using System;
using System.Management;

namespace WcfService
{
 internal   class MachineCode
{
  /// <summary>
  /// 获取当前机器的机器码
  /// </summary>
  public static MachineCode getInstance()
   {
     if (_CurrentMachineCode == null)
     {
       _CurrentMachineCode = new MachineCode();
     }
     return _CurrentMachineCode;
  }
  private static MachineCode _CurrentMachineCode=null;

  /// <summary>
  /// 静态构造
  /// </summary>
  private MachineCode()
  {
    try
    {
      ManagementObjectCollection collection = new ManagementClass("Win32_BaseBoard").GetInstances();

      foreach (ManagementBaseObject mo in collection)
      {
        if (mo.Properties["SerialNumber"] != null && mo.Properties["SerialNumber"].Value != null)
        {
          BaseBoardID += String.Format("{0} ", mo.Properties["SerialNumber"].Value.ToString().Trim());
        }
      }
      BaseBoardID = string.Format("{0}", BaseBoardID).Trim();

      collection = new ManagementClass("Win32_Processor").GetInstances();
      foreach (ManagementBaseObject mo in collection)
      {
        if (mo.Properties["ProcessorId"] != null && mo.Properties["ProcessorId"].Value != null)
        {
          CpuID += String.Format("{0} ", mo.Properties["ProcessorId"].Value.ToString().Trim());
        }
      }
      CpuID = string.Format("{0}", CpuID).Trim();

      collection = new ManagementClass("Win32_BIOS").GetInstances();
      foreach (ManagementBaseObject mo in collection)
      {
        if (mo.Properties["SerialNumber"] != null && mo.Properties["SerialNumber"].Value != null)
        {
          BiosID += String.Format("{0} ", mo.Properties["SerialNumber"].Value.ToString().Trim());
        }
      }
      BiosID = string.Format("{0}", BiosID).Trim();
      collection = new ManagementClass("Win32_PhysicalMedia").GetInstances();
      foreach (ManagementBaseObject mo in collection)
      {
        if (mo.Properties["SerialNumber"] != null && mo.Properties["SerialNumber"].Value != null)
        {
          HardDiskID += String.Format("{0} ", mo.Properties["SerialNumber"].Value.ToString().Trim());
        }
      }
      HardDiskID = string.Format("{0}", HardDiskID).Trim();
      collection = new ManagementClass("Win32_NetworkAdapterConfiguration").GetInstances();
      foreach (ManagementBaseObject mo in collection)
      {
        if (mo.Properties["MacAddress"] != null && mo.Properties["MacAddress"].Value != null)
        {
          Mac += String.Format("{0} ", mo.Properties["MacAddress"].Value.ToString().Trim());
        }
      }
      Mac = string.Format("{0}", Mac).Trim();
    }
    catch (Exception e)
    {
      System.Windows.Forms.MessageBox.Show(e.ToString());
    }
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
  /// 硬盘编号
  /// </summary>
  public String HardDiskID { get; set; }

  /// <summary>
  /// BIOS编号
  /// </summary>
  public String BiosID { get; set; }

  /// <summary>
  /// 网卡物理地址
  /// </summary>
  public String Mac { get; set; }

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
