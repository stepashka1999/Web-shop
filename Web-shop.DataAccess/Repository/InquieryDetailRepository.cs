using Web_shop.DataAccess.Data;
using Web_shop.Models;

namespace Web_shop.DataAccess.Repository
{
    public class InquieryDetailRepository : Repository<InquieryDetail>
    {
        public InquieryDetailRepository(ApplicationDbContext dbContext) : base(dbContext) { }
    }
}
