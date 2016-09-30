using System;
using System.ComponentModel.DataAnnotations;
using Vulcan.Core.DataAccess.Validations;

namespace Vulcan.Core.DataAccess.Models
{
    public class FieldValidation
    {
        private IValidator _validator;
        public int Id { get; set; }
        [Required]
        public Guid ValidatorId { get; set; }
        [Required]
        public string Message { get; set; }
        public string Data { get; set; }
        [Required]
        public int FieldId { get; set; }
        [MaxLength(64)]
        [Required]
        public string FieldName { get; set; }
        public IValidator Validator
        {
            get
            {
                if (_validator == null)
                {
                    _validator = ValidatorFactory.Get(this.ValidatorId.ToString());
                    _validator.Message = Message;
                    _validator.FieldName = FieldName;
                    _validator.DecodeData(this.Data);
                    _validator.ValidationId = this.Id;
                }
                return _validator;
            }
        }
        public FieldValidation()
        {
        }
        public FieldValidation(IValidator validator)
        {
            ValidatorId = validator.Id;
            Data = validator.EncodeData();
            Message = validator.Message;
            Id = validator.ValidationId;
            _validator = validator;
        }

    }
}
