using System;
namespace CognitoUserManager.Contracts.DTO
{
    public class BaseResponseModel
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public CognitoStatusCode Status { get; set; }
    }
}

