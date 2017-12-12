using System;
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
using log4net.Config;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace WcfService
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall,ConcurrencyMode =ConcurrencyMode.Multiple,UseSynchronizationContext = false)]
    public class SimpDbServer : ISimpDbServer
    {
        private FileInfo logCfg = new FileInfo(AppDomain.CurrentDomain.BaseDirectory + "log4net.config");
        private log4net.ILog log = log4net.LogManager.GetLogger("testApp.Logging");

        SimpDbServer()
        {
            XmlConfigurator.ConfigureAndWatch(logCfg);
        } 
       

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
           
            log.Info(methodRequests);

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
            log.Info(methodRequests);
            var metodReq = jsonserial.Deserialize<MethodRequest>(methodRequests);
            var sh = new ShareSqlManager();
            var dt = (DataTable)sh.ExecStoredProc(metodReq.ProceName, metodReq.ParamKeys, metodReq.ParamVals, metodReq.ProceDb, RetType.Table);
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
            log.Info(methodRequests);
            var metodReq = jsonserial.Deserialize<MethodRequest>(methodRequests);
            var sh = new ShareSqlManager();
            var ds = (DataSet)sh.ExecStoredProc(metodReq.ProceName, metodReq.ParamKeys, metodReq.ParamVals, metodReq.ProceDb, RetType.DataSet);
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
        public byte[] DataRequest_By_SimpDEs_GZip(string methodRequests)
        {
#if SUDID_KEY_REGE
      if (!MachineCode.CurrentMachineCode.Hash.Equals(SUDID_KEY_REGE))
      {
        return null;
      }
#endif
            log.Info(methodRequests);

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
            log.Info(methodRequests);


            MethodRequest[] metodReqs = jsonserial.Deserialize<MethodRequest[]>(methodRequests);
            ShareSqlManager sh = new ShareSqlManager();
            List<List<SimpDataEntery>> simpEtys = new List<List<SimpDataEntery>>();
            for (int i = 0, iCnt = metodReqs.Length; i < iCnt; i++)
            {
                List<SimpDataEntery> lis = (List<SimpDataEntery>)sh.ExecStoredProc(metodReqs[i].ProceName, metodReqs[i].ParamKeys, metodReqs[i].ParamVals, metodReqs[i].ProceDb, RetType.SimpDEs);
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

      }

    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.PerSession)]
    public class JsonDbServer : IJsonDbServer
    {
        log4net.ILog log = log4net.LogManager.GetLogger("testApp.logging");
        #region IJsonDbServer 成员
        public string GetResultJson(string proceDb, string proceName, string[] paramKeys, string[] paramVals)
        {
            return new ShareSqlManager().ExecStoredProc(proceName, paramKeys, paramVals, proceDb, RetType.Json).ToString();
        }
        public string GetSimpDEsJson(string proceDb, string proceName, string[] paramKeys, string[] paramVals)
        {
            var list = new ShareSqlManager().ExecStoredProc(proceName, paramKeys, paramVals, proceDb, RetType.SimpDEs);
            return JsonConvert.SerializeObject(list);
        }
        #endregion
    }

}
