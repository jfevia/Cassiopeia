using System;
using System.Collections;
using System.Globalization;
using System.Windows.Data;

namespace Cassiopeia.Converters
{
    internal class ItemToRowNumberConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values.Length != 2)
                return null;

            if (values[0] == null || values[1] == null)
                throw new ArgumentNullException(nameof(values));

            if (!(values[1] is IList))
                throw new ArgumentException(nameof(values));

            return (((IList) values[1]).IndexOf(values[0]) + 1).ToString();
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}