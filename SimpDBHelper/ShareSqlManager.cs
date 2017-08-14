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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="storedProcedure">存储过程的名字</param>
        /// <param name="paramKeys">参数列表，,隔开</param>
        /// <param name="paramVals">参数值数组</param>
        /// <param name="strRetType">Table返回DataTable；String时返回string；Int时返回int</param>
        /// <returns></returns>
        public object ExecStoredProc(string storedProcedure, string[] paramKeys, string[] paramVals, string dbConName, RetType retType)
        {
            ConnectionStringSettings _conStrs = ConfigurationManager.ConnectionStrings[dbConName];
            if (_conStrs == null)
            {
                throw (new System.Exception(string.Format("调用数据库 {0} 错误.", dbConName)));
            }
            string _dbConString = _conStrs.ConnectionString;
            SimpDataDBHelper dbComObj = new SimpDataDBHelper(_dbConString);

            if (string.IsNullOrWhiteSpace(storedProcedure))
            {
                throw (new System.Exception("调用名称为空."));
            }
            using (DbCommand cmd = dbComObj.GetStoredProcCommond(storedProcedure))
            {
                if (paramKeys != null && paramVals != null && paramKeys.Length != 0 && paramVals.Length != 0)
                {
                    for (int _i = 0, _iCnt = paramKeys.Length; _i < _iCnt; _i++)
                    {
                        dbComObj.AddInParameter(cmd, "@" + paramKeys[_i], DbType.String, paramVals[_i].ToString());
                    }
                }
                switch (retType)
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
                }
                return null;
            }
        }
    }
}
