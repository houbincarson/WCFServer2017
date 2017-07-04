using System.Data;
using System.ServiceModel;

namespace WcfService
{
    [ServiceContract]
    public interface ISimpDbServer
    {
        [OperationContract]
        string DataRequest_By_String(string methodRequests);
        [OperationContract]
        DataTable DataRequest_By_DataTable(string methodRequests);
        [OperationContract]
        DataSet DataRequest_By_DataSet(string methodRequests);
        [OperationContract]
        string DataRequest_By_SimpDEs(string methodRequests);
        [OperationContract]
        byte[] DataRequest_By_SimpDEs_GZip(string methodRequests);
        [OperationContract]
        byte[] DataRequest_By_SimpDEs_All_GZip(byte[] methodBts);

        [OperationContract]
        string GetPicture(string methodRequests);

        [OperationContract]
        string DataRequest_By_JsonString(string methodRequests); 
    }
}
