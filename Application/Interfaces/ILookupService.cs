using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface ILookupService
    {
        Task<IEnumerable<County>> GetCountiesAsync();
        Task<IEnumerable<ProductCategory>> GetProductCatergoriesAsync();
        Task<IEnumerable<User>> GetUsersAsync();

        string GetCountiesNameString(int id);
        string GetProductCatergoriesNameString(int id);
        string GetUserNameString(int id);
    }
}
