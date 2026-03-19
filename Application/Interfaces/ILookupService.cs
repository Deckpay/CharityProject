using Domain.Entities;

namespace Application.Interfaces
{
    public interface ILookupService
    {
        Task<IEnumerable<County>> GetCountiesAsync();
        Task<IEnumerable<ProductCategory>> GetProductCategoriesAsync();
        Task<IEnumerable<User>> GetUsersAsync();

        string GetCountiesNameString(int id);
        string GetProductCatergoriesNameString(int id);
        string GetUserNameString(int id);
    }
}
