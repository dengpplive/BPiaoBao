using PnrAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PnrAnalysis.Model;

namespace BPiaoBao.Common
{
    public class PnrData
    {
        public PnrModel PnrMode { get; set; }
        public PatModel PatMode { get; set; }
        public PatModel InfPatMode { get; set; }
        public string AdultPnr { get; set; }
        public string ChdPnr { get; set; }
    }
    public class PnrHelper
    {
        public static string DefAccount = "ctuadmin";
        public static string DefOffice = "ctu186";
        public static string GetArea(string pnrContent)
        {
            string area = "";
            PnrAnalysis.PnrModel pnrMode = GetPnrModel(pnrContent);
            area = pnrMode != null && pnrMode._LegList.Count > 0 ? pnrMode._LegList[0].FromCode : "";
            return area;
        }
        public static PnrModel GetPnrModel(string pnrContent)
        {
            return GetPnrData(pnrContent).PnrMode;
        }
        public static PnrData GetPnrData(string pnrContent)
        {
            FormatPNR format = new FormatPNR();
            string Msg = string.Empty;
            SplitPnrCon splitPnrCon = format.GetSplitPnrCon(pnrContent);
            string RTCon = splitPnrCon.RTCon;
            string PatCon = splitPnrCon.AdultPATCon != string.Empty ? splitPnrCon.AdultPATCon : splitPnrCon.ChdPATCon;
            string Pnr = format.GetPNR(RTCon, out Msg);
            PnrModel pnrMode = format.GetPNRInfo(Pnr, RTCon, false, out Msg);
            //成人或者儿童PAT
            PatModel patMode = format.GetPATInfo(PatCon, out Msg);
            return new PnrData()
            {
                PnrMode = pnrMode,
                PatMode = patMode
            };
        }
    }
}
