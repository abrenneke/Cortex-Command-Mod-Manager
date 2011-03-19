using System;
using System.Collections.Generic;

using System.Text;
using System.Windows.Controls;

namespace CortexCommandModManager
{
    class SettingsValidNumeral : ValidationRule
    {
        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            string numberString = value as string;
            if (numberString == null)
            {
                return new ValidationResult(false, null);
            }
            int number;
            try
            {
                number = Int32.Parse(numberString);
            }
            catch (Exception)
            {
                return new ValidationResult(false, null);
            }
            if (number < 1)
            {
                return new ValidationResult(false, null);
            }
            return new ValidationResult(true, null);
        }
    }
}
