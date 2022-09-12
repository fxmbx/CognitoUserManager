using CognitoUserManager.Contracts.DTO;
using CognitoUserManager.Contracts.DTO.Change_Passwird;
using CognitoUserManager.Contracts.DTO.Change_Password;
using CognitoUserManager.Contracts.DTO.Forgot_Password;
using CognitoUserManager.Contracts.DTO.Reset_Password;
using CognitoUserManager.Contracts.DTO.Update_Profile;
using CognitoUserManager.Contracts.DTO.User_Profile;
using CognitoUserManager.Contracts.DTO.User_Signin;
using CognitoUserManager.Contracts.DTO.User_Signout;
using CognitoUserManager.Contracts.DTO.User_Signup;

namespace CognitoUserManager.Contracts
{
    public interface IUserRepository
    {
        Task<UserSignUpResponse> CreateUserAsync(UserSignUpModel model);
        Task<UserSignUpResponse> ConfirmUserSignUpAsync(UserConfirmSignUpModel model);

        Task<UserProfileResponse> GetUserAync(string userID);
        Task<ChanagePasswordResponse> TryPasswordChanageAsync(ChangePasswordModel model);

        Task<InitForgotPasswordResponse> TryInitForgotPassword(InitForgotPasswordModel model);
        Task<AuthResponseModel> TryLoginAsync(UserLoginModel model);
        Task<UserSignOutResponse> TryLogOutAsync(UserSignOutModel model);
        Task<ResetPasswordResponse> TryResetPasswordWithConfirmationCodeAsync(ResetPasswordModel model);
        Task<UpdateProfileResponse> UpdateUserAttributesAsync(UpdateProfileModel model);
    }
}