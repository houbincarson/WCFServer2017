using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Web.Script.Serialization;
using SimpDBHelper;
using WcfSimpData;
using System.Configuration;

namespace WcfService
{
  public class FileServer : IFileServer
  {
    private static readonly object _Sny = new object();
    private const string SUDID_KEY_REGE = "MF7097G06704851-BFEBFBFF0001067A BFEBFBFF0001067A BFEBFBFF0001067A BFEBFBFF0001067A";
    /// <summary>
    /// 创建图片文件存放路径
    /// </summary>
    private static string CreateFullFileCachePath(string fullPathName)
    {
      lock (_Sny)
      {
        if (!Directory.Exists(fullPathName))
        {
          Directory.CreateDirectory(fullPathName);
        }
        return fullPathName;
      };
    }
    #region 变量区
    private static JavaScriptSerializer __JsonSerial = new JavaScriptSerializer();
    private static string upappdll_path = System.AppDomain.CurrentDomain.BaseDirectory + "UpAppDlls";
    private string UPAPPDLL_PATH
    {
      get
      {
        return CreateFullFileCachePath(upappdll_path);
      }
    }
    private static string upappdll_cache_path = System.AppDomain.CurrentDomain.BaseDirectory + "UpAppDlls_Cache";
    private string UPAPPDLL_CACHE_PATH
    {
      get
      {
        return CreateFullFileCachePath(upappdll_cache_path);
      }
    }
    private static string modelpic_path = System.AppDomain.CurrentDomain.BaseDirectory + "ModelPic";
    private string MODELPIC_PATH
    {
      get
      {
        return CreateFullFileCachePath(modelpic_path);
      }
    }

    private static string modelpic_cache_path = System.AppDomain.CurrentDomain.BaseDirectory + "ModelPic_Cache";
    private string MODELPIC_CACHE_PATH
    {
      get
      {
        return CreateFullFileCachePath(modelpic_cache_path);
      }
    }
    private static string reportfile_path = System.AppDomain.CurrentDomain.BaseDirectory + "ReportFiles";
    private string ReportFile_PATH
    {
      get
      {
        return CreateFullFileCachePath(reportfile_path);
      }
    }


    private static string requestInfpic_cache_path = System.AppDomain.CurrentDomain.BaseDirectory + "RequestPic_Cache";
    private string REQUESTPIC_CACHE_PATH
    {
      get
      {
        return CreateFullFileCachePath(requestInfpic_cache_path);
      }
    }


    private static string requestInfpic_path = System.AppDomain.CurrentDomain.BaseDirectory + "RequestPic";
    private string REQUESTPIC_PATH
    {
      get
      {
        return CreateFullFileCachePath(requestInfpic_path);
      }
    }


    private static string productpic_cache_path = System.AppDomain.CurrentDomain.BaseDirectory + "ProductPic_Cache";
    private string PRODUCTPIC_CACHE_PATH
    {
      get
      {
        return CreateFullFileCachePath(productpic_cache_path);
      }
    }


    private static string productpic_path = System.AppDomain.CurrentDomain.BaseDirectory + "ProductPic";
    private string PRODUCTPIC_PATH
    {
      get
      {
        return CreateFullFileCachePath(productpic_path);
      }
    }

    private Dictionary<string, string> GetClientDllVersionInfos(string Infs)
    {
      string[] _dllInfs = Infs.Split(',');
      Dictionary<string, string> _dlls = new Dictionary<string, string>();
      for (int _i = 0, _iCnt = _dllInfs.Length; _i < _iCnt; _i++)
      {
        string[] _infs = _dllInfs[_i].Split(':');
        _dlls.Add(_infs[0].ToUpper(), _infs[1]);
      }
      return _dlls;
    }
    #endregion

    #region IFileServer 文件传输函数
    /// <summary>
    /// 客户端下升级文件用
    /// </summary>
    /// <param name="Infs"></param>
    /// <returns></returns>
    public List<FileEntery> LoadUpdataByte(string Infs)
    {
#if SUDID_KEY_REGE
      if (!MachineCode.CurrentMachineCode.Hash.Equals(SUDID_KEY_REGE))
      {
        return null;
      }
#endif
      List<FileEntery> _dllFileEntys = new List<FileEntery>();
      Dictionary<string, string> _client_File_INFOs = GetClientDllVersionInfos(Infs);//将文件名全转成大写
      string[] _server_Files = Directory.GetFiles(UPAPPDLL_PATH);
      for (int _i = 0, _iCnt = _server_Files.Length; _i < _iCnt; _i++)
      {
        string _severFileName = _server_Files[_i].Substring(_server_Files[_i].LastIndexOf('\\') + 1);
        string _upperName = _severFileName.ToUpper();
        if (_client_File_INFOs.ContainsKey(_upperName))//如果客户端存在就对比版本
        {
          FileVersionInfo _fileVer = FileVersionInfo.GetVersionInfo(_server_Files[_i]);
          if (_fileVer.FileVersion.Trim().ToUpper().Equals(
            _client_File_INFOs[_upperName].Trim().ToUpper()
            ))
          {
            continue;
          }
        }
        byte[] _bts = File.ReadAllBytes(_server_Files[_i]); 
        _dllFileEntys.Add(new FileEntery() { Buff = GZipStreamHelper.GZipCompress(_bts), Length = _bts.Length, Name = _severFileName });
      }

      return _dllFileEntys;
    }

    /// <summary>
    /// 下截文件用
    /// </summary>
    /// <param name="_methodRequests"></param>
    /// <returns></returns>
    public byte[] FileDownLoad(string _methodRequests)
    {

#if SUDID_KEY_REGE
      if (!MachineCode.CurrentMachineCode.Hash.Equals(SUDID_KEY_REGE))
      {
        return null;
      }
#endif
      MethodRequest _metodReq = __JsonSerial.Deserialize<MethodRequest>(_methodRequests);
      byte[] _bts = null;
      string _filefullpath = string.Empty;
      switch (_metodReq.ProceName)
      {
        /// <summary>
        /// 下截款式图片
        /// </summary>
        case "DownLoad_MODELPIC_File":
          _filefullpath = string.Format("{0}\\{1}", MODELPIC_PATH, _metodReq.ParamVals[0]);
          if (File.Exists(_filefullpath))
          {
            _bts = File.ReadAllBytes(_filefullpath);
          }
          break;
        /// <summary>
        /// 下截订单图片
        /// </summary>
        case "DownLoad_RequsetPIC_File":
          _filefullpath = string.Format("{0}\\{1}", REQUESTPIC_PATH, _metodReq.ParamVals[0]);
          if (File.Exists(_filefullpath))
          {
            _bts = File.ReadAllBytes(_filefullpath);
          }
          break;
        /// <summary>
        /// 下截单件产品图片
        /// </summary>
        case "DownLoad_ProductPIC_File":
          _filefullpath = string.Format("{0}\\{1}", PRODUCTPIC_PATH, _metodReq.ParamVals[0]);
          if (File.Exists(_filefullpath))
          {
            _bts = File.ReadAllBytes(_filefullpath);
          }
          break;
        /// <summary>
        /// 下截报表
        /// </summary>
        case "DownLoad_Report_File":
          _filefullpath = string.Format("{0}\\{1}", ReportFile_PATH, _metodReq.ParamVals[0]);
          if (File.Exists(_filefullpath))
          {
            _bts = File.ReadAllBytes(_filefullpath);
          }
          _bts = GZipStreamHelper.GZipCompress(_bts);
          break;
        /// <summary>
        /// 下截SYS DLL
        /// </summary>
        case "DownLoad_SYSDLL_File":
          _filefullpath = string.Format("{0}\\{1}", UPAPPDLL_PATH, _metodReq.ParamVals[0]);
          if (File.Exists(_filefullpath))
          {
            _bts = File.ReadAllBytes(_filefullpath);
          }
          _bts = GZipStreamHelper.GZipCompress(_bts);
          break;
        default:
          string _setPath = ConfigurationManager.AppSettings[_metodReq.ProceName];
          if (!string.IsNullOrWhiteSpace(_setPath))
          {
            string _savePath = System.AppDomain.CurrentDomain.BaseDirectory + _setPath;
            _filefullpath = string.Format("{0}\\{1}", _savePath, _metodReq.ParamVals[0]);
            if (File.Exists(_filefullpath))
            {
              _bts = File.ReadAllBytes(_filefullpath);
            }
            //_bts = GZipStreamHelper.GZipCompress(_bts);
          }
          break;
      }
      return _bts;
    }
    /// <summary>
    /// 上传文件用
    /// </summary>
    /// <param name="fileBuf"></param>
    /// <param name="_methodRequests"></param>
    /// <returns></returns>
    public string FileUpLoad(byte[] fileBuf, string _methodRequests)
    {
#if SUDID_KEY_REGE
      if (!MachineCode.CurrentMachineCode.Hash.Equals(SUDID_KEY_REGE))
      {
        return null;
      }
#endif
      MethodRequest _metodReq = __JsonSerial.Deserialize<MethodRequest>(_methodRequests);
      string _result = string.Empty;
      switch (_metodReq.ProceName)
      {
        /// <summary>
        /// 上传款式图片
        /// </summary>
        case "UpLoad_MODELPIC_File":
          _result = FileSplitUpLoad.FilesUpLoad(
                                    MODELPIC_CACHE_PATH,
                                    MODELPIC_PATH,
                                  _metodReq.ParamVals[0],
                                  _metodReq.ParamVals[1],
                                  _metodReq.ParamVals[2],
                                  fileBuf);

          if (_result.Equals(FileSplitUpLoad.Up_Load_Complete_Msg))
          {
            ShareSqlManager _sh = new ShareSqlManager();
            DataTable _dt = (DataTable)_sh.ExecStoredProc(
                                  "Prop_ModelMgr_Editer",
                                  new string[] { "Mod_Id", "Flag" },
                                  new string[] { _metodReq.ParamVals[0], "2" },
                                  _metodReq.ProceDb,
                                  RetType.Table);
            if (_dt.Columns.IndexOf("ERROR") != -1)
            {
              return string.Format("{0}={1}", FileSplitUpLoad.Error_Msg, _dt.Rows[0]["ERROR"]);
            }
            return _dt.Rows[0][0].ToString();
          }
          break;
        /// <summary>
        /// 上传订单图片
        /// </summary>
        case "UpLoad_RequsetPIC_File":

          _result = FileSplitUpLoad.FilesUpLoad(
                                    REQUESTPIC_CACHE_PATH,
                                    REQUESTPIC_PATH,
                                  _metodReq.ParamVals[0],
                                  _metodReq.ParamVals[1],
                                  _metodReq.ParamVals[2],
                                  fileBuf);
          return _result;
        /// <summary>
        /// 上单件产品图片
        /// </summary>
        case "UpLoad_ProductPIC_File":

          _result = FileSplitUpLoad.FilesUpLoad(
                                    PRODUCTPIC_CACHE_PATH,
                                    PRODUCTPIC_PATH,
                                  _metodReq.ParamVals[0],
                                  _metodReq.ParamVals[1],
                                  _metodReq.ParamVals[2],
                                  fileBuf);

          if (_result.Equals(FileSplitUpLoad.Up_Load_Complete_Msg))
          {
            ShareSqlManager _sh = new ShareSqlManager();
            DataTable _dt = (DataTable)_sh.ExecStoredProc(
                                  "Prop_ModelMgr_Editer",
                                  new string[] { "Mod_Id", "Flag" },
                                  new string[] { _metodReq.ParamVals[0], "20" },
                                  _metodReq.ProceDb,
                                  RetType.Table);
            if (_dt.Columns.IndexOf("ERROR") != -1)
            {
              return string.Format("{0}={1}", FileSplitUpLoad.Error_Msg, _dt.Rows[0]["ERROR"]);
            }
            return _dt.Rows[0][0].ToString();
          }
          break;
        /// <summary>
        /// 上 SYS DLL
        /// </summary>
        case "UpLoad_SYSDLL_File":

          _result = FileSplitUpLoad.FilesUpLoad(
                                    UPAPPDLL_CACHE_PATH,
                                    UPAPPDLL_PATH,
                                  _metodReq.ParamVals[0],
                                  _metodReq.ParamVals[1],
                                  _metodReq.ParamVals[2],
                                  fileBuf);

          if (_result.Equals(FileSplitUpLoad.Up_Load_Complete_Msg))
          {
            return _result;
          }
          break;
        default:
          string _setPath = ConfigurationManager.AppSettings[_metodReq.ProceName];
          if (!string.IsNullOrWhiteSpace(_setPath))
          {
            string _cachePath = System.AppDomain.CurrentDomain.BaseDirectory + _setPath + "_Cache";
            string _savePath = System.AppDomain.CurrentDomain.BaseDirectory + _setPath;
            CreateFullFileCachePath(_cachePath);
            CreateFullFileCachePath(_savePath);
            _result = FileSplitUpLoad.FilesUpLoad(
                                      _cachePath,
                                      _savePath,
                                    _metodReq.ParamVals[0],
                                    _metodReq.ParamVals[1],
                                    _metodReq.ParamVals[2],
                                    fileBuf);

            if (_result.Equals(FileSplitUpLoad.Up_Load_Complete_Msg))
            {
              return _result;
            }
          }
          break;
      }
      return string.Empty;//
    }

    #endregion

  }
}
