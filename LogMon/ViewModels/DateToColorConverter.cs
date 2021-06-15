using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Data;
using System.Windows.Media;

namespace LogMon.ViewModels
{
    public class DateToColorConverter : IValueConverter
    {
        public object Convert(object value, 
                              Type targetType,
                              object parameter,
                              CultureInfo culture)
        {
            DateTime date = (DateTime)value;
            bool isWeekEnd = (date.DayOfWeek == DayOfWeek.Saturday) ||
                             (date.DayOfWeek == DayOfWeek.Sunday);

            return isWeekEnd ? Brushes.Red : Brushes.Black;
        }

        public object ConvertBack(object value, 
                                  Type targetType,
                                  object parameter,
                                  CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
