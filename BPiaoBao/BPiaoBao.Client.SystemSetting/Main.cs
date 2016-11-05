using BPiaoBao.Client.Module;
using System.Collections.Generic;

namespace BPiaoBao.Client.SystemSetting
{
    [Plugin(ProjectCode, "系统设置", "pack://application:,,,/BPiaoBao.Client.SystemSetting;component/Image/systemsetting.png", 4, UserSettingCode)]
    public class Main : IPlugin
    {
        public IEnumerable<MainMenuItem> Parts
        {
            get
            {
                return new List<MainMenuItem>
                { 
                    new MainMenuItem{ Name="个人中心",MenuCode=UserSettingCode ,Icon="pack://application:,,,/BPiaoBao.Client.SystemSetting;component/Image/shezhi_02.png"},
                    new MainMenuItem{ Name="员工管理",MenuCode=EmployeeCode,Icon="pack://application:,,,/BPiaoBao.Client.SystemSetting;component/Image/shezhi_01.png"},
                    new MainMenuItem{ Name="短信管理",MenuCode=SmsCode,Icon="pack://application:,,,/BPiaoBao.Client.SystemSetting;component/Image/shezhi_03.png"},
                    new MainMenuItem{ Name="公告列表",MenuCode=BulletinCode,Icon="pack://application:,,,/BPiaoBao.Client.SystemSetting;component/Image/notice.png"},
                    //new MainMenuItem{ Name="POS机绑定",MenuCode="6004",Icon="pack://application:,,,/BPiaoBao.Client.SystemSetting;component/Image/shezhi_04.png"},
                    //new MainMenuItem{ Name="系统设置",MenuCode="6005",Icon="pack://application:,,,/BPiaoBao.Client.SystemSetting;component/Image/shezhi_05.png"}
                    new MainMenuItem{ Name="支付设置",MenuCode="6005",Icon="pack://application:,,,/BPiaoBao.Client.SystemSetting;component/Image/shezhi_05.png"}                    
                };
            }
        }

        public const string ProjectCode = "6000";
        public const string EmployeeCode = "6001";
        public const string UserSettingCode = "6002";
        public const string SmsCode = "6003";
        public const string BulletinCode = "6004";
        public const string PaySettingCode = "6005";


    }
}
