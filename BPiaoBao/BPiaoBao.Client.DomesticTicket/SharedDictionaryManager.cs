using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace BPiaoBao.Client.DomesticTicket
{
    /// <summary>
    /// 共享的资源，如果在每个地方引用xaml会重复初始化
    /// </summary>
    internal static class SharedDictionaryManager
    {
        internal static ResourceDictionary SharedDictionary
        {
            get
            {
                if (_sharedDictionary == null)
                {
            
                    _sharedDictionary = new ResourceDictionary();
                    _sharedDictionary.Source = new System.Uri("/BPiaoBao.Client.DomesticTicket;component/ProjectDataSources.xaml", UriKind.Relative);
                }

                return _sharedDictionary;
            }
        }

        private static ResourceDictionary _sharedDictionary;
    }
}
