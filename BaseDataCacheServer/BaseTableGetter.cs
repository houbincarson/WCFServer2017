
using System.Collections.Generic;
using System.Data;
using SimpDBHelper;
using WcfSimpData;
namespace BaseDataCacheServer
{
  internal class BaseTablesGetter
  {
    private const string DATA_DELETE_TIME = "Delete_Time";
    private const string DATA_DELETE_KEY = "Delete_Key";
    private DataSet DeleteKeyDataSets
    {
      get
      {
        if (_DeleteKeyDataSets == null)
        {
          _DeleteKeyDataSets = new DataSet();
        }
        return _DeleteKeyDataSets;
      }
    }
    private DataSet _DeleteKeyDataSets;

    private const string DB_PROC_NAME = "Sys_BasicTable_Geter";
    private const string DELETE_IDS_COLNAME = "DeleteIds";
    private static Dictionary<string, BaseTablesGetter> BaseTablesGetterInstances
    {
      get
      {
        if (_BaseTablesGetterInstances == null)
        {
          _BaseTablesGetterInstances = new Dictionary<string, BaseTablesGetter>();
        }
        return _BaseTablesGetterInstances;
      }
      
    }
    private static Dictionary<string, BaseTablesGetter> _BaseTablesGetterInstances;
    private readonly static object _Syn = new object();

    #region 变量区
    private BaseTableChangLog TableChgLog
    {
      get
      {
        if (_TableChgLog == null)
        {
          _TableChgLog = new BaseTableChangLog();
        }
        return _TableChgLog;
      }
    }
    private BaseTableChangLog _TableChgLog;

    private DataSet CacheDataSets
    {
      get
      {
        if (_CacheDataSets == null)
        {
          _CacheDataSets = new DataSet();
        }
        return _CacheDataSets;
      }
    }
    private DataSet _CacheDataSets;

    private string TableChgLogKey { get; set; }
    #endregion

    private BaseTablesGetter()
    {
    }
    private BaseTablesGetter(string tableChgLogKey)
    {
      TableChgLogKey = tableChgLogKey;
    }
    public static BaseTablesGetter GetBaseTablesGetterInstances(string basGetterKey)
    {
      lock (_Syn)
      {
        if (!BaseTablesGetterInstances.ContainsKey(basGetterKey))
        {
          BaseTablesGetterInstances.Add(basGetterKey, new BaseTablesGetter(basGetterKey));
        }
        return BaseTablesGetterInstances[basGetterKey];
      }
    }

    public List<SimpDataEntery> GetBaseTable(string tableName, object editTime, object schemaMD5, object contentsMD5)
    {
      RefreshBaseTable(tableName);
      bool isContentsMD5Equals;//用来记录 判断客户端数据与服务端是否相同 
      DataTable _dt = GetBaseTableByCacheDataSets(tableName, editTime, schemaMD5, contentsMD5, out isContentsMD5Equals);//获取更新记录集
      string _clientDelKeys = GetDeleteKeysString(tableName, editTime);//获取被删除记录　主键字符串

      bool _hasNew = (_dt != null && _dt.Rows.Count > 0);
      bool _hasDelete = !_clientDelKeys.Equals(string.Empty);
      if (!_hasNew && //不存在需要更新数据
        !_hasDelete &&
        isContentsMD5Equals)//用来判断客户端数据与服务端是否相同  (客户端数据不准确情况下会出现 _hasNew=true _hasDelete=true的情况)
      {
        return null;
      }


      List<SimpDataEntery> _lisSimp = new List<SimpDataEntery>();
      SimpDataEntery _schemaEty, _deleteEty, _contentEty;
      _schemaEty = _deleteEty = _contentEty = null;

      if (_hasDelete)
      {
        _deleteEty = new SimpDataEntery()
        {
          Cols = new SimpDataColInf[] { new SimpDataColInf() { name = DELETE_IDS_COLNAME, type = DotNetType.String } },
          Rows = new List<object[]>() { new object[] { _clientDelKeys } },
          TVal = System.DateTime.Now.Ticks
        };
      }

      _schemaEty = TableChgLog.GetBaseTableChangInfo(tableName);//不管有没有必须近观回。需要返回MD5等信息
      if (_hasNew)
      {
        _contentEty = SimpDataConvertHelper.DataTableToSimpDataEntery(_dt);
      }

      _lisSimp.Add(_schemaEty);
      _lisSimp.Add(_deleteEty);
      _lisSimp.Add(_contentEty);
      return _lisSimp;
    }
    /// <summary>
    /// 根据　服务端记录的 EditTime更新　缓存数据表　然后更新MD5 根据MainKey 取MD5
    /// </summary>
    /// <param name="tableName"></param>
    private void RefreshBaseTable(string tableName)
    {
      object _svcChangTime, _svcSchemaMD5, _svcContentsMD5, _svcTableKey;
      TableChgLog.ReadLog(tableName, out _svcChangTime, out _svcTableKey, out _svcSchemaMD5, out _svcContentsMD5);

      object _dbTableKey, _dbChangTime, _dbDelKeys;
      DataTable _dtNew, _dtDelKey;
      ReadBaseTableFormDb(tableName, _svcChangTime, out _dbTableKey, out _dbChangTime, out _dtDelKey, out _dtNew);

      AddDeleteKeyTable(tableName, _dtDelKey);

      _dbDelKeys = GetDeleteKeysString(tableName, _svcChangTime);//获取删除键集合


      //CASE 1:缓存记录结构信息为空 
      //重新写入缓存，
      //重新写入更新信息TableChgLog
      bool _tableIsNull = (CacheDataSets.Tables[tableName] == null);
      if (_tableIsNull)//表示还未记录缓存
      {
        InitTablePrimaryKey(_dtNew, _dbTableKey.ToString());
        _dtNew.TableName = tableName;
        CacheDataSets.Tables.Add(_dtNew);
        CacheDataSets.AcceptChanges();
        TableChgLog.UpdateLog(tableName, _dbChangTime, _dbTableKey, _dtNew);
        return;
      }


      bool _tableChgTimeIsEqual = _dbChangTime.Equals(_svcChangTime);//判断修改时间是否一至
      bool _tableNotNeedDelete = _dbDelKeys.Equals(string.Empty);//判断删是否存在需要删除的记录
      bool _tableKeyIsEqual = _dbTableKey.Equals(_svcTableKey);//判断主键是否一至
      bool _tableSchemaIsEqual = DataTableExtend.DataTableHelper.TableColumnsEquals(_dtNew, CacheDataSets.Tables[tableName]);//判断表结构是否一至
      //CASE 2:版本一至
      //不需要更新
      if (!_tableIsNull &&//判断是否存在缓存表  ＊＊不满足CASE 1 肯定为 BaseDataSets.Tables[tableName] != nul
        _tableChgTimeIsEqual &&//判断修改时间是否一至
         _tableNotNeedDelete &&//判断删是否存在需要删除的记录
        _tableKeyIsEqual &&//判断主键是否一至
        _tableSchemaIsEqual)//判断表结构是否一至
      {
        return;//版本一至，不需要更新
      }



      //CASE 3:版本发生变化
      //更新数据
      if (!_tableSchemaIsEqual)//表结构不一至，清空缓存，重新运行RefreshBaseTable(tableName);　进入　－－＞CASE 1
      {
        if (!_tableIsNull)
        {
          CacheDataSets.Tables[tableName].Dispose();
          CacheDataSets.Tables.Remove(tableName);
        }
        TableChgLog.UpdateLog(tableName, string.Empty, _dbTableKey, null);//需清空 变化记录
        RefreshBaseTable(tableName);
        return;
      }

      if (!_tableIsNull)
      {
        DataTable _cacheDt = CacheDataSets.Tables[tableName];//记录缓存表引用

        if (!_tableKeyIsEqual)//主键发生变化，更新主键
        {
          InitTablePrimaryKey(_cacheDt, _dbTableKey.ToString());
        }

        if (!_tableNotNeedDelete)// /删除记录发生变化  删除变删除的记录
        {
          DataTableExtend.DataTableHelper.DeleteDataTableForFilterExpression(
            _cacheDt, string.Format("{0} IN ({1})", _dbTableKey, _dbDelKeys), true);
        }

        if (!_tableChgTimeIsEqual)//修改时间发生变化 加入被修改的记录
        {
          _cacheDt.Merge(_dtNew);
        }

        _cacheDt.AcceptChanges();
        TableChgLog.UpdateLog(tableName, _dbChangTime, _dbTableKey, _cacheDt);//更新缓存记录表
        return;
      }
    }

    /// <summary>
    /// 根据更新时间获取　更新数据　　
    /// 在RefreshBaseTable　后调用
    /// </summary>
    /// <param name="tableName"></param>
    /// <param name="editTime"></param>
    /// <returns></returns>
    private DataTable GetBaseTableByCacheDataSets(string tableName, object editTime, object schemaMD5, object contentsMD5, out bool isContentsMD5Equals)
    {
      object _svcChangTime, _svcSchemaMD5, _svcContentsMD5, _svcTableKey;
      TableChgLog.ReadLog(tableName, out _svcChangTime, out _svcTableKey, out _svcSchemaMD5, out _svcContentsMD5);
      bool _isSchemaMD5Equals = schemaMD5.Equals(_svcSchemaMD5);
      isContentsMD5Equals = contentsMD5.Equals(_svcContentsMD5);
      if (_isSchemaMD5Equals && isContentsMD5Equals)//结构和数据MD5相同，
      {
        return null;
      }
      if (!_isSchemaMD5Equals || editTime.Equals(string.Empty))//结构不相同　或者更新时间为空 返回整表
      {
        return CacheDataSets.Tables[tableName];
      }

      DataTable _cacheDt;
      ReadClientBaseTableFormDb(tableName, editTime.ToString(), out _cacheDt);
      //服务器数据没有Edit_Time字段，所以只能从数据库取数据
      //DataTable _cacheDt = CacheDataSets.Tables[tableName];
      //_cacheDt.DefaultView.RowFilter = string.Format("Edit_Time>'{0}'", editTime);
      return _cacheDt.DefaultView.ToTable();
    }

    private void AddDeleteKeyTable(string tableName, DataTable deleteKeyDt)
    {
      if (DeleteKeyDataSets.Tables.IndexOf(tableName) != -1)
      {
        DeleteKeyDataSets.Tables[tableName].Dispose();
        DeleteKeyDataSets.Tables.Remove(tableName);
      }
      deleteKeyDt.TableName = tableName;
      DeleteKeyDataSets.Tables.Add(deleteKeyDt);
      DeleteKeyDataSets.AcceptChanges();
    }
    private string GetDeleteKeysString(string tableName, object editTime)
    {
      DataTable _dt = DeleteKeyDataSets.Tables[tableName];
      if (_dt == null)
      {
        return string.Empty;
      }
      string _keys = string.Empty;
      if (editTime.Equals(string.Empty))
      {
        _keys = DataTableExtend.DataTableHelper.GetDataTableFieldVals(_dt, DATA_DELETE_KEY);//时间为空返回所有删除记录
      }
      else
      {
        _dt.DefaultView.RowFilter = string.Format("{0}>'{1}'", DATA_DELETE_TIME, editTime);
        _keys = DataTableExtend.DataTableHelper.GetDataTableFieldVals(_dt.DefaultView, DATA_DELETE_KEY);
      }
      if (_keys.Length == 0)
      {
        return _keys;
      }
      string[] _keyAry = _keys.Split(',');
      System.Text.StringBuilder _sb = new System.Text.StringBuilder();
      for (int _i = 0, _iCnt = _keyAry.Length; _i < _iCnt; _i++)
      {
        if (_sb.Length > 0)
        {
          _sb.Append(',');
        }
        _sb.Append('\'');
        _sb.Append(_keyAry[_i]);
        _sb.Append('\'');
      }
      return _sb.ToString();
    }

    /// <summary>
    /// 从数据库读取更新信息　和更新数据
    /// </summary>
    /// <param name="tableName">表名称</param>
    /// <param name="svcChangTime">服务端保存的更新时候</param>
    /// <param name="dbTableKey">数据库返回主键信息</param>
    /// <param name="dbChangTime">数据库返回的更新时间</param>
    /// <param name="dbDelKeys">数据库返回删除信息</param>
    /// <param name="dtNew">数据库返回的更新记录</param>
    private void ReadBaseTableFormDb(string tableName, object svcChangTime,
      out object dbTableKey, out object dbChangTime, 
      out DataTable dtDel, out DataTable dtNew)
    {
      dbTableKey = dbChangTime = string.Empty;
      dtNew = null;
      ShareSqlManager _sh = new ShareSqlManager();
      DataSet _ds = (DataSet)_sh.ExecStoredProc(DB_PROC_NAME,
                           new string[] { "tableName", "ChangTime" },
                            new string[] { tableName, svcChangTime.ToString() },
                            TableChgLogKey,
                            RetType.DataSet);
      //SELECT DataTableName,DataKeyField,CONVERT(varchar(30),DataChangTime,121) AS DataChangTime
      //FROM dbo.Sys_DataChangLog		
      //WHERE DataTableName=@tableName
      DataTable _dtTableInf = _ds.Tables[0];
      if (_dtTableInf != null)
      {
        if (_dtTableInf.Rows.Count == 0)
        {
          throw (new System.Exception(string.Format("{0} 表设置错误.",tableName)));
        }
        if (_dtTableInf.Columns.Contains("ERROR"))
        {
          throw (new System.Exception(_dtTableInf.Rows[0]["ERROR"].ToString()));
        }
      }
      DataRow _dr = _dtTableInf.Rows[0];
      dbTableKey = _dr[BaseTableChangLog.DATA_TABLE_KEY];
      dbChangTime = _dr[BaseTableChangLog.DATA_CHANG_TIME];

      //SELECT A.DataDeTime AS DeleteTime,A.DataDelId AS DeleteKey
      //FROM Sys_DataDeleteLog AS A
      //WHERE DataTableName=@tableName
      //dbDelKeys = _ds.Tables[1].Rows[0][BaseTableChangLog.DATA_DELETE_KEYS];
      dtDel = _ds.Tables[1];

      //SELECT x,x,x FROM BaseTable
      dtNew = _ds.Tables[2];

      _ds.Tables.Remove(_ds.Tables[2]);
      _ds.Tables.Remove(_ds.Tables[1]);
      _ds.Tables[0].Dispose();
      _ds.Dispose();
      _ds = null;
    }

    private static void InitTablePrimaryKey(DataTable dt, string keyField)
    {
      string[] _keyFields = keyField.Split(',');
      int _k = 0, _kCnt = _keyFields.Length;
      DataColumn[] _primaryKeys = new DataColumn[_kCnt];
      for (; _k < _kCnt; _k++)//Delete问题，暂不用双ＫＥＹ
      {
        _primaryKeys[_k] = dt.Columns[_keyFields[_k]];
      }
      dt.PrimaryKey = _primaryKeys;
    }

    private void ReadClientBaseTableFormDb(string tableName, string clientChangTime, out DataTable dtNew)
    {
      ShareSqlManager _sh = new ShareSqlManager();
      DataSet _dsClient = (DataSet)_sh.ExecStoredProc(DB_PROC_NAME,
                           new string[] { "tableName", "ChangTime" },
                            new string[] { tableName, clientChangTime.ToString() },
                            TableChgLogKey,
                            RetType.DataSet);
      dtNew = _dsClient.Tables[2];
      _dsClient.Tables.Remove(_dsClient.Tables[2]);
      _dsClient.Tables[1].Dispose();
      _dsClient.Tables[0].Dispose();
      _dsClient.Dispose();
      _dsClient = null;
    }

  }
}
