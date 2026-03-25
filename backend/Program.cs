using backend.Extensions;
using backend.Helpers;
using backend.Hubs;

var builder = WebApplication.CreateBuilder(args);

ConfigProvider.Initialize(builder.Configuration);

builder.Services
    .AddDatabase(builder.Configuration)
    .AddRepositories()
    .AddServices()
    .AddJwtAuthentication()
    .AddCorsPolicy()
    .AddSwaggerDocumentation();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSignalR();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAngular");
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapHub<ChatHub>("/hubs/chat");

await app.SeedDefaultUsers();

app.Run();
