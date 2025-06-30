using Com.Microsoft.Multimodal.Clientsdk.Models;
using Microsoft.UI;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;
using System;

namespace SampleApp.Converters
{
    public class RoleToBackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is MessageRole role)
            {
                return role == MessageRole.Agent ? new SolidColorBrush(Colors.LightBlue) : new SolidColorBrush(Colors.LightGray);
            }

            throw new ArgumentException("Invalid value type", nameof(value));
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
            => throw new NotImplementedException();
    }

}
