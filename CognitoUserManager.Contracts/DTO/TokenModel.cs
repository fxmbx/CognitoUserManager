using System;
namespace CognitoUserManager.Contracts.DTO
{
    public class TokenModel
    {
       public string TokenID { get; set; }
       public string AccessToken { get; set; }
       public int ExpiresIn { get; set; }
       public string RefreshToken { get; set; }

    }
}

