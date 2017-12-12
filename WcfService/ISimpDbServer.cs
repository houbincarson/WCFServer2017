using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System.Data;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Web;
using WcfSimpData;

namespace WcfService
{
    [ServiceContract]
    public interface ISimpDbServer
    {
        [OperationContract]
        string DataRequest_By_String(string _methodRequests);
        [OperationContract]
        DataTable DataRequest_By_DataTable(string _methodRequests);
        [OperationContract]
        DataSet DataRequest_By_DataSet(string _methodRequests);
        [OperationContract]
        string DataRequest_By_SimpDEs(string _methodRequests);
        [OperationContract]
        byte[] DataRequest_By_SimpDEs_GZip(string _methodRequests);
        [OperationContract]
        byte[] DataRequest_By_SimpDEs_All_GZip(byte[] _methodBts);
    }

    [ServiceContract]
    public interface IJsonDbServer
    {
        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "GetResultJson")]
        string GetResultJson(string proceDb, string proceName, string[] paramKeys, string[] paramVals);

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "GetSimpDEsJson")]
        string GetSimpDEsJson(string proceDb, string proceName, string[] paramKeys, string[] paramVals); 
      
    }
}
