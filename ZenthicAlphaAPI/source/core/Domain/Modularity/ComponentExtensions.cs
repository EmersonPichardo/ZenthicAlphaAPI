using System.Reflection;

namespace Domain.Modularity;

public static class ComponentExtensions
{
    public static Module GetModule(this Component component)
    {
        return component
            .GetType()
            .GetCustomAttribute<ModuleAttribute>()?
            .GetModule()
        ?? Module.None;
    }
}
