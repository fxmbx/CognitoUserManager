using System;
namespace CognitoUserManager.Contracts.DTO.User_Signup
{
    public class UserConfirmSignUpModel
    {
       public string ConfirmationCode { get; set; }
       public string Email { get; set; }
       public string UserID { get; set; }
    }
}

