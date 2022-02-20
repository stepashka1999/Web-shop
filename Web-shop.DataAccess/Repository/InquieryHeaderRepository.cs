using Web_shop.DataAccess.Data;
using Web_shop.Models;

namespace Web_shop.DataAccess.Repository
{
    public class InquiryHeaderRepository : Repository<InquiryHeader>
    {
        public InquiryHeaderRepository(ApplicationDbContext dbContext) : base(dbContext) { }
    }
}
