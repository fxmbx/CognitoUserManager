namespace CognitoUserManager.Contracts
{
    public class AppConfig
    {
        public string Region { get; set; }
        public string UserPoolID { get; set; }
        public string AppClientID { get; set; }
        public string AccessKeyID { get; set; }
        public string AccessSecretKey { get; set; }
    }
}