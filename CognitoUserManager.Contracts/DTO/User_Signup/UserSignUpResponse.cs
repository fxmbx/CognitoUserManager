namespace CognitoUserManager.Contracts.DTO.User_Signup
{
    public class UserSignUpResponse : BaseResponseModel
    {
        public string  UserID { get; set; }
        public string Email { get; set; }
    }
}