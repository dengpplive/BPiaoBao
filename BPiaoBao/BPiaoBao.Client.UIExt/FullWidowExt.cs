/***********************************************************************   
* Copyright(c) 信誉金融   
* CLR 版本: 4.0.30319.34014   
* 命名空间: BPiaoBao.Client.UIExt
* 文 件 名：FullWidowExt.cs   
* 创 建 人：duanwei   
* 创建日期：2014/10/27 17:57:47       
* 备注描述：           
************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace BPiaoBao.Client.UIExt
{
    public class FullWidowExt : Window
    {
        public List<Control> CacheControlList = new List<Control>();
        private FullWidowExt fullView = null;
        /// <summary>
        /// 设置新的content时把原来的设置为不是全屏模式
        /// </summary>
        /// <param name="control"></param>
        public void SetContent(Control control)
        {

            this.IsFullScreen = true;
            if (fullView != null)
            {
                fullView.Content = control;
                fullView.CacheControlList.Add(control);
            }
            var vm = control.DataContext as BaseVM;
            if (vm != null)
            {
                vm.FullWidowExt = new FullWidowExt();
                vm.FullWidowExt.IsFullScreen = true;
                vm.FullWidowExt.InitFullWindow(fullView, control);
            } 

        }

        /// <summary>
        /// 设置全屏时的托管窗口
        /// </summary>
        /// <param name="window"></param>
        /// <param name="control"></param>
        public void InitFullWindow(FullWidowExt window, Control control)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                this.fullView = window;
                this.IsFullScreen = true;
                if (fullView != null)
                {
                    fullView.Content = control;
                    fullView.CacheControlList.Add(control);
                }

            }));

        }

        /// <summary>
        /// 设置成正常窗口显示
        /// </summary>
        public void SetNormalWindow()
        {
            Dispatcher.Invoke(new Action(() =>
                {
                    this.fullView = null;
                    this.IsFullScreen = false;
                }));
        }

        public bool IsFullScreen { get; set; }


    }
}
