using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Communication.AccountDto
{
    public class UserSignInDto
    {
        [MaxLength(80)]
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [MaxLength(100)]
        [MinLength(8)]
        public string Password { get; set; }
    }
}
