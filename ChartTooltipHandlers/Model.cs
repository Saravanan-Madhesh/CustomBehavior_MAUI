using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ChartTooltipHandlers;

public class Model : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    protected void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public Model() { }

    public Model(double x, double y, SolidColorBrush color)
    {
        X = x;
        Y = y;
        brush = color;
    }

    private SolidColorBrush brush;
    public SolidColorBrush Background {
        get { return brush; } set {  brush = value; NotifyPropertyChanged(); }
    }


    private double _x;
    public double X
    {
        get { return _x; }
        set
        {
            if (Math.Abs(_x - value) > 0.001)
            {
                _x = value;
                NotifyPropertyChanged();
            }
        }
    }

    private double _y;
    public double Y
    {
        get { return _y; }
        set
        {
            if (Math.Abs(_y - value) > 0.001)
            {
                _y = value;
                NotifyPropertyChanged();
            }
        }
    }
}
