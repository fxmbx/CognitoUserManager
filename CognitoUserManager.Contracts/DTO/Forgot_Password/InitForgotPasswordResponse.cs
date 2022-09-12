using System;
namespace CognitoUserManager.Contracts.DTO.Forgot_Password
{
    public class InitForgotPasswordResponse : BaseResponseModel
    {
        public string UserID { get; set; }
        public string Email { get; set; }
    }
}

