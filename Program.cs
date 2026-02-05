using UserManagementAPI.Middleware;
using UserManagementAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add basic services
builder.Services.AddControllers();

// Add service
builder.Services.AddSingleton<UserService>();

var app = builder.Build();

// Add middleware
app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseMiddleware<RequestLoggingMiddleware>();
app.UseMiddleware<AuthenticationMiddleware>();

app.UseHttpsRedirection();
app.MapControllers();

// Set API key for testing
app.Configuration["ApiKey"] = "valid-token-123";

// Startup message
Console.WriteLine("=====================================");
Console.WriteLine("🚀 User Management API Started!");
Console.WriteLine("📡 API Endpoints:");
Console.WriteLine("   GET    /api/users");
Console.WriteLine("   GET    /api/users/{id}");
Console.WriteLine("   POST   /api/users");
Console.WriteLine("   PUT    /api/users/{id}");
Console.WriteLine("   DELETE /api/users/{id}");
Console.WriteLine("🔐 Test Token: valid-token-123");
Console.WriteLine("=====================================");

app.Run();