using AutoMapper;
using Backend.Services.UserService;
using Backend.Data.Models;

namespace Backend {
    public class MappingProfile : Profile {
        public MappingProfile () {
            CreateMap<User, UserDto>();
        }
    }
}