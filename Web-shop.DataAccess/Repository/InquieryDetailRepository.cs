using Web_shop.DataAccess.Data;
using Web_shop.Models;

namespace Web_shop.DataAccess.Repository
{
    public class InquiryDetailRepository : Repository<InquiryDetail>
    {
        public InquiryDetailRepository(ApplicationDbContext dbContext) : base(dbContext) { }
    }
}
