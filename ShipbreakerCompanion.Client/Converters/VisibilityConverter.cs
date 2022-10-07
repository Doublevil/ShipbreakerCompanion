using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace ShipbreakerCompanion.Client.Converters
{
    public class VisibilityConverter : IValueConverter
    {
        public enum ConvertMode
        {
            FalseToCollapsed,
            TrueToCollapsed,
            ListEmptyToCollapsed,
            ListNotEmptyToCollapsed,
            StringEqualToCollapsed,
            StringNotEqualToCollapsed
        }

        /// <summary>Converts a value.</summary>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>A converted value. If the method returns <see langword="null" />, the valid null value is used.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!Enum.TryParse(parameter?.ToString(), out ConvertMode mode))
                mode = ConvertMode.FalseToCollapsed;

            bool boolValue = (value as bool?) == true;
            var collectionValue = value as IList;
            
            switch (mode)
            {
                case ConvertMode.FalseToCollapsed:
                default:
                    return boolValue ? Visibility.Visible : Visibility.Collapsed;
                case ConvertMode.TrueToCollapsed:
                    return boolValue ? Visibility.Collapsed : Visibility.Visible;
                case ConvertMode.ListEmptyToCollapsed:
                    return collectionValue?.Count > 0 ? Visibility.Visible : Visibility.Collapsed;
                case ConvertMode.ListNotEmptyToCollapsed:
                    return collectionValue?.Count > 0 ? Visibility.Collapsed : Visibility.Visible;
                case ConvertMode.StringEqualToCollapsed:
                    return value?.ToString() == parameter?.ToString() ? Visibility.Collapsed : Visibility.Visible;
                case ConvertMode.StringNotEqualToCollapsed:
                    return value?.ToString() == parameter?.ToString() ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        /// <summary>Converts a value.</summary>
        /// <param name="value">The value that is produced by the binding target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>A converted value. If the method returns <see langword="null" />, the valid null value is used.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
