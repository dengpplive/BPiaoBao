using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace BPiaoBao.Client.UIExt.CommonControl
{
    public class PromptAdorner : Adorner
    {

        public static readonly DependencyProperty PromptCountProperty =
            DependencyProperty.RegisterAttached("PromptCount", typeof(int), typeof(PromptAdorner),
            new FrameworkPropertyMetadata(0, new PropertyChangedCallback(PromptCountChangedCallBack), new CoerceValueCallback(CoercePromptCountCallback)));

        public static int GetPromptCount(DependencyObject obj)
        {
            return (int)obj.GetValue(PromptCountProperty);
        }

        public static void SetPromptCount(DependencyObject obj, int value)
        {
            obj.SetValue(PromptCountProperty, value);
        }


        public static readonly DependencyProperty IsPromptEnabledProperty =
            DependencyProperty.RegisterAttached("IsPromptEnabled", typeof(bool), typeof(PromptAdorner),
            new FrameworkPropertyMetadata(false, new PropertyChangedCallback(IsPromptEnabledChangedCallBack), null));

        public static bool GetIsPromptEnabled(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsPromptEnabledProperty);
        }

        public static void SetIsPromptEnabled(DependencyObject obj, bool value)
        {
            obj.SetValue(IsPromptEnabledProperty, value);
        }


        private static object CoercePromptCountCallback(DependencyObject d, object value)
        {
            int promptCount = (int)value;
            promptCount = Math.Max(0, promptCount);

            return promptCount;
        }

        public static void PromptCountChangedCallBack(DependencyObject d, DependencyPropertyChangedEventArgs e) { }

        public static void IsPromptEnabledChangedCallBack(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var source = d as FrameworkElement;

            bool isEnabled = (bool)e.NewValue;
            if (isEnabled)
            {
                //装饰件可用，添加装饰件

                AdornerLayer layer = AdornerLayer.GetAdornerLayer(source);
                if (layer != null)
                {
                    //能够获取装饰层，说明已经load过了，直接生成装饰件
                    var adorner = new PromptAdorner(source);
                    layer.Add(adorner);
                }
                else
                {
                    //layer为null，说明还未load过（整个可视化树中没有装饰层的情况不考虑）
                    //在控件的loaded事件内生成装饰件
                    source.Loaded += (s1, e1) =>
                    {
                        var adorner = new PromptAdorner(source);
                        AdornerLayer.GetAdornerLayer(source).Add(adorner);
                    };
                }
            }
            else
            {
                //装饰件不可用，移除装饰件
                AdornerLayer layer = AdornerLayer.GetAdornerLayer(source);
                if (layer != null)
                {
                    Adorner[] AllAdorners = layer.GetAdorners(source);
                    if (AllAdorners != null)
                    {
                        IEnumerable<Adorner> desAdorners = AllAdorners.Where(p => p is PromptAdorner);
                        if (desAdorners.Any())
                        {
                            desAdorners.ToList().ForEach(layer.Remove);
                        }
                    }
                }
            }

        }


        protected override int VisualChildrenCount
        {
            get { return 1; }
        }

        public PromptAdorner(UIElement adornedElement)
            : base(adornedElement)
        {

            _chrome = new PromptChrome {DataContext = adornedElement};
            this.AddVisualChild(_chrome);
        }

        protected override Visual GetVisualChild(int index)
        {
            return _chrome;
        }

        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            _chrome.Arrange(new Rect(arrangeBounds));
            return arrangeBounds;
        }


        PromptChrome _chrome;
    }
}
