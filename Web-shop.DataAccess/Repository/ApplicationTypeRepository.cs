using Web_shop.DataAccess.Data;
using Web_shop.Models;

namespace Web_shop.DataAccess.Repository
{
    public class ApplicationTypeRepository : Repository<ApplicationType>
    {
        public ApplicationTypeRepository(ApplicationDbContext dbContext) : base(dbContext) { }
    }
}
