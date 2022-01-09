using Web_shop.DataAccess.Data;
using Web_shop.Models;

namespace Web_shop.DataAccess.Repository
{
    public class CategoryRepository : Repository<Category>
    {
        public CategoryRepository(ApplicationDbContext dbContext) : base(dbContext) { }
    }
}
