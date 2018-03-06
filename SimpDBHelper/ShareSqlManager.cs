using System;
using System.Configuration;
using System.Data;
using System.Data.Common;
using WcfSimpData;

namespace SimpDBHelper
{
    public enum RetType
    {
        String,
        Table,
        DataSet,
        SimpDEs,
        Json,
    }
    public class ShareSqlManager
    {
        private string DeEncryptKey = "#QJLLHB*";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="storedProcedure">存储过程的名字</param>
        /// <param name="paramKeys">参数列表，,隔开</param>
        /// <param name="paramVals">参数值数组</param>
        /// <param name="strRetType">Table返回DataTable；String时返回string；Int时返回int</param>
        /// <returns></returns>
        public object ExecStoredProc(string storedProcedure, string[] paramKeys, string[] paramVals, string dbConName, RetType strRetType)
        {
            ConnectionStringSettings conStrs = ConfigurationManager.ConnectionStrings[dbConName];
            if (conStrs == null)
            {
                throw (new Exception(string.Format("调用数据库 {0} 错误.", dbConName)));
            }
            string dbConString = MyEncrypt.DecryptDES(conStrs.ConnectionString, DeEncryptKey);
            SimpDataDBHelper dbComObj = new SimpDataDBHelper(dbConString);

            if (string.IsNullOrWhiteSpace(storedProcedure))
            {
                throw (new Exception("调用名称为空."));
            }
            using (DbCommand cmd = dbComObj.GetStoredProcCommond(storedProcedure))
            {
                if (paramKeys != null && paramVals != null && paramKeys.Length != 0 && paramVals.Length != 0)
                {
                    for (int i = 0, iCnt = paramKeys.Length; i < iCnt; i++)
                    {
                        dbComObj.AddInParameter(cmd, "@" + paramKeys[i], DbType.String, paramVals[i].ToString());
                    }
                }
                switch (strRetType)
                {
                    case RetType.String:
                        return dbComObj.ExecuteScalar(cmd);
                    case RetType.Table:
                        return dbComObj.ExecuteDataTable(cmd);
                    case RetType.DataSet:
                        return dbComObj.ExecuteDataSet(cmd);
                    case RetType.SimpDEs:
                        return dbComObj.ExecuteSimpData(cmd);
                    case RetType.Json:
                        return dbComObj.ExecuteJsonScalar(cmd);
                    default:
                        return null;
                }
            }
        }
    }
}
