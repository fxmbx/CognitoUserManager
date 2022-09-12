using System.Buffers;
using System.Net;
using System.Reflection;
using System.Reflection.Metadata;
using Amazon;
using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using Amazon.Extensions.CognitoAuthentication;
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
// using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.VisualBasic;
using Newtonsoft.Json;

namespace CognitoManager.Core.Services
{
    public class UserRepository : IUserRepository
    {
        private readonly AppConfig _awsconfig;
        private readonly AmazonCognitoIdentityProviderClient _cognitoProvider;

        private readonly CognitoUserPool _userpool;
        // private readonly IHttpContextAccessor _httpContext;
        // private readonly UserContextManager _userManager;  

        public UserRepository(IOptions<AppConfig> appconfig)
        {
            _awsconfig = appconfig.Value;
            _cognitoProvider = new AmazonCognitoIdentityProviderClient(
                _awsconfig.AccessKeyID,
                _awsconfig.AccessSecretKey,
                RegionEndpoint.GetBySystemName(_awsconfig.Region)
            ) ;
            _userpool = new CognitoUserPool(
                _awsconfig.UserPoolID,
                _awsconfig.AppClientID,
                _cognitoProvider
            );
            // _httpContext = httpContextAccessor;
        }

        public async Task<UserSignUpResponse> ConfirmUserSignUpAsync(UserConfirmSignUpModel model)
        {
            try{
                var confirmSignUprequest = new ConfirmSignUpRequest{
                    ClientId = _awsconfig.AppClientID,
                    ConfirmationCode = model.ConfirmationCode,
                    Username = model.Email
                };

                var response = await _cognitoProvider.ConfirmSignUpAsync(confirmSignUprequest);

                return new UserSignUpResponse {
                    Email = model.Email,
                    UserID = model.UserID,
                    Message = "User Confirmed",
                    IsSuccess = true
                };
            }catch(CodeMismatchException ex){
                return new UserSignUpResponse {
                    Email = model.Email,
                    UserID = model.UserID,
                    Message = String.Format("Wrong Code \n {0}", ex.Message),
                    IsSuccess = true
                };
            }
        }

        public async Task<UserSignUpResponse> CreateUserAsync(UserSignUpModel model)
        {
         try{   
           SignUpRequest signUpRequest = new SignUpRequest{
               ClientId = _awsconfig.AppClientID,
               Password = model.Password,
               Username = model.Email,
              
           };
           signUpRequest.UserAttributes.Add(new AttributeType{
               Name = "email",
               Value = model.Email
           });
           
           signUpRequest.UserAttributes.Add(new AttributeType{
               Name = "given_name",
               Value = model.GivenName
           });
           signUpRequest.UserAttributes.Add(new AttributeType{
               Name = "phone_number",
               Value = model.PhoneNumber
           });


            //add later on , upload image to s3 account
            //if (model.ProfilePhoto != null)
            //{
            //    // upload the incoming profile photo to user's S3 folder
            //    // and get the s3 url
            //    // add the s3 url to the profile_photo attribute of the userCognito
            //    var picUrl = await _storage.AddItem(model.ProfilePhoto, "profile");

            //    signUpRequest.UserAttributes.Add(new AttributeType
            //    {
            //        Value = picUrl,
            //        Name = "picture"
            //    });
            //}

            var signUpResponse = await _cognitoProvider.SignUpAsync(signUpRequest);
            return  new UserSignUpResponse{
                UserID = signUpResponse.UserSub,
                Email = model.Email,
                Message = String.Format("Confirmation code sent to: {0} via {1}",signUpResponse.CodeDeliveryDetails.Destination, signUpResponse.CodeDeliveryDetails.DeliveryMedium.Value),
                IsSuccess = true,
                Status = CognitoStatusCode.USER_UNCONFIRMED
            };
            }catch(UsernameExistsException e){

                return new UserSignUpResponse {
                    IsSuccess= false,
                    Message = e.Message
                };
            }catch(Exception ex){
                return new UserSignUpResponse{
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<UserProfileResponse> GetUserAync(string email)
        {
            //    var userResponse = await _cognitoProvider.AdminGetUserAsync(new AdminGetUserRequest{
            //        Username = userID,
            //        UserPoolId= _awsconfig.UserPoolID
            //    });
            var users = await FindUsersByEmail(email);
            var userResponse = users.Users.FirstOrDefault();

            var attributes = userResponse.Attributes;
            var response = new UserProfileResponse{
                Email = attributes.GetValueOrDefault("email",string.Empty),
                GivenName = attributes.GetValueOrDefault("given_name",string.Empty),
                PhoneNumber = attributes.GetValueOrDefault("phone_number",string.Empty),
                Gender = attributes.GetValueOrDefault("gender",string.Empty),
                
            };
                var address = attributes.GetValueOrDefault("addres",string.Empty);

                if (!string.IsNullOrEmpty(address)){
                    response.Address =JsonConvert.DeserializeObject<Dictionary<string, string>>(address);
                }
                return response;

        }

        public async Task<InitForgotPasswordResponse> TryInitForgotPassword(InitForgotPasswordModel model)
        {
            try{

            var listUserResponse = await FindUsersByEmail(model.Email);
            if ( listUserResponse != null && listUserResponse.HttpStatusCode == HttpStatusCode.OK)
            {
                var users = listUserResponse.Users;
                UserType filtered_user = users?.FirstOrDefault(x=>x.Attributes.Any(x=>x.Name == "email" && x.Value == model?.Email));

                if (filtered_user != null){
                Console.WriteLine("👻: {0}", filtered_user.Username);
                    var forgotPasswordRequest = new ForgotPasswordRequest{
                        ClientId = _awsconfig.AppClientID,
                        Username = filtered_user.Username
                    };
                    ForgotPasswordResponse forgotPasswordRespose = await _cognitoProvider.ForgotPasswordAsync(forgotPasswordRequest);

                    if (forgotPasswordRespose.HttpStatusCode == HttpStatusCode.OK && forgotPasswordRespose != null){
                        return new InitForgotPasswordResponse{

                            UserID = filtered_user.Username,
                            Email = model.Email,
                            IsSuccess = true,
                            Message= String.Format("Confirmation Code sent to: {0} via {1}",forgotPasswordRespose.CodeDeliveryDetails.Destination,forgotPasswordRespose.CodeDeliveryDetails.DeliveryMedium.Value),
                            Status = CognitoStatusCode.USER_UNCONFIRMED
                        };
                    }else{
                         return new InitForgotPasswordResponse{
                            IsSuccess = false,
                            Message= String.Format("Forgotpassword response :{0}",forgotPasswordRespose.HttpStatusCode.ToString()),
                            Status = CognitoStatusCode.API_ERROR
                        };
                    }
                }else {
                    return new InitForgotPasswordResponse{
                        IsSuccess= false,
                        Message= String.Format("No user with the give email found: 😞 {0}", filtered_user!.Username),
                        Status = CognitoStatusCode.API_ERROR
                    };
                }
            }else {
                return new InitForgotPasswordResponse{
                        IsSuccess= false,
                        Message= String.Format("ListUsers Response : 😞 {0}", listUserResponse?.HttpStatusCode.ToString()),
                        Status = CognitoStatusCode.API_ERROR
                    };
            }
            }catch(Exception ex){
                 return new InitForgotPasswordResponse{
                        IsSuccess= false,
                        Message= String.Format("Something went Wrong : 😞 {0}", ex.Message),
                        Status = CognitoStatusCode.API_ERROR
                    };
            }
            
        }

        public async Task<AuthResponseModel> TryLoginAsync(UserLoginModel model)
        {
          try{
                    var result =  await AuthenticateUserAsync(model.Email, model.Password);

                    var response = new AuthResponseModel {
                        UserID = result.Item1.Username,
                        Email = result.Item1.UserID,
                        Tokens = new TokenModel {
                            TokenID = result.Item2.IdToken,
                            AccessToken = result.Item2.AccessToken,
                            RefreshToken = result.Item2.RefreshToken,
                            ExpiresIn = result.Item2.ExpiresIn
                        },
                        //from baseResponse
                        IsSuccess =true,
                        Message = "Successful Login",
                    };
                    return response;
             }catch(UserNotConfirmedException)
               {
                    var listuserReponse = await FindUsersByEmail(model.Email);
                    if(listuserReponse != null && listuserReponse.HttpStatusCode == HttpStatusCode.OK)
                    {
                        var users = listuserReponse.Users;
                        var filtered_user = users.FirstOrDefault(x =>x.Attributes.Any(x=>x.Name == "email" && x.Value == model.Email));

                        var resendCodeResponse = await _cognitoProvider.ResendConfirmationCodeAsync(new ResendConfirmationCodeRequest{
                            ClientId = _awsconfig.AppClientID,
                            Username = filtered_user.Username
                        });

                        if (resendCodeResponse.HttpStatusCode == HttpStatusCode.OK){
                                return new AuthResponseModel {
                                    IsSuccess = false,
                                    Message = String.Format("Confirmation COde Sent to {0} via {1}",resendCodeResponse.CodeDeliveryDetails, resendCodeResponse.CodeDeliveryDetails.DeliveryMedium.Value),
                                    Status = CognitoStatusCode.USER_UNCONFIRMED,
                                    UserID = filtered_user.Username
                                };
                        }else{
                                return new AuthResponseModel {
                                    IsSuccess = false,
                                    Message = String.Format("Resend Confirmation Code Response: {0}",resendCodeResponse.HttpStatusCode.ToString()),
                                    Status = CognitoStatusCode.API_ERROR,
                                    UserID = filtered_user.Username
                                };
                        }

                    }else{
                        return new AuthResponseModel{
                            IsSuccess= false,
                            Message = "Incorrect username or password",
                            Status = CognitoStatusCode.API_ERROR
                        };
                    }
                }catch(UserNotFoundException){
                    return new AuthResponseModel
                    {
                        IsSuccess = false,
                        Message = "EmailAddress not found.",
                        Status = CognitoStatusCode.USER_NOTFOUND
                    };
                }catch (NotAuthorizedException){
                    return new AuthResponseModel
                    {
                        IsSuccess = false,
                        Message = "Incorrect username or password",
                        Status = CognitoStatusCode.API_ERROR
                    };
                }
        }

        public async Task<UserSignOutResponse> TryLogOutAsync(UserSignOutModel model)
        {
           var request = new GlobalSignOutRequest {
               AccessToken = model.AccessToken
           };
           var response = await _cognitoProvider.GlobalSignOutAsync(request);
           return new UserSignOutResponse {
               UserID = model.UserID,
               Message = "User Signed Out"
           };
        }

        public async Task<ChanagePasswordResponse> TryPasswordChanageAsync(ChangePasswordModel model)
        {
          try{
              //authenticate to be sure password is correct
             var result = await AuthenticateUserAsync(model.Email, model.CurrentPassword);

             ChangePasswordRequest request = new ChangePasswordRequest{
                 AccessToken = result.Item2.AccessToken,
                 PreviousPassword = model.CurrentPassword,
                 ProposedPassword = model.NewPassword
             };

             ChangePasswordResponse response = await _cognitoProvider.ChangePasswordAsync(request);

             if(response.HttpStatusCode == HttpStatusCode.OK){
                 return new ChanagePasswordResponse {
                     IsSuccess = true,
                     Message = "Password Changed",
                     UserID = result.Item1.UserID,
                 };
             }else{

             return new ChanagePasswordResponse {
                     IsSuccess = false,
                     Message = "Password not Changed idk what happened",
                     UserID = result.Item1.UserID,
                     Status = CognitoStatusCode.API_ERROR
                 };
             }



          }catch(UserNotFoundException){
                return new ChanagePasswordResponse {
                     IsSuccess = false,
                     Message = "User Not Found",
                     Status = CognitoStatusCode.USER_NOTFOUND
                 };
          }catch(UnauthorizedException){
                return new ChanagePasswordResponse {
                     IsSuccess = false,
                     Message = "User Unathorized",
                     Status = CognitoStatusCode.API_ERROR
                 };
          }catch(Exception ex){
                return new ChanagePasswordResponse {
                     IsSuccess = false,
                     Message = $"Something went wrong {ex.Message}",
                     Status = CognitoStatusCode.API_ERROR
                 };
          }
        }

        public async Task<ResetPasswordResponse> TryResetPasswordWithConfirmationCodeAsync(ResetPasswordModel model)
        {
           var response = await _cognitoProvider.ConfirmForgotPasswordAsync(new ConfirmForgotPasswordRequest{
               ClientId = _awsconfig.AppClientID,
               Username = model.UserID,
               Password = model.NewPassword,
               ConfirmationCode = model.ConfirmationCode
           });

           if (response.HttpStatusCode == HttpStatusCode.OK){
               return new ResetPasswordResponse{
                   IsSuccess = true,
                   Message = "Password Updated. Please Login"
               };
           }else {
               return new ResetPasswordResponse{
                   IsSuccess = false,
                   Message = $"Reset Password Response: {response.HttpStatusCode.ToString()}",
                   Status = CognitoStatusCode.API_ERROR
               };
           }

        }

        public async Task<UpdateProfileResponse> UpdateUserAttributesAsync(UpdateProfileModel model)
        {
           try
           {
               UpdateUserAttributesRequest userAttributesRequest = new UpdateUserAttributesRequest{
                   AccessToken = model.AccessToken
               };
               userAttributesRequest.UserAttributes.Add(new AttributeType{
                   Name = "given_name",
                   Value= model.GivenName
               });
               userAttributesRequest.UserAttributes.Add(new AttributeType{
                   Name = "phone_number",
                   Value= model.PhoneNumber
               });
               if(model.Gender != null){

               userAttributesRequest.UserAttributes.Add(new AttributeType{
                   Name = "gender",
                   Value= model.Gender
               });
               }
                if (!string.IsNullOrEmpty(model.Address) ||
                string.IsNullOrEmpty(model.State) ||
                string.IsNullOrEmpty(model.Country) ||
                string.IsNullOrEmpty(model.Pincode)){
                    var dictionary = new Dictionary<string, string>();

                    dictionary.Add("street_address", model.Address);
                    dictionary.Add("region", model.State);
                    dictionary.Add("country", model.Country);
                    dictionary.Add("postal_code", model.Pincode);


                    userAttributesRequest.UserAttributes.Add(new AttributeType {
                        Name = "address",
                        Value= JsonConvert.SerializeObject(dictionary)
                    });
                }
                
                var response = await _cognitoProvider.UpdateUserAttributesAsync(userAttributesRequest);
                return new UpdateProfileResponse { 
                    UserID = model.UserID,
                     Message = "Profile Updated",
                      IsSuccess = true
                };
           }catch(Exception ex){
                return new UpdateProfileResponse { 
                      Message = $"SOmething went wrong {ex.Message}",
                      IsSuccess = true
                };
           }
        }

        private async Task<Tuple<CognitoUser, AuthenticationResultType>> AuthenticateUserAsync(string email, string password) {
           
            var user = new CognitoUser(email, _awsconfig.AppClientID, _userpool,_cognitoProvider);
          
            var authRequest = new InitiateSrpAuthRequest(){
               Password = password
            };

            AuthFlowResponse authFlowResponse = await user.StartWithSrpAuthAsync(authRequest);
           
            var result = authFlowResponse.AuthenticationResult;

            return new Tuple<CognitoUser, AuthenticationResultType>(user, result);
        }

       private async Task<ListUsersResponse> FindUsersByEmail(string email){
           var req = new ListUsersRequest{
               UserPoolId = _awsconfig.UserPoolID,
               Filter = $"email=\"{email}\""
           };

           var res = await _cognitoProvider.ListUsersAsync(req);
           return res;
       }

    }


    internal static class AttributeTypeExtension
    {
        public static string GetValueOrDefault(this List<AttributeType> attributeType, string propertyName, string defaultValue){
            var prop = attributeType.FirstOrDefault(x =>x.Name == propertyName);
            if (prop != null){
                return prop.Value;
            }else return defaultValue;

        }
    }
}

