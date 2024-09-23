using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Infrastructure.Modularity;

public interface IInfrastructureInstaller
{
    public void AddInfrastructure(IServiceCollection services, IConfiguration configuration);
    public void UseInfrastructure(IHost host);
}
