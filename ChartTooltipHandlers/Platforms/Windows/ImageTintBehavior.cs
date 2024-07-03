using Microsoft.Maui.Platform;
using Microsoft.UI.Composition;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Hosting;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using WButton = Microsoft.UI.Xaml.Controls.Button;
using WImage = Microsoft.UI.Xaml.Controls.Image;
using WImageSource = Microsoft.UI.Xaml.Media.ImageSource;

namespace ChartTooltipHandlers;

partial class ImageTintBehavior : PlatformBehavior<CustomButton, WButton>
{
    protected override void OnAttachedTo(CustomButton bindable, WButton platformView)
    {
        base.OnAttachedTo(bindable, platformView);
        ApplyTintColor(platformView, bindable);
        bindable.PropertyChanged += OnElementPropertyChanged;
    }

    protected override void OnDetachedFrom(CustomButton bindable, WButton platformView)
    {
        base.OnDetachedFrom(bindable, platformView);
        bindable.PropertyChanged -= OnElementPropertyChanged;
        RemoveTintColor(platformView);
    }

    private void OnElementPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName is not string propertyName ||
            sender is not CustomButton button ||
            button.Handler?.PlatformView is not WButton control)
        {
            return;
        }

        if (propertyName.Equals(CustomButton.ImageSourceProperty.PropertyName, StringComparison.Ordinal))
        {
            ApplyTintColor(control, button, isLoaded: false);
        }
        else if (propertyName.Equals(CustomButton.TintProperty.PropertyName, StringComparison.Ordinal) ||
                 propertyName.Equals(CustomButton.IsEnabledProperty.PropertyName, StringComparison.Ordinal))
        {
            ApplyTintColor(control, button, isLoaded: true);
        }
    }

    private void RemoveTintColor(WButton control)
    {
        if (_currentSpriteVisual is null)
        {
            return;
        }

        if (TryGetButtonImage(control, out var image))
        {
            image.Source = _originalImage;
            ElementCompositionPreview.SetElementChildVisual(image, null);
        }

        _currentSpriteVisual.Brush = null;
        _currentSpriteVisual = null;
        _currentColorBrush = null;
    }

    private void ApplyTintColor(WButton control, CustomButton button, bool isLoaded = false)
    {
        RemoveTintColor(control);

        if (TryGetButtonImage(control, out var image))
        {
            if (isLoaded)
            {
                if (image.ActualSize != Vector2.Zero)
                {
                    ApplyImageTintColor(button, image);
                }
            }
            else
            {
                image.ImageOpened += OnImageOpened;

                void OnImageOpened(object sender, RoutedEventArgs e)
                {
                    image.ImageOpened -= OnImageOpened;

                    if (image.ActualSize != Vector2.Zero)
                    {
                        ApplyImageTintColor(button, image);
                    }
                    else
                    {
                        image.SizeChanged += OnImageSizeChanged;

                        void OnImageSizeChanged(object sender, SizeChangedEventArgs e)
                        {
                            image.SizeChanged -= OnImageSizeChanged;
                            ApplyImageTintColor(button, image);
                        }
                    }
                }
            }
        }
    }

    private void ApplyImageTintColor(CustomButton button, WImage image)
    {
        if (!TryGetSourceImageUri(image, button, out var uri))
        {
            return;
        }

        var width = (float)image.ActualWidth;
        var height = (float)image.ActualHeight;
        var anchorPoint = new Vector2((float)button.AnchorX, (float)button.AnchorY);

        var offset = new Vector3(width * anchorPoint.X, height * anchorPoint.Y, 0f);

        Color? color = button.IsEnabled ? button.Tint ?? Colors.Black : Colors.Gray;
        ApplyTintCompositionEffect(image, color, width, height, offset, anchorPoint, uri);

        if (_blankImage is null ||
           (_blankImage.PixelWidth != (int)width && _blankImage.PixelHeight != (int)height))
        {
            _blankImage = new WriteableBitmap((int)width, (int)height);
        }

        _originalImage = image.Source;
        image.Source = _blankImage;
    }

    private void ApplyTintCompositionEffect(FrameworkElement platformView,
                                            Color color,
                                            float width,
                                            float height,
                                            Vector3 offset,
                                            Vector2 anchorPoint,
                                            Uri surfaceMaskUri)
    {
        var compositor = ElementCompositionPreview.GetElementVisual(platformView).Compositor;

        _currentColorBrush = compositor.CreateColorBrush();
        _currentColorBrush.Color = color.ToWindowsColor();

        var loadedSurfaceMask = LoadedImageSurface.StartLoadFromUri(surfaceMaskUri);

        var maskBrush = compositor.CreateMaskBrush();
        maskBrush.Source = _currentColorBrush;
        maskBrush.Mask = compositor.CreateSurfaceBrush(loadedSurfaceMask);

        _currentSpriteVisual = compositor.CreateSpriteVisual();
        _currentSpriteVisual.Brush = maskBrush;
        _currentSpriteVisual.Size = new Vector2(width, height);
        _currentSpriteVisual.Offset = offset;
        _currentSpriteVisual.AnchorPoint = anchorPoint;

        ElementCompositionPreview.SetElementChildVisual(platformView, _currentSpriteVisual);
    }

    private static bool TryGetButtonImage(WButton button, [NotNullWhen(true)] out WImage? image)
    {
        image = button.GetContent<WImage>();
        return image is not null;
    }

    private static bool TryGetSourceImageUri(WImage? imageControl, IImageElement? imageElement, [NotNullWhen(true)] out Uri? uri)
    {
        if (imageElement?.Source is UriImageSource uriImageSource)
        {
            uri = uriImageSource.Uri;
            return true;
        }

        if (imageControl?.Source is BitmapImage bitmapImage)
        {
            uri = bitmapImage.UriSource;
            return true;
        }

        uri = null;
        return false;
    }

    private SpriteVisual? _currentSpriteVisual;
    private CompositionColorBrush? _currentColorBrush;
    private BitmapSource? _blankImage;
    private WImageSource? _originalImage;
}
