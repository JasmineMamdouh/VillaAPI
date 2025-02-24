using VillaAPI.Models;

namespace VillaAPI.Repository.IRepository
{
    public interface IVillaNumberRepository : IRepositoryy<VillaNumber>
    {
        Task<VillaNumber> UpdateAsync(VillaNumber entity);//let it return the updated villaNumber
    }
}
