using Bunnymail;
using Bunnymail.Configuration;
using Bunnymail.Messaging;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Configuration.AddEnvironmentVariables();

builder.Services.AddHealthChecks();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthentication("Basic")
    .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("Basic", null);
builder.Services.AddAuthorization();

builder.Services.AddDbContextFactory<ConfigDbContext>(sp => ConfigDbContextFactory.Initialise(builder.Configuration));

//Make the database
ConfigDbContextFactory.Instance.CreateDbContext().Database.Migrate();

RabbitMQClient rabbitMqClient = new(builder.Configuration);
rabbitMqClient.RegisterConsumer(new SendGridMessenger(builder.Configuration));

var app = builder.Build();

app.UseHealthChecks("/healthz");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || !string.IsNullOrWhiteSpace(app.Configuration["swagger"]))
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
