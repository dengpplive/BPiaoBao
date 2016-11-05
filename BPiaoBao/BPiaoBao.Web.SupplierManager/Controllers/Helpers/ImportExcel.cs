using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;

namespace BPiaoBao.Web.SupplierManager.Controllers.Helpers
{
    public class ImportExcel
    {
        /// <summary>
        /// Excel文档流是否有数据
        /// </summary>
        /// <param name="excelFileStream">Excel文档流</param>
        /// <returns></returns>
        public static bool HasData(Stream excelFileStream)
        {
            return HasData(excelFileStream, 0);
        }
        /// <summary>
        /// Excel文档流是否有数据
        /// </summary>
        /// <param name="excelFileStream">Excel文档流</param>
        /// <param name="sheetIndex">表索引号，如第一个表为0</param>
        /// <returns></returns>
        public static bool HasData(Stream excelFileStream, int sheetIndex)
        {
            using (excelFileStream)
            {
               IWorkbook workbook = new HSSFWorkbook(excelFileStream);

               if (workbook.NumberOfSheets > 0)
               {
                   if (sheetIndex < workbook.NumberOfSheets)
                   {
                       ISheet sheet = workbook.GetSheetAt(sheetIndex);

                       return sheet.PhysicalNumberOfRows > 0;

                   }
               }
            }
            return false;
        }
        private static string GetCellValue(ICell cell)
        {
            if (cell == null)
                return null;
            switch (cell.CellType)
            {
                case CellType.BLANK:
                    return string.Empty;
                case CellType.BOOLEAN:
                    return cell.BooleanCellValue.ToString();
                case CellType.ERROR:
                    return cell.ErrorCellValue.ToString();
                case CellType.FORMULA:
                    try
                    {
                        HSSFFormulaEvaluator e = new HSSFFormulaEvaluator(cell.Sheet.Workbook);
                        e.EvaluateInCell(cell);
                        return cell.ToString();
                    }
                    catch
                    {
                        return cell.NumericCellValue.ToString();
                    }
                case CellType.STRING:
                    return string.IsNullOrWhiteSpace(cell.StringCellValue) ? string.Empty : cell.StringCellValue.Trim();
                case CellType.NUMERIC:
                    if (DateUtil.IsCellDateFormatted(cell))
                    {
                        //  如果是date类型则 ，获取该cell的date值   
                        return cell.DateCellValue.ToString();
                    }
                    else
                    {
                        // 纯数字   
                        return cell.NumericCellValue.ToString();
                    }
                case CellType.Unknown:
                default:
                    return cell.ToString();
            }
        }
        /// <summary>
        /// 写入到datatable
        /// </summary>
        /// <param name="excelFileStream"></param>
        /// <returns></returns>
        public static DataTable RenderFromExcel(Stream excelFileStream)
        {
            using (excelFileStream)
            {
                IWorkbook workbook = new HSSFWorkbook(excelFileStream);

                ISheet sheet = workbook.GetSheetAt(0);
                //excel数据表
                DataTable table = new DataTable();
                //标题行
                IRow headerRow = sheet.GetRow(0);
                int cellCount = headerRow.LastCellNum;
                int rowCount = sheet.LastRowNum;
                //加入标题
                for (int i = headerRow.FirstCellNum; i < cellCount; i++)
                {
                    DataColumn column = new DataColumn(headerRow.GetCell(i).StringCellValue.Trim());
                    table.Columns.Add(column);
                }
                for (int i = (sheet.FirstRowNum + 1); i <= rowCount; i++)
                {
                    IRow row = sheet.GetRow(i);
                    DataRow dataRow = table.NewRow();
                    if (row != null)
                    {
                        for (int j = 0; j < cellCount; j++)
                        {
                           // if (row.GetCell(j) != null)
                                dataRow[j] = GetCellValue(row.GetCell(j));
                        }
                    }
                    table.Rows.Add(dataRow);
                }
                return table;
            }
        }
    }
}