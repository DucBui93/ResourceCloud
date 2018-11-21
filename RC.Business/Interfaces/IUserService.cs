using System.Threading.Tasks;
using RC.Models.Account.Request;
using RC.Models.Account.Response;
using RC.Models.EntityModels;

namespace RC.Business.Interfaces
{
    public interface IUserService:IService<User>
    {
        Task<LoginResponse> LoginAsync(LoginRequest request);

        Task<LoginResponse> RefreshLoginAsync(RefreshLoginRequest request);

        //Task<ResponseMessage> ChangePasswordAsync(ChangePasswordRequest request);

        //Task<ResponseMessage> ForgotPasswordAsync(ForgotPasswordRequest request);

        //Task<ResponseMessage> ResetPasswordAsync(ResetPasswordRequest request);
    }
}
