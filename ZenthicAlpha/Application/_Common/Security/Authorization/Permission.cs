namespace Application._Common.Security.Authorization;

[Flags]
public enum Permission
{
    Read = 1,
    Export = 2,
    Add = 4,
    Update = 8,
    Delete = 16
}
