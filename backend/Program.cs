using backend.Extensions;
using backend.Helpers;

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

app.Run();
