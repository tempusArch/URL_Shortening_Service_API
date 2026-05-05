using AutoMapper;
using ShortenUrlApi.Models;

namespace ShortenUrlApi.Mapper;

public class MappingProfile : Profile {
    public MappingProfile() {
        CreateMap<ShortUrlModel, ShortUrlModelResponseDto>();
    }
}