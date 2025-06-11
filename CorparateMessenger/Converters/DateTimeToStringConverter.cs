using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.Windows.Data;

namespace CorparateMessenger.Converters
{
    public class DateTimeToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DateTime dateTime)
            {
                DateTime localTime = dateTime;
                DateTime now = DateTime.Now;

                int daysDiff = (now.Date - localTime.Date).Days;

                string timePart = localTime.ToString("HH:mm");

                if (daysDiff == 0)
                    return $"{timePart}";
                else if (daysDiff == 1)
                    return $"Вчера";
                else
                    return $"{localTime:dd.MM.yyyy}";
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
