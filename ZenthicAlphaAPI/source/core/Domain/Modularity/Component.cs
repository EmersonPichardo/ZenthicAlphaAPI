namespace Domain.Modularity;

public enum Component
{
    [Module(Module.Identity)] Users = 1,
    [Module(Module.Identity)] Roles = 2
}