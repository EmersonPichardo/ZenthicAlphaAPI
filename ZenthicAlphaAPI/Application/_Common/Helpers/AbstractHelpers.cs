using System.Reflection;

namespace Application._Common.Helpers;

public static class AbstractHelpers
{
    public static IReadOnlyList<Type> GetTypesAssignableFromType<T>(Assembly? assembly = null)
        where T : class
    {
        return
            (assembly ?? Assembly.GetExecutingAssembly()).ExportedTypes
                .Where(type => typeof(T).IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract)
                .ToList();
    }

    public static IReadOnlyList<T> CreateInstancesAssignableFromType<T>(Assembly? assembly = null, params object[] args)
        where T : class
    {
        return
            GetTypesAssignableFromType<T>(assembly)
                .Select(type => Activator.CreateInstance(type, args))
                .Cast<T>()
                .ToList();
    }
}