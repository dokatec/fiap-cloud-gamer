using Fcg.Domain.Entities;

namespace Fcg.Domain.Repositories
{
    public interface IPromotionRepository
    {
        Task<Guid> CreatePromotionAsync(Promotion promotion);
        Task<Promotion?> GetPromotionByTitleAsync(string title);
        Task<IEnumerable<Promotion>> GetValidPromotionsAsync();
        Task UpdatePromotionAsync(Promotion promotion);
        Task<bool> DeletePromotionAsync(Guid id);
    }
}