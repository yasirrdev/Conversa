using Conversa.Models.Databases;
using Conversa.Models.Databases.Repository;
using Conversa.Models.Interfaces;
using Conversa.Models.Mapper;
using Conversa.Repositories;
using Conversa.Seeders;
using Conversa.Services;
using Conversa.Websockets;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Conversa;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddScoped<DataContext>();
        builder.Services.AddScoped<UserMapper>();
        builder.Services.AddScoped<DatabaseSeeder>();

        builder.Services.AddScoped<IPasswordHasher, PasswordService>();
        builder.Services.AddScoped<IUserRepository, UserRepository>();
        builder.Services.AddScoped<IContactRepository, ContactsRepository>();
        builder.Services.AddScoped<IGroupRepository, GroupRepository>();
        builder.Services.AddScoped<MessagesRepository>();
        builder.Services.AddScoped<IGroupService, GroupService>();

        builder.Services.AddSingleton<websocketHandler>();
        builder.Services.AddSingleton<ChatHandler>();

        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            string key = Environment.GetEnvironmentVariable("JWT_KEY");
            if (string.IsNullOrEmpty(key))
            {
                throw new Exception("JWT_KEY variable de entorno no está configurada.");
            }

            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                ValidateIssuer = false,
                ValidateAudience = false
            };
        });

        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowFrontend", policy =>
            {
                policy.AllowAnyOrigin()
                      .AllowAnyMethod()
                      .AllowAnyHeader();
            });
        });

        var app = builder.Build();

        using (var scope = app.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<DataContext>();
            dbContext.Database.EnsureCreated();
            var seeder = scope.ServiceProvider.GetRequiredService<DatabaseSeeder>();
            seeder.SeedAsync();
        }

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseRouting();

        app.UseCors("AllowFrontend");
        //app.UseMiddleware<middleware>();
        app.UseAuthentication();
        app.UseAuthorization();

        app.UseWebSockets();

        app.UseStaticFiles();

        app.MapControllers();

        app.Run();
    }
}