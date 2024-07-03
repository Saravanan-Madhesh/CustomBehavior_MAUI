using Microsoft.Maui.Controls;
using Microsoft.Maui.Layouts;
using Syncfusion.Maui.Charts;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChartTooltipHandlers
{
    public class SfItemViewCell : ContentView
    {
        public bool IsRequiredLayoutChange { get; set; } = true;

        public double XPosition
        {
            get { return (double)GetValue(XPositionProperty); }

            set { SetValue(XPositionProperty, value); }
        }

        public static readonly BindableProperty XPositionProperty =
            BindableProperty.Create(nameof(XPosition), typeof(double), typeof(SfItemViewCell), double.NaN, BindingMode.Default, null, OnPositionChanged);

        public double YPosition
        {
            get { return (double)GetValue(YPositionProperty); }
            set { SetValue(YPositionProperty, value); }
        }

        public static readonly BindableProperty YPositionProperty =
           BindableProperty.Create(nameof(YPosition), typeof(double), typeof(SfItemViewCell), double.NaN, BindingMode.Default, null, OnPositionChanged);

        private static void OnPositionChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is SfItemViewCell view && view.Parent is Microsoft.Maui.ILayout layout)
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    view.IsRequiredLayoutChange = true;
                    layout.InvalidateMeasure();
                });
            }
        }

        public override SizeRequest Measure(double widthConstraint, double heightConstraint, MeasureFlags flags = MeasureFlags.None)
        {
            return base.Measure(widthConstraint, heightConstraint, flags);
        }

        public DataTemplate ItemTemplate
        {
            get => (DataTemplate)GetValue(ItemTemplateProperty);
            set => SetValue(ItemTemplateProperty, value);
        }

        public static readonly BindableProperty ItemTemplateProperty = BindableProperty.Create(nameof(ItemTemplate), typeof(DataTemplate), typeof(SfItemViewCell), propertyChanged: ItemTemplateChanged);

        public object Item
        {
            get => (object)GetValue(ItemProperty);
            set => SetValue(ItemProperty, value);
        }

        public static readonly BindableProperty ItemProperty = BindableProperty.Create(nameof(Item), typeof(object), typeof(SfItemViewCell), null, propertyChanged: SourceChanged);

        private static void ItemTemplateChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var control = bindable as SfItemViewCell;
            if (control != null && control.ItemTemplate != null)
                control.GenerateItem();
        }

        private static void SourceChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var control = bindable as SfItemViewCell;
            if (control != null && control.ItemTemplate != null)
                control.GenerateItem();
        }

        public bool HideOnNullContent { get; set; } = false;

        protected internal void GenerateItem()
        {
            if (Item == null)
            {
                Content = null;
                return;
            }

            //Create the content
            try
            {
                if (Content != null && ItemTemplate is not DataTemplateSelector)
                {
                    Content.BindingContext = Item;
                }
                else
                {
                    Content = CreateTemplateForItem(Item, ItemTemplate, false);
                }
            }
            catch
            {
                Content = null;
            }
            finally
            {
                if (HideOnNullContent)
                    IsVisible = Content != null;
            }
        }

        public static View? CreateTemplateForItem(object item, DataTemplate itemTemplate, bool createDefaultIfNoTemplate = true)
        {
            //Check to see if we have a template selector or just a template
            var templateToUse = itemTemplate is DataTemplateSelector templateSelector ? templateSelector.SelectTemplate(item, null) : itemTemplate;
            //If we still don't have a template, create a label
            if (templateToUse == null)
                return createDefaultIfNoTemplate ? new Label() { Text = item.ToString() } : null;
            //Create the content
            //If a view wasn't created, we can't use it, exit

            if (!(templateToUse.CreateContent() is View view))
                return new Label() { Text = item.ToString() }; ;
            //Set the binding
            view.BindingContext = item;
            return view;
        }
    }


    internal class TooltipLayout : Layout
    {
        protected override ILayoutManager CreateLayoutManager()
        {
            return new TemplatedViewLayoutManager(this);
        }

        public TooltipLayout()
        {
            this.IsClippedToBounds = true;
        }
    }

    internal class TemplatedViewLayoutManager : LayoutManager
    {
        public TemplatedViewLayoutManager(Microsoft.Maui.ILayout layout) : base(layout)
        {

        }

        public override Size Measure(double widthConstraint, double heightConstraint)
        {
            return new Size(widthConstraint, heightConstraint);
        }

        public override Size ArrangeChildren(Rect bounds)
        {
            var padding = Layout.Padding;

            double top = padding.Top + bounds.Top;
            double left = padding.Left + bounds.Left;
            double availableWidth = bounds.Width - padding.HorizontalThickness;
            double availableHeight = bounds.Height - padding.VerticalThickness;

            for (int n = 0; n < Layout.Count; n++)
            {
                var child = Layout[n];

                if (child.Visibility == Visibility.Collapsed)
                {
                    continue;
                }

                if (child is SfItemViewCell customView)
                {
                    var size = child.DesiredSize;

                    var destination = new Rect(customView.XPosition, customView.YPosition, child.DesiredSize.Width, child.DesiredSize.Height);
                    
                    destination.X += left;
                    destination.Y += top;

                    customView.IsRequiredLayoutChange = false;
                    child.Arrange(destination);
                }
            }

            return new Size(availableWidth, availableHeight);
        }
    }

}
