using System;
using System.Collections.Generic;
using System.IO;

namespace WcfService
{
  public abstract class FileSplitUpLoad
  {
    public const string Error_Msg = "ERROR";
    public const string Up_Load_Complete_Msg = "Complete";
    public const string Up_Load_Down_Msg = "OK";
    #region 文件分段上传处理
    public static string FilesUpLoad(string cachePath, string savePath, string fileName, string fileCount, string fileState, byte[] fileBuf)
    {
      string PicFileName = string.Format(
        "{0}\\{1}_{2}", cachePath, fileName, fileCount//F_Cnt  分段索引值，从１开始
        );
      if (File.Exists(PicFileName))
      {
        File.SetAttributes(PicFileName, FileAttributes.Normal);
      }
      try
      {
        File.WriteAllBytes(PicFileName, fileBuf);
      }
      catch (Exception e)
      {
        return string.Format("{0}={1}{2}", Error_Msg, "上传文件出错：", e.ToString());
      }
      if (fileState.Equals("1"))//F_State=1　文件上传结束
      {
        string _outMsg = string.Empty;
        if (!MarkFinshFile(fileName, fileCount, out _outMsg, cachePath, savePath))
        {
          return string.Format("{0}={1},{2}", Error_Msg, "文件合成失败", _outMsg);
        }
        else
        {
          return Up_Load_Complete_Msg;
        }
      }
      return Up_Load_Down_Msg;
    }
    /// <summary>
    /// 将被分割的缓存文件，合成最终文件
    /// </summary>
    /// <param name="model_id"></param>
    /// <param name="f_cnt"></param>
    /// <returns></returns>
    public static bool MarkFinshFile(string filename, string f_cnt, out string errorMsg, string cachePath, string savePath)
    {
      errorMsg = string.Empty;
      string PicFileName = string.Empty;
      int _i = 1, _iCnt = 0;
      int.TryParse(f_cnt, out _iCnt);
      _iCnt++;//文件记数从１开始
      List<byte> _btlis = new List<byte>();
      byte[] _bts = null;
      for (; _i < _iCnt; _i++)
      {
        PicFileName = string.Format(
          "{0}\\{1}_{2}",
          cachePath,
          filename,
          _i.ToString()
          );
        if (!File.Exists(PicFileName))//如果缓存文件不存在，取消合成
        {
          errorMsg = string.Format("不存在文件：{0}_{1}", filename, _i.ToString());
          _btlis.Clear();
          _btlis = null;
          return false;
        }
        else
        {
          _bts = File.ReadAllBytes(PicFileName);
          _btlis.AddRange(_bts);
          _bts = null;
        }
      }
      PicFileName = string.Format("{0}\\{1}", savePath, filename);
      File.WriteAllBytes(PicFileName, _btlis.ToArray());
      _btlis.Clear();
      _btlis = null;

      for (_i = 1; _i < _iCnt; _i++)
      {
        PicFileName = string.Format(
          "{0}\\{1}_{2}",
          cachePath,
          filename,
          _i.ToString()
          );
        try
        {
          File.Delete(PicFileName);
        }
        catch (Exception)
        {
        }
      }
      return true;
    }
    #endregion
  }
}
