using Fcg.Domain.Queries;
using Fcg.Domain.Queries.Responses;
using Fcg.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Fcg.Infrastructure.Queries
{
    public class PromotionQuery : IPromotionQuery
    {
        private readonly FcgDbContext _context;

        public PromotionQuery(FcgDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<PromotionResponse>> GetAllPromotionsAsync()
        {
            var promotions = await (from p in _context.Promotions
                                    select new PromotionResponse
                                    {
                                        Id = p.Id,
                                        Title = p.Title,
                                        Description = p.Description,
                                        DiscountPercent = p.DiscountPercent,
                                        StartDate = p.StartDate,
                                        EndDate = p.EndDate
                                    }).ToListAsync();

            return promotions;
        }

        public async Task<PromotionResponse?> GetByIdPromotionAsync(Guid promotionId)
        {
            var promotion = await (from p in _context.Promotions
                                    where p.Id == promotionId
                                    select new PromotionResponse
                                    {
                                        Id = p.Id,
                                        Title = p.Title,
                                        Description = p.Description,
                                        DiscountPercent = p.DiscountPercent,
                                        StartDate = p.StartDate,
                                        EndDate = p.EndDate
                                    }).FirstOrDefaultAsync();

            return promotion;
        }
    }
}
