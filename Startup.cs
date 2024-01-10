using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ArtistAwards.Services;
using Microsoft.EntityFrameworkCore;
using ArtistAwards.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using ArtistAwards;
using Microsoft.AspNetCore.Identity;
using PollAwards.Services;
using Microsoft.AspNetCore.Http;

namespace DotNetAPI
{
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
      services.AddControllers();
      services.AddHttpContextAccessor();
      //services.AddTransient<ArtistService>();
      services.AddTransient<UserService>();
      services.AddTransient<PollService>();
      services.AddTransient<ConfigService>();

      services.AddDbContext<AppDbContext>(options =>
              options.UseNpgsql(Configuration.GetConnectionString("ArtistContext")));


      services.Configure<CookiePolicyOptions>(options =>
      {
        // This lambda determines whether user consent for non-essential cookies is needed for a given request.
        options.CheckConsentNeeded = context => true;
        options.MinimumSameSitePolicy = SameSiteMode.None;
      });
      //Authentication
      services.AddAuthentication(opt =>
      {
        opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
      })
    .AddJwtBearer(options =>
    {
      options.TokenValidationParameters = new TokenValidationParameters
      {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = "http://localhost:5000",
        ValidAudience = "http://localhost:5000",
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration.GetValue<string>("SecretKey")))
      };

      options.Events = new JwtBearerEvents
      {
        OnMessageReceived = context =>
        {
          context.Token = context.Request.Cookies["accessToken"];
          return Task.CompletedTask;
        }
      };
    });

      services.AddCors(options =>
      {
        options.AddPolicy("EnableCORS", builder =>
        {
          builder
          //.SetIsOriginAllowed(origin => true)
          .WithOrigins("http://localhost:4200", "https://pollarama.davidojes.dev")
             .AllowAnyHeader()
             .AllowAnyMethod()
             .AllowCredentials();
        });
      });

      services.AddControllersWithViews()
        .AddNewtonsoftJson(options => options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }

      app.UseDefaultFiles();
      app.UseStaticFiles();

      app.UseHttpsRedirection();

      app.UseRouting();
      app.UseCors("EnableCORS");
      app.UseAuthentication();
      app.UseAuthorization();
      

      app.UseEndpoints(endpoints =>
      {
        endpoints.MapControllers();
      });
    }
  }
}
