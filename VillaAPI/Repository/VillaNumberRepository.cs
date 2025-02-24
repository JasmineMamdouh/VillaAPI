using VillaAPI.Data;
using VillaAPI.Models;
using VillaAPI.Repository.IRepository;

namespace VillaAPI.Repository
{
    public class VillaNumberRepository: Repository<VillaNumber>, IVillaNumberRepository
    {
        private readonly ApplicationDbContext _db;

        //we need to pass to repository class the appdbcontext 
        //so :base(db) -> calls the constructor of the base class (Repository<Villa>) and passes db to it.
        public VillaNumberRepository(ApplicationDbContext db) : base(db)
        {
            _db = db; // stores db in the private field _db for use inside VillaRepository.
        }


        public async Task<VillaNumber> UpdateAsync(VillaNumber entity)
        {
            entity.UpdatedDate = DateTime.Now;
            _db.VillaNumbers.Update(entity);
            await _db.SaveChangesAsync();
            return entity;
        }
    }
}
