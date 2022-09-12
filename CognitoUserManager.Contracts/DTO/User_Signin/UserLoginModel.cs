using System.ComponentModel.DataAnnotations;

namespace CognitoUserManager.Contracts.DTO.User_Signin
{
    public class UserLoginModel
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
}