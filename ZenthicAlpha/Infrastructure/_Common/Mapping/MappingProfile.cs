using Application;
using Application._Common.Mapping;
using AutoMapper;
using System.Reflection;

namespace Infrastructure._Common.Mapping;

internal class MappingProfileMapFrom : Profile
{
    private readonly Type[] argumentTypes = [typeof(Profile)];

    public MappingProfileMapFrom()
        => ApplyMappingsFromAssembly(
            nameof(IMapFrom<object>.Mapping),
            typeof(IApplicationAssembly).Assembly,
            Assembly.GetExecutingAssembly()
        );

    private void ApplyMappingsFromAssembly(string mappingMethodName, params Assembly[] assemblies)
    {
        var types = assemblies
            .SelectMany(GetTypesImplementingIMapFromInterfaceInAssembly)
            .ToList();

        foreach (var type in types)
        {
            var instance = Activator.CreateInstance(type);
            var methodInfo = type.GetMethod(mappingMethodName);

            if (methodInfo is not null)
            {
                methodInfo.Invoke(instance, new object[] { this });
                continue;
            }

            var foundInterfaces = type
                .GetInterfaces()
                .Where(IsIMapFromInterface)
                .ToList();

            foreach (var foundInterface in foundInterfaces)
            {
                var interfaceMethodInfo = foundInterface.GetMethod(mappingMethodName, argumentTypes);
                interfaceMethodInfo?.Invoke(instance, new object[] { this });
            }
        }
    }
    private static bool IsIMapFromInterface(Type type)
        => type.IsGenericType
        && type.GetGenericTypeDefinition() == typeof(IMapFrom<>);
    private static IEnumerable<Type> GetTypesImplementingIMapFromInterfaceInAssembly(Assembly assembly)
        => assembly
            .GetExportedTypes()
            .Where(type => type
                .GetInterfaces()
                .ToList()
                .Exists(IsIMapFromInterface)
            );
}
