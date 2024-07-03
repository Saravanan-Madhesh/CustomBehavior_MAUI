using Microsoft.Maui.Platform;
using UIKit;

namespace ChartTooltipHandlers;

partial class ImageTintBehavior : PlatformBehavior<CustomButton, UIButton>
{
    protected override void OnAttachedTo(CustomButton bindable, UIButton platformView)
    {
        base.OnAttachedTo(bindable, platformView);

        var image = platformView.ImageView.Image;
        if (image != null)
        {
            if (bindable.Tint == default)
            {
                platformView.SetImage(image.ImageWithRenderingMode(UIImageRenderingMode.AlwaysOriginal), UIControlState.Normal);
            }
            else
            {
                platformView.TintColor = bindable.Tint.ToPlatform();
                platformView.SetImage(image.ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate), UIControlState.Normal);
            }
        }
    }
}
