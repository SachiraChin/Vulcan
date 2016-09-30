using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace TestWebAPI.ViewModels
{
    public class LeadViewModel
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Type { get; set; }
        [Required]
        public string IsHidden { get; set; }
        [Required]
        public string IsAutoGenarated { get; set; }


    }
}