using System.Data;
using System;
using System.Diagnostics;
using WcfSimpData;
using System.IO;
namespace WcfService
{
  public class ClientRunDll_Helper
  {
    private const string FILE_INFO_NAME = "FILE_INFO";
    private static DataTable FileINFOSchema
    {
      get
      {
        if (_FileINFOSchema == null)
        {
          _FileINFOSchema = new DataTable();
          _FileINFOSchema.Columns.Add("File_Name", Type.GetType("System.String"));
          _FileINFOSchema.Columns.Add("File_Ver", Type.GetType("System.String"));
          _FileINFOSchema.Columns.Add("File_Length", Type.GetType("System.String"));
          _FileINFOSchema.PrimaryKey = new DataColumn[] { _FileINFOSchema.Columns["File_Name"] };
          _FileINFOSchema.AcceptChanges();
        }
        return _FileINFOSchema;
      }
    }
    private static DataTable _FileINFOSchema;
    private DataTable ClientDLL_INFO
    {
      get
      {
        if (BaseDataCache.BaseDataTables.getInstance()[FILE_INFO_NAME] == null)
        {
          BaseDataCache.BaseDataTables.getInstance()[FILE_INFO_NAME] = FileINFOSchema;
        }
        return BaseDataCache.BaseDataTables.getInstance()[FILE_INFO_NAME];
      }
    }

    public void CheckRullDllINFO(string dllpath)
    {
      string[] _server_Files = Directory.GetFiles(dllpath);
      DataTable _dt = ClientDLL_INFO.Clone();
      for (int _i = 0, _iCnt = _server_Files.Length; _i < _iCnt; _i++)
      {
        string _severFileName = _server_Files[_i].Substring(_server_Files[_i].LastIndexOf('\\') + 1);
        string _fileName = _severFileName.ToUpper();
        string _fileVer = FileVersionInfo.GetVersionInfo(_server_Files[_i]).FileVersion.Trim().ToUpper();
        if (ClientDLL_INFO.Select(string.Format("File_Name={0} AND File_Ver={1}", _fileName, _fileVer)).Length == 0)
        {
          byte[] _bts = File.ReadAllBytes(_server_Files[_i]);
          File.WriteAllBytes(string.Format("{0}/{1}/{2}", System.AppDomain.CurrentDomain.BaseDirectory, FILE_INFO_NAME, _fileName), GZipStreamHelper.GZipCompress(_bts));
          _dt.LoadDataRow(new object[] { _fileName, _fileVer, _bts.Length }, false);
        }
      }
      _dt.AcceptChanges();
      BaseDataCache.BaseDataTables.getInstance().Merge(FILE_INFO_NAME, _dt);
    }
  }
}
