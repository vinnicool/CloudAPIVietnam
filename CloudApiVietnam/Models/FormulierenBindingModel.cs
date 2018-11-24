﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CloudApiVietnam.Models
{
    public class FormulierenBindingModel
    {
     
        [Required(ErrorMessage = "Naam is verplicht")]
        [Display(Name = "Email")]
        public string Name { get; set; }

        public string Region { get; set; }
        public string FormTemplate { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
        public List<FormContent> FormContent { get; set; }
    }
}