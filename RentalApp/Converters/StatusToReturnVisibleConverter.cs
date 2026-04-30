using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace RentalApp.Converters
{
    public class StatusToReturnVisibleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return false;

            // Normalize status text
            string status = value.ToString().Trim().ToLowerInvariant();

            // Only show the "Return" button when the item is currently out for rent
            return status == "out for rent" || status == "outforrent";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
