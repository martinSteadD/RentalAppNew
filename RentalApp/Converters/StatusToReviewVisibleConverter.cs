using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace RentalApp.Converters
{
    public class StatusToReviewVisibleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string status)
                return status == "Completed";

            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}