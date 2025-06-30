using Com.Microsoft.Multimodal.Clientsdk.Models;
using Microsoft.UI;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleApp.Converters
{
    public class BubbleStyleByRole: IValueConverter
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
