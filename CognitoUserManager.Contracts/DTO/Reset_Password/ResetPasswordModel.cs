using System;
namespace CognitoUserManager.Contracts.DTO.Reset_Password
{
    public class ResetPasswordModel
    {
        public string UserID { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmationCode { get; set; }
        public string EmailAddess { get; set; }
    }
}

