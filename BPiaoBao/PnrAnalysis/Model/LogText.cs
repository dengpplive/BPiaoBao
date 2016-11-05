using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Data;

namespace PnrAnalysis
{
    /// <summary>
    /// 文本日志
    /// </summary>
    public class LogText
    {
        public static object root = new object();
        public static void LogWrite(string Con, string dir)
        {
            lock (root)
            {
                string path = "";
                if (!string.IsNullOrEmpty(Con))
                {
                    try
                    {
                        path = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase;// +"Log";
                        if (!path.EndsWith("\\"))
                        {
                            path = path + "\\Log";
                        }
                        else
                        {
                            path = path + "Log";
                        }
                        if (!string.IsNullOrEmpty(dir))
                        {
                            path = path + "\\" + dir.Trim(new char[] { '\\' });
                        }
                        if (!System.IO.Directory.Exists(path))
                        {
                            System.IO.Directory.CreateDirectory(path);
                        }
                        if (!path.EndsWith("\\"))
                        {
                            path = path + "\\";
                        }
                        System.IO.File.AppendAllText(path + System.DateTime.Now.ToString("yyyy-MM-dd HH") + ".txt", Con);
                    }
                    catch
                    {
                        System.IO.File.AppendAllText(path + System.DateTime.Now.ToString("yyyy-MM-dd HH") + ".txt", Con);
                    }
                }
            }
        }
    }

    /// <summary>
    /// 实体转化文本日志
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class LogModel<T>
    {
        /// <summary>
        /// 记录一个实体中的所有基本数据 key=value
        /// </summary>
        /// <param name="strDesc">实体说明</param>
        /// <param name="model">实体对象</param>
        /// <param name="dir">子目录 没有可为空</param>
        public static string LogWrite(string strDesc, T model, string dir)
        {
            StringBuilder sbLog = new StringBuilder();
            try
            {
                sbLog.AppendFormat("【时间:{0}】=======================================================\r\n", System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                if (!string.IsNullOrEmpty(strDesc))
                {
                    sbLog.AppendFormat("\r\n{0}:\r\n", strDesc);
                }
                List<string> PropertyList = new List<string>();
                Type t = model.GetType();

                if (t.FullName == "System.Data.DataTable")
                {
                    DataTable table = model as DataTable;
                    if (table != null && table.Rows.Count > 0)
                    {
                        DataRowCollection rcs = table.Rows;
                        DataColumnCollection dcs = table.Columns;
                        int i = 0;
                        foreach (DataRow dr in rcs)
                        {
                            i++;
                            PropertyList.Add("表名:" + table.TableName + "" + "第" + i + "行数据:\r\n");
                            StringBuilder sbRowData = new StringBuilder();
                            object obj = null;
                            foreach (DataColumn dc in dcs)
                            {
                                obj = dr[dc.ColumnName] == DBNull.Value ? "null" : dr[dc.ColumnName].ToString();
                                sbRowData.Append(dc.ColumnName + "=" + obj + "\r\n");
                            }
                            PropertyList.Add(sbRowData.ToString());
                        }
                    }
                }
                else if (t.FullName == "System.Data.DataSet")
                {
                    DataSet ds = model as DataSet;
                    if (ds != null && ds.Tables.Count > 0)
                    {
                        foreach (DataTable table in ds.Tables)
                        {
                            if (table != null && table.Rows.Count > 0)
                            {
                                DataRowCollection rcs = table.Rows;
                                DataColumnCollection dcs = table.Columns;
                                int i = 0;
                                foreach (DataRow dr in rcs)
                                {
                                    i++;
                                    PropertyList.Add("表名:" + table.TableName + "" + "第" + i + "行数据:\r\n");
                                    StringBuilder sbRowData = new StringBuilder();
                                    object obj = null;
                                    foreach (DataColumn dc in dcs)
                                    {
                                        obj = dr[dc.ColumnName] == DBNull.Value ? "null" : dr[dc.ColumnName].ToString();
                                        sbRowData.Append(dc.ColumnName + "=" + obj + "\r\n");
                                    }
                                    PropertyList.Add(sbRowData.ToString());
                                }
                            }
                        }
                    }
                }
                else
                {
                    PropertyInfo[] properties = t.GetProperties(BindingFlags.Public | BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.GetProperty);
                    object obj = null;
                    foreach (PropertyInfo p in properties)
                    {
                        obj = p.GetValue(model, null);
                        PropertyList.Add(p.Name + "=" + (obj == null ? "null" : obj));
                    }
                }
                if (PropertyList.Count > 0)
                {
                    sbLog.Append(string.Join("\r\n", PropertyList.ToArray()));
                }
                sbLog.Append("结束=======================================================\r\n");
                //记录日志
                LogText.LogWrite(sbLog.ToString(), dir);
            }
            catch (Exception)
            {
            }
            finally
            {

            }
            return sbLog.ToString();
        }
    }
}
