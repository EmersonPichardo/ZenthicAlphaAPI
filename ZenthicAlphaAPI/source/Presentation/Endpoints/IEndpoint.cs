using Domain.Modularity;
using System.Net;

namespace Presentation.Endpoints;

public interface IEndpoint
{
    public Component Component { get; init; }
    public HttpVerbose Verbose { get; init; }
    public string Route { get; init; }
    public HttpStatusCode SuccessStatusCode { get; init; }
    public Type? SuccessType { get; init; }
    public Delegate Handler { get; init; }
}
