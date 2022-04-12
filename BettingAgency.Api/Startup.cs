using System.IdentityModel.Tokens.Jwt;
using System.Net.Mime;
using System.Text;
using BettingAgency.Application.Abstraction.IServices;
using BettingAgency.Application.Abstraction.Models;
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
       
        services.AddSwaggerGen(setup =>
        {
            // Include 'SecurityScheme' to use JWT Authentication
            var jwtSecurityScheme = new OpenApiSecurityScheme
            {
                Scheme = "bearer",
                BearerFormat = "JWT",
                Name = "JWT Authentication",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Description = "Put **_ONLY_** your JWT Bearer token on textbox below!",

                Reference = new OpenApiReference
                {
                    Id = JwtBearerDefaults.AuthenticationScheme,
                    Type = ReferenceType.SecurityScheme
                }
            };

            setup.AddSecurityDefinition(jwtSecurityScheme.Reference.Id, jwtSecurityScheme);

            setup.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                { jwtSecurityScheme, Array.Empty<string>() }
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
                Email = "ubaid@gmail.com",
                UserName = "ubaid45",
                Password = "Ubaid",
                FullName = "Ubaid Rana",
                Balance = 10000,
                Timestamp = DateTime.Now
            },
            new()
            {
                Email = "mario@gmail.com",
                UserName = "mario12",
                Password = "Mario",
                FullName = "Mario F.",
                Balance = 10000,
                Timestamp = DateTime.Now
            }
            ,
            new()
            {
                Email = "alex@gmail.com",
                UserName = "alex34",
                Password = "Alex",
                FullName = "Alex P.",
                Balance = 10000,
                Timestamp = DateTime.Now
            }
        };
        context.Users.AddRange(userList);

        context.SaveChanges();
    }
}