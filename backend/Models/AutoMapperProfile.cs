using AutoMapper;
using LogisticsTroubleManagement.DTOs;
using LogisticsTroubleManagement.Models;

namespace LogisticsTroubleManagement.Models
{
    /// <summary>
    /// AutoMapperプロファイル
    /// </summary>
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            // Incident マッピング
            CreateMap<Incident, IncidentResponseDto>();
            CreateMap<CreateIncidentDto, Incident>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedByUser, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedByUser, opt => opt.Ignore());

            // UserRole マッピング
            CreateMap<UserRole, UserRoleResponseDto>();

            // User マッピング
            CreateMap<User, UserResponseDto>()
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.UserRole.RoleName));
            CreateMap<CreateUserDto, User>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.LastLoginAt, opt => opt.Ignore())
                .ForMember(dest => dest.UserRole, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedIncidents, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedIncidents, opt => opt.Ignore());
        }
    }
}

