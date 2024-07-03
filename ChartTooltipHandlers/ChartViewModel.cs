using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ChartTooltipHandlers;

public class ChartViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    protected void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public ObservableCollection<Model> ListOne { get; } = new ObservableCollection<Model>();

    public ChartViewModel()
    {
        Generate();
    }

    private SolidColorBrush brushs = Brush.Maroon;

    public SolidColorBrush Background { get; set; } = Brush.Maroon;

    private void Generate()
    {
        var random = new Random();
        for (int i = 0; i < 500; i++)
        {
            ListOne.Add(new Model(i + random.Next(0, 10), random.Next(0, 25) + random.NextDouble(), i % 2 == 0 ? SolidColorBrush.LightBlue: SolidColorBrush.WhiteSmoke  ));
        }
    }
}
