using Web_shop.DataAccess.Data;
using Web_shop.Models;

namespace Web_shop.DataAccess.Repository
{
    public class ApplicationUserRepository : Repository<ApplicationUser>
    {
        public ApplicationUserRepository(ApplicationDbContext dbContext) : base(dbContext) { }
    }
}
