using JoveZhao.Framework;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace BPiaoBao.Client.UIExt.Model
{
    /// <summary>
    /// 城市数据读取
    /// </summary>
    public sealed class CityData
    {
        /// <summary>
        /// 获取所有省，直辖市，行政区
        /// </summary>
        /// <returns></returns>
        public static List<CityModel> GetAllState()
        {
            HSSFWorkbook book = null;
            try
            {
                using (FileStream file = new FileStream("city.xls", FileMode.Open, FileAccess.Read))
                {
                    book = new HSSFWorkbook(file);
                }

                var tbArea = book.GetSheet("city");//表

                List<CityModel> result = new List<CityModel>();
                //排除第一行
                for (int i = 1; i < tbArea.PhysicalNumberOfRows; i++)
                {
                    int idIndex = SearchColumnIndex("id", tbArea);
                    int cityIndex = SearchColumnIndex("city", tbArea);
                    int nameIndex = SearchColumnIndex("name", tbArea);
                    int stateIndex = SearchColumnIndex("state", tbArea);
                    int szCodeIndex = SearchColumnIndex("sz_code", tbArea);
                    int romeIndex = SearchColumnIndex("Rome", tbArea);
                    int zmCodeIndex = SearchColumnIndex("zm_code", tbArea);

                    var model = new CityModel();
                    model.State = GetValue(tbArea, i, stateIndex);
                    //排除重复
                    if (result.FirstOrDefault(m => m.State == model.State) != null)
                        continue;
                    model.Id = GetValue(tbArea, i, idIndex);
                    model.City = GetValue(tbArea, i, cityIndex);
                    model.Rome = GetValue(tbArea, i, romeIndex);
                    model.SZCode = GetValue(tbArea, i, szCodeIndex);
                    model.ZMCode = GetValue(tbArea, i, zmCodeIndex);
                    result.Add(model);
                }

                return result;
            }
            catch (Exception ex)
            {
                Logger.WriteLog(LogType.ERROR, "获取所有城市失败", ex);
                return null;
            }
        }

        /// <summary>
        /// 获取区域下的城市
        /// </summary>
        /// <param name="areaName"></param>
        /// <returns></returns>
        public static List<CityModel> GetCity(string areaName)
        {
            HSSFWorkbook book = null;
            try
            {
                using (FileStream file = new FileStream("city.xls", FileMode.Open, FileAccess.Read))
                {
                    book = new HSSFWorkbook(file);
                }

                var tbArea = book.GetSheet("city");//表

                List<CityModel> result = new List<CityModel>();
                //排除第一行
                for (int i = 1; i < tbArea.PhysicalNumberOfRows; i++)
                {
                    int idIndex = SearchColumnIndex("id", tbArea);
                    int cityIndex = SearchColumnIndex("city", tbArea);
                    int nameIndex = SearchColumnIndex("name", tbArea);
                    int stateIndex = SearchColumnIndex("state", tbArea);
                    int szCodeIndex = SearchColumnIndex("sz_code", tbArea);
                    int romeIndex = SearchColumnIndex("Rome", tbArea);
                    int zmCodeIndex = SearchColumnIndex("zm_code", tbArea);

                    var model = new CityModel();
                    model.State = GetValue(tbArea, i, stateIndex);
                    //排除重复
                    if (model.State != areaName)
                        continue;

                    model.Id = GetValue(tbArea, i, idIndex);
                    model.City = GetValue(tbArea, i, cityIndex);
                    model.Rome = GetValue(tbArea, i, romeIndex);
                    model.SZCode = GetValue(tbArea, i, szCodeIndex);
                    model.ZMCode = GetValue(tbArea, i, zmCodeIndex);
                    result.Add(model);
                }

                return result;
            }
            catch (Exception ex)
            {
                Logger.WriteLog(LogType.ERROR, "获取城市失败", ex);
                return null;
            }
        }

        /// <summary>
        /// 根据省份名称获取省份信息
        /// </summary>
        /// <param name="stateName"></param>
        public static CityModel GetStatesByName(string stateName)
        {
            var list = GetAllState();
            return list.FirstOrDefault(p => p.State.Equals(stateName));
        }

        /// <summary>
        /// 根据省份名称和城市名称获取城市信息
        /// </summary>
        /// <param name="stateName"></param>
        /// <param name="cityName"></param>
        /// <returns></returns>
        public static CityModel GetCityByStateAndCityName(string stateName, string cityName)
        {
            var list = GetCity(stateName);
            return list.FirstOrDefault(p => p.City.Equals(cityName));
        }


        private static string GetValue(ISheet tbArea, int rowIndex, int columnIndex)
        {
            var row = tbArea.GetRow(rowIndex);
            if (row == null)
                return null;

            var cell = row.GetCell(columnIndex);
            if (cell == null)
                return null;

            switch (cell.CellType)
            {
                case CellType.String: return cell.StringCellValue;
                case CellType.Numeric: return cell.NumericCellValue.ToString();
            }

            return null;
        }

        private static int SearchColumnIndex(string columnName, ISheet tb)
        {
            var firstRow = tb.GetRow(0);

            int columnCount = firstRow.PhysicalNumberOfCells;
            for (int i = 0; i < columnCount; i++)
            {
                var cell = firstRow.GetCell(i);
                if (cell == null)
                    continue;

                var str = cell.StringCellValue;
                if (str != null && str.ToLower() == columnName.ToLower())
                    return i;
            }
            return -1;
        }
    }

    /// <summary>
    /// 表示一个区域
    /// </summary>
    public class AreaModel
    {
        /// <summary>
        /// 编码
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 区域名
        /// </summary>
        public string Name { get; set; }

        public string Rome { get; set; }

        public string SZCode { get; set; }

        public string ZmCode { get; set; }
    }

    /// <summary>
    /// 表示一个城市
    /// </summary>
    public class CityModel
    {
        /// <summary>
        /// 编码
        /// </summary>
        public string Id { get; set; }

        public string State { get; set; }

        public string City { get; set; }

        public string SZCode { get; set; }

        public string Rome { get; set; }

        public string ZMCode { get; set; }
    }





    /// <summary>
    /// 机票预订页面城市选择Model
    /// </summary>
    public class CityNewModel
    {
        public CityInfoModel Info { get; set; }
        public string Key { get; set; }
    }

    public class CityInfoModel
    {
        public string AirPortName { get; set; }
        public string Code { get; set; }
        public string JPPinyin { get; set; }
        public string Name { get; set; }
        public string PinYin { get; set; }

        public override string ToString()
        {
            var buf = string.Format("AirPortName={0},Code={1},JPPinyin={2},Name={3},PinYin={4}",
                AirPortName, Code, JPPinyin, Name, PinYin);
            return buf;
        }

    }
}
