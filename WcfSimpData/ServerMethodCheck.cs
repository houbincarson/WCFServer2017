
using System.Collections.Generic;
namespace WcfSimpData
{
    public class ServerMethodCheck
    {
        /// <summary>
        /// 权限字符串,id,拼接 
        /// </summary>
        public string methodKeys;
        public ServerMethodCheck(string _methodKeys)
        {
            methodKeys = _methodKeys;
        }
        public bool CheckMethods(List<MethodRequest> _methodRequests)
        {
            //for (int _i = 0; _i < _methodRequests.Count; _i++)
            //{
            //  if (!CheckMethod(_methodRequests[_i].MethodKey)) return false;
            //}
            return true;
        }
        /// <summary>
        /// 存在返回true 不存在返回false
        /// </summary>
        /// <param name="_method"></param>
        /// <returns></returns>
        public bool CheckMethod(string _method)
        {
            return (methodKeys.IndexOf(string.Format(",{0},", _method)) != -1);
        }

    }
}
