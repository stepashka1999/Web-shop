using Web_shop.DataAccess.Data;
using Web_shop.Models;

namespace Web_shop.DataAccess.Repository
{
    public class InquieryHeaderRepository : Repository<InquieryHeader>
    {
        public InquieryHeaderRepository(ApplicationDbContext dbContext) : base(dbContext) { }
    }
}
