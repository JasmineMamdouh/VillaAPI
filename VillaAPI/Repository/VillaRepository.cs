using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Linq.Expressions;
using VillaAPI.Data;
using VillaAPI.Models;
using VillaAPI.Repository.IRepository;

namespace VillaAPI.Repository
{

    //here we will deal with the db context so add it using DI
    //let it inherit from the genric repo too
    public class VillaRepository :Repository<Villa>, IVillaRepository
    {
        private readonly ApplicationDbContext _db;

        //we need to pass to repository class the appdbcontext 
        //so :base(db) -> calls the constructor of the base class (Repository<Villa>) and passes db to it.
        public VillaRepository(ApplicationDbContext db):base(db)
        {
            _db = db; // stores db in the private field _db for use inside VillaRepository.
        }
       

        public async Task<Villa> UpdateAsync(Villa entity)
        {
            entity.UpdatedDate = DateTime.Now;
            _db.Villas.Update(entity);
            await _db.SaveChangesAsync();
            return entity;
        }
    }
}
