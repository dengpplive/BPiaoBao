using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;

namespace JoveZhao.Framework.Excel
{
    public class ExportExcelContext
    {
        private ExportExcel exportExcel = null;
        public ExportExcelContext(string type)
        {
            switch (type)
            {
                case "Excel2003":
                    exportExcel = new NPOIExcel();
                    break;
                default:
                    exportExcel = new NPOIExcel();
                    break;
            }
        }
        public string TypeName
        {
            get
            {
                return exportExcel.TypeName;
            }
        }
        public MemoryStream GetMemoryStream(DataTable dataTable)
        {
            return exportExcel.GetMemoryStream(dataTable);
        }
        public string Write(string path, DataTable dataTable)
        {
            return exportExcel.Write(path, dataTable);
        }
    }
    public abstract class ExportExcel
    {
        public abstract string TypeName { get; }
        public abstract MemoryStream GetMemoryStream(DataTable dataTable);
        public abstract string Write(string path, DataTable dataTable);
    }
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


        public override string Write(string path, DataTable dataTable)
        {
            string fileName = string.Format("{0}.{1}", Guid.NewGuid().ToString(), this.TypeName);

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
            using (FileStream fileStream = new FileStream(Path.Combine(path, fileName), FileMode.Create, FileAccess.Write))
            {
                workbook.Write(fileStream);
            }
            return fileName;
        }
    }
}
