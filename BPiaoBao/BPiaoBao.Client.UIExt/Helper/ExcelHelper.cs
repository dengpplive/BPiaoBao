using System.Data.OleDb;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System;
using System.Data;
using System.IO;
using NPOI.XSSF.UserModel;

namespace BPiaoBao.Client.UIExt.Helper
{
    /// <summary>
    /// Excel生成助手
    /// </summary>
    public class ExcelHelper
    {
        #region 公开方法

        /// <summary>
        /// DataTable转换成Excel文档流
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        public static MemoryStream RenderToExcel(DataTable table)
        {
            MemoryStream ms = new MemoryStream();

            using (table)
            {
                IWorkbook workbook = new HSSFWorkbook();

                ISheet sheet = workbook.CreateSheet();

                IRow headerRow = sheet.CreateRow(0);

                //表头
                foreach (DataColumn column in table.Columns)
                    headerRow.CreateCell(column.Ordinal).SetCellValue(column.Caption);

                // handling value.
                int rowIndex = 1;

                foreach (DataRow row in table.Rows)
                {
                    IRow dataRow = sheet.CreateRow(rowIndex);

                    foreach (DataColumn column in table.Columns)
                    {
                        dataRow.CreateCell(column.Ordinal).SetCellValue(row[column].ToString());
                    }

                    rowIndex++;
                }
                AutoSizeColumns(sheet);

                workbook.Write(ms);
                ms.Flush();
                ms.Position = 0;
            }

            return ms;
        }

        /// <summary>
        /// DataTable转换成Excel文档流，并保存到文件
        /// </summary>
        /// <param name="table"></param>
        /// <param name="filePath">保存的路径</param>
        public static void RenderToExcel(DataTable table, string filePath)
        {
            using (MemoryStream ms = RenderToExcel(table))
            {
                using (FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                {
                    byte[] data = ms.ToArray();

                    fs.Write(data, 0, data.Length);
                    fs.Flush();

                    data = null;
                }
            }
        }


        /// <summary>
        /// 通过OLEDB导入Excel
        /// </summary>
        /// <param name="excelPath"></param>
        /// <returns></returns>
        public static DataTable RenderToTableByOLEDB(string excelPath)
        {
            var strConn = "Provider=Microsoft.Jet.OleDb.4.0;" + "data source=" + excelPath + ";Extended Properties='Excel 8.0; HDR=YES; IMEX=1'";
            var conn = new OleDbConnection(strConn);
            conn.Open();
            try
            {
                var table = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                var tableName = table == null ? "Sheet0" : table.Rows[0]["Table_Name"].ToString();
                var strExcel = "select * from " + "[" + tableName + "]";
                var odda = new OleDbDataAdapter(strExcel, conn);
                var ds = new DataSet();
                odda.Fill(ds);
                return ds.Tables[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                conn.Close();
            }
        }

        /// <summary>
        /// 通过NOPI导入Excel文件
        /// </summary>
        /// <param name="excelPath"></param>
        /// <param name="isFirstRowColumn"></param>
        /// <returns></returns>
        public static DataTable RenderToTableByNOPI(string excelPath, bool isFirstRowColumn = true)
        {
            IWorkbook workbook = null;
            FileStream fs = null;

            ISheet sheet = null;
            var data = new DataTable();
            int startRow = 0;
            try
            {
                
                fs = new FileStream(excelPath, FileMode.Open, FileAccess.Read);
                if (excelPath.IndexOf(".xlsx") > 0) // 2007版本
                    workbook = new XSSFWorkbook(fs);
                else if (excelPath.IndexOf(".xls") > 0) // 2003版本
                    workbook = new HSSFWorkbook(fs);

                sheet = workbook.GetSheetAt(0);

                if (sheet != null)
                {
                    IRow firstRow = sheet.GetRow(0);
                    int cellCount = firstRow.LastCellNum; //一行最后一个cell的编号 即总的列数

                    if (isFirstRowColumn)
                    {
                        for (int i = firstRow.FirstCellNum; i < cellCount; ++i)
                        {
                            DataColumn column = new DataColumn(firstRow.GetCell(i).StringCellValue);
                            data.Columns.Add(column);
                        }
                        startRow = sheet.FirstRowNum + 1;
                    }
                    else
                    {
                        startRow = sheet.FirstRowNum;
                    }

                    //最后一列的标号
                    int rowCount = sheet.LastRowNum;
                    for (int i = startRow; i <= rowCount; ++i)
                    {
                        IRow row = sheet.GetRow(i);
                        if (row == null) continue; //没有数据的行默认是null　　　　　　　

                        DataRow dataRow = data.NewRow();
                        for (int j = row.FirstCellNum; j < cellCount; ++j)
                        {
                            if (row.GetCell(j) != null) //同理，没有数据的单元格都默认是null
                                dataRow[j] = row.GetCell(j).ToString();
                        }
                        data.Rows.Add(dataRow);
                    }
                }

                return data;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
                throw new Exception("请先关闭要导入的Excel文件");
            }
            return data;
        }
        #endregion

        #region 私有方法

        /// <summary>
        /// 自动设置Excel列宽
        /// </summary>
        /// <param name="sheet">Excel表</param>
        private static void AutoSizeColumns(ISheet sheet)
        {
            if (sheet.PhysicalNumberOfRows <= 0)
                return;

            IRow headerRow = sheet.GetRow(0);

            for (int i = 0, l = headerRow.LastCellNum; i < l; i++)
            {
                sheet.AutoSizeColumn(i);
            }
        }

        #endregion
    }
}
