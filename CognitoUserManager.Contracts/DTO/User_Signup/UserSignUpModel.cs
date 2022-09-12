using System;
namespace CognitoUserManager.Contracts.DTO.User_Signup
{
    public class UserSignUpModel
    {
        public string Email { get; set; }
        public string GivenName { get; set; }
        public string Password { get; set; }
        public string PhoneNumber { get; set; }
    }
}

