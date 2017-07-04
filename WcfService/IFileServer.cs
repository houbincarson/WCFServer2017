using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ServiceModel;

namespace WcfService
{
  [ServiceContract]
  public interface IFileServer
  {
    [OperationContract]
    List<FileEntery> LoadUpdataByte(string _methodRequests);
    [OperationContract]
    byte[] FileDownLoad(string _methodRequests);
    [OperationContract]
    string FileUpLoad(byte[] fileBuf, string _methodRequests);
  }
  
  [DataContract]
  public class FileEntery
  {
    [DataMember]
    public string Name { get; set; }
    [DataMember]
    public int Length { get; set; }
    [DataMember]
    public byte[] Buff { get; set; }
  }
}
