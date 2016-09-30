using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Vulcan.Core.DataAccess.Exceptions
{
    public class ValidationErrorException : Exception
    {
        public List<ValidationResult> ValidationResults { get; set; }
        public ValidationErrorException(List<ValidationResult> errors) : base("Data validation error occurred")
        {
            ValidationResults = errors;
        }
    }
}
