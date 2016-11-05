using System.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GalaSoft.MvvmLight;

namespace BPiaoBao.Client.DomesticTicket.ViewModel
{
    public class KeyValueItemViewModel<TKey, TValue> : ViewModelBase
    {
        #region Key
        protected const string c_KeyName = "Key";

        protected TKey _Key;

        /// <summary>
        /// Key
        /// </summary>
        public TKey Key
        {
            get { return _Key; }

            set
            {
                RaisePropertyChanging(c_KeyName);
                _Key = value;
                RaisePropertyChanged(c_KeyName);
            }
        }
        #endregion

        #region Value
        protected const string c_ValueName = "Value";

        protected TValue _Value;

        /// <summary>
        /// Value
        /// </summary>
        public TValue Value
        {
            get { return _Value; }

            set
            {
                RaisePropertyChanging(c_ValueName);
                _Value = value;
                RaisePropertyChanged(c_ValueName);
            }
        }
        #endregion

        public KeyValueItemViewModel(TKey key, TValue value)
        {
            this.Key = key;
            this.Value = value;
        }
    }

    public class KeyValueViewModel<TObj, TKey, TValue> : ViewModelBase
    {
        #region Items
        protected const string c_ItemsName = "Items";

        protected List<KeyValueItemViewModel<TKey, TValue>> _Items;

        /// <summary>
        /// Items
        /// </summary>
        public List<KeyValueItemViewModel<TKey, TValue>> Items
        {
            get { return _Items; }

            set
            {
                if (_Items != value)
                {
                    RaisePropertyChanging(c_ItemsName);
                    _Items = value;
                    RaisePropertyChanged(c_ItemsName);
                }
            }
        }
        #endregion
        public KeyValueViewModel(IEnumerable<TObj> items, Func<TObj, TKey> funcKey, Func<TObj, TValue> funcValue)
        {
            this.Items = items.Select(p => new KeyValueItemViewModel<TKey, TValue>(funcKey(p), funcValue(p))).ToList();
        }

        public KeyValueViewModel()
        {
            this.Items = new List<KeyValueItemViewModel<TKey, TValue>>();
        }
    }
}
