using Fcg.Domain.Queries.Responses;

namespace Fcg.Domain.Queries
{
    public interface IPromotionQuery
    {
        Task<IEnumerable<PromotionResponse>> GetAllPromotionsAsync();
        Task<PromotionResponse?> GetByIdPromotionAsync(Guid promotionId);
    }
}
