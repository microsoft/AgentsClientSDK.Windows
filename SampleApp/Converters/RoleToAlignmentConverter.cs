using Com.Microsoft.AgentsClientSDK.Models;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using System;

namespace SampleApp.Converters
{
    public class RoleToAlignmentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is MessageRole role)
            {
                return role == MessageRole.Agent ? HorizontalAlignment.Left : HorizontalAlignment.Right;
            }
            throw new ArgumentException("Invalid value type", nameof(value));
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
            => throw new NotImplementedException();
    }
}
