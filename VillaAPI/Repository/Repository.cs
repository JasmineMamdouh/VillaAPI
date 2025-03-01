using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using VillaAPI.Data;
using VillaAPI.Models;
using VillaAPI.Repository.IRepository;

namespace VillaAPI.Repository
{
    /* Instead of making Repository for each model and cause duplication
     * just make a generic repo with the CRUD operations
     * 
     */
    public class Repository<T> : IRepositoryy<T> where T : class
    {

        private readonly ApplicationDbContext _db;
        
        //access to the db table for T
        internal DbSet<T> _dbSet;    //to allow the repo to interact with the db table corresponding to generic type T
        public Repository(ApplicationDbContext db)
        {
            _db = db;
            this._dbSet = _db.Set<T>();  //Dynamically assign the correctDbSet
        }
        public async Task CreateAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            await SaveAsync();
        }

        public async Task<T> GetAsync(Expression<Func<T, bool>>? filter = null, bool tracked = true)
        {
            IQueryable<T> query = _dbSet;
            if (!tracked)
            {
                query = query.AsNoTracking();
            }
            if (filter != null)
            {
                query = query.Where(filter);
            }
            return await query.FirstOrDefaultAsync();   //deferred execution, by using ToList it turns it to immediate execution

        }

        public async Task<List<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null, int pageSize = 2, int pageNumber = 1, string? includeProperties = null)
        {
            IQueryable<T> query = _dbSet;
            if (filter != null)
            {
                query = query.Where(filter);
            }
            //add pagination
            if (pageSize > 0)
            {
                if (pageSize > 100)
                {
                    pageSize = 100;
                }
                query = query.Skip(pageSize * (pageNumber - 1)).Take(pageSize);
            }
            if (includeProperties != null)
            {
                foreach (var includeProp in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProp);
                }
            }
            return await query.ToListAsync();   //deferred execution, by using ToList it turns it to immediate execution
        }

        public async Task RemoveAsync(T entity)
        {
            _dbSet.Remove(entity);
            await SaveAsync();
        }

        public async Task SaveAsync()
        {
            await _db.SaveChangesAsync();
        }


    }
}
