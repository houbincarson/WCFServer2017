using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;

namespace WcfSimpData
{
    public abstract class SimpDataConvertHelper
    {
        public static List<DataSet> SimpDataToDataSets(SimpDataEntery[][] simpDEs)
        {
            if (simpDEs == null)
                return null;

            var dss = new List<DataSet>();
            for (int i = 0, iCnt = simpDEs.Length; i < iCnt; i++)
            {
                SimpDataEntery[] simplis;
                if ((simplis = simpDEs[i]) != null)
                {
                    var ds = new DataSet();
                    for (int s = 0, sCnt = simplis.Length; s < sCnt; s++)
                    {
                        var tableName = string.Format("{0}_{1}", i, s);
                        var dt = SimpDataToDataTable(simplis[s], tableName) ?? new DataTable(tableName);
                        ds.Tables.Add(dt);
                    }
                    dss.Add(ds);
                }
                else
                {
                    dss.Add(null);
                }
            }
            return dss;
        }

        public static List<DataTable> SimpDataToDataTables(SimpDataEntery[][] simpDEs)
        {
            if (simpDEs == null)
                return null;
            var dts = new List<DataTable>();
            for (int i = 0, iCnt = simpDEs.Length; i < iCnt; i++)
            {
                var simplis = simpDEs[i];
                for (int s = 0, sCnt = simplis.Length; s < sCnt; s++)
                {
                    dts.Add(SimpDataToDataTable(simplis[s], string.Format("{0}_{1}", i, s)));
                }
            }
            return dts;
        }

        private static DataTable SimpDataToDataTable(SimpDataEntery simpEnty, string tbname)
        {
            if (simpEnty == null)
                return null;
            var dt = new DataTable();
            for (int i = 0, iCnt = simpEnty.Cols.Length; i < iCnt; i++)
            {
                dt.Columns.Add(simpEnty.Cols[i].name, GetDotNetType(simpEnty.Cols[i].type));
            }
            if (simpEnty.Key != null) dt.PrimaryKey = new[] {dt.Columns[simpEnty.Key]};
            for (int i = 0, iCnt = simpEnty.Rows.Count; i < iCnt; i++)
            {
                dt.LoadDataRow(simpEnty.Rows[i], false);
            }
            dt.TableName = tbname;
            return dt;
        }

        public static SimpDataEntery DataTableToSimpDataEntery(DataTable covDt)
        {
            int i = 0, iCnt = covDt.Columns.Count, r = 0, rCnt = covDt.Rows.Count;
            var simpCols = new SimpDataColInf[iCnt];
            for (; i < iCnt; i++)
            {
                simpCols[i].name = covDt.Columns[i].ColumnName;
                simpCols[i].type = (DotNetType) Enum.Parse(typeof (DotNetType), covDt.Columns[i].DataType.Name);
            }

            var simpRows = new List<object[]>();
            for (; r < rCnt; r++)
            {
                var objs = covDt.Rows[r].ItemArray;
                ConvertObjects(ref objs);
                simpRows.Add(objs);
            }
            var simpDt = new SimpDataEntery {Cols = simpCols, Rows = simpRows, TVal = DateTime.Now.Ticks};
            return simpDt;
        }

        public static SimpDataEntery DataRowToSimpDataEntery(DataRow dr)
        {
            var dt = dr.Table;
            int i = 0, iCnt = dt.Columns.Count;
            var simpCols = new SimpDataColInf[iCnt];
            for (; i < iCnt; i++)
            {
                simpCols[i].name = dt.Columns[i].ColumnName;
                simpCols[i].type = (DotNetType) Enum.Parse(typeof (DotNetType), dt.Columns[i].DataType.Name);
            }

            var objs = dr.ItemArray;
            ConvertObjects(ref objs);
            var simpRows = new List<object[]> {objs};
            var simpDt = new SimpDataEntery {Cols = simpCols, Rows = simpRows, TVal = DateTime.Now.Ticks};
            return simpDt;
        }

        private static void ConvertObjects(ref object[] objs)
        {
            for (int i = 0, iCnt = objs.Length; i < iCnt; i++)
            {
                if (objs[i].Equals(DBNull.Value))
                {
                    objs[i] = null;
                }
                if (objs[i] is DateTime)
                {
                    objs[i] = ((DateTime) objs[i]).ToString(CultureInfo.InvariantCulture);
                }
                if (objs[i] is bool)
                {
                    objs[i] = objs[i].Equals(true) ? 1 : 0;
                }
            }
        }

        public static Type GetDotNetType(DotNetType type)
        {
            switch (type)
            {
                case DotNetType.Boolean:
                    return typeof (bool);
                case DotNetType.Byte:
                    return typeof (byte);
                case DotNetType.DateTime:
                    return typeof (DateTime);
                case DotNetType.Decimal:
                    return typeof (decimal);
                case DotNetType.Int32:
                    return typeof (int);
                case DotNetType.Int64:
                    return typeof (long);
                case DotNetType.String:
                    return typeof (string);
                default:
                    return typeof (string);
            }
        }
    }
}
