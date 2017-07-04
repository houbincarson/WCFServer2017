using System.Data;
using System.ServiceModel;

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

        [OperationContract]
        string GetPicture(string _methodRequests); 
    }
}
