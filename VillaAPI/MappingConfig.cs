using AutoMapper;
using VillaAPI.Models;
using VillaAPI.Models.Dto;

namespace VillaAPI
{
    //automapping is much better for models of many features
    public class MappingConfig : Profile
    {

        public MappingConfig()  // ✅ Constructor needed!
        {
            CreateMap<Villa, VillaDTO>();  // Maps properties automatically <src, dst>
            CreateMap<VillaDTO, Villa>();  // Reverse mapping
            CreateMap<Villa, VillaCreateDTO>().ReverseMap(); //to include its reverse as well
            CreateMap<Villa, VillaUpdateDTO>();
            CreateMap<VillaUpdateDTO,Villa>();
        }
        //we can also do custom mappings 
    }
}
