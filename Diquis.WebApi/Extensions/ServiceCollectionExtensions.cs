using System.Text;
using Diquis.Domain.Entities.Common;
using Diquis.Infrastructure.Auth.JWT;
using Diquis.Infrastructure.Images;
using Diquis.Infrastructure.Mailer;
using Diquis.Infrastructure.Mapper;
using Diquis.Infrastructure.Persistence.Contexts;
using Diquis.Infrastructure.Persistence.Extensions;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;


namespace Diquis.WebApi.Extensions
{
    public static class ServiceCollectionExtensions // configure application services
    {

        public static void ConfigureApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            #region [-- CORS --]
            _ = services.AddCors(p => p.AddPolicy("defaultPolicy", builder =>
            {
                _ = builder.WithOrigins("http://localhost:3000", "https://localhost:3000")
                          .AllowAnyMethod()
                          .AllowAnyHeader()
                          .AllowCredentials()
                          .WithExposedHeaders("Content-Disposition");
            }));
            #endregion

            #region [-- ADD CONTROLLERS AND SERVICES --]

            _ = services.AddControllers(opt =>
            {
                AuthorizationPolicy policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
                opt.Filters.Add(new AuthorizeFilter(policy)); // makes so that all the controllers require authorization by default

            }).AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
            });

            // Modern FluentValidation registration
            _ = services.AddFluentValidationAutoValidation()
                       .AddFluentValidationClientsideAdapters();

            // Register validators from assemblies
            _ = services.AddValidatorsFromAssemblyContaining<Application.Utility.IRequestValidator>();
            _ = services.AddValidatorsFromAssemblyContaining<Infrastructure.Utility.IRequestValidator>();
            _ = services.AddEndpointsApiExplorer();
            _ = services.AddAutoMapper(config =>
            {
                config.AddProfile<MappingProfiles>();
            });
            _ = services.Configure<MailSettings>(configuration.GetSection("MailSettings"));
            _ = services.Configure<CloudinarySettings>(configuration.GetSection("Cloudinary"));
            //_ = services.AddSingleton(new BlobServiceClient(configuration.GetConnectionString("AzureBlobStorage"))); // -- uncomment and add your connection string for file storage


            _ = services.AddServices(); // dynamic services registration

            //----------- Add Services (Dependency Injection) -------------------------------------------

            // From DynamicServiceRegistrationExtensions
            // Auto registers scoped/transient marked services 

            // ICurrentTenantUserService -- registered as Scoped (resolve the tenant/user from token/header)
            // IIdentityService, ITokenService, IRepositoryAsync, ITenantManagementService -- registered as Scoped

            // Any additional app services should be registered as Scoped

            //---------------------------------------------------------------------------

            #endregion

            #region [-- REGISTERING DB CONTEXT SERVICE --]
            _ = services.AddDbContext<TenantDbContext>(options => options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));
            _ = services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));
            _ = services.AddDbContext<BaseDbContext>(options => options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));
            _ = services.AddAndMigrateTenantDatabases<TenantDbContext, BaseDbContext, ApplicationDbContext>(configuration);
            #endregion

            #region [-- SETTING UP IDENTITY CONFIGURATIONS --]

            _ = services.AddIdentity<ApplicationUser, IdentityRole>(o =>
            {
                o.SignIn.RequireConfirmedAccount = false; // password requirements - set as needed
                o.Password.RequiredLength = 6;
                o.Password.RequireDigit = false;
                o.Password.RequireLowercase = false;
                o.Password.RequireUppercase = false;
                o.Password.RequireNonAlphanumeric = false;
            }).AddEntityFrameworkStores<BaseDbContext>()
             .AddDefaultTokenProviders();


            #endregion

            #region [-- JWT SETTINGS --]

            _ = services.Configure<JWTSettings>(configuration.GetSection("JWTSettings")); // get settings from appsettings.json
            _ = services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
               .AddJwtBearer(o =>
               {
                   o.RequireHttpsMetadata = false;
                   o.SaveToken = false;
                   o.TokenValidationParameters = new TokenValidationParameters
                   {
                       ValidateIssuerSigningKey = true,
                       ValidateIssuer = true,
                       ValidateAudience = true,
                       ValidateLifetime = true,
                       ClockSkew = TimeSpan.Zero,
                       ValidIssuer = configuration["JWTSettings:Issuer"],
                       ValidAudience = configuration["JWTSettings:Audience"],
                       IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWTSettings:Key"]))
                   };
                   o.Events = new JwtBearerEvents()
                   {
                       OnMessageReceived = context =>
                       {
                           // Allow SignalR to read the token from query string
                           var accessToken = context.Request.Query["access_token"];
                           var path = context.HttpContext.Request.Path;
                           
                           if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs"))
                           {
                               context.Token = accessToken;
                           }
                           
                           return Task.CompletedTask;
                       },
                       OnChallenge = context =>
                       {
                           context.HandleResponse();

                           context.Response.ContentType = "application/json";
                           context.Response.StatusCode = 401;

                           return context.Response.WriteAsync("Not Authorized");
                       },
                       OnForbidden = context =>
                       {
                           context.Response.StatusCode = 403;
                           context.Response.ContentType = "application/json";

                           return context.Response.WriteAsync("Not Authorized");
                       },
                   };
               });

            #endregion

        }
    }
}
