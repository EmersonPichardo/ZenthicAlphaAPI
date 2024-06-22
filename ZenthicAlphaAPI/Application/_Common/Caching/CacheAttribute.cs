using Domain._Common.Modularity;

namespace Application._Common.Caching;

/// <summary>
/// Specifies the class cache behavior.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="CacheAttribute"/> class with a specific tag.
/// </remarks>
/// <param name="module">Specifies the module use as caching tag.</param>
[AttributeUsage(AttributeTargets.Class, Inherited = true)]
public class CacheAttribute(
    Component module
)
    : Attribute
{
    private readonly string tag = module.ToString();

    public string GetTag()
        => tag;
}