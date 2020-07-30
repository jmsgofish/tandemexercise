using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using TandemExercise.Business.Entities;
using TandemExercise.Business.Entities.DTO;

namespace TandemExercise.API
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<User, UserDTO>()
                .ForMember(dest => dest.userId, opt => opt.MapFrom(src => src.id))
                .ForMember(dest =>
                        dest.name,
                    opt => opt.MapFrom(src => src.firstName + " " + src.middleName + " " + src.lastName));
        }
    }
}
