using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace BPiaoBao.Client.Account
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
                    //System.Uri resourceLocater =
                    //    new System.Uri("/BPiaobao.Client.Account;component/ProjectDataSources.xaml",
                    //                    System.UriKind.Relative);

                    //_sharedDictionary =
                    //    (ResourceDictionary)Application.LoadComponent(resourceLocater);

                    _sharedDictionary = new ResourceDictionary();
                    _sharedDictionary.Source = new System.Uri("/BPiaobao.Client.Account;component/ProjectDataSources.xaml", UriKind.Relative);
                }

                return _sharedDictionary;
            }
        }

        private static ResourceDictionary _sharedDictionary;
    }
}
