using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace alharamApp.Models
{
    public class login
    {
        [Required(ErrorMessage = "*Email field required")]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email")]
        public string email { get; set; }


        [Required(ErrorMessage = "*Password field required")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string password { get; set; }
    }
}