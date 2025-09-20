using Fcg.Domain.Entities;
using Fcg.Domain.Repositories;
using Fcg.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Fcg.Infrastructure.Repositories
{
    public class PromotionRepository : IPromotionRepository
    {
        private readonly FcgDbContext _context;

        public PromotionRepository(FcgDbContext context)
        {
            _context = context;
        }

        public async Task<Guid> CreatePromotionAsync(Promotion promotion)
        {
            var entity = new Tables.Promotion
            {
                Id = promotion.Id,
                Title = promotion.Title,
                Description = promotion.Description,
                DiscountPercent = promotion.DiscountPercent,
                StartDate = promotion.StartDate,
                EndDate = promotion.EndDate,
                Genre = (int)promotion.Genre
            };

            _context.Promotions.Add(entity);

            await _context.SaveChangesAsync();

            return entity.Id;
        }

        public async Task<Promotion?> GetPromotionByTitleAsync(string title)
        {
            var promotion = await (from g in _context.Promotions
                                   where g.Title == title
                                   select new Promotion(
                                       g.Id,
                                       g.Title,
                                       g.Description,
                                       g.DiscountPercent,
                                       g.StartDate,
                                       g.EndDate,
                                       (GenreEnum)g.Genre)
                                 ).FirstOrDefaultAsync();

            return promotion;
        }

        public async Task<IEnumerable<Promotion>> GetValidPromotionsAsync()
        {
            var today = DateTime.UtcNow;

            var promotions = await (from p in _context.Promotions
                                    where p.StartDate <= today && p.EndDate >= today
                                    select new Promotion(
                                        p.Id,
                                        p.Title,
                                        p.Description,
                                        p.DiscountPercent,
                                        p.StartDate,
                                        p.EndDate,
                                        (GenreEnum)p.Genre)
                                   ).ToListAsync();

            return promotions;
        }

        public async Task UpdatePromotionAsync(Promotion promotion)
        {
            var promotionEntity = await _context.Promotions.FindAsync(promotion.Id);

            if (promotionEntity != null)
            {
                promotionEntity.Title = promotion.Title;
                promotionEntity.Description = promotion.Description;
                promotionEntity.DiscountPercent = promotion.DiscountPercent;
                promotionEntity.StartDate = promotion.StartDate;
                promotionEntity.EndDate = promotion.EndDate;
                promotionEntity.Genre = (int)promotion.Genre;

                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> DeletePromotionAsync(Guid promotionId)
        {
            var promotion = await _context.Promotions.FindAsync(promotionId);
            if (promotion == null)
                return false;

            _context.Promotions.Remove(promotion);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}