using System.ComponentModel.DataAnnotations;

namespace Communication.AccountDto
{
    public class UserSignUpDto
    {
        [MaxLength(80)]
        [Required]
        [RegularExpression("^[\\w-\\.]+@([\\w-]+\\.)+[\\w-]{2,4}$")]
        public string Email { get; set; }
        [MaxLength(100)]
        [MinLength(8)]
        [Required]
        public string Password { get; set; }
        [MaxLength(20)]
        [Required]
        public string UserName { get; set; }
    }
}
