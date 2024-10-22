using DotNetEnv;
using Infrastructure;
using ZenthicAlpha;

Env.LoadMulti([
    ".env",
    ".env.local",
    ".env.stagging",
    ".env.production"
]);

await WebApplication.CreateBuilder(args)

.AddInfrastructure()
.AddPresentation()
.AddModules()
.Build()

.UseModules()
.UsePresentation()
.RunAsync();