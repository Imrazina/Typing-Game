using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace Game.ChangeColor;
    public class ColorConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string colorName)
            {
                return colorName switch
                {
                    "Black" => new SolidColorBrush(Colors.Black),
                    "Red" => new SolidColorBrush(Colors.Red),
                    "LightGray" => new SolidColorBrush(Colors.LightGray),
                    _ => new SolidColorBrush(Colors.Black) // По умолчанию
                };
            }

            return new SolidColorBrush(Colors.Black);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
