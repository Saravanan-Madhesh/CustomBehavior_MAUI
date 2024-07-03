
using System.Collections;

namespace ChartTooltipHandlers;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
    }

    private void Button_Clicked(object sender, EventArgs e)
    {
        var view = new SfItemViewCell();
        view.Item = this.BindingContext;
        view.ItemTemplate = (DataTemplate)this.Resources["Itemtemplate"];
        AbsoluteLayout.SetLayoutBounds(view, new Rect(0.5, 0.5, -1, -1));
        AbsoluteLayout.SetLayoutFlags(view, Microsoft.Maui.Layouts.AbsoluteLayoutFlags.PositionProportional);
        customControl.BehaviorLayout.Add(view);
    }
}