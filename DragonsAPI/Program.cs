using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

class Program
{
    static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Services
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        // Get JWT values from appsettings.json
        IConfiguration jwtConfig = Configuration.GetConfig().GetSection("JWT");

        // Custom parameters for JWT
        TokenValidationParameters tokenValidationParameters = new TokenValidationParameters
        {
            // This applications is the only one generating & validating JWT's
            // So asymetric keys are unnecessary
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig["Key"] ?? throw new Exception("Invalid appsettings.json"))),
            ValidateLifetime = true,
            ValidateAudience = false,
            ValidateIssuer = false,
        };

        // AddJwtBearer takes an Action, which allows changing options in AddJwtBearer
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(
            options => options.TokenValidationParameters = tokenValidationParameters
        );

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}
