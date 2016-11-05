using BPiaoBao.Client.UIExt.Helper;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace BPiaoBao.Client.DomesticTicket.View
{
    /// <summary>
    /// 协调信息
    /// </summary>
    public partial class CoordinationWindow : Window
    {
        public CoordinationWindow()
        {
            InitializeComponent();

            CollectionView collectionView = (CollectionView)CollectionViewSource.GetDefaultView(dataGrid.Items);
            ((INotifyCollectionChanged)collectionView).CollectionChanged += CoordinationWindow_CollectionChanged;

            KeyDown += CoordinationWindow_KeyDown;
        }

        void CoordinationWindow_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Enter:
                    UIHelper.PerformClick(addBtn);
                    break;
            }
        }

        void CoordinationWindow_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (dataGrid.Items == null || dataGrid.Items.Count == 0)
                return;

            var lastObj = dataGrid.Items[dataGrid.Items.Count - 1];
            dataGrid.ScrollIntoView(lastObj);
        }

    }
}
