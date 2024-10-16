using Domain.Modularity;
using System.Net;

namespace Presentation.Endpoints;

public interface IEndpoint
{
    public Component Component { get; init; }
    public HttpVerbose Verbose { get; init; }
    public IReadOnlyCollection<string> Routes { get; init; }
    public HttpStatusCode SuccessStatusCode { get; init; }
    public IReadOnlyCollection<Type> SuccessTypes { get; init; }
    public Delegate Handler { get; init; }
}
