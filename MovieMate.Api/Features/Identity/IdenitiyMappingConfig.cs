using Mapster;
using MovieMate.Api.Features.Identity.Contracts;
using MovieMate.Api.Features.Identity.Entities;

namespace MovieMate.Api.Features.Identity;

public class IdenitiyMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<RegisterRequest, ApplicationUser>()
            .Map(dest => dest.UserName, src => src.Email)
            .Map(dest => dest.Email, src => src.Email)
            .Map(dest => dest.FirstName, src => src.FirstName)
            .Map(dest => dest.LastName, src => src.LastName);

    }
}