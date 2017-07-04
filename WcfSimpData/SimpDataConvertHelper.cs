using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace WcfSimpData
{
    public abstract class SimpDataConvertHelper
    {
        public static System.Collections.Generic.List<DataSet> SimpDataToDataSets(SimpDataEntery[][] simpDEs)
        {
            if (simpDEs == null)
                return null;
            SimpDataEntery[] _simplis;

            System.Collections.Generic.List<DataSet> _dss = new System.Collections.Generic.List<DataSet>();
            for (int _i = 0, _iCnt = simpDEs.Length; _i < _iCnt; _i++)
            {
                if ((_simplis = simpDEs[_i]) != null)
                {
                    DataSet _ds = new DataSet();
                    for (int _s = 0, _sCnt = _simplis.Length; _s < _sCnt; _s++)
                    {
                        string _tableName = string.Format("{0}_{1}", _i, _s);
                        DataTable _dt = SimpDataToDataTable(_simplis[_s], _tableName);
                        if (_dt == null)
                        {
                            _dt = new DataTable(_tableName);
                        }
                        _ds.Tables.Add(_dt);
                    }
                    _dss.Add(_ds);
                }
                else
                {
                    _dss.Add(null);
                }
            }
            return _dss;
        }

        public static System.Collections.Generic.List<DataTable> SimpDataToDataTables(SimpDataEntery[][] simpDEs)
        {
            if (simpDEs == null)
                return null;
            SimpDataEntery[] _simplis;

            System.Collections.Generic.List<DataTable> _dts = new System.Collections.Generic.List<DataTable>();
            for (int _i = 0, _iCnt = simpDEs.Length; _i < _iCnt; _i++)
            {
                _simplis = simpDEs[_i];
                for (int _s = 0, _sCnt = _simplis.Length; _s < _sCnt; _s++)
                {
                    _dts.Add(SimpDataToDataTable(_simplis[_s], string.Format("{0}_{1}", _i, _s)));
                }
            }
            return _dts;
        }

        private static DataTable SimpDataToDataTable(SimpDataEntery simpEnty, string tbname)
        {
            if (simpEnty == null)
                return null;
            DataTable _dt = new DataTable();
            for (int _i = 0, _iCnt = simpEnty.Cols.Length; _i < _iCnt; _i++)
            {
                _dt.Columns.Add(simpEnty.Cols[_i].name, GetDotNetType(simpEnty.Cols[_i].type));
            }
            if (simpEnty.Key != null) _dt.PrimaryKey = new DataColumn[] { _dt.Columns[simpEnty.Key] };
            for (int _i = 0, _iCnt = simpEnty.Rows.Count; _i < _iCnt; _i++)
            {
                _dt.LoadDataRow(simpEnty.Rows[_i], false);
            }
            _dt.TableName = tbname;
            return _dt;
        }

        public static SimpDataEntery DataTableToSimpDataEntery(DataTable covDt)
        {
            int _i = 0, _iCnt = covDt.Columns.Count, _r = 0, _rCnt = covDt.Rows.Count;
            SimpDataColInf[] _simpCols = new SimpDataColInf[_iCnt];
            for (; _i < _iCnt; _i++)
            {
                _simpCols[_i].name = covDt.Columns[_i].ColumnName;
                _simpCols[_i].type = (DotNetType)Enum.Parse(typeof(DotNetType), covDt.Columns[_i].DataType.Name);
            }

            List<object[]> _simpRows = new List<object[]>();
            for (; _r < _rCnt; _r++)
            {
                object[] _objs = covDt.Rows[_r].ItemArray;

                ConvertObjects(ref _objs);
                _simpRows.Add(_objs);
            }
            SimpDataEntery _simpDt = new SimpDataEntery() { Cols = _simpCols, Rows = _simpRows, TVal = System.DateTime.Now.Ticks };
            return _simpDt;
        }

        public static SimpDataEntery DataRowToSimpDataEntery(DataRow _dr)
        {
            DataTable _dt = _dr.Table;
            int _i = 0, _iCnt = _dt.Columns.Count;
            SimpDataColInf[] _simpCols = new SimpDataColInf[_iCnt];
            for (; _i < _iCnt; _i++)
            {
                _simpCols[_i].name = _dt.Columns[_i].ColumnName;
                _simpCols[_i].type = (DotNetType)Enum.Parse(typeof(DotNetType), _dt.Columns[_i].DataType.Name);
            }

            object[] _objs = _dr.ItemArray;
            ConvertObjects(ref _objs);
            List<object[]> _simpRows = new List<object[]>() { _objs };
            SimpDataEntery _simpDt = new SimpDataEntery() { Cols = _simpCols, Rows = _simpRows, TVal = System.DateTime.Now.Ticks };
            return _simpDt;
        }

        private static void ConvertObjects(ref object[] objs)
        {
            for (int _i = 0, _iCnt = objs.Length; _i < _iCnt; _i++)
            {
                if (objs[_i].Equals(DBNull.Value))
                {
                    objs[_i] = null;
                }
                if (objs[_i] is DateTime)
                {
                    objs[_i] = ((DateTime)objs[_i]).ToString();
                }
                if (objs[_i] is Boolean)
                {
                    objs[_i] = objs[_i].Equals(true) ? 1 : 0;
                }
            }
        }

        public static System.Type GetDotNetType(DotNetType type)
        {
            switch (type)
            {
                case DotNetType.Boolean:
                    return typeof(System.Boolean);
                case DotNetType.Byte:
                    return typeof(System.Byte);
                case DotNetType.DateTime:
                    return typeof(System.DateTime);
                case DotNetType.Decimal:
                    return typeof(System.Decimal);
                case DotNetType.Int32:
                    return typeof(System.Int32);
                case DotNetType.Int64:
                    return typeof(System.Int64);
                case DotNetType.String:
                    return typeof(System.String);
                default:
                    return typeof(System.String);
            }
        }
    }
}
