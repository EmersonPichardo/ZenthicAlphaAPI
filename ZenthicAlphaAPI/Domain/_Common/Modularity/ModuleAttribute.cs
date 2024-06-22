namespace Domain._Common.Modularity;

/// <summary>
/// Specifies the module of a component.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="ModuleAttribute"/> class with a specific module.
/// </remarks>
/// <param name="module">Specifies the module of the component.</param>
[AttributeUsage(AttributeTargets.Field)]
public class ModuleAttribute(
    Module module
)
    : Attribute
{
    private readonly Module module = module;

    public Module GetModule() => module;
}
