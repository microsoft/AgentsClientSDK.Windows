﻿using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using System;

namespace SampleApp.Converters
{
    public class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language) =>
            
            value is bool b && b ? Visibility.Visible : Visibility.Collapsed;


        public object ConvertBack(object value, Type targetType, object parameter, string language) =>
            throw new NotImplementedException();

    }

}
