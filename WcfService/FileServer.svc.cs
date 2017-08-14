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
        private static readonly object Sny = new object();
        private const string SUDID_KEY_REGE = "MF7097G06704851-BFEBFBFF0001067A BFEBFBFF0001067A BFEBFBFF0001067A BFEBFBFF0001067A";
        /// <summary>
        /// 创建图片文件存放路径
        /// </summary>
        private static string CreateFullFileCachePath(string fullPathName)
        {
            lock (Sny)
            {
                if (!Directory.Exists(fullPathName))
                {
                    Directory.CreateDirectory(fullPathName);
                }
                return fullPathName;
            };
        }
        #region 变量区
        private static readonly JavaScriptSerializer JsonSerial = new JavaScriptSerializer();
        private static string upappdll_path = System.AppDomain.CurrentDomain.BaseDirectory + "UpAppDlls";
        private string UPAPPDLL_PATH
        {
            get
            {
                return CreateFullFileCachePath(upappdll_path);
            }
        }
        private static readonly string UpappdllCachePath = System.AppDomain.CurrentDomain.BaseDirectory + "UpAppDlls_Cache";
        private string UPAPPDLL_CACHE_PATH
        {
            get
            {
                return CreateFullFileCachePath(UpappdllCachePath);
            }
        }
        private static readonly string ModelpicPath = System.AppDomain.CurrentDomain.BaseDirectory + "ModelPic";
        private string MODELPIC_PATH
        {
            get
            {
                return CreateFullFileCachePath(ModelpicPath);
            }
        }

        private static readonly string ModelpicCachePath = System.AppDomain.CurrentDomain.BaseDirectory + "ModelPic_Cache";
        private string MODELPIC_CACHE_PATH
        {
            get
            {
                return CreateFullFileCachePath(ModelpicCachePath);
            }
        }
        private static readonly string ReportfilePath = System.AppDomain.CurrentDomain.BaseDirectory + "ReportFiles";
        private string ReportFile_PATH
        {
            get
            {
                return CreateFullFileCachePath(ReportfilePath);
            }
        }


        private static readonly string RequestInfpicCachePath = System.AppDomain.CurrentDomain.BaseDirectory + "RequestPic_Cache";
        private string REQUESTPIC_CACHE_PATH
        {
            get
            {
                return CreateFullFileCachePath(RequestInfpicCachePath);
            }
        }


        private static readonly string RequestInfpicPath = System.AppDomain.CurrentDomain.BaseDirectory + "RequestPic";
        private string REQUESTPIC_PATH
        {
            get
            {
                return CreateFullFileCachePath(RequestInfpicPath);
            }
        }


        private static readonly string ProductpicCachePath = System.AppDomain.CurrentDomain.BaseDirectory + "ProductPic_Cache";
        private string PRODUCTPIC_CACHE_PATH
        {
            get
            {
                return CreateFullFileCachePath(ProductpicCachePath);
            }
        }


        private static readonly string ProductpicPath = System.AppDomain.CurrentDomain.BaseDirectory + "ProductPic";
        private string PRODUCTPIC_PATH
        {
            get
            {
                return CreateFullFileCachePath(ProductpicPath);
            }
        }

        private Dictionary<string, string> GetClientDllVersionInfos(string infs)
        {
            var dllInfs = infs.Split(',');
            var dlls = new Dictionary<string, string>();
            for (int i = 0, iCnt = dllInfs.Length; i < iCnt; i++)
            {
                var _infs = dllInfs[i].Split(':');
                dlls.Add(_infs[0].ToUpper(), _infs[1]);
            }
            return dlls;
        }
        #endregion

        #region IFileServer 文件传输函数
        /// <summary>
        /// 客户端下升级文件用
        /// </summary>
        /// <param name="infs"></param>
        /// <returns></returns>
        public List<FileEntery> LoadUpdataByte(string infs)
        {
#if SUDID_KEY_REGE
      if (!MachineCode.CurrentMachineCode.Hash.Equals(SUDID_KEY_REGE))
      {
        return null;
      }
#endif
            var dllFileEntys = new List<FileEntery>();
            var clientFileInfOs = GetClientDllVersionInfos(infs);//将文件名全转成大写
            string[] serverFiles = Directory.GetFiles(UPAPPDLL_PATH);
            for (int i = 0, iCnt = serverFiles.Length; i < iCnt; i++)
            {
                var severFileName = serverFiles[i].Substring(serverFiles[i].LastIndexOf('\\') + 1);
                var upperName = severFileName.ToUpper();
                if (clientFileInfOs.ContainsKey(upperName))//如果客户端存在就对比版本
                {
                    var fileVer = FileVersionInfo.GetVersionInfo(serverFiles[i]);
                    if (fileVer.FileVersion.Trim().ToUpper().Equals(clientFileInfOs[upperName].Trim().ToUpper()))
                    {
                        continue;
                    }
                }
                var bts = File.ReadAllBytes(serverFiles[i]);
                dllFileEntys.Add(new FileEntery() { Buff = GZipStreamHelper.GZipCompress(bts), Length = bts.Length, Name = severFileName });
            }

            return dllFileEntys;
        }

        /// <summary>
        /// 下截文件用
        /// </summary>
        /// <param name="methodRequests"></param>
        /// <returns></returns>
        public byte[] FileDownLoad(string methodRequests)
        {

#if SUDID_KEY_REGE
      if (!MachineCode.CurrentMachineCode.Hash.Equals(SUDID_KEY_REGE))
      {
        return null;
      }
#endif
            var metodReq = JsonSerial.Deserialize<MethodRequest>(methodRequests);
            byte[] bts = null;
            string filefullpath;
            switch (metodReq.ProceName)
            {
                case "DownLoad_MODELPIC_File":
                    filefullpath = string.Format("{0}\\{1}", MODELPIC_PATH, metodReq.ParamVals[0]);
                    if (File.Exists(filefullpath))
                    {
                        bts = File.ReadAllBytes(filefullpath);
                    }
                    break;
                case "DownLoad_RequsetPIC_File":
                    filefullpath = string.Format("{0}\\{1}", REQUESTPIC_PATH, metodReq.ParamVals[0]);
                    if (File.Exists(filefullpath))
                    {
                        bts = File.ReadAllBytes(filefullpath);
                    }
                    break;
                case "DownLoad_ProductPIC_File":
                    filefullpath = string.Format("{0}\\{1}", PRODUCTPIC_PATH, metodReq.ParamVals[0]);
                    if (File.Exists(filefullpath))
                    {
                        bts = File.ReadAllBytes(filefullpath);
                    }
                    break;
                case "DownLoad_Report_File":
                    filefullpath = string.Format("{0}\\{1}", ReportFile_PATH, metodReq.ParamVals[0]);
                    if (File.Exists(filefullpath))
                    {
                        bts = File.ReadAllBytes(filefullpath);
                    }
                    bts = GZipStreamHelper.GZipCompress(bts);
                    break;
                case "DownLoad_SYSDLL_File":
                    filefullpath = string.Format("{0}\\{1}", UPAPPDLL_PATH, metodReq.ParamVals[0]);
                    if (File.Exists(filefullpath))
                    {
                        bts = File.ReadAllBytes(filefullpath);
                    }
                    bts = GZipStreamHelper.GZipCompress(bts);
                    break;
                default:
                    var setPath = ConfigurationManager.AppSettings[metodReq.ProceName];
                    if (!string.IsNullOrWhiteSpace(setPath))
                    {
                        var savePath = System.AppDomain.CurrentDomain.BaseDirectory + setPath;
                        filefullpath = string.Format("{0}\\{1}", savePath, metodReq.ParamVals[0]);
                        if (File.Exists(filefullpath))
                        {
                            bts = File.ReadAllBytes(filefullpath);
                        }
                    }
                    break;
            }
            return bts;
        }
        /// <summary>
        /// 上传文件用
        /// </summary>
        /// <param name="fileBuf"></param>
        /// <param name="methodRequests"></param>
        /// <returns></returns>
        public string FileUpLoad(byte[] fileBuf, string methodRequests)
        {
#if SUDID_KEY_REGE
      if (!MachineCode.CurrentMachineCode.Hash.Equals(SUDID_KEY_REGE))
      {
        return null;
      }
#endif
            var metodReq = JsonSerial.Deserialize<MethodRequest>(methodRequests);
            string result;
            switch (metodReq.ProceName)
            {
                case "UpLoad_MODELPIC_File":
                    result = FileSplitUpLoad.FilesUpLoad(MODELPIC_CACHE_PATH,MODELPIC_PATH,metodReq.ParamVals[0],metodReq.ParamVals[1],metodReq.ParamVals[2],fileBuf);

                    if (result.Equals(FileSplitUpLoad.UpLoadCompleteMsg))
                    {
                        var sh = new ShareSqlManager();
                        var dt = (DataTable)sh.ExecStoredProc(
                            "Prop_ModelMgr_Editer",
                            new string[] { "Mod_Id", "Flag" },
                            new string[] { metodReq.ParamVals[0], "2" },
                            metodReq.ProceDb,
                            RetType.Table);
                        if (dt.Columns.IndexOf("ERROR") != -1)
                        {
                            return string.Format("{0}={1}", FileSplitUpLoad.ErrorMsg, dt.Rows[0]["ERROR"]);
                        }
                        return dt.Rows[0][0].ToString();
                    }
                    break; 
                case "UpLoad_RequsetPIC_File":
                    result = FileSplitUpLoad.FilesUpLoad(REQUESTPIC_CACHE_PATH,REQUESTPIC_PATH,metodReq.ParamVals[0],metodReq.ParamVals[1],metodReq.ParamVals[2],fileBuf);
                    return result;
                case "UpLoad_ProductPIC_File": 
                    result = FileSplitUpLoad.FilesUpLoad(PRODUCTPIC_CACHE_PATH,PRODUCTPIC_PATH,metodReq.ParamVals[0],metodReq.ParamVals[1],metodReq.ParamVals[2],
                                            fileBuf);

                    if (result.Equals(FileSplitUpLoad.UpLoadCompleteMsg))
                    {
                        var sh = new ShareSqlManager();
                        var dt = (DataTable)sh.ExecStoredProc(
                                              "Prop_ModelMgr_Editer",
                                              new string[] { "Mod_Id", "Flag" },
                                              new string[] { metodReq.ParamVals[0], "20" },
                                              metodReq.ProceDb,
                                              RetType.Table);
                        if (dt.Columns.IndexOf("ERROR") != -1)
                        {
                            return string.Format("{0}={1}", FileSplitUpLoad.ErrorMsg, dt.Rows[0]["ERROR"]);
                        }
                        return dt.Rows[0][0].ToString();
                    }
                    break; 
                case "UpLoad_SYSDLL_File":
                    result = FileSplitUpLoad.FilesUpLoad(UPAPPDLL_CACHE_PATH,UPAPPDLL_PATH,metodReq.ParamVals[0],metodReq.ParamVals[1],metodReq.ParamVals[2],fileBuf);
                    if (result.Equals(FileSplitUpLoad.UpLoadCompleteMsg))
                    {
                        return result;
                    }
                    break;
                default:
                    var setPath = ConfigurationManager.AppSettings[metodReq.ProceName];
                    if (!string.IsNullOrWhiteSpace(setPath))
                    {
                        var cachePath = System.AppDomain.CurrentDomain.BaseDirectory + setPath + "_Cache";
                        var savePath = System.AppDomain.CurrentDomain.BaseDirectory + setPath;
                        CreateFullFileCachePath(cachePath);
                        CreateFullFileCachePath(savePath);
                        result = FileSplitUpLoad.FilesUpLoad(cachePath,savePath,metodReq.ParamVals[0],metodReq.ParamVals[1],metodReq.ParamVals[2],fileBuf); 
                        if (result.Equals(FileSplitUpLoad.UpLoadCompleteMsg))
                        {
                            return result;
                        }
                    }
                    break;
            }
            return string.Empty;
        }

        #endregion

    }
}
