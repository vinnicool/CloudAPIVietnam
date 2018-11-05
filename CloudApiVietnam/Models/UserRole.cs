using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Linq;
using System.Web;

namespace CloudApiVietnam.Models
{
    public class UserRole
    {
        [Key]
        public int UserId { get; set; }
        public int RoleId { get; set; }
    }
}