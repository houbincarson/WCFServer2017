
namespace WcfSimpData
{
    public enum MethodType
    {
        DataQuery,
        BllExecute
    }
    public struct MethodRequestEntery
    {
        public string MethodKey { get; set; }
        public MethodRequest MethodEntery { get; set; }
    }
    /// <summary>
    /// 客户端请求 数据结构
    /// </summary>
    public struct MethodRequest
    {
        //public MethodType MethodType { get; set; }
        //public string MethodKey { get; set; }
        public string ProceName { get; set; }
        public string ProceDb { get; set; }
        public string[] ParamKeys { get; set; }
        public string[] ParamVals { get; set; }

    }

    public static class MethodRequestHelper
    {
        public static string GetParam(MethodRequest metQst, string _paramKey)
        {
            for (int _i = 0, _iCnt = metQst.ParamKeys.Length; _i < _iCnt; _i++)
            {
                if (metQst.ParamKeys[_i].Equals(_paramKey))
                {
                    return metQst.ParamVals[_i];
                }
            }
            return null;
        }
    }
}
