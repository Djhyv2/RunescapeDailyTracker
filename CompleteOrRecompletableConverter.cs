using System;
using System.Globalization;
using System.Windows.Data;

namespace RunescapeDailyTracker
{
    class CompletedOrRecompletableConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Task task = (Task)value;
            return task.NotCompleted || task.Recompletable;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return true;
        }
    }
}
