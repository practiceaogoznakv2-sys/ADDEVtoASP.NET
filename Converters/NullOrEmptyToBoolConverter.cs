﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace AccessManager.Converters
{
   public class NullOrEmptyToBoolConverter : IValueConverter
    {
       
        
            // Возвращает true, если строка пустая или null
            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                return string.IsNullOrWhiteSpace(value as string);
            }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                throw new NotImplementedException();
            }
        }

    
}
