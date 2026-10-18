using CinemaHikes.Application;
using CinemaHikes.Infrastructure;
using CinemaHikes.Infrastructure.Persistence.Extensions;
using CinemaHikes.WebApi.Extensions;
using CinemaHikes.WebApi.Middlewares;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddWebApi(builder.Configuration);
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();


await app.Services.InitializeDatabaseAsync(app.Lifetime.ApplicationStopping);
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseWebApi();

app.Run();