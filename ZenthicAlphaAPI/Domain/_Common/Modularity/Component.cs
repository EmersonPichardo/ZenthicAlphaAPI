namespace Domain._Common.Modularity;

public enum Component
{
    [Module(Module.Security)] Users = 0,
    [Module(Module.Security)] Roles = 1
}