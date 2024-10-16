using Infrastructure;
using ZenthicAlpha;

await WebApplication.CreateBuilder(args)

.AddInfrastructure()
.AddPresentation()
.AddModules()
.Build()

.UseModules()
.UsePresentation()
.RunAsync();