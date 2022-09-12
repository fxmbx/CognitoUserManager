using System;
namespace CognitoUserManager.Contracts.DTO.User_Profile
{
    public class UserProfileResponse : BaseResponseModel
    {

        public string Email { get; set; }
        public string GivenName { get; set; }
        public string PhoneNumber { get; set; }
        public string UserID { get; set; }
        public Dictionary<string, string> Address { get; set; } = new Dictionary<string, string>();
        public string Gender { get; set; }
    
    }
}

