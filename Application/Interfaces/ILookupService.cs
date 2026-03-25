using Domain.Entities;

namespace Application.Interfaces
{
    public interface ILookupService
    {
        Task<IEnumerable<County>> GetCountiesAsync();
        Task<IEnumerable<ProductCategory>> GetProductCategoriesAsync();
        Task<IEnumerable<User>> GetUsersAsync();
        Task<IEnumerable<Product>> GetProductsAsync();

        string GetCountiesNameString(int id);
        string GetProductCatergoriesNameString(int id);
        string GetUserNameString(int id);
        string GetProductNameString(int id);
    }
}
