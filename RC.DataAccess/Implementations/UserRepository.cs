using RC.DataAccess.Interfaces;
using RC.Models.EntityModels;

namespace RC.DataAccess.Implementations
{
    public class UserRepository : Repository<User>,IUserRepositry
    {
        public UserRepository(RCDataContext context): base(context)
        {
            
        }
    }
}
