using App;
using Infrastructure;

var builder = WebApplication.CreateBuilder(args);
builder.AddInfrastructure();
builder.AddPresentation();
builder.AddModules();

var app = builder.Build();
app.UseModules();
app.UsePresentation();
app.Run();
