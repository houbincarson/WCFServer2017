using System;
using System.Collections.Generic;
using System.IO;

namespace WcfService
{
    public abstract class FileSplitUpLoad
    {
        public const string ErrorMsg = "ERROR";
        public const string UpLoadCompleteMsg = "Complete";
        public const string UpLoadDownMsg = "OK";
        #region 文件分段上传处理
        public static string FilesUpLoad(string cachePath, string savePath, string fileName, string fileCount, string fileState, byte[] fileBuf)
        {
            var picFileName = string.Format("{0}\\{1}_{2}", cachePath, fileName, fileCount);//F_Cnt  分段索引值，从１开始
            if (File.Exists(picFileName))
            {
                File.SetAttributes(picFileName, FileAttributes.Normal);
            }
            try
            {
                File.WriteAllBytes(picFileName, fileBuf);
            }
            catch (Exception e)
            {
                return string.Format("{0}={1}{2}", ErrorMsg, "上传文件出错：", e);
            }
            if (fileState.Equals("1"))//F_State=1　文件上传结束
            {
                string outMsg;
                if (!MarkFinshFile(fileName, fileCount, out outMsg, cachePath, savePath))
                {
                    return string.Format("{0}={1},{2}", ErrorMsg, "文件合成失败", outMsg);
                }
                else
                {
                    return UpLoadCompleteMsg;
                }
            }
            return UpLoadDownMsg;
        }
        /// <summary>
        /// 将被分割的缓存文件，合成最终文件
        /// </summary>
        /// <param name="model_id"></param>
        /// <param name="f_cnt"></param>
        /// <returns></returns>
        public static bool MarkFinshFile(string filename, string f_cnt, out string errorMsg, string cachePath, string savePath)
        {
            errorMsg = string.Empty;
            string picFileName;
            int i = 1, iCnt;
            int.TryParse(f_cnt, out iCnt);
            iCnt++;//文件记数从１开始
            List<byte> btlis = new List<byte>();
            for (; i < iCnt; i++)
            {
                picFileName = string.Format("{0}\\{1}_{2}",cachePath,filename,i);
                if (!File.Exists(picFileName))//如果缓存文件不存在，取消合成
                {
                    errorMsg = string.Format("不存在文件：{0}_{1}", filename, i);
                    btlis.Clear();
                    return false;
                }
                else
                {
                    var bts = File.ReadAllBytes(picFileName);
                    btlis.AddRange(bts);
                    bts = null;
                }
            }
            picFileName = string.Format("{0}\\{1}", savePath, filename);
            File.WriteAllBytes(picFileName, btlis.ToArray());
            btlis.Clear();

            for (i = 1; i < iCnt; i++)
            {
                picFileName = string.Format("{0}\\{1}_{2}",cachePath,filename,i);
                try
                {
                    File.Delete(picFileName);
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
            return true;
        }
        #endregion
    }
}
