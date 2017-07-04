using System.Collections.Generic;
using System.Data;
using SimpDBHelper;

namespace WcfService
{
  public class BaseTablesCheck
  {
    private struct Check
    {
      public string EditTime { get; set; }
      public string MD5 { get; set; }
    }
    private static Dictionary<string, Check> _BaseTableCheck = new Dictionary<string, Check>();
    private static readonly Dictionary<string, object> _SnyMD5 = new Dictionary<string, object>();
    private static readonly object _Sny = new object();
    public static string GetMd5(string tableName, string editTime, string dbCon)
    {
      string _md5Key = string.Format("{0}-{1}", dbCon, tableName);
      if (!_SnyMD5.ContainsKey(_md5Key))
      {
        lock (_Sny)
        {
          if (!_SnyMD5.ContainsKey(_md5Key))
          {
            _SnyMD5.Add(_md5Key, new object());
          }
        }
      }
      lock (_SnyMD5[_md5Key])
      {
        if (!_BaseTableCheck.ContainsKey(_md5Key) ||
          (editTime.Length > 0 && !_BaseTableCheck[_md5Key].EditTime.Equals(editTime)))
        {
          ShareSqlManager _sh = new ShareSqlManager();
          DataSet _ds = (DataSet)_sh.ExecStoredProc(
                                "Sys_BasicTable_Geter",
                                new string[] { "tableName" },
                                new string[] { tableName },
                                dbCon,
                                RetType.DataSet);
            InitTablePrimaryKey(_ds.Tables[2],
               _ds.Tables[0].Rows[0]["DataKeyField"].ToString().Trim());
            _ds.Tables[2].TableName = tableName;
            _BaseTableCheck[_md5Key] = new Check()
            {
              EditTime = editTime,
              MD5 = System.Text.Encoding.UTF8.GetString(DataTableMD5(_ds.Tables[2]))
            };
        }
        return _BaseTableCheck[_md5Key].MD5;
      }
    }
    private static byte[] DataTableMD5(DataTable dt)
    {
      using (System.IO.TextWriter tw = new System.IO.StringWriter())
      {
        dt.WriteXml(tw);
        dt.WriteXmlSchema(tw);
        System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
        byte[] _bys = md5.ComputeHash(
          System.Text.Encoding.UTF8.GetBytes(tw.ToString())
          );
        md5.Clear();
        return _bys;
      }
    }
    private static void InitTablePrimaryKey(DataTable dt, string _keyField)
    {
      string[] _keyFields = _keyField.Split(',');
      int _k = 0, _kCnt = _keyFields.Length;
      DataColumn[] _primaryKeys = new DataColumn[_kCnt];
      for (; _k < _kCnt; _k++)//Delete问题，暂不用双ＫＥＹ
      {
        _primaryKeys[_k] = dt.Columns[_keyFields[_k]];
      }
      dt.PrimaryKey = _primaryKeys;
    }
  }
}
