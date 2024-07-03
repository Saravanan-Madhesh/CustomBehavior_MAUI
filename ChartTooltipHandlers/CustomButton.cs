namespace ChartTooltipHandlers;

public class CustomButton : Button
{
    public static readonly BindableProperty TintProperty =
        BindableProperty.Create(nameof(Tint), typeof(Color), typeof(CustomButton), Colors.Black, BindingMode.OneWay);

    public Color Tint
    {
        get { return (Color)GetValue(TintProperty); }
        set { SetValue(TintProperty, value); }
    }
}
