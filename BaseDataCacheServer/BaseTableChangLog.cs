using System;
using System.Data;
using WcfSimpData;

namespace BaseDataCacheServer
{
  /// <summary>
  /// 记录表更新时间 ,初始化时从本地读，本地数据的最后更新时间
  /// </summary>
  internal class BaseTableChangLog
  {
    private const string DATA_CHANG_LOG = "DataChangLog";
    public const string DATA_TABLE_NAME = "DataTableName";
    public const string DATA_TABLE_KEY = "DataKeyField";
    public const string DATA_CHANG_TIME = "DataChangTime";
    //public const string DATA_DELETE_KEYS = "DataDelIds";
    public const string DATA_SCHEMA_MD5 = "DataSchemaMD5";
    public const string DATA_CONTENTS_MD5 = "DataContentsMD5";
    private DataTable DataChangLogSchema
    {
      get
      {
        if (_DataChangLogSchema == null)
        {
          _DataChangLogSchema = new DataTable();
          _DataChangLogSchema.Columns.Add(DATA_TABLE_NAME, Type.GetType("System.String"));
          _DataChangLogSchema.Columns.Add(DATA_TABLE_KEY, Type.GetType("System.String"));
          _DataChangLogSchema.Columns.Add(DATA_CHANG_TIME, Type.GetType("System.String"));
          //_DataChangLogSchema.Columns.Add(DATA_DELETE_KEYS, Type.GetType("System.String"));
          _DataChangLogSchema.Columns.Add(DATA_SCHEMA_MD5, Type.GetType("System.String"));
          _DataChangLogSchema.Columns.Add(DATA_CONTENTS_MD5, Type.GetType("System.String"));
          _DataChangLogSchema.PrimaryKey = new DataColumn[] { _DataChangLogSchema.Columns[DATA_TABLE_NAME] };
          _DataChangLogSchema.AcceptChanges();
        }
        return _DataChangLogSchema;
      }
    }
    private DataTable _DataChangLogSchema = null;
    public BaseTableChangLog()
    {
    }
    public void UpdateLog(string tableName, DataTable baseDt)
    {
      UpdateLog(tableName, null, null,  baseDt);
    }
    public void UpdateLog(string tableName, object changTime, object tableKey,  DataTable baseDt)
    {
      DataRow _dr = DataChangLogSchema.Rows.Find(tableName);
      if (_dr == null)
      {
        _dr = DataChangLogSchema.NewRow();
        _dr[DATA_TABLE_NAME] = tableName;
        DataChangLogSchema.Rows.Add(_dr);
      }
      object _schemaMD5, _contentsMD5;
      MatchBaseTableMD5(baseDt, out _schemaMD5, out _contentsMD5);
      if (changTime != null)
      {
        _dr[DATA_CHANG_TIME] = changTime;
      }

      _dr[DATA_TABLE_KEY] = tableKey;
      //_dr[DATA_DELETE_KEYS] = delKeys;

      _dr[DATA_SCHEMA_MD5] = _schemaMD5;
      _dr[DATA_CONTENTS_MD5] = _contentsMD5;
      DataChangLogSchema.AcceptChanges();
    }
    /// <summary>
    /// 读取 表的更新时间 
    /// </summary>
    /// <param name="tableName"></param>
    /// <returns></returns>
    public void ReadLog(string tableName, out object changTime, out object tableKey,  out object schemaMD5, out object contentsMD5)
    {
      changTime = schemaMD5 = contentsMD5 = tableKey = string.Empty;
      DataRow _dr = DataChangLogSchema.Rows.Find(tableName);
      if (_dr != null)
      {
        changTime = _dr[DATA_CHANG_TIME];
        schemaMD5 = _dr[DATA_SCHEMA_MD5];
        contentsMD5 = _dr[DATA_CONTENTS_MD5];
        tableKey = _dr[DATA_TABLE_KEY];
        //delKeys = _dr[DATA_DELETE_KEYS];
      }
    }

    public SimpDataEntery GetBaseTableChangInfo(string tableName)
    {
      DataRow _dr = DataChangLogSchema.Rows.Find(tableName);
      return SimpDataConvertHelper.DataRowToSimpDataEntery(_dr);
    }

    private void MatchBaseTableMD5(DataTable dt, out object schemaMD5, out object contentsMD5)
    {
      schemaMD5 = contentsMD5 = string.Empty;
      if (dt == null)
      {
        return;
      }
      System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
      ///计算结构的ＭＤ５
      using (System.IO.TextWriter tw = new System.IO.StringWriter())
      {
        dt.WriteXmlSchema(tw);
        byte[] _bys = md5.ComputeHash(
          System.Text.Encoding.UTF8.GetBytes(tw.ToString())
          );
        if (_bys != null)
        {
          schemaMD5 = System.Text.Encoding.UTF8.GetString(_bys);
        }
        _bys = null;
      }

      ///计算数据的ＭＤ５
      using (System.IO.TextWriter tw = new System.IO.StringWriter())
      {
        if (dt.PrimaryKey.Length == 0)
        {
          throw (new Exception("数据表没有主键."));
        }
        dt.DefaultView.Sort = dt.PrimaryKey[0].ColumnName;
        using (DataTable _sortDt = dt.DefaultView.ToTable())
        {
          _sortDt.WriteXml(tw);
        }
        byte[] _bys = md5.ComputeHash(
          System.Text.Encoding.UTF8.GetBytes(tw.ToString())
          );
        if (_bys != null)
        {
          contentsMD5 = System.Text.Encoding.UTF8.GetString(_bys);
        }
        _bys = null;
      }

      md5.Clear();
      md5.Dispose();
      md5 = null;
    }
  }
}



////public string this[string tableName]
////{
////  get
////  {
////    if (_BaseDataSet[DATA_CHANG_LOG] == null)
////    {
////      return string.Empty;
////    }
////    DataRow _dr = GetDataChangLogRow(tableName);
////    if (_dr != null)
////    {
////      return _dr[DATA_CHANG_TIME].ToString();
////    }
////    return string.Empty;
////  }
////}
