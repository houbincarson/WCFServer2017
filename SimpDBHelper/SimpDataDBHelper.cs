using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace WcfSimpData
{
  /// <summary> 
  /// DbHelper 的摘要说明 
  /// </summary>
  /// 

  public class SimpDataDBHelper
  {
    //数据类型
    private static string dbProviderName = "System.Data.SqlClient";
    //数据库连接字串
    private static string dbConnectionString = "";

    private DbConnection connection;

    //构造函数
    public SimpDataDBHelper()
    {
      connection = CreateConnection(SimpDataDBHelper.dbConnectionString);
    }
    public SimpDataDBHelper(string connectionString)
    {
      connection = CreateConnection(connectionString);
    }
    public SimpDataDBHelper(string dbServer, string dbCatalog, string dbUID, string dbPWD)
    {
      connection = CreateConnection("server=" + dbServer + ";database=" + dbCatalog + ";uid=" + dbUID + ";pwd=" + dbPWD);

    }

    public static DbConnection CreateConnection()
    {
      DbProviderFactory dbfactory = DbProviderFactories.GetFactory(SimpDataDBHelper.dbProviderName);
      DbConnection dbconn = dbfactory.CreateConnection();
      dbconn.ConnectionString = SimpDataDBHelper.dbConnectionString;
      return dbconn;
    }
    public static DbConnection CreateConnection(string connectionString)
    {
      DbProviderFactory dbfactory = DbProviderFactories.GetFactory(SimpDataDBHelper.dbProviderName);
      DbConnection dbconn = dbfactory.CreateConnection();
      dbconn.ConnectionString = connectionString;
      return dbconn;
    }

    /// <summary>
    /// 直接执行存储过程
    /// </summary>
    /// <param name="storedProcedure">存储过程名称</param>
    /// <returns>返回dbCommand对象</returns>
    public DbCommand GetStoredProcCommond(string storedProcedure)
    {
      DbCommand dbCommand = connection.CreateCommand();
      dbCommand.CommandText = storedProcedure;
      dbCommand.CommandType = CommandType.StoredProcedure;
      return dbCommand;
    }

    /// <summary>
    /// 直接执行sql语句,适应于数据表的增,删,改操作
    /// </summary>
    /// <param name="sqlQuery">SQL语句</param>
    /// <returns>返回dbCommand对象</returns>
    public DbCommand GetSqlStringCommond(string sqlQuery)
    {
      DbCommand dbCommand = connection.CreateCommand();
      dbCommand.CommandText = sqlQuery;
      dbCommand.CommandType = CommandType.Text;
      return dbCommand;
    }
    #region 错误信息表

    private static DataTable CreateErrorTable(string erroeMsg)
    {
      DataTable _erroDt = ErrorTable;
      _erroDt.Rows[0]["ERROR"] = erroeMsg;
      return _erroDt;
    }
    private static SimpDataEntery CreateErrorSimpDataEntery(string erroeMsg)
    {
      return new SimpDataEntery()
      {
        Cols = new SimpDataColInf[] { new SimpDataColInf() { name = "ERROR", type = DotNetType.String } },
        Rows = new List<object[]>() { new object[] { erroeMsg } }
      };
    }
    private static object _Syn = new object();
    private static DataTable ErrorTable
    {
      get
      {
        if (_ErrorTable == null)
        {
          lock (_Syn)
          {
            if (_ErrorTable == null)
            {
              _ErrorTable = new DataTable();
              _ErrorTable.Columns.Add("ERROR", typeof(System.String));
              _ErrorTable.Rows.Add(_ErrorTable.NewRow());
            }
          }
        }
        return _ErrorTable.Copy();
      }
    }
    private static DataTable _ErrorTable = null;
    #endregion

    #region 增加参数
    public void AddParameterCollection(DbCommand cmd, DbParameterCollection dbParameterCollection)
    {
      foreach (DbParameter dbParameter in dbParameterCollection)
      {
        cmd.Parameters.Add(dbParameter);
      }
    }

    /// <summary>
    /// 存储过程添加输出参数
    /// </summary>
    /// <param name="cmd">DbCommand对象</param>
    /// <param name="parameterName">参数名称</param>
    /// <param name="dbType">参数类型</param>
    /// <param name="size">参数大小</param>
    /// <returns>无返回结果</returns>
    public void AddOutParameter(DbCommand cmd, string parameterName, DbType dbType, int size)
    {
      DbParameter dbParameter = cmd.CreateParameter();
      dbParameter.DbType = dbType;
      dbParameter.ParameterName = parameterName;
      dbParameter.Size = size;
      dbParameter.Direction = ParameterDirection.Output;
      cmd.Parameters.Add(dbParameter);
    }

    /// <summary>
    /// 存储过程添加输入参数
    /// </summary>
    /// <param name="cmd">DbCommand对象</param>
    /// <param name="parameterName">参数名称</param>
    /// <param name="dbType">参数类型</param>
    /// <param name="value">参数值</param>
    /// <returns>无返回结果</returns>
    public void AddInParameter(DbCommand cmd, string parameterName, DbType dbType, object value)
    {
      DbParameter dbParameter = cmd.CreateParameter();
      dbParameter.DbType = dbType;
      dbParameter.ParameterName = parameterName;
      dbParameter.Value = value;
      dbParameter.Direction = ParameterDirection.Input;
      cmd.Parameters.Add(dbParameter);
    }

    /// <summary>
    /// 存储过程添加返回参数
    /// </summary>
    /// <param name="cmd">DbCommand对象</param>
    /// <param name="parameterName">参数名称</param>
    /// <param name="dbType">参数类型</param>
    /// <returns>无返回结果</returns>
    public void AddReturnParameter(DbCommand cmd, string parameterName, DbType dbType)
    {
      DbParameter dbParameter = cmd.CreateParameter();
      dbParameter.DbType = dbType;
      dbParameter.ParameterName = parameterName;
      dbParameter.Direction = ParameterDirection.ReturnValue;
      cmd.Parameters.Add(dbParameter);
    }

    /// <summary>
    /// 存储过程添加获得参数
    /// </summary>
    /// <param name="cmd">DbCommand对象</param>
    /// <param name="parameterName">参数名称</param>
    /// <returns>返回参数对象值</returns>
    public DbParameter GetParameter(DbCommand cmd, string parameterName)
    {
      return cmd.Parameters[parameterName];
    }
    #endregion

    #region 执行
    //返回DataSet对象
    public DataSet ExecuteDataSet(DbCommand cmd)
    {
      DbProviderFactory dbfactory = DbProviderFactories.GetFactory(SimpDataDBHelper.dbProviderName);
      DbDataAdapter dbDataAdapter = dbfactory.CreateDataAdapter();
      DataSet ds = new DataSet();
      MyTransaction _tran = MyTransaction.BeginTransaction(cmd);
      dbDataAdapter.SelectCommand = cmd;
      try
      {
        dbDataAdapter.Fill(ds);
        _tran.Commit();
      }
      catch (Exception e)
      {
        _tran.Rollback();
        ds.Tables.Add( CreateErrorTable(e.ToString()) );
      }
      return ds;
    }

    //返回DataTable对象
    public DataTable ExecuteDataTable(DbCommand cmd)
    {
      DbProviderFactory dbfactory = DbProviderFactories.GetFactory(SimpDataDBHelper.dbProviderName);
      DbDataAdapter dbDataAdapter = dbfactory.CreateDataAdapter();
      DataTable dataTable = null;
      MyTransaction _tran = MyTransaction.BeginTransaction(cmd);
      dbDataAdapter.SelectCommand = cmd;
      try
      {
        dataTable = new DataTable();
        dbDataAdapter.Fill(dataTable);
        _tran.Commit();
      }
      catch (Exception e)
      {
        _tran.Rollback();
        dataTable = CreateErrorTable(e.ToString());
      }
      return dataTable;
    }
    public List<SimpDataEntery> ExecuteSimpData(DbCommand cmd)
    {
      MyTransaction _tran = MyTransaction.BeginTransaction(cmd);
      List<SimpDataEntery> _simpDbEnterys = new List<SimpDataEntery>();
      SimpDataColInf[] _simpCols;
      List<object[]> _simpRows;
      string _fieldType = string.Empty;
      try
      {
        using (DbDataReader _reader = cmd.ExecuteReader())
        {
          do
          {
            _simpCols = new SimpDataColInf[_reader.FieldCount];
            for (int _i = 0, _iCnt = _reader.FieldCount; _i < _iCnt; _i++)
            {
              _simpCols[_i].name = _reader.GetName(_i);
              _simpCols[_i].type = (DotNetType)Enum.Parse(typeof(DotNetType), _reader.GetFieldType(_i).Name);
            }
            _simpRows = new List<object[]>();
            while (_reader.Read())
            {
              object[] _objs = new object[_reader.FieldCount];

              for (int _i = 0, _iCnt = _reader.FieldCount; _i < _iCnt; _i++)
              {
                if ((_objs[_i] = _reader.GetValue(_i)).Equals(DBNull.Value))
                {
                  _objs[_i] = null;
                }
                if (_objs[_i] is DateTime)
                {
                  _objs[_i] = ((DateTime)_objs[_i]).ToString();
                }
                if (_objs[_i] is Boolean)
                {
                  _objs[_i] = _objs[_i].Equals(true) ? 1 : 0;
                }
              }
              //_reader.GetValues(_objs);
              _simpRows.Add(_objs);
            }
            _simpDbEnterys.Add(new SimpDataEntery() { Cols = _simpCols, Rows = _simpRows, TVal = System.DateTime.Now.Ticks });
          } while (_reader.NextResult());
          _reader.Close();
          _tran.Commit();
        }
      }
      catch (Exception e)
      {
        _tran.Rollback();
        _simpDbEnterys.Add(CreateErrorSimpDataEntery(e.ToString()));
      }
      return _simpDbEnterys;
    }

    //返回DataReader对象
    public DbDataReader ExecuteReader(DbCommand cmd)
    {
      cmd.CommandTimeout = 100;
      cmd.Connection.Open();
      DbDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
      return reader;
    }

    //无结果SQL操作,适用于数据表的增,删,改,存储过程操作
    public int ExecuteNonQuery(DbCommand cmd)
    {
      cmd.CommandTimeout = 100;
      cmd.Connection.Open();
      int ret = cmd.ExecuteNonQuery();
      cmd.Connection.Close();
      return ret;
    }

    //返回SQL影响的行数
    public object ExecuteScalar(DbCommand cmd)
    {
      MyTransaction _tran = MyTransaction.BeginTransaction(cmd);
      object ret = null;
      try
      {
        ret = cmd.ExecuteScalar();
        _tran.Commit();
      }
      catch (Exception e)
      {
        _tran.Rollback();
        return e.ToString();
      }
      return ret;
    }
    #endregion

    internal string ExecuteJsonScalar(DbCommand cmd)
    {
        var dbfactory = DbProviderFactories.GetFactory(dbProviderName);
        var dbDataAdapter = dbfactory.CreateDataAdapter();
        var ds = new DataSet();
        var tran = MyTransaction.BeginTransaction(cmd);
        if (dbDataAdapter == null) 
            return "{\"ERROR\":\"\"}";
        dbDataAdapter.SelectCommand = cmd;
        try
        {
            dbDataAdapter.Fill(ds);
            tran.Commit();
        }
        catch (Exception e)
        {
            tran.Rollback();
            ds.Tables.Add(CreateErrorTable(e.ToString()));
        }
        return JsonHelper.DataSetToJson(ds);
    }
  }

  public class MyTransaction
  {
    private DbTransaction _Transaction = null;
    private DbCommand _Command = null;
    private MyTransaction()
    {
    }
    private MyTransaction(DbCommand cmd)
    {
      cmd.CommandTimeout = 100;
      if (cmd.Connection.State != ConnectionState.Open)
      {
        cmd.Connection.Open();
      }
      cmd.Transaction = cmd.Connection.BeginTransaction();
      _Transaction = cmd.Transaction;
      _Command = cmd;
    }
    private void Close()
    {
      if (_Transaction.Connection != null)
      {
        _Transaction.Connection.Close();
        _Transaction.Connection.Dispose();
      }
      if (_Command.Connection != null && _Command.Connection.State == ConnectionState.Open)
      {
        _Command.Connection.Close();
        _Command.Connection.Dispose();
      }
      if (_Command != null)
      {
        _Command.Dispose();
      }
      if (_Transaction != null)
      {
        _Transaction.Dispose();
      }
    }
    public void Commit()
    {
      if (_Transaction.Connection != null)
      {
        _Transaction.Commit();
      }
      Close();
    }
    public void Rollback()
    {
      if (_Transaction.Connection != null)
      {
        _Transaction.Rollback();
      }
      Close();
    }
    public static MyTransaction BeginTransaction(DbCommand cmd)
    {
      return new MyTransaction(cmd);
    }
  }
}
