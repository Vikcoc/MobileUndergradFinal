using System.ComponentModel.DataAnnotations;

namespace Communication.AccountDto
{
    public class UserSignUpDto
    {
        [MaxLength(80)]
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [MaxLength(100)]
        [MinLength(8)]
        public string Password { get; set; }
        [MaxLength(20)]
        public string PhoneNumber { get; set; }
    }
}
