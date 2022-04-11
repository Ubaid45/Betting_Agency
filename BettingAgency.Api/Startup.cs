using System.IdentityModel.Tokens.Jwt;
using System.Net.Mime;
using System.Text;
using BettingAgency.Application.Abstraction.IServices;
using BettingAgency.Application.Abstraction.Models.JWT;
using BettingAgency.Application.Common;
using BettingAgency.Application.Middleware;
using BettingAgency.Application.Services;
using BettingAgency.Persistence;
using BettingAgency.Persistence.Abstraction.Entities;
using BettingAgency.Persistence.Abstraction.Interfaces;
using BettingAgency.Persistence.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace BettingAgency;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddOptions<JsonWebTokenKeys>()
            .Bind(Configuration.GetSection(nameof(JsonWebTokenKeys)))
            .ValidateDataAnnotations();
        services.AddAutoMapper(typeof(AutoMapperProfile));
        services.AddScoped<IGameService, GameService>();
        services.AddScoped<ITokenService, TokenService>();

        services.AddDbContext<ApiContext>(opt => opt.UseInMemoryDatabase("BettingAgency"));
        services.AddScoped<IApiContext, ApiContext>();
        services.AddScoped<IGameRepository, GameRepository>();
        services.AddHttpClient("JwtTokenClient",
            c => { c.DefaultRequestHeaders.Add("Accept", MediaTypeNames.Application.Json); });

        services.AddScoped<JwtSecurityTokenHandler>();
        services.AddControllers();
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)    
            .AddJwtBearer(options =>    
            {    
                options.TokenValidationParameters = new TokenValidationParameters    
                {    
                    ValidateIssuer = true,    
                    ValidateAudience = true,    
                    ValidateLifetime = true,    
                    ValidateIssuerSigningKey = true,    
                    ValidIssuer = Configuration["JsonWebTokenKeys:ValidIssuer"],    
                    ValidAudience = Configuration["JsonWebTokenKeys:ValidAudience"],    
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JsonWebTokenKeys:Secret"]))    
                };    
            });
        services.AddSwaggerGen();
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "DU.User.Api v1"));
        }

        var scope = app.ApplicationServices.CreateScope();
        var context = scope.ServiceProvider.GetService<IApiContext>();
        SeedData(context);

        app.UseHttpsRedirection();

        app.UseRouting();

        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }


        app.UseMiddleware<ExceptionMiddleware>();
        app.UseAuthentication();
        app.UseAuthorization();

        app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
    }

    private static void SeedData(IApiContext? context)
    {
        IEnumerable<UserEntity?> userList = new List<UserEntity?>
        {
            new()
            {
                Email = "adminakp@gmail.com",
                UserName = "admin",
                Password = "admin",
                FullName = "Administrator",
                Balance = 10000,
                Timestamp = DateTime.Now
            },
            new()
            {
                Email = "adminakp@gmail.com",
                UserName = "User1",
                Password = "Admin",
                FullName = "User 1",
                Balance = 10000,
                Timestamp = DateTime.Now
            }
        };
        context.Users.AddRange(userList);

        context.SaveChanges();
    }
}