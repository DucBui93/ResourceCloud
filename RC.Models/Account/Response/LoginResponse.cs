namespace RC.Models.Account.Response
{
    public class LoginResponse: IResponse
    {
        public ResponseMessage ResponseMessage { get; set; }

        public string AccessToken { get; set; }

        public string RefreshAccessToken { get; set; }
    }
}
