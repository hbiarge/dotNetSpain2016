﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Authorization.Models;

namespace Authorization.Services
{
    public interface IProductsStore
    {
        Task<IEnumerable<Product>> GetAllAsync();

        Task<Product> GetByIdAsync(int id);

        Task AddAsync(Product product);

        Task UpdateAsync(Product product);

        Task RemoveAsync(Product product);
    }
}
