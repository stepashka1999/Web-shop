using Web_shop.DataAccess.Data;
using Web_shop.Models;

namespace Web_shop.DataAccess.Repository
{
    public class OrderHeaderRepository : Repository<OrderHeader>
    {
        public OrderHeaderRepository(ApplicationDbContext dbContext) : base(dbContext) { }
    }
}
