using VillaAPI.Models;
using VillaAPI.Models.Dto;

namespace VillaAPI.Repository.IRepository
{
    public interface IUserRepository
    {
        /*we need a method to check if it is a unique user*/
        bool IsUniqueUser(string username);

        Task<LoginResponseDTO> Login(LoginRequestDTO loginRequestDTO);
        Task<LocalUser> Register(RegisterRequestDTO registerRequestDTO);
    }
}
