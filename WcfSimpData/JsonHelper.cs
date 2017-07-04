using System;
using System.Collections.Generic;
using System.Data;
using System.Web.Script.Serialization;
using System.Xml;

namespace WcfSimpData
{
    public class JsonHelper
    {
        /// <summary>  
        ///     将JSON解析成DataSet（只限标准的JSON数据）  
        ///     例如：Json＝{t1:[{name:'数据name',type:'数据type'}]} 或 Json＝{t1:[{name:'数据name',type:'数据type'}],t2:[{id:'数据id',gx:'数据gx',val:'数据val'}]}  
        /// </summary>  
        /// <param name="json">Json字符串</param>  
        /// <returns>DataSet</returns>  
        public static DataSet JsonToDataSet(string json)
        {
            try
            {
                var ds = new DataSet();
                var jss = new JavaScriptSerializer();
                object obj = jss.DeserializeObject(json);
                var datajson = (Dictionary<string, object>)obj;
                foreach (var item in datajson)
                {
                    var dt = new DataTable(item.Key);
                    var rows = (object[])item.Value;
                    foreach (object row in rows)
                    {
                        var val = (Dictionary<string, object>)row;
                        DataRow dr = dt.NewRow();
                        foreach (var sss in val)
                        {
                            if (!dt.Columns.Contains(sss.Key))
                            {
                                dt.Columns.Add(sss.Key);
                                dr[sss.Key] = sss.Value;
                            }
                            else
                                dr[sss.Key] = sss.Value;
                        }
                        dt.Rows.Add(dr);
                    }
                    ds.Tables.Add(dt);
                }
                return ds;
            }
            catch
            {
                return null;
            }
        }


        /// <summary>  
        ///     将DataSet转化成JSON数据  
        /// </summary>  
        /// <param name="ds"></param>  
        /// <returns></returns>  
        public static string DataSetToJson(DataSet ds)
        {
            string json;
            try
            {
                if (ds.Tables.Count == 0)
                    throw new Exception("DataSet中Tables为0");
                json = "{";
                for (int i = 0; i < ds.Tables.Count; i++)
                {
                    json += "T" + (i + 1) + ":[";
                    for (int j = 0; j < ds.Tables[i].Rows.Count; j++)
                    {
                        json += "{";
                        for (int k = 0; k < ds.Tables[i].Columns.Count; k++)
                        {
                            json += ds.Tables[i].Columns[k].ColumnName + ":'" + ds.Tables[i].Rows[j][k] + "'";
                            if (k != ds.Tables[i].Columns.Count - 1)
                                json += ",";
                        }
                        json += "}";
                        if (j != ds.Tables[i].Rows.Count - 1)
                            json += ",";
                    }
                    json += "]";
                    if (i != ds.Tables.Count - 1)
                        json += ",";
                }
                json += "}";
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return json;
        }

        /// <summary>  
        ///     json字符串转换为Xml对象  
        /// </summary>  
        /// <param name="sJson"></param>  
        /// <returns></returns>  
        public static XmlDocument JsonToXml(string sJson)
        {
            var serializer = new JavaScriptSerializer();
            var dic = (Dictionary<string, object>)serializer.DeserializeObject(sJson);
            var doc = new XmlDocument();
            XmlDeclaration xmlDec = doc.CreateXmlDeclaration("1.0", "gb2312", "yes");
            doc.InsertBefore(xmlDec, doc.DocumentElement);
            XmlElement root = doc.CreateElement("root");
            doc.AppendChild(root);
            foreach (var item in dic)
            {
                XmlElement element = doc.CreateElement(item.Key);
                KeyValue2Xml(element, item);
                root.AppendChild(element);
            }
            return doc;
        }

        private static void KeyValue2Xml(XmlElement node, KeyValuePair<string, object> source)
        {
            object kValue = source.Value;
            if (kValue.GetType() == typeof(Dictionary<string, object>))
            {
                var dictionary = kValue as Dictionary<string, object>;
                if (dictionary != null)
                    foreach (var item in dictionary)
                    {
                        if (node.OwnerDocument != null)
                        {
                            XmlElement element = node.OwnerDocument.CreateElement(item.Key);
                            KeyValue2Xml(element, item);
                            node.AppendChild(element);
                        }
                    }
            }
            else if (kValue.GetType() == typeof(object[]))
            {
                var o = kValue as object[];
                if (o != null)
                    foreach (object t in o)
                    {
                        if (node.OwnerDocument != null)
                        {
                            XmlElement xitem = node.OwnerDocument.CreateElement("Item");
                            var item = new KeyValuePair<string, object>("Item", t);
                            KeyValue2Xml(xitem, item);
                            node.AppendChild(xitem);
                        }
                    }
            }
            else
            {
                if (node.OwnerDocument != null)
                {
                    XmlText text = node.OwnerDocument.CreateTextNode(kValue.ToString());
                    node.AppendChild(text);
                }
            }
        }
    }  
}
