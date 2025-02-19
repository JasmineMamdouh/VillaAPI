using System.Linq.Expressions;
using VillaAPI.Models;

namespace VillaAPI.Repository.IRepository
{

    //deals with data
    //let it inherit from the generic repo interface
    public interface IVillaRepository : IRepositoryy<Villa>
    {
        Task<Villa> UpdateAsync(Villa entity);//let it return the updated villa
        
    }
}
