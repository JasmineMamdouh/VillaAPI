using System.Linq.Expressions;
using VillaAPI.Models;

namespace VillaAPI.Repository.IRepository
{
    //generic repository for any generic class
    public interface IRepositoryy <T> where T: class
    {
        Task<List<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null, int pageSize = 2, int pageNumber = 1, string? includeProperties = null);
        Task<T> GetAsync(Expression<Func<T, bool>>? filter = null, bool tracked = true);
        Task CreateAsync(T entity);
        Task RemoveAsync(T entity);
        Task SaveAsync();
    }
}
