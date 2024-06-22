using Infrastructure;
using Presentation;

var builder = WebApplication.CreateBuilder(args);
builder.AddInfrastructure();
builder.AddPresentation();

var app = builder.Build();
app.UseInfrastructure();
app.UsePresentation();
app.Run();
