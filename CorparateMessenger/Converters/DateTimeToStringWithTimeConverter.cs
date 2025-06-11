using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace CorparateMessenger.Converters
{
    class DateTimeToStringWithTimeConverter : IValueConverter
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
                    return $"Вчера в {timePart}";
                else
                    return $"{localTime:dd.MM.yyyy} в {timePart}";
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
