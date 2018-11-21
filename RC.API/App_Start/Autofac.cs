using Microsoft.Extensions.DependencyInjection;
using RC.Business.Implementations;
using RC.Business.Interfaces;
using RC.DataAccess.Implementations;
using RC.DataAccess.Interfaces;

namespace RC.API
{
    public partial class Startup
    {
        public void DependenceInjection(IServiceCollection services)
        {
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped(typeof(IService<>), typeof(Service<>));

            services.AddTransient<IUserRepositry, UserRepository>();
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IUserTokenRepository, UserTokenRepository>();
        }
    }
}
