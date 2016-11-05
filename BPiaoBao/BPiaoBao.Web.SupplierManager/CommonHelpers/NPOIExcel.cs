using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;

namespace BPiaoBao.Web.SupplierManager.CommonHelpers
{
    public class NPOIExcel : ExportExcel
    {
        public override MemoryStream GetMemoryStream(DataTable dataTable)
        {
            MemoryStream ms = new MemoryStream();

            using (dataTable)
            {
                IWorkbook workbook = new HSSFWorkbook();

                ISheet sheet = workbook.CreateSheet();

                IRow headerRow = sheet.CreateRow(0);

                //表头
                foreach (DataColumn column in dataTable.Columns)
                    headerRow.CreateCell(column.Ordinal).SetCellValue(column.Caption);

                // handling value.
                int rowIndex = 1;

                foreach (DataRow row in dataTable.Rows)
                {
                    IRow dataRow = sheet.CreateRow(rowIndex);

                    foreach (DataColumn column in dataTable.Columns)
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

        public override string TypeName
        {
            get { return "xls"; }
        }
    }
    public class EPPlusExcel : ExportExcel
    {
        public override MemoryStream GetMemoryStream(DataTable dataTable)
        {
            using (ExcelPackage excel = new ExcelPackage())
            {
                ExcelWorksheet sheet = excel.Workbook.Worksheets.Add(dataTable.TableName);
                sheet.Cells["A1"].LoadFromDataTable(dataTable, true);
                var data = excel.GetAsByteArray();
                return new MemoryStream(data);
            }
        }

        public override string TypeName
        {
            get { return "xlsx"; }
        }
    }
}