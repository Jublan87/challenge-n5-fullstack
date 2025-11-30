using AutoMapper;
using N5.Permissions.Application.DTOs;
using N5.Permissions.Domain.Entities;

namespace N5.Permissions.Application.Mappings;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<CreatePermissionDto, Permission>();

        CreateMap<UpdatePermissionDto, Permission>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        CreateMap<Permission, PermissionDto>()
            .ForMember(dest => dest.TipoPermisoDescripcion, 
                opt => opt.MapFrom(src => src.PermissionType != null ? src.PermissionType.Descripcion : null));
    }
}

