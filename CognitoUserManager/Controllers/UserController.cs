using CognitoUserManager.Contracts;
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
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace CognitoUserManager.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
         private IUserRepository _userRepo;
        public UserController(IUserRepository userrepo)
        {
            _userRepo = userrepo;
        }
        
        [HttpPost("signup")]
        public async Task<IActionResult> SignUp(UserSignUpModel model) 
        {
            var response = new ServiceResponse<UserSignUpResponse>(){
                Data = await _userRepo.CreateUserAsync(model)
            };
            if (response.Data.IsSuccess){
                return Ok(response);
            }
            return BadRequest(response);
        }
        [HttpPost("confirm_signup")]
        public async Task<IActionResult> ConfirmSignUp(UserConfirmSignUpModel model) 
        {
            var response = new ServiceResponse<UserSignUpResponse>(){
                Data = await _userRepo.ConfirmUserSignUpAsync(model)
            };
            if (response.Data.IsSuccess){
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserLoginModel model) 
        {
            var response = new ServiceResponse<AuthResponseModel>(){
                Data = await _userRepo.TryLoginAsync(model)
            };
             if (response.Data.IsSuccess){
                return Ok(response);
            }
            return BadRequest(response);
        }
        [HttpPost("logout")]
        public async Task<IActionResult> LogOut(UserSignOutModel model) 
        {
            var response = new ServiceResponse<UserSignOutResponse>(){
                Data = await _userRepo.TryLogOutAsync(model)
            };
             if (response.Data.IsSuccess){
                return Ok(response);
            }
            return BadRequest(response);
        }
        [HttpPost("get_user")]
        public async Task<IActionResult> GetUser(string userID) 
        {
            var response = new ServiceResponse<UserProfileResponse>(){
                Data = await _userRepo.GetUserAync(userID)
            };
             if (response.Data.IsSuccess){
                return Ok(response);
            }
            return BadRequest(response);
        }
        [HttpPost("update_profile")]
        public async Task<IActionResult> UpdateProfile(UpdateProfileModel model) 
        {
            var response = new ServiceResponse<UpdateProfileResponse>(){
                Data = await _userRepo.UpdateUserAttributesAsync(model)
            };
             if (response.Data.IsSuccess){
                return Ok(response);
            }
            return BadRequest(response);
        }
        [HttpPost("forgot_password")]
        public async Task<IActionResult> ForgotPassword(InitForgotPasswordModel model)
        {
            var response = new ServiceResponse<InitForgotPasswordResponse>(){
                Data = await _userRepo.TryInitForgotPassword(model)
            };
            if (response.Data.IsSuccess){
                return Ok(response);
            }
            return BadRequest(response);
           
        }
        [HttpPost("reset_password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordModel model)
        {
           var response = new ServiceResponse<ResetPasswordResponse>();
            response.Data = await _userRepo.TryResetPasswordWithConfirmationCodeAsync(model);
            if (response.Data.IsSuccess){
                return Ok(response);
            }
            return BadRequest(response);
        }
        [HttpPost("change_password")]
        public async Task<IActionResult> ChangePassword(ChangePasswordModel model)
        {
            var response = new ServiceResponse<ChanagePasswordResponse>();
            response.Data = await _userRepo.TryPasswordChanageAsync(model);
            if(!response.Data.IsSuccess){
                 return BadRequest(response);
             }
             return Ok(response);
        }

    }
}