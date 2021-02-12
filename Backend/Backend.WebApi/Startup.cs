using AutoMapper;
using Backend.Contract.Security;
using Backend.Core.Validation;
using Backend.DAL.Models;
using Backend.DAL.QueryHandlers;
using Backend.DAL.Security;
using Backend.WebApi.Util.Security;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Backend.WebApi
{
  public class Startup
  {
    public IConfiguration Configuration { get; }

    public Startup(IConfiguration configuration)
    {
      Configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
      services.AddDbContext<FrontedContext>(options => options.UseSqlServer(Configuration.GetConnectionString("FrontEd")));

      services.AddControllers()
              //.AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<Contract.DTOs.MjestoValidator>())
              .AddJsonOptions(configure => configure.JsonSerializerOptions.PropertyNamingPolicy = null);

      services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
      services.AddValidatorsFromAssemblyContaining(typeof(Backend.CommandValidators.AddTownValidator));

      services.AddSwaggerGen(c =>
      {
        //c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
        var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        c.IncludeXmlComments(xmlPath);

        c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
          Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
          Name = "Authorization",
          In = ParameterLocation.Header,
          Type = SecuritySchemeType.ApiKey
        });
      });

      services.AddMediatR(typeof(TownsQueryHandler));

      SetupAuth(services);

      Action<IServiceProvider, IMapperConfigurationExpression> mapperConfigAction = (serviceProvider, cfg) =>
      {
        cfg.ConstructServicesUsing(serviceProvider.GetService);        
      };
      services.AddAutoMapper(mapperConfigAction, typeof(Mappings.MappingProfile), typeof(DAL.Mappings.EFMappingProfile)); //assemblies containing mapping profiles            
    }
  
    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
      #region Used for nginx + Kestrel
      app.UseForwardedHeaders(new ForwardedHeadersOptions
      {
        ForwardedHeaders = ForwardedHeaders.XForwardedFor |
                           ForwardedHeaders.XForwardedProto
      });
      string pathBase = Configuration["PathBase"];
      if (!string.IsNullOrWhiteSpace(pathBase))
      {
        app.UsePathBase(pathBase);
      }
      #endregion

      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }

      app.UseSwagger();
      app.UseSwaggerUI(c =>
      {              
        c.RoutePrefix = "docs";
        c.DocumentTitle = "FrontEd Workshop Demo WebAPI";
      });
   
      
      app.UseRouting();

      // https://docs.microsoft.com/en-us/aspnet/core/fundamentals/middleware/?view=aspnetcore-3.1#middleware-order
      app.UseCors(builder =>
      {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader()
               .WithExposedHeaders("Token-Expired");
      });

      app.UseAuthentication();
      app.UseAuthorization();

      app.UseEndpoints(endpoints =>
      {
        endpoints.MapControllers();
      });
    }

    private void SetupAuth(IServiceCollection services)
    {
      services.AddTransient<IPasswordHasher<string>, PasswordHasher<string>>();
      services.AddTransient<IUserManagementService, UserManagementService>();
      services.AddTransient<ITokenUtil, TokenUtil>();

      var tokenSection = Configuration.GetSection("TokenConfiguration");
      services.Configure<TokenConfig>(tokenSection);
      var token = tokenSection.Get<TokenConfig>();
      var secret = Encoding.Default.GetBytes(token.Secret);

      services.AddAuthentication(opt =>
              {
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
              })
              .AddJwtBearer(opt =>
              {
                opt.SaveToken = true;
                opt.TokenValidationParameters = new TokenValidationParameters
                {
                  ValidateIssuer = true,
                  ValidateAudience = true,
                  ValidateLifetime = true,
                  ValidateIssuerSigningKey = true,

                  IssuerSigningKey = new SymmetricSecurityKey(secret),
                  ValidIssuer = token.Issuer,
                  ValidAudience = token.Audience,
                };
                opt.Events = new JwtBearerEvents
                {
                  OnAuthenticationFailed = context =>
                  {
                    if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                    {
                      context.Response.Headers.Add("Token-Expired", "true");
                    }
                    return Task.CompletedTask;
                  }
                };
              });

      services.AddAuthorization(options =>
      {
        foreach (var policy in Policies.All)
        {
          options.AddPolicy(policy.Key, policy.Value);
        }
      });
    }

  }
}
