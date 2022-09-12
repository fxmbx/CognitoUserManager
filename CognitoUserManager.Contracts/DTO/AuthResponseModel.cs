using System;
namespace CognitoUserManager.Contracts.DTO
{
    public class AuthResponseModel : BaseResponseModel
    {
       public string Email { get; set; }
       public string UserID { get; set; }
       public TokenModel Tokens { get; set; }
    }
}

