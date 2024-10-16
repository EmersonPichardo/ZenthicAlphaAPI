using Microsoft.AspNetCore.Builder;

namespace Infrastructure.Modularity;

public interface IModuleInstaller
{
    public void AddInfrastructure(WebApplicationBuilder builder);
    public void UseInfrastructure(WebApplication app);
}
