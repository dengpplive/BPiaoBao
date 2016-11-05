using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;

namespace BPiaoBao.Web.SupplierManager.CommonHelpers
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
                case "Excel2007":
                    exportExcel = new EPPlusExcel();
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
    }
    public abstract class ExportExcel
    {
        public abstract string TypeName { get; }
        public abstract MemoryStream GetMemoryStream(DataTable dataTable);
    }
}