using RC.DataAccess.Interfaces;
using RC.Models.EntityModels;

namespace RC.DataAccess.Implementations
{
    public class UserTokenRepository: Repository<UserToken>,IUserTokenRepository
    {
        public UserTokenRepository(RCDataContext context) : base(context)
        {
            
        }
    }
}
