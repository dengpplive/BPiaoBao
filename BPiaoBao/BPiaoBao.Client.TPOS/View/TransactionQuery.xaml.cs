using BPiaoBao.Client.Module;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BPiaoBao.Client.TPOS.View
{
    /// <summary>
    /// TransactionQuery.xaml 的交互逻辑
    /// </summary>
    [Part(Main.TransactionQueryCode)]
    public partial class TransactionQuery : UserControl, IPart
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TransactionQuery"/> class.
        /// </summary>
        public TransactionQuery()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 获取内容
        /// </summary>
        /// <returns></returns>
        public object GetContent()
        {
            return this;
        }

        /// <summary>
        /// 获取标题
        /// </summary>
        public string Title
        {
            get { return Main.TransactionQueryName; }
        }
    }
}
