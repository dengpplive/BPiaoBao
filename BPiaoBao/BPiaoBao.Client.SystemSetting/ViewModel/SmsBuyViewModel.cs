using BPiaoBao.AppServices.Contracts.SystemSetting;
using BPiaoBao.Client.SystemSetting.Model;
using BPiaoBao.Client.UIExt;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace BPiaoBao.Client.SystemSetting.ViewModel
{
    public class SmsBuyViewModel : ViewModelBase
    {
        public SmsBuyViewModel()
        {
            if (IsInDesignMode)
                return;

            CommunicateManager.Invoke<IBusinessmanService>(p =>
           {
               SmsBuyPrice = p.GetSystemInfo().Item3;
               //BuyCount = 100;
               var tuple = p.GetRecieveAndCreditMoney();
               if (tuple != null)
               {
                   RecieveMoney = tuple.Item1;
                   CreditMoney = tuple.Item2;
                   _balance = tuple.Item1;
               }
               var model= p.GetAllChargeSet();
               SmsChargeModels = model.Select(sms => new SmsChargeModel
               {
                   ID = sms.ID,
                   Code = sms.Code,
                   Count = sms.Count,
                   CreateTime = sms.CreateTime,
                   IsChecked = false,
                   Price = sms.Price,
                   State = sms.State,
                   TotalPrice = sms.TotalPrice
               }).OrderBy(s => s.Count).ToList();

           }, UIManager.ShowErr);


        }

        private List<SmsChargeModel> _smsChargeModelss;

        /// <summary>
        /// 短信费用设置列表
        /// </summary>
        public List<SmsChargeModel> SmsChargeModels
        {
            get { return _smsChargeModelss; }
            set
            {
                if (_smsChargeModelss == value) return;
                RaisePropertyChanging("SmsChargeModels");
                _smsChargeModelss = value;
                RaisePropertyChanged("SmsChargeModels");
            }
        }


        /// <summary>
        /// 短信价格
        /// </summary>
        public decimal SmsBuyPrice
        {
            get;
            private set;
        }
        /// <summary>
        /// 现金账户
        /// </summary>
        public decimal RecieveMoney
        {
            get;
            private set;
        }

        /// <summary>
        /// 信用账户
        /// </summary>
        public decimal CreditMoney
        {
            get;
            private set;
        }
        private decimal _balance;
        /// <summary>
        /// 余额
        /// </summary>
        public decimal Balance
        {
            get { return _balance; }
            set
            {
                if (_balance == value) return;
                _balance = value;
                RaisePropertyChanged("Balance");
            }
        }
        /// <summary>
        /// 购买条数
        /// </summary>
        private int _buyCount;
        public int BuyCount
        {
            get { return _buyCount; }
            set
            {
                if (_buyCount == value || value <= 0) return;
                _buyCount = value;
                RaisePropertyChanged("BuyCount");
                PayAmount = Math.Round((value * SmsBuyPrice), 2);
            }
        }
        /// <summary>
        /// 支付价格
        /// </summary>
        private decimal _payAmount;
        public decimal PayAmount
        {
            get { return _payAmount; }
            private set
            {
                if (_payAmount == value) return;
                _payAmount = value;
                RaisePropertyChanged("PayAmount");
            }
        }
        /// <summary>
        /// 支付方式【0:现金账户，1:信用账户】
        /// </summary>
        private int _payAccountWay;
        public int PayAccountWay
        {
            get { return _payAccountWay; }
            set
            {
                if (_payAccountWay == value) return;
                _payAccountWay = value;
                RaisePropertyChanged("PayAccountWay");
                switch (value)
                {
                    case 0:
                        Balance = RecieveMoney;
                        break;
                    case 1:
                        Balance = CreditMoney;
                        break;
                }
            }
        }

        /// <summary>
        /// 账户支付命令
        /// </summary>
        public ICommand PayAccountCommand
        {
            get
            {
                return new RelayCommand<object>(param =>
                {
                    var sms = SmsChargeModels.FirstOrDefault(m => m.IsChecked);
                    if (sms == null)
                    {
                        UIManager.ShowMessage("请选择购买短信条数");
                        return;
                    }
                    var pb = param as System.Windows.Controls.PasswordBox;
                    if (pb == null) return;
                    var password = pb.Password;
                    if (string.IsNullOrEmpty(password))
                    {
                        UIManager.ShowMessage("请输入支付密码");
                        pb.Focus();
                        return;
                    }

                    CommunicateManager.Invoke<IBusinessmanService>(p =>
                    { 
                        p.BuySmsByAccount(sms.Count, sms.Price, _payAccountWay, pb.Password);
                        var result = MessageBoxExt.Show("提示", "购买成功!", MessageImageType.Info);
                        if (result.HasValue == false || result.Value)
                            Messenger.Default.Send(true, "CloseSMSPay");
                    }, UIManager.ShowErr);
                });
            }
        }
        /// <summary>
        /// 银行卡支付命令
        /// </summary>
        public ICommand PayCommand
        {
            get
            {
                return new RelayCommand<string>(param =>
                {
                    var sms = SmsChargeModels.FirstOrDefault(m => m.IsChecked);
                    if (sms == null)
                    {
                        UIManager.ShowMessage("请选择购买短信条数");
                        return;
                    }
                    if (string.IsNullOrEmpty(param))
                    {
                        UIManager.ShowMessage("请选择银行支付");
                        return;
                    }

                    CommunicateManager.Invoke<IBusinessmanService>(p =>
                    {
                       
                        var resultUrl = p.BuySmsByBank(sms.Count, sms.Price, param);
                        UIManager.OpenDefaultBrower(resultUrl);
                        var result = UIManager.ShowPayWindow();
                        if (result.HasValue && result.Value)
                            Messenger.Default.Send(true, "CloseSMSPay");
                    }, UIManager.ShowErr);
                });
            }
        }
        /// <summary>
        /// 支付平台支付命令
        /// </summary>
        public ICommand PayPlatformCommand
        {
            get
            {
                return new RelayCommand<string>(param =>
                {
                    var sms = SmsChargeModels.FirstOrDefault(m => m.IsChecked);
                    if (sms == null)
                    {
                        UIManager.ShowMessage("请选择购买短信条数");
                        return;
                    }
                    if (string.IsNullOrEmpty(param))
                    {
                        UIManager.ShowMessage("请选择支付平台");
                        return;
                    }
                    CommunicateManager.Invoke<IBusinessmanService>(p =>
                    {
                        
                        var resultUrl = p.BuySmsByPlatform(sms.Count,sms.Price, param);
                        UIManager.OpenDefaultBrower(resultUrl);
                        var result = UIManager.ShowPayWindow();
                        if (result.HasValue && result.Value)
                            Messenger.Default.Send(true, "CloseSMSPay");
                    }, UIManager.ShowErr);
                });
            }
        }

    }
}
