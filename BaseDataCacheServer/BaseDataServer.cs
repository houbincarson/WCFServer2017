using System.Collections.Generic;
using System.Web.Script.Serialization;
using SimpDBHelper;
using WcfSimpData;
///client:"WcfTestClient.exe" 直接调试用
namespace BaseDataCacheServer
{
  // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码和配置文件中的类名“BaseDataServer”。
  public class BaseDataServer : IBaseDataServer
  {
    private static JavaScriptSerializer __JsonSerial
    {
      get
      {
        if (_jsonserial == null)
        {
          _jsonserial = new JavaScriptSerializer();
          _jsonserial.MaxJsonLength = 60000000;
        }
        return _jsonserial;
      }
    }
    private static JavaScriptSerializer _jsonserial = null;
    //BaseTablesGetter
    public byte[] BaseDataRequest(byte[] methodBytes)
    {
      byte[] _metBts = GZipStreamHelper.GZipDecompress(methodBytes);
      MethodRequest[] _metodReqs = __JsonSerial.Deserialize<MethodRequest[]>(System.Text.Encoding.UTF8.GetString(_metBts));
      if (_metodReqs == null)
      {
        return null;
      }
      ShareSqlManager _sh = new ShareSqlManager();
      List<List<SimpDataEntery>> _simpEtys = new List<List<SimpDataEntery>>();
      for (int _i = 0, _iCnt = _metodReqs.Length; _i < _iCnt; _i++)
      {
        List<SimpDataEntery> _lis = BaseTablesGetter.GetBaseTablesGetterInstances(_metodReqs[_i].ProceDb).GetBaseTable(
              MethodRequestHelper.GetParam(_metodReqs[_i], "tableName"),
              MethodRequestHelper.GetParam(_metodReqs[_i], "ChangTime"),
              MethodRequestHelper.GetParam(_metodReqs[_i], "schemaMD5"),
              MethodRequestHelper.GetParam(_metodReqs[_i], "contentsMD5"));

        _simpEtys.Add(_lis);
      }
      string _jsonSimpEtys = __JsonSerial.Serialize(_simpEtys); 
      byte[] _bts = System.Text.Encoding.UTF8.GetBytes(_jsonSimpEtys);
      return GZipStreamHelper.GZipCompress(_bts);
    }
  }
}
