var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDaprClient(); //Dapr
builder.Services.AddControllers();

var app = builder.Build();

app.UseCloudEvents();
app.MapSubscribeHandler();  // Enables pub/sub with Dapr

// Configure the HTTP request pipeline.

app.UseAuthorization();

app.MapControllers();

app.Run();
