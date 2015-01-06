using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace ScheduleBSUIR.Converters
{
    public class LessonTypeToColorConverter: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string result;
            switch ((string)value)
            {
                case "лр":
                    result = "Crimson";
                    break;
                case "пз":
                    result = "LimeGreen";
                    break;
                case "лк":
                    result = "DeepSkyBlue";
                    break;
                default:
                    {
                        result = (Visibility)Application.Current.Resources["PhoneDarkThemeVisibility"] ==
                                 Visibility.Visible
                            ? "White"
                            : "Black";
                    }
                    break;
            }

            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
