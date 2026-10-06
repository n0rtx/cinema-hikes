using CinemaHikes.Application;
using CinemaHikes.Infrastructure;
using CinemaHikes.Infrastructure.Persistence.Extensions;
using CinemaHikes.WebApi.Extensions;
using CinemaHikes.WebApi.Middlewares;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddWebApi(builder.Configuration);
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();

var app = builder.Build();

await app.Services.InitializeDatabaseAsync(app.Lifetime.ApplicationStopping);

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseWebApi();

app.Run();