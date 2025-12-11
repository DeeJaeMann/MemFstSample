using BooksAPI.Endpoints;

var config = new ConfigurationBuilder()
    // Non-sensitive settings
    .AddJsonFile("appsettings.json")
    // Connection string stored in secrets
    .AddUserSecrets<Program>()
    .Build();

string? connString = config.GetConnectionString("DefaultConnection");

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapUsersEndpoints(connString);
app.MapBooksEndpoints(connString);
app.MapUserBooksEndpoints(connString);

app.Run();





