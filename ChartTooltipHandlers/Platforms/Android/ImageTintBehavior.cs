using Android.Content.Res;
using Microsoft.Maui.Controls.Compatibility.Platform.Android;
using System.ComponentModel;
using AButton = Google.Android.Material.Button.MaterialButton;
using AColor = Android.Graphics.Color;
using AndroidResources = Android.Resource;
namespace ChartTooltipHandlers;

partial class ImageTintBehavior : PlatformBehavior<CustomButton, AButton>
{
    protected override void OnAttachedTo(CustomButton bindable, AButton platformView)
    {
        base.OnAttachedTo(bindable, platformView);
        ApplyTintColor(bindable, platformView);
        bindable.PropertyChanged += OnElementPropertyChanged;
    }

    protected override void OnDetachedFrom(CustomButton bindable, AButton platformView)
    {
        base.OnDetachedFrom(bindable, platformView);
        bindable.PropertyChanged -= OnElementPropertyChanged;
    }

    private void OnElementPropertyChanged(object? sender, PropertyChangedEventArgs args)
    {
        if (args.PropertyName is not string propertyName ||
            sender is not CustomButton bindable ||
            bindable.Handler?.PlatformView is not AButton platformView)
        {
            return;
        }

        if (propertyName.Equals(CustomButton.ImageSourceProperty.PropertyName, StringComparison.Ordinal) ||
            propertyName.Equals(CustomButton.TintProperty.PropertyName, StringComparison.Ordinal) ||
            propertyName.Equals(CustomButton.IsEnabledProperty.PropertyName, StringComparison.Ordinal))
        {
            ApplyTintColor(bindable, platformView);
        }
    }

    private static void ApplyTintColor(CustomButton button, AButton control)
    {
        AColor imageColor = button.Tint.ToAndroid(Colors.Black);
        AColor pressedImageColor = AdjustBrightness(imageColor, 0.7f);

        ColorStateList? csl;
        if (button.Tint == default && button.IsEnabled)
        {
            csl = null;
        }
        else
        {
            csl = new ColorStateList(
            [
                [AndroidResources.Attribute.StatePressed],
                [AndroidResources.Attribute.StateEnabled],
                [-AndroidResources.Attribute.StateEnabled],
            ],
            [
                pressedImageColor,
                imageColor,
                Colors.LightGray.ToAndroid(),
            ]);
        }

        control.SupportCompoundDrawablesTintList = csl;
    }

    private static AColor AdjustBrightness(AColor color, float brightness)
    {
        float[] hsv = new float[3];
        AColor.ColorToHSV(color, hsv);
        hsv[2] = brightness;
        return AColor.HSVToColor(hsv);
    }
}
