using MySQLDriverCS;
using System.Data.Common;
using System.Data;
using System.Configuration;
using System.Collections;
internal class OA_DB_MSG_Helper
{
  private const string OA_MSG_SEND_SQL = "INSERT INTO sms2 (FROM_ID,PHONE,CONTENT,SEND_TIME) VALUES(@FROM_ID,@PHONE,@CONTENT,@SEND_TIME)";
  private static string[] OA_MSG_PARAMS = new string[] { "FROM_ID", "PHONE", "CONTENT", "SEND_TIME" };
  public static void Send_MSG_By_QA_DB(string[][] msgs)
  {
    string _dbConString = ConfigurationManager.ConnectionStrings["TD_OA"].ConnectionString;
    using (MySQLConnection conn =
      new MySQLConnection(_dbConString))
    {
      conn.Open();

      MySQLCommand commn = new MySQLCommand("set names gb2312; ", conn);
      commn.ExecuteNonQuery();
      commn.Dispose();
      for (int _s = 0, _sCnt = msgs.Length; _s < _sCnt; _s++)
      {
        if (msgs[_s].Length < 2)
        {
          continue;
        }
        commn = new MySQLCommand(OA_MSG_SEND_SQL, conn);
        string[] _values = new string[] { 
                        "yutao", 
                        msgs[_s][0], 
                        msgs[_s][1], 
                        System.DateTime.Now.ToLocalTime().ToString() };
        for (int _i = 0, _iCnt = OA_MSG_PARAMS.Length; _i < _iCnt; _i++)
        {
          DbParameter dbParameter = commn.CreateParameter();
          dbParameter.DbType = DbType.String;
          dbParameter.ParameterName = string.Format("@{0}", OA_MSG_PARAMS[_i]);
          dbParameter.Value = _values[_i];
          dbParameter.Direction = ParameterDirection.Input;
          commn.Parameters.Add(dbParameter);
        }
        commn.ExecuteNonQuery();
      }
      conn.Close();
    }
  }
}