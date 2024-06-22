using AutoMapper;

namespace Application._Common.Mapping;

public interface IMapFrom<TSource>
    where TSource : class
{
    void Mapping(Profile profile)
        => profile.CreateMap(typeof(TSource), GetType());
}