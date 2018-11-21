using System;
using RC.DataAccess;

namespace RC.API
{
    public partial class Startup
    {
        public void InitializeData(IServiceProvider serviceProvider, RCDataContext context)
        {
            // if have more data need to create for first time, add in this
            RCDAtaContextSeed.Initialize(serviceProvider, context);
        }
    }
}
