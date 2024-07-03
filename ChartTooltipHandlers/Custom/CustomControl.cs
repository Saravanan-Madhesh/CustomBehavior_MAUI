using Microsoft.Maui.Layouts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChartTooltipHandlers.Custom
{
    public class CustomControl : View, IContentView
    {
        public object Title
        {
            get { return (object)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public static readonly BindableProperty TitleProperty =
           BindableProperty.Create(nameof(Title), typeof(object), typeof(CustomControl), null,
                                   propertyChanged: OnTitlePropertyChanged);

        internal View? Content { get; set; }

        object? IContentView.Content => Content;

        /// <summary>
        /// Gets the presented content value.
        /// </summary>
        IView? IContentView.PresentedContent => Content;
        Thickness IPadding.Padding => Thickness.Zero;

        Size IContentView.CrossPlatformMeasure(double widthConstraint, double heightConstraint)
        {
            return this.MeasureContent(widthConstraint, heightConstraint);
        }

        Size IContentView.CrossPlatformArrange(Rect bounds)
        {
            this.ArrangeContent(bounds);
            return bounds.Size;
        }
        
        internal AbsoluteLayout BehaviorLayout { get; set; }

        internal CustomTitleView TitleView { get; set; }

        public CustomControl()
        {
            TitleView = new CustomTitleView();
            BehaviorLayout = new AbsoluteLayout();
            Content = CreateTemplate(BehaviorLayout);
        }

        private View CreateTemplate(AbsoluteLayout layout)
        {
            Grid grid = new Grid();
            grid.AddRowDefinition(new RowDefinition() { Height = GridLength.Auto });
            grid.AddRowDefinition(new RowDefinition() { Height = GridLength.Star });
            Grid.SetRow(TitleView, 0);
            grid.Add(TitleView);
            Grid.SetRow(layout, 1);
            grid.Add(layout);

            return grid;
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            if(TitleView != null)
            {
                SetInheritedBindingContext(TitleView, this.BindingContext);
            }
        }

        private static void OnTitlePropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var control = bindable as CustomControl;
            if (control != null && control.TitleView != null)
            {
                if (newValue != null)
                {
                    if (newValue is View view)
                    {
                        control.TitleView.Content = view;
                        control.TitleView.Parent.Parent = control;
                        SetInheritedBindingContext(control.TitleView, control.BindingContext);
                    }
                    else
                    {
                        control.TitleView.InitTitle(newValue.ToString());
                    }
                }
            }
        }
    }


    internal class CustomTitleView : Frame
    {
        internal CustomTitleView()
        {
            HasShadow = false;
            Padding = 0; //For default size to be empty
            BackgroundColor = Colors.Transparent;
            HorizontalOptions = LayoutOptions.Fill;
            VerticalOptions = LayoutOptions.Center;
        }

        internal void InitTitle(string? content)
        {
            var label = new Label()
            {
                Text = content,
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Fill,
                TextColor = Color.FromArgb("#49454F"),
                FontSize = 16,
                HorizontalTextAlignment = TextAlignment.Center,
            };

            this.Content = label;
        }
    }
}
