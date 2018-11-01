using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CloudApiVietnam.Models
{
    public class ImageBindingModel
    {
        public IFormFile Image { get; set; }
    }
}