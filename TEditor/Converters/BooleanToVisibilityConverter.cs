using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows;

namespace TEditor.Converters
{
    // <mvvm:BooleanToVisibilityConverter x:Key="TrueToVisibleConverter" />
    // <mvvm:BooleanToVisibilityConverter x:Key="FalseToVisibleConverter" IsReversed="True" />

    /// <summary>
    /// Converts a Boolean into a Visibility.
    /// </summary>
    public class BooleanToVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// If set to True, conversion is reversed: True will become Collapsed.
        /// </summary>
        public bool IsReversed { get; set; }

        public object Convert(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            var val = System.Convert.ToBoolean(value);
            if (this.IsReversed)
            {
                val = !val;
            }

            if (val)
            {
                return Visibility.Visible;
            }

            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
