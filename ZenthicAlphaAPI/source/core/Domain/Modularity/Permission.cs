namespace Domain.Modularity;

[Flags]
public enum Permission
{
    None = 0,
    Read = 1,
    Export = 2,
    Add = 4,
    Update = 8,
    Delete = 16
}
