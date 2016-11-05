using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using PnrAnalysis;

namespace BPiaoBao.Common.CommanPnrAnalyzer
{
    public class PnrFormattor
    {
        public string pnrContext;
        public string pnrPat;
        public PnrFormattor(string context)
        {
            var t = context.IndexOf("pat:a", 0, StringComparison.CurrentCultureIgnoreCase);
            if (t == -1) t = context.Length;
            this.pnrContext = context.Substring(0, t);
            this.pnrPat = context.Substring(t, context.Length - t);

        }
        public Tuple<string[], string> getPnrLines()
        {
            Dictionary<int, string> lines = new Dictionary<int, string>();
            string _pnrType = "1";
            Regex regex = new Regex(@"(?<li>\d{1,2})\.\D");
            var mc = regex.Match(pnrContext);
            int start; int end;
            while (mc.Success)
            {
                start = mc.Index;
                var line = mc.Groups["li"].Value.ToInt();
                mc = mc.NextMatch();
                if (mc.Success)
                    end = mc.Index;
                else
                    end = pnrContext.Length;

                string lineStr = pnrContext.Substring(start, end - start);

                lines[line] = lineStr;


            }
            if (lines.Min(p => p.Key) == 0)
                _pnrType = "2";
            return Tuple.Create<string[], string>(lines.Select(p => p.Value).ToArray(), _pnrType);

        }


        public PnrObject Format()
        {
            var lines = getPnrLines();
            var analyzer = new NameAnalyzer();
            IAnalyzer[] analyzers = { new SkywayAnalyzer(),new IDCardAnalyzer(),new TelAnalyzer(),new TicketNumAnalyzer(),
                                        new BigCodeAnalyzer(),new OfficeAnalyzer()};

            PnrObject obj = new PnrObject();
            obj.PnrType = lines.Item2;
            foreach (var pnrline in lines.Item1)
            {
                if (string.IsNullOrEmpty(obj.Pnr))
                {
                    analyzer.Analyze(obj, pnrline);
                    continue;
                }
                var oa = analyzers.Where(p => p.CanAnalyze(pnrline)).FirstOrDefault();
                if (oa != null)
                    oa.Analyze(obj, pnrline);
            }

            PatAnalyzer pat = new PatAnalyzer();
            pat.Analyzer(obj, pnrPat);


            return obj;
        }

    }

    #region rt解析
    public interface INameAnalyzer
    {
        void Analyze(PnrObject pnrObject, string pnrLine);

    }
    public interface IAnalyzer : INameAnalyzer
    {
        bool CanAnalyze(string pnrLine);
    }

    public class NameAnalyzer : INameAnalyzer //1.蔡佳诚 JYC2ZV or 1.蔡佳诚 or 1.魏嘉琪CHD HS73DL 
    {
        Regex nameAndPnrExp = new Regex(@"(?<id>\d)\.(?<name>.+)(?<pnr>\w{6})");
        Regex nameExp = new Regex(@"(?<id>\d)\.(?<name>.+)");
        public void Analyze(PnrObject pnrObject, string pnrLine)
        {
            Match mc;
            mc = nameAndPnrExp.Match(pnrLine);
            if (mc.Success)
                pnrObject.Pnr = mc.Groups["pnr"].Value.Trim();
            else
                mc = nameExp.Match(pnrLine);
            var passengerName = mc.Groups["name"].Value.Trim();
            var passenger = new Passenger();
            passenger.Id = mc.Groups["id"].Value;
            passenger.Name = passengerName;
            passenger.PassengerType = EnumPassengerType.ADU;
            if (passengerName.EndsWith("CHD"))
            {
                passenger.Name = passengerName.Substring(0, passengerName.Length - 3);
                passenger.PassengerType = EnumPassengerType.CHD;
            }

            pnrObject.Passengers.Add(passenger);

        }
    }
    public class SkywayAnalyzer : IAnalyzer //2.  CA1463 H SA13SEP PEKKWE HK1 1455 1800+1 E T3T2   _ 
    {
        Regex skyWay = new Regex(@"\.\s*(?<fn>\w{6})\s*(?<set>\w{1,2})\s*(?<t>\w{7})\s*(?<c>\w{6})\s*(?<state>\w{2,3})\s*(?<ft>\d{4})\s*(?<et>\d{4}(\+\d)?)\s*(?<ty>\w)\s*(?<tm>[\w\-]{4}|[\w\-]{2}|[\s]{4})(?<other>.*?)");
        public void Analyze(PnrObject pnrObject, string pnrLine)
        {
            var mc = skyWay.Match(pnrLine);
            if (mc.Success)
            {
                string Seat = mc.Groups["set"].Value;
                string childSeat = string.Empty;
                string other = mc.Groups["other"].Value;
                if (other.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries).Contains(Seat + "1"))
                {
                    childSeat = Seat + "1";
                }
                pnrObject.Skyways.Add(new PnrSkyway()
                {
                    FlightNo = mc.Groups["fn"].Value,
                    FormCode = mc.Groups["c"].Value.Substring(0, 3),
                    FormTerminal = Units.GetFTerminal(mc.Groups["tm"].Value),
                    Seat = mc.Groups["set"].Value,
                    ToCode = mc.Groups["c"].Value.Substring(3, 3),
                    ToTerminal = Units.GetTTerminal(mc.Groups["tm"].Value),
                    FormTime = Units.GetDateTime(mc.Groups["t"].Value, mc.Groups["ft"].Value),
                    ToTime = Units.GetDateTime(mc.Groups["t"].Value, mc.Groups["et"].Value),
                    SkyState = mc.Groups["state"].Value,
                    ChildSeat = childSeat
                });
            }

        }

        public bool CanAnalyze(string pnrLine)
        {
            return skyWay.IsMatch(pnrLine);
        }
    }

    public class IDCardAnalyzer : IAnalyzer //7.SSR FOID CA HK1 NI640121198011300811/P1 --
    {
        Regex cardReg = new Regex(@"\.\s*SSR\s*FOID\s*\w{2}\s*(?<st>\w{2,3})\s*NI(?<no>[^/]*)/P(?<in>\d)");
        public void Analyze(PnrObject pnrObject, string pnrLine)
        {
            var mc = cardReg.Match(pnrLine);
            if (mc.Success)
            {
                var i = mc.Groups["in"].Value;
                var passenger = pnrObject.Passengers.FirstOrDefault(p => p.Id == i);
                if (passenger != null)
                    passenger.CardNo = mc.Groups["no"].Value;
            }

        }
        public bool CanAnalyze(string pnrLine)
        {
            return cardReg.IsMatch(pnrLine);
        }
    }
    public class ChildBornAnlyzer : IAnalyzer // SSR CHLD CZ HK1 02JUL08/P1
    {
        Regex bornReg = new Regex(@"\.\s*SSR\s*CHLD\s*\w{2}\s*(?<st>\w{2,3})\s*(?<no>[^/]*)/P(?<in>\d)");
        public bool CanAnalyze(string pnrLine)
        {
            return bornReg.IsMatch(pnrLine);
        }

        public void Analyze(PnrObject pnrObject, string pnrLine)
        {
            var mc = bornReg.Match(pnrLine);
            if (mc.Success)
            {
                var i = mc.Groups["in"].Value;
                var passenger = pnrObject.Passengers.FirstOrDefault(p => p.Id == i);
                if (passenger != null)
                {
                    passenger.BornDay = mc.Groups["no"].Value;
                    passenger.PassengerType = EnumPassengerType.CHD;
                }
            }
        }
    }
    public class BabyAnlyzer : IAnalyzer // XN/IN/马晓INF(MAY12/P1 
    {
        Regex babyReg = new Regex(@"\s*(?<=XN\/IN\/)(?<YinerName>\w+\s*(INF)?\(?).*?/P(?<Num>\d+)\s*");
        public bool CanAnalyze(string pnrLine)
        {
            return babyReg.IsMatch(pnrLine);
        }
        public void Analyze(PnrObject pnrObject, string pnrLine)
        {
            var mc = babyReg.Match(pnrLine);
            if (mc.Success)
            {
                var i = mc.Groups["YinerName"].Value;
                pnrObject.Passengers.Add(new Passenger
                {
                    Name = i,
                    Id = string.Format("P{0}", mc.Groups["Num"].Value),
                    PassengerType = EnumPassengerType.BABY
                });
            }
        }
    }
    public class BabyCardNoAnlyzer : IAnalyzer //SSR INFT 3U HK1 CTUXIY 8555 Y31JUL SHANG/LING 23APR12/P1
    {
        Regex babyCarNoReg = new Regex(@"(?<=SSR\s*INFT)\s*(.*?)\s*\w{1,2}\d{2}\w{3}\s*(?<YinerName>.*?)\s*(?<YinerBirthday>\d{2}[A-Za-z]{3}\d{2})\s*(\/S(?<S>\d+))?\/P(?<Num>\d+)\s*");
        public bool CanAnalyze(string pnrLine)
        {
            return babyCarNoReg.IsMatch(pnrLine);
        }

        public void Analyze(PnrObject pnrObject, string pnrLine)
        {
            var mc = babyCarNoReg.Match(pnrLine);
            if (mc.Success)
            {
                var i = string.Format("P{0}", mc.Groups["Num"].Value);
                var passenger = pnrObject.Passengers.FirstOrDefault(p => p.PassengerType == EnumPassengerType.BABY && p.Id == i && mc.Groups["YingerName"].Value.Replace("/", "").Equals(PinYingMange.GetSpellByChinese(p.Name)));
                if (passenger != null)
                    passenger.BornDay = mc.Groups["YinerBirthday"].Value;
                else
                    pnrObject.Passengers.Add(new Passenger
                    {
                        Id = i,
                        PassengerType = EnumPassengerType.BABY,
                        Name = mc.Groups["YinerName"].Value.Trim(),
                        BornDay = mc.Groups["YinerBirthday"].Value.Trim()
                    });
            }
        }
    }


    public class TelAnalyzer : IAnalyzer //10.OSI CZ CTCM13940582959/P1 
    {
        Regex telReg = new Regex(@"\.\s*OSI\s*\w{2}\s*CTCM(?<tel>\d{11})/P(?<in>\d)");
        public void Analyze(PnrObject pnrObject, string pnrLine)
        {
            var mc = telReg.Match(pnrLine);
            if (mc.Success)
            {
                var i = mc.Groups["in"].Value;
                var passenger = pnrObject.Passengers.FirstOrDefault(p => p.Id == i);
                if (passenger != null)
                    passenger.Tel = mc.Groups["tel"].Value;
            }

        }
        public bool CanAnalyze(string pnrLine)
        {
            return telReg.IsMatch(pnrLine);
        }
    }
    public class TicketNumAnalyzer : IAnalyzer //13.SSR TKNE MU HK1 CTUNKG 2806 M22SEP 7812173436197/1/P1
    {
        Regex tnReg = new Regex(@"\.\s*SSR\s*TKNE\s*\w{2}\s*\w{2,3}\s*\w{6}\s*\d{4}\s*\w{6}\s*(?<tn>[\d\-]*)/\d/P(?<in>\d)");
        public void Analyze(PnrObject pnrObject, string pnrLine)
        {
            var mc = tnReg.Match(pnrLine);
            if (mc.Success)
            {
                var i = mc.Groups["in"].Value;
                var passenger = pnrObject.Passengers.FirstOrDefault(p => p.Id == i);
                if (passenger != null)
                    passenger.TicketNum = mc.Groups["tn"].Value;
            }

        }
        public bool CanAnalyze(string pnrLine)
        {
            return tnReg.IsMatch(pnrLine);
        }
    }
    public class BigCodeAnalyzer : IAnalyzer //13.RMK CA/MFM3BT -
    {
        Regex bigCodeReg = new Regex(@"\.(RMK\s*CA/|-CA-)(?<bc>\w{6})");
        public void Analyze(PnrObject pnrObject, string pnrLine)
        {
            var mc = bigCodeReg.Match(pnrLine);
            if (mc.Success)
            {
                pnrObject.BigPnr = mc.Groups["bc"].Value;
            }

        }
        public bool CanAnalyze(string pnrLine)
        {
            return bigCodeReg.IsMatch(pnrLine);
        }
    }
    public class OfficeAnalyzer : IAnalyzer //14.CTU186
    {
        Regex officeReg = new Regex(@"\.(?<of>[A-Z]{3}\d{3})");
        public void Analyze(PnrObject pnrObject, string pnrLine)
        {
            var mc = officeReg.Match(pnrLine);
            if (mc.Success)
            {
                pnrObject.Office = mc.Groups["of"].Value;
            }

        }
        public bool CanAnalyze(string pnrLine)
        {
            return officeReg.IsMatch(pnrLine);
        }
    }
    #endregion
    #region pat解析
    public class PatAnalyzer
    {
        //01 L FARE:CNY1050.00 TAX:CNY50.00 YQ:CNY120.00  TOTAL:1220.00
        //PAT:A 01 Y FARE:CNY1440.00 TAX:CNY50.00 YQ:CNY110.00 TOTAL:1600.00 SFC:01 SFN:01
        //PAT:A*CH>PAT:A*CH01 Y FARE:CNY670.00 TAX:TEXEMPTCN YQ:CNY60.00 TOTAL:730.00>SFC:01 
        Regex patReg = new Regex(@"FARE:(?<fa>\S*)\s*TAX:(?<t>\S*)\s*YQ:(?<y>\S*)\s*TOTAL:(?<to>\d*).00");
        public void Analyzer(PnrObject obj, string patContext)
        {
            var ms = patReg.Matches(patContext);
            foreach (Match m in ms)
            {

                obj.Pats.Add(new PatObject()
                {
                    FARE = Units.GetPrice(m.Groups["fa"].Value),
                    TAX = Units.GetPrice(m.Groups["t"].Value),
                    TOTAL = Units.GetPrice(m.Groups["to"].Value),
                    YQ = Units.GetPrice(m.Groups["y"].Value)
                });
            }
        }
    }
    #endregion
    public class PnrObject
    {
        public PnrObject()
        {
            this.PnrType = "1";
        }
        public string Pnr { get; set; }
        public string BigPnr { get; set; }
        public string Office { get; set; }
        /// <summary>
        /// 编码类型 1普通编码 2团编码
        /// </summary>
        public string PnrType { get; set; }
        private List<Passenger> passengers = new List<Passenger>();
        private List<PnrSkyway> skyways = new List<PnrSkyway>();
        public List<PnrSkyway> Skyways { get { return skyways; } }
        public List<Passenger> Passengers { get { return passengers; } }

        private List<PatObject> pats = new List<PatObject>();
        public List<PatObject> Pats { get { return pats; } }

    }
    public class PnrSkyway
    {
        /// <summary>
        /// 出发城市三字码
        /// </summary>
        public string FormCode { get; set; }
        /// <summary>
        /// 到达城市三字码
        /// </summary>
        public string ToCode { get; set; }
        /// <summary>
        /// 出发航站楼
        /// </summary>
        public string FormTerminal { get; set; }
        /// <summary>
        /// 到达航站楼
        /// </summary>
        public string ToTerminal { get; set; }
        /// <summary>
        /// 出发时间
        /// </summary>
        public DateTime FormTime { get; set; }
        /// <summary>
        /// 到达时间
        /// </summary>
        public DateTime ToTime { get; set; }
        /// <summary>
        /// 航班号
        /// </summary>
        public string FlightNo { get; set; }
        /// <summary>
        /// 舱位
        /// </summary>
        public string Seat { get; set; }
        /// <summary>
        /// 子舱位
        /// </summary>
        public string ChildSeat { get; set; }
        /// <summary>
        /// pnr航程状态
        /// </summary>
        public string SkyState { get; set; }
    }
    public class Passenger
    {
        private string name;
        internal string Id { get; set; }
        public string Name
        {
            get
            {//以CHD结尾儿童
                if (name.Contains("CHD"))
                    return name.Replace("CHD", "");
                return name;
            }
            set { name = value; }
        }
        /// <summary>
        /// 证件号
        /// </summary>
        public string CardNo { get; set; }
        /// <summary>
        /// 生日
        /// </summary>
        public string BornDay { get; set; }
        public string Tel { get; set; }
        public string TicketNum { get; set; }
        private EnumPassengerType _PassengerType = EnumPassengerType.ADU;
        public EnumPassengerType PassengerType
        {
            get
            {
                if (name.Contains("CHD"))
                    _PassengerType = EnumPassengerType.CHD;
                return _PassengerType;
            }
            set
            {
                _PassengerType = value;
            }
        }
    }
    public enum EnumPassengerType
    {
        ADU,
        CHD,
        BABY
    }
    public class PatObject
    {
        public decimal FARE { get; set; }
        public decimal TAX { get; set; }
        public decimal YQ { get; set; }
        public decimal TOTAL { get; set; }
    }
}
