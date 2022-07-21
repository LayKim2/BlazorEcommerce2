using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorEcommerce.Shared
{
    public class UserLogin
    {
        [Required]
        public string Email { get; set; } = String.Empty;
        [Required]
        public string Password { get; set; } = String.Empty;
    }
}
