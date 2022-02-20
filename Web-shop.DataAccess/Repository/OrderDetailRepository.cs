using Web_shop.DataAccess.Data;
using Web_shop.Models;

namespace Web_shop.DataAccess.Repository
{
    public class OrderDetailRepository : Repository<OrderDetail>
    {
        public OrderDetailRepository(ApplicationDbContext dbContext) : base(dbContext) { }
    }
}
