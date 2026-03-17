using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class LookupService : ILookupService
    {
        private readonly IUnitOfWork _unitOfWork;

        public LookupService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork; 
        }

        private List<County>? _counties;
        private List<ProductCategory>? _productCategories;
        private List<User>? _users;
                
        public async Task<IEnumerable<County>> GetCountiesAsync()
        {
            if (_counties == null)
            {
                var result = await _unitOfWork.Counties.GetAllAsync();
                _counties = result.ToList();
            }
            return _counties;
        }

        public string GetCountiesNameString(int id) =>
            _counties?.FirstOrDefault(c => c.CountyId == id)?.CountyName ?? "Ismeretlen";

        public async Task<IEnumerable<ProductCategory>> GetProductCatergoriesAsync()
        {
            if (_productCategories == null)
            {
                var resul = await _unitOfWork.Categories.GetAllAsync();
                _productCategories = resul.ToList();
            }
            return _productCategories;
        }

        public string GetProductCatergoriesNameString(int id) =>
            _productCategories?.FirstOrDefault(c => c.ProductCategoryId == id)?.ProductCategoryName ?? "Ismeretlen";


        public async Task<IEnumerable<User>> GetUsersAsync()
        {
            if (_users == null)
            {
                var result = await _unitOfWork.Users.GetAllAsync();
                _users = result.ToList();
            }
            return _users;
        }

        public string GetUserNameString(int id) =>
            _users?.FirstOrDefault(u => u.UserId == id)?.UserName ?? "Ismeretlen";
    }
}
