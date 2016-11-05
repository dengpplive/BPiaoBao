using System;
using System.Collections.Generic;
using System.Text;
using PnrAnalysis.Model;
namespace PnrAnalysis
{
    /// <summary>
    /// Pnr实体信息
    /// </summary>
    /// 
    [Serializable]
    public class PnrModel
    {
        /// <summary>
        /// 传入编码
        /// </summary>
        public string Pnr = string.Empty;
        /// <summary>
        ///  pnr内容中解析出来编码
        /// </summary>
        public string _Pnr = string.Empty;
        /// <summary>
        /// 传入编码与Pnr内容解析出来的编码是否一致
        /// </summary>
        public bool _IsPnrSame = false;
        /// <summary>
        /// pnr内容中解析出来大编码
        /// </summary>
        public string _BigPnr = string.Empty;

        /// <summary>
        /// 处理过该编码后的RT数据
        /// </summary>
        public string _FormatPnrContent = string.Empty;

        /// <summary>
        /// 未处理过该编码后的RT数据
        /// </summary>
        public string _OldPnrContent = string.Empty;

        /// <summary>
        /// pnr内容中解析出来的Office
        /// </summary>
        public string _Office = string.Empty;

        /// <summary>
        /// 当前用于提指令使用的Office
        /// </summary>
        public string _CurrUseOffice = string.Empty;

        /// <summary>
        /// Pnr中是否存在SSR FOID证件号项 但不一定取的到证件号
        /// 
        /// </summary>
        public bool _IsExistSsrFoid = false;

        private int _GetMaxNumber = 0;
        /// <summary>
        /// 获取编码中的最大序号是多少
        /// </summary>
        public int GetMaxNumber
        {
            get
            {
                return _GetMaxNumber;
            }
            set
            {
                _GetMaxNumber = value;
            }
        }
        private bool _PnrConIsOver = false;
        /// <summary>
        /// 编码内容是否完整 true 完整 false 不完整
        /// </summary>
        public bool PnrConIsOver
        {
            get { return _PnrConIsOver; }
            set { _PnrConIsOver = value; }
        }

        private bool _IsExistChildSeat = false;
        /// <summary>
        /// 是否存在子舱位 true存在 false 不存在
        /// </summary>
        public bool IsExistChildSeat
        {
            get { return _IsExistChildSeat; }
            set { _IsExistChildSeat = value; }
        }
        /// <summary>
        /// 乘机人中是否有婴儿 true有false没有
        /// </summary>
        public bool HasINF
        {
            get
            {
                bool IsInf = false;
                foreach (PassengerInfo pas in _PassengerList)
                {
                    if (pas.PassengerType == "3")
                    {
                        IsInf = true;
                        break;
                    }
                }
                return IsInf;
            }
        }
        /// <summary>
        /// 乘机人中证件号是否重复  true有false没有
        /// </summary>
        public bool SsrIsRepeat
        {
            get
            {
                bool _SsrIsRepeat = false;
                List<string> lst = new List<string>();
                foreach (PassengerInfo pas in _PassengerList)
                {
                    if (pas.SsrCardID.Trim() != "" && pas.PassengerType != "3")
                    {
                        if (!lst.Contains(pas.SsrCardID.Trim()))
                        {
                            lst.Add(pas.SsrCardID.Trim());
                        }
                        else
                        {
                            _SsrIsRepeat = true;
                            break;
                        }
                    }
                }
                return _SsrIsRepeat;
            }
        }

        /// <summary>
        /// 乘客是否含有证件号
        /// </summary>
        public bool HasExistNoSsr
        {
            get
            {
                bool _HasExistNoSsr = false;
                foreach (PassengerInfo pas in _PassengerList)
                {
                    if (pas.PassengerType != "3")
                    {
                        if (pas.SsrCardID.Trim() == "")
                        {
                            _HasExistNoSsr = true;
                            break;
                        }
                    }
                }
                return _HasExistNoSsr;
            }
        }


        /// <summary>
        /// 乘机人证件号是否全部为空 true是false否
        /// </summary>
        /// <returns></returns>
        public bool SsrIsAllEmpty
        {
            get
            {
                bool _SsrIsRepeat = true;
                List<string> lst = new List<string>();
                foreach (PassengerInfo pas in _PassengerList)
                {
                    if (pas.SsrCardID.Trim() != "")
                    {
                        _SsrIsRepeat = false;
                        break;
                    }
                }
                return _SsrIsRepeat;
            }
        }


        /// <summary>
        /// 是否为缺口程 true是 false否
        /// </summary>
        public bool HasGapProcess
        {
            get
            {
                bool GapProcess = false;
                if (_LegList.Count >= 2)
                {
                    LegInfo leg0 = _LegList[0];
                    LegInfo leg1 = _LegList[1];
                    if (leg0.ToCode != leg1.FromCode)
                    {
                        GapProcess = true;
                    }
                }
                return GapProcess;
            }
        }
        /// <summary>
        /// 1=单程，2=往返，3=中转联程 4缺口程 5多程
        /// </summary>
        public int TravelType
        {
            get
            {
                int _TravelType = 1;
                if (_LegList.Count >= 2)
                {
                    if (_LegList.Count > 2)
                    {
                        _TravelType = 5;
                    }
                    else
                    {
                        LegInfo leg0 = _LegList[0];
                        LegInfo leg1 = _LegList[1];
                        if (leg0.ToCode == leg1.FromCode)
                        {
                            if (leg0.FromCode == leg1.ToCode)
                            {
                                _TravelType = 2;
                            }
                            else
                            {
                                _TravelType = 3;
                            }
                        }
                        else
                        {
                            _TravelType = 4;
                        }
                    }
                }
                return _TravelType;
            }
        }
        /// <summary>
        /// Pnr状态用"|"隔开 HK1|HK2
        /// </summary>
        public string PnrStatus
        {
            get
            {
                string _PnrStatus = "";
                foreach (LegInfo leg in _LegList)
                {
                    _PnrStatus += leg.PnrStatus + "|";
                }
                return _PnrStatus.Trim(new char[] { '|' }).ToUpper();
            }
        }


        private bool _PassengerNameIsCorrent = true;
        /// <summary>
        /// 乘机人名字中是否有不正确的名字 true正确 false不正确
        /// </summary>
        public bool PassengerNameIsCorrent
        {
            get
            {
                return _PassengerNameIsCorrent;
            }
            set
            {
                _PassengerNameIsCorrent = value;
            }
        }
        private bool _PnrConHasFirstNum = false;
        /// <summary>
        /// Pnr内容中乘客姓名序号是否是以1.开始
        /// </summary>
        public bool PnrConHasFirstNum
        {
            get
            {
                return _PnrConHasFirstNum;
            }
            set
            {
                _PnrConHasFirstNum = value;
            }
        }



        private List<string> _strErrList = new List<string>();
        /// <summary>
        /// 出错的乘机人名字
        /// </summary>
        public List<string> ErrorPassengerNameList
        {
            get
            {
                return _strErrList;
            }
            set
            {
                _strErrList = value;
            }
        }

        /// <summary>
        /// 乘客类型 1：成人编码 2儿童编码
        /// </summary>
        public string _PasType = "1";

        /// <summary>
        /// 编码类型 1普通编码 2团编码
        /// </summary>
        public string _PnrType = "1";

        /// <summary>
        /// 错误提示
        /// </summary>
        public string _ErrMsg = string.Empty;

        /// <summary>
        /// 解析时间 格式: 刻度值|时间
        /// </summary>
        public string _AnalysisTime = "0";

        /// <summary>
        /// 航空公司二字码
        /// </summary>
        public string _CarryCode = string.Empty;

        /// <summary>
        /// 每段航程的航空公司二字码 CA|CZ
        /// </summary>
        public string LegAirCode
        {
            get
            {
                List<string> airCodeList = new List<string>();
                foreach (var item in this._LegList)
                {
                    airCodeList.Add(item.AirCode.ToUpper());
                }
                return string.Join("|", airCodeList.ToArray());
            }
        }
        /// <summary>
        /// 编码中授权的Office号集合
        /// </summary>
        public List<string> _RMK_TJ_AUTH_List = new List<string>();

        /// <summary>
        /// 乘客姓名列表
        /// </summary>
        public List<PassengerInfo> _PassengerList = new List<PassengerInfo>();
        /// <summary>
        /// 航段信息列表
        /// </summary>
        public List<LegInfo> _LegList = new List<LegInfo>();
        /// <summary>
        /// 乘客证件号列表
        /// </summary>
        public List<SSRInfo> _SSRList = new List<SSRInfo>();


        /// <summary>
        /// 一些较杂的信息
        /// </summary>
        public OtherInfo _Other = new OtherInfo();

        /// <summary>
        /// 编码价格项信息
        /// </summary>
        public List<PatInfo> _PatList = new List<PatInfo>();

        /// <summary>
        /// 团体信息列表
        /// </summary>
        public GroupInfo _Tuan = new GroupInfo();


        /// <summary>
        /// 票号列表
        /// </summary>
        public List<TicketNumInfo> _TicketNumList = new List<TicketNumInfo>();
        /// <summary>
        /// 移除重复票号
        /// </summary>
        /// <returns></returns>
        public List<TicketNumInfo> MoveRepeatTicketNum(List<TicketNumInfo> TicketNumList)
        {
            System.Collections.Hashtable table = new System.Collections.Hashtable();
            List<TicketNumInfo> m_TicketNumList = new List<TicketNumInfo>();
            string key = "";
            foreach (TicketNumInfo item in TicketNumList)
            {
                key = item.PasName + item.TicketNum.Replace("-", "");
                if (!table.ContainsKey(key))
                {
                    table.Add(key, item);
                    m_TicketNumList.Add(item);
                }
            }
            return m_TicketNumList;
        }
    }
}
