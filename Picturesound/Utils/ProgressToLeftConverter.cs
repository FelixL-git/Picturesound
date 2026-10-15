using System;
using System.Globalization;
using System.Windows.Data;

namespace Picturesound.Utils
{
    // MultiValueConverter: values[0]=progress (0..1), values[1]=width (double)
    // parameter: halfWidth (double) to center the playhead
    public class ProgressToLeftConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values.Length < 2) return 0.0;
            if (!(values[0] is double progress)) return 0.0;
            if (!(values[1] is double width)) return 0.0;

            double half = 0.0;
            if (parameter != null && double.TryParse(parameter.ToString(), out var p)) half = p;

            var x = progress * width - half;
            if (x < 0) x = 0;
            if (x > width) x = width;
            return x;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
