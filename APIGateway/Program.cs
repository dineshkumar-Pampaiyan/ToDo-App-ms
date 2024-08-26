
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDaprClient(); //Dapr
builder.Services.AddControllers();

// Add Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "API Gateway", Version = "v1" });
});

// Add AuthenticationService (you can replace with actual implementation)
builder.Services.AddSingleton<IAuthenticationService, AuthenticationService>();

var app = builder.Build();

// Enable Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "API Gateway V1");
        c.RoutePrefix = string.Empty;  // Set Swagger UI to launch at root URL
    });
}

app.UseRouting();
app.UseCloudEvents(); //his middleware enables the handling of cloud events, which is necessary for Dapr pub/sub.
app.MapSubscribeHandler(); //This maps the subscription endpoint required for Dapr to route pub/sub messages to the correct handlers.

// Configure the HTTP request pipeline.

app.UseAuthorization();

app.MapControllers();

app.Run();
