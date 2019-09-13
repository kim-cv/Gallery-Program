using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Gallery.API.Entities;
using Gallery.API.Interfaces;
using Gallery.API.Models;
using Gallery.API.Repositories;
using Gallery.API.Services;

namespace Gallery.API
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
            services.AddControllers().SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

            // Logging
            services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.AddConsole(options => options.IncludeScopes = true);
                loggingBuilder.AddDebug();
            });

            // Auth Configuration
            services.Configure<TokenData>(Configuration.GetSection("tokenManagement"));
            TokenData token = Configuration.GetSection("tokenManagement").Get<TokenData>();
            //byte[] secret = Encoding.ASCII.GetBytes(token.Secret);

            // Configure authentication schema
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(token.Secret)),
                    ValidIssuer = token.Issuer,
                    ValidAudience = token.Audience,
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });

            // Auth
            services.AddScoped<IAuthenticateService, AuthenticationService>();
            services.AddScoped<IUserManagementService, UserManagementService>();

            // Database
            services.AddDbContext<GalleryDBContext>(opt => opt.UseSqlServer(Configuration.GetConnectionString("GalleryDBContext")));
            //services.AddDbContext<GalleryDBContext>(opt => opt.UseInMemoryDatabase("GalleryDBContext"));

            // Users
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUserService, UserService>();

            // Galleries
            services.AddScoped<IGalleryService, GalleryService>();
            services.AddScoped<IGalleryRepository, GalleryRepository>();

            // Images
            services.AddScoped<IImageRepository, ImageRepository>();
            services.AddScoped<IImageService, ImageService>();

            // File System
            services.AddScoped<IFileSystemRepository, FileSystemRepository>();

            // Content Folder Configuration
            services.Configure<ContentFolders>(options => Configuration.GetSection("ContentFolders").Bind(options));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseRouting();

            app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
