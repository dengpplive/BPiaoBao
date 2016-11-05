using System.Windows;
using System.Windows.Input;
using BPiaoBao.Client.DomesticTicket.ViewModel;

namespace BPiaoBao.Client.DomesticTicket.View
{
    /// <summary>
    /// RelationOrderNoWindow.xaml 的交互逻辑
    /// </summary>
    public partial class RelationOrderNoWindow : Window
    {
        public RelationOrderNoWindow()
        {
            InitializeComponent();
        }

        private void TxtBox_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter)  return;
            this.txtBox.Text = LocalUIManager.ShowAduldtsOrderIdList(null);
        }
    }
}
