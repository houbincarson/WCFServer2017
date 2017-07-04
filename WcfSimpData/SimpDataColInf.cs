
using System.Runtime.Serialization;
namespace WcfSimpData
{
    public struct SimpDataColInf
    {
        public string name { get; set; }
        public DotNetType type { get; set; }
    }
    public enum DotNetType
    {
        Byte,
        Int64,
        Int32,
        String,
        Boolean,
        DateTime,
        Decimal
    }
}
