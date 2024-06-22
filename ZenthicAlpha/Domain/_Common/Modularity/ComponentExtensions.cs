using System.Reflection;

namespace Domain._Common.Modularity;

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
