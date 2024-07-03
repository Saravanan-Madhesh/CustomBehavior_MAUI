using Google.Android.Material.Button;

namespace ChartTooltipHandlers;

partial class CustomButtonHandler
{
    protected override void ConnectHandler(MaterialButton platformView)
    {
        if (VirtualView is CustomButton button)
        {
            if (button.Behaviors.Any(x => x is ImageTintBehavior) == false)
            {
                button.Behaviors.Add(new ImageTintBehavior());
            }
        }
        base.ConnectHandler(platformView);
    }
}
