namespace Application.Interfaces
{
    public interface ILimitService
    {
        Task<bool> CanUserRequestProduct(int userId, int categoryId);
        Task<bool> UpdateLimitUsage(int userId, int categoryId);
        Task<bool> DecreaseLimitUsage(int userId, int categoryId);
    }
}
