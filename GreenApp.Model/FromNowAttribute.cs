using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace GreenApp.Model
{
    public class FromNowAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (Convert.ToDateTime(value) >= DateTime.Today)
            {
                return ValidationResult.Success;
            }
            else
            {
                return new ValidationResult("A kezdődátum csak a mai naptól lehetséges.");
            }
        }
    }
}
