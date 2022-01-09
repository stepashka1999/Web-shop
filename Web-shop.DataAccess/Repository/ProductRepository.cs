using Web_shop.DataAccess.Data;
using Web_shop.Models;

namespace Web_shop.DataAccess.Repository
{
    public class ProductRepository : Repository<Product>
    {
        public ProductRepository(ApplicationDbContext dbContext) : base(dbContext) { }
    }
}
