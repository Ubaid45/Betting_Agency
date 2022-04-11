using BettingAgency.Application.Abstraction.IServices;
using BettingAgency.Application.Abstraction.Models.JWT;
using BettingAgency.Application.Extensions;
using BettingAgency.Application.Middleware;
using BettingAgency.Application.Services;
using BettingAgency.Persistence;
using BettingAgency.Persistence.Abstraction.Entities;
using BettingAgency.Persistence.Abstraction.Interfaces;
using BettingAgency.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
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
        services.AddJwtTokenServices(Configuration);
        services.AddOptions<JwtSettings>()
            .Bind(Configuration.GetSection(nameof(JwtSettings)))
            .ValidateDataAnnotations();

        services.AddScoped<IGameService, GameService>();
//builder.Services.AddTransient<ITokenService, TokenService>();

        services.AddDbContext<ApiContext>(opt => opt.UseInMemoryDatabase("BettingAgency"));
        services.AddScoped<IApiContext, ApiContext>();
        services.AddScoped<IGameRepository, GameRepository>();

        services.AddControllers();
        services.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "JWT Authorization header using the Bearer scheme."
            });
            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    new string[] { }
                }
            });
        });
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
        AddTestData(context);

        app.UseHttpsRedirection();

        app.UseRouting();

        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }


        app.UseMiddleware<ExceptionMiddleware>();
        //app.UseAuthentication();
        app.UseAuthorization();

        app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
    }

    private static void AddTestData(IApiContext? context)
    {
        IEnumerable<UserEntity> userList = new List<UserEntity>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Email = "adminakp@gmail.com",
                UserName = "Admin",
                Password = "Admin",
                FullName = "Administrator",
                Balance = 10000
            },
            new()
            {
                Id = Guid.NewGuid(),
                Email = "adminakp@gmail.com",
                UserName = "User1",
                Password = "Admin",
                FullName = "User 1",
                Balance = 10000
            }
        };
        context.Users.AddRange(userList);
        context.SaveChanges();

        context.SaveChanges();
    }
}