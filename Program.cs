using AuthTest.Services;
using Microsoft.IdentityModel.Logging;
using AuthTest.Middlewares;
using AuthTest.Extensions.JwtBearer;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

IdentityModelEventSource.ShowPII = true;

// Add services to the container.
builder.Services.AddJwtBearerAuth(configuration);

builder.Services.AddAuthorization();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddTransient<JwtMiddleware>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<JwtMiddleware>();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
