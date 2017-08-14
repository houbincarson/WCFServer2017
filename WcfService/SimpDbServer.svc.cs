using System.Collections.Generic;
using System.Data;
using System.Web.Script.Serialization;
using SimpDBHelper;
using WcfSimpData;
using System.Threading;
using System.IO;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Channels;
using System.ServiceModel.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace WcfService
{
    [System.ServiceModel.ServiceBehavior(InstanceContextMode = System.ServiceModel.InstanceContextMode.PerCall,ConcurrencyMode = System.ServiceModel.ConcurrencyMode.Multiple,UseSynchronizationContext = false)]
    public class SimpDbServer : ISimpDbServer
    {
        private static JavaScriptSerializer CreateJsonSerial()
        {
            return new JavaScriptSerializer { MaxJsonLength = 60000000 };
        }
        private const string ERROR_COLUMN_NAME = "ERROR";
        private const string SUDID_KEY_REGE = "MF7097G06704851-BFEBFBFF0001067A BFEBFBFF0001067A BFEBFBFF0001067A BFEBFBFF0001067A";

        #region ISimpDbServer 成员
        

        public string DataRequest_By_String(string methodRequests)
        {
            JavaScriptSerializer jsonserial = CreateJsonSerial();
#if SUDID_KEY_REGE
      if (!MachineCode.CurrentMachineCode.Hash.Equals(SUDID_KEY_REGE))
      {
        return null;
      }
#endif
            if (methodRequests.Equals("SUDID_KEY_REGE"))
            {
                return MachineCode.CurrentMachineCode.Hash;
            }
            else
            {
                MethodRequest _metodReq = jsonserial.Deserialize<MethodRequest>(methodRequests);
                ShareSqlManager _sh = new ShareSqlManager();
                switch (_metodReq.ProceName)
                {
                    case "Sys_MSG_Mgr":
                        DataTable _dt = _sh.ExecStoredProc(
                                              _metodReq.ProceName,
                                              _metodReq.ParamKeys,
                                              _metodReq.ParamVals,
                                              _metodReq.ProceDb,
                                              RetType.Table) as DataTable;
                        if (_dt != null && _dt.Rows.Count > 0)
                        {
                            if (_dt.Columns.IndexOf(ERROR_COLUMN_NAME) != -1)
                            {
                                return string.Format("{0}={1}", ERROR_COLUMN_NAME, _dt.Rows[0][ERROR_COLUMN_NAME]);
                            }
                            int _i = 0, _iCnt = _dt.Rows.Count;
                            string[][] _msgs = new string[_iCnt][];
                            for (; _i < _iCnt; _i++)
                            {
                                string _phone = string.Format("{0}", _dt.Rows[_i]["Phone"]).Trim(), _msg = string.Format("{0}", _dt.Rows[_i]["Msg"]).Trim();
                                if (_phone.Length > 0)
                                {
                                    _msgs[_i] = new string[] { _phone, _msg };
                                }
                            }
                            try
                            {
                                Web_Post_Msg_Helper.Send_MSG_By_QA_DB(
                                                                        _msgs
                                                                );
                            }
                            catch (System.Exception e)
                            {
                                return string.Format("{0}={1}", ERROR_COLUMN_NAME, e.ToString());
                            }
                            return "OK";
                        }
                        return ERROR_COLUMN_NAME;
                    default:
                        return _sh.ExecStoredProc(
                                              _metodReq.ProceName,
                                              _metodReq.ParamKeys,
                                              _metodReq.ParamVals,
                                              _metodReq.ProceDb,
                                              RetType.String).ToString();
                }
            }
            //private int InstanceVariable = 0;
            //private static int StaticVariable = 0;
            //  return string.Format("{0},{1},{2},{3}", 
            //    System.Threading.Thread.CurrentThread.ManagedThreadId,
            //    this.GetHashCode(),
            //    ++InstanceVariable,
            //    ++StaticVariable);
        }

        public DataTable DataRequest_By_DataTable(string methodRequests)
        {
            JavaScriptSerializer jsonserial = CreateJsonSerial();

#if SUDID_KEY_REGE
      if (!MachineCode.CurrentMachineCode.Hash.Equals(SUDID_KEY_REGE))
      {
        return null;
      }
#endif
            var metodReq = jsonserial.Deserialize<MethodRequest>(methodRequests);
            var sh = new ShareSqlManager();
            var dt = (DataTable)sh.ExecStoredProc(metodReq.ProceName,metodReq.ParamKeys,metodReq.ParamVals,metodReq.ProceDb,RetType.Table);
            dt.TableName = metodReq.ProceName;
            return dt;
        }

        public DataSet DataRequest_By_DataSet(string methodRequests)
        {
            JavaScriptSerializer jsonserial = CreateJsonSerial();
#if SUDID_KEY_REGE
      if (!MachineCode.CurrentMachineCode.Hash.Equals(SUDID_KEY_REGE))
      {
        return null;
      }
#endif
            var metodReq = jsonserial.Deserialize<MethodRequest>(methodRequests);
            var sh = new ShareSqlManager();
            var ds = (DataSet)sh.ExecStoredProc(metodReq.ProceName,metodReq.ParamKeys,metodReq.ParamVals,metodReq.ProceDb,RetType.DataSet);
            for (int i = 0, iCnt = ds.Tables.Count; i < iCnt; i++)
            {
                ds.Tables[i].TableName = string.Format("{0}_{1}", metodReq.ProceName, i);
            }
            return ds;
        }

        public byte[] DataRequest_By_SimpDEs_All_GZip(byte[] methodBts)
        {
#if SUDID_KEY_REGE
      if (!MachineCode.CurrentMachineCode.Hash.Equals(SUDID_KEY_REGE))
      {
        return null;
      }
#endif
            var metBts = GZipStreamHelper.GZipDecompress(methodBts);
            var jsonSimpEtys = DataRequest_By_SimpDEs(System.Text.Encoding.UTF8.GetString(metBts));
            var bts = System.Text.Encoding.UTF8.GetBytes(jsonSimpEtys);
            return GZipStreamHelper.GZipCompress(bts);
        }

        public string Get_String(string methodRequests)
        {
            return methodRequests;
        }

        public byte[] DataRequest_By_SimpDEs_GZip(string methodRequests)
        {
#if SUDID_KEY_REGE
      if (!MachineCode.CurrentMachineCode.Hash.Equals(SUDID_KEY_REGE))
      {
        return null;
      }
#endif
            var jsonSimpEtys = DataRequest_By_SimpDEs(methodRequests);
            var bts = System.Text.Encoding.UTF8.GetBytes(jsonSimpEtys);
           
            return GZipStreamHelper.GZipCompress(bts);//WCF消息传至客户端的时候会自动用Base64编码
        }
        public string DataRequest_By_SimpDEs(string methodRequests)
        {
            JavaScriptSerializer jsonserial = CreateJsonSerial();

#if SUDID_KEY_REGE
      if (!MachineCode.CurrentMachineCode.Hash.Equals(SUDID_KEY_REGE))
      {
        return null;
      }
#endif
            //System.ServiceModel.OperationContext.Current
            //if (System.ServiceModel.OperationContext.Current.SessionId.IndexOf("id=1") != -1)
            //{
            //  System.Threading.Thread.Sleep(1000 * 25);
            //}
            MethodRequest[] metodReqs = jsonserial.Deserialize<MethodRequest[]>(methodRequests);
            ShareSqlManager sh = new ShareSqlManager();
            List<List<SimpDataEntery>> simpEtys = new List<List<SimpDataEntery>>();
            for (int i = 0, iCnt = metodReqs.Length; i < iCnt; i++)
            {
                List<SimpDataEntery> lis =(List<SimpDataEntery>)sh.ExecStoredProc(metodReqs[i].ProceName,metodReqs[i].ParamKeys,metodReqs[i].ParamVals,metodReqs[i].ProceDb,RetType.SimpDEs);
                if (string.IsNullOrWhiteSpace(metodReqs[i].ProceName))
                {
                    throw (new System.Exception("调用名称为空."));
                }
                if (metodReqs[i].ProceName.Equals("Sys_BasicTable_Geter"))
                {
                    if (lis[2].Rows.Count == 0 && lis[1].Rows[0][0].Equals(string.Empty))
                    {
                        simpEtys.Add(null);
                        continue;
                    }

                    lis.Add
                    (
                      new SimpDataEntery()
                      {
                         Cols = new SimpDataColInf[] 
                         {
                           new SimpDataColInf ()
                           {
                                name = "MD5",
                                type = (DotNetType)System.Enum.Parse(typeof(DotNetType), "String") 
                           }
                         },
                         Rows = new List<object[]>() 
                         {
                            new object[]
                            {
                                BaseTablesCheck.GetMd5(MethodRequestHelper.GetParam(metodReqs[i],"tableName"),
                                MethodRequestHelper.GetParam(metodReqs[i],"ChangTime"),
                                metodReqs[i].ProceDb)
                            } 
                        }
                      }
                    );
                }
                simpEtys.Add(lis);
            }
            return jsonserial.Serialize(simpEtys);
        }

        #endregion


        public string GetPicture(string methodRequests)
        {
            /// 主要写你处理的代码；
            /// 1、前台上传的数据；进行反序列化成数据库请求的对象和约定的内容；
            List<List<SimpDataEntery>> result = new List<List<SimpDataEntery>>();
            List<SimpDataEntery> _lis = new List<SimpDataEntery>();
            var jsonserial = CreateJsonSerial();
#if SUDID_KEY_REGE
      if (!MachineCode.CurrentMachineCode.Hash.Equals(SUDID_KEY_REGE))
      {
        return null;
      }
#endif
            MethodRequest[] metodReq1 = jsonserial.Deserialize<MethodRequest[]>(methodRequests);
            MethodRequest metodReq = metodReq1[0];
            var sh = new ShareSqlManager();
            var _ds = (DataSet)sh.ExecStoredProc(metodReq.ProceName, metodReq.ParamKeys, metodReq.ParamVals, metodReq.ProceDb, RetType.DataSet);
            for (var i = 0; i < _ds.Tables.Count; i++)
            {
                _ds.Tables[i].TableName = string.Format("{0}_{1}", metodReq.ProceName, i);
            }

            if (_ds.Tables[0].Rows[0][0].ToString() == "1")
            {
                //直接返回结果，返回地址给客户端
                _lis.Add(SimpDataConvertHelper.DataTableToSimpDataEntery(_ds.Tables[0]));
                _lis.Add(SimpDataConvertHelper.DataTableToSimpDataEntery(_ds.Tables[1]));
                _lis.Add(SimpDataConvertHelper.DataTableToSimpDataEntery(_ds.Tables[2]));
                result.Add(_lis);
                _ds.Dispose();
                return jsonserial.Serialize(result);
            }
            else if (_ds.Tables[0].Rows[0][0].ToString() == "0")
            {
                //需要调用DcmToJpg算法，再返回地址给客户端
                DataSet ds = updatePicFile(metodReq.ParamVals[0], metodReq.ProceDb, _ds.Tables[1]);
                _lis.Add(SimpDataConvertHelper.DataTableToSimpDataEntery(ds.Tables[0]));
                _lis.Add(SimpDataConvertHelper.DataTableToSimpDataEntery(ds.Tables[1]));
                _lis.Add(SimpDataConvertHelper.DataTableToSimpDataEntery(ds.Tables[2]));
                result.Add(_lis);
                _ds.Dispose();
                return jsonserial.Serialize(result);
            }
            _ds.Dispose();
            return jsonserial.Serialize(result);

        }
        DataSet updatePicFile(string ptn_id_id, string ProceDb, DataTable dt)
        {
            string s = dt.Rows[0][1].ToString();
            string[] sArray = s.Split('\\');
            string[] sArray2 = s.Split('\\');
            string newPicfilePath = "";
            string newHttpPath = "";

            for (int i = 0; i < sArray.Length - 1; i++)
            {
                newPicfilePath += sArray[i] + @"\";
                newHttpPath += sArray2[i] + "/";
            }
            newPicfilePath = @"D:\RayPicture\" + newPicfilePath;
            newHttpPath = "http://218.17.30.5:8899/" + newHttpPath;
            //        newHttpPath = "http://192.168.1.115:8001/" + newHttpPath;


            string instanceId = "";
            string Error = "";
            //检查Dcm异常，大小是否合适，Dcm是否存在。
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                string dicomFile = @"D:\RayMage\" + dt.Rows[i][1].ToString();
                FileInfo file = new FileInfo(dicomFile);
                if (!file.Exists)
                {
                    //如果Dcm不存在,错误代码Error=3
                    instanceId = dt.Rows[i][0].ToString() + "," + instanceId;
                    Error = "3," + Error;
                }
                else
                {
                    //如果Dcm文件小于100KB,错误代码Error=1
                    if (file.Length < 102400)
                    {
                        instanceId = dt.Rows[i][0].ToString() + "," + instanceId;
                        Error = "1," + Error;
                    }
                }
            }

            //获取新的图片地址,刷新Http地址和Error
            string[] keys = new string[] { "ptn_id_id", "Picfilepath", "PicHttpPath", "instanceId", "Error" };
            string[] values = new string[] { ptn_id_id, newPicfilePath, newHttpPath, instanceId, Error };
            var sh = new ShareSqlManager();
            var _ds = (DataSet)sh.ExecStoredProc("JpgFileUriAdd", keys, values, ProceDb, RetType.DataSet);

            //后台线程生成图片
            Thread oGetArgThread = new Thread(new System.Threading.ThreadStart(() =>
            {
                if (Directory.Exists(newPicfilePath) == false)//如果不存在就创建file文件夹
                {
                    Directory.CreateDirectory(newPicfilePath);
                }
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    string dicomFile = @"D:\RayMage\" + dt.Rows[i][1].ToString();
                    FileInfo file = new FileInfo(dicomFile);
                    //判断dcm是否存在，和dcm是否小于100k
                    if (file.Exists)
                    {
                        long size = file.Length;
                        if (size > 102400)
                        {
                            DicomHandler reader = new DicomHandler(dicomFile);
                            try
                            {
                                string str = reader.readAndShow();
                                reader.getImg();
                                string savafilename = newPicfilePath + dt.Rows[i][0] + ".jpg";
                                reader.saveAs(savafilename);
                                reader.gdiImg.Dispose();
                            }
                            catch
                            {
                                //Dcm解析错误，Error=2
                                string[] keys1 = new string[] { "instance_uid", "Error", };
                                string[] values1 = new string[] { dt.Rows[i][0].ToString(), "2" };
                                var sh1 = new ShareSqlManager();
                                var _dt1 = (DataTable)sh.ExecStoredProc("JpgFileUriUpdate", keys1, values1, ProceDb, RetType.Table);
                            }
                            finally
                            {
                                System.GC.Collect();
                            }
                        }
                    }
                }
                System.GC.Collect();
            }));
            oGetArgThread.IsBackground = true;
            oGetArgThread.Start();

            return _ds;
        }
    }

    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)] 
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.PerSession)]
    public class JsonDbServer : IJsonDbServer
    {
        #region IJsonDbServer 成员 
        public string GetResultJson(string proceDb, string proceName, string[] paramKeys, string[] paramVals)
        {
            return  new ShareSqlManager().ExecStoredProc(proceName, paramKeys, paramVals, proceDb, RetType.Json).ToString(); 
        }
        public string GetSimpDEsJson(string proceDb, string proceName, string[] paramKeys, string[] paramVals)
        {
            var list = new ShareSqlManager().ExecStoredProc(proceName, paramKeys, paramVals, proceDb, RetType.SimpDEs);
            return JsonConvert.SerializeObject(list);
        }
        #endregion
    }

}
