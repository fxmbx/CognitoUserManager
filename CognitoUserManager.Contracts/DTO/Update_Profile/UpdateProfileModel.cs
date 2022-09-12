using System;
using Microsoft.AspNetCore.Http;
namespace CognitoUserManager.Contracts.DTO.Update_Profile
{
    public class UpdateProfileModel
    {
     public string UserID { get; set; }
       public string GivenName { get; set; }
       public string PhoneNumber { get; set; }
       public string ProfilePicture { get; set; }
       public string Gender { get; set; }
       public string Address { get; set; }
       public string State { get; set; }
       public string Country  { get; set; }
       public string Pincode { get; set; }
       public string AccessToken { get; set; }

    }
}

