
using System.Runtime.Serialization;
using System.Collections.Generic;
using System;
namespace WcfSimpData
{
    public class SimpDataEntery
    {
        public object[] SimpDataArry { get; set; }
        private SimpDataColInf[] _Clos = null;
        private List<object[]> _Rows = null;

        public SimpDataEntery()
        {
            SimpDataArry = new object[4];
        }
        [System.Web.Script.Serialization.ScriptIgnore]
        public long TVal
        {
            set
            {
                SimpDataArry[0] = value;
            }
            get
            {
                long _tval = 0;
                if (SimpDataArry[0] != null)
                {
                    long.TryParse(SimpDataArry[0].ToString(), out _tval);
                }
                return _tval;
            }
        }
        [System.Web.Script.Serialization.ScriptIgnore]
        public string Key
        {
            set
            {
                SimpDataArry[1] = value;
            }
            get
            {
                if (SimpDataArry[1] != null)
                {
                    return SimpDataArry[1].ToString();
                }
                return string.Empty;
            }
        }
        [System.Web.Script.Serialization.ScriptIgnore]
        public SimpDataColInf[] Cols
        {
            set
            {
                object[] _cols = (object[])SimpDataArry[2];
                _cols = null;
                _cols = new object[value.Length];
                SimpDataArry[2] = _cols;
                _Clos = null;
                _Clos = value;
                for (int _i = 0, _iCnt = value.Length; _i < _iCnt; _i++)
                {
                    _cols[_i] = (new object[] { value[_i].name, (int)value[_i].type });
                }
            }
            get
            {
                if (_Clos != null)
                    return _Clos;
                if (SimpDataArry[2] == null)
                    return null;
                object[] _cols = (object[])SimpDataArry[2];
                int _i = 0, _iCnt = _cols.Length;
                _Clos = new SimpDataColInf[_iCnt];
                for (; _i < _iCnt; _i++)
                {
                    object[] _colinf = (object[])_cols[_i];
                    _Clos[_i] = new SimpDataColInf() { name = _colinf[0].ToString(), type = (DotNetType)_colinf[1] };
                }
                return _Clos;
            }
        }
        [System.Web.Script.Serialization.ScriptIgnore]
        public List<object[]> Rows
        {
            set
            {
                _Rows = value;
                SimpDataArry[3] = _Rows.ToArray();
            }
            get
            {
                if (_Rows != null)
                    return _Rows;
                if (SimpDataArry[3] == null)
                    return null;
                _Rows = new List<object[]>();
                object[] _rows = (object[])SimpDataArry[3];
                int _i = 0, _iCnt = _rows.Length;
                for (; _i < _iCnt; _i++)
                {
                    _Rows.Add((object[])_rows[_i]);
                }
                return _Rows;
            }
        }
    }
}
