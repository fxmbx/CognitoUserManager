using System;
namespace CognitoUserManager.Contracts.DTO.Change_Passwird
{
    public class ChangePasswordModel
    {
        public string CurrentPassword { get; set; }
        public string Email { get; set; }
        public string NewPassword { get; set; }
    }
}

