using System.IO;
using System.IO.Compression;

namespace WcfSimpData
{
  public class GZipStreamHelper
  {
    /// <summary>
    /// GZip解压函数
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public static byte[] GZipDecompress(byte[] data)
    {
      using (MemoryStream stream = new MemoryStream())
      {
        using (GZipStream gZipStream = new GZipStream(new MemoryStream(data), CompressionMode.Decompress))
        {
          byte[] _bytes = new byte[40960];
          int n;
          while ((n = gZipStream.Read(_bytes, 0, _bytes.Length)) != 0)
          {
            stream.Write(_bytes, 0, n);
          }
          gZipStream.Close();
        }

        return stream.ToArray();
      }
    }

    /// <summary>
    /// GZip压缩函数
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public static byte[] GZipCompress(byte[] data)
    {
      using (MemoryStream stream = new MemoryStream())
      {
        using (GZipStream gZipStream = new GZipStream(stream, CompressionMode.Compress))
        {
          gZipStream.Write(data, 0, data.Length);
          gZipStream.Close();
        }

        return stream.ToArray();
      }
    }
  }
}
