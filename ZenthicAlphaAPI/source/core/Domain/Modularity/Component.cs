namespace Domain.Modularity;

public enum Component
{
    [Module(Module.Identity)] Users = 0,
    [Module(Module.Identity)] Roles = 1
}