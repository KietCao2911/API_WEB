using API_DSCS2_WEBBANGIAY.Hubs;
using API_DSCS2_WEBBANGIAY.Models;
using API_DSCS2_WEBBANGIAY.Utils.Mail;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API_DSCS2_WEBBANGIAY
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
            services.AddCors(policy =>
            {
                policy.AddPolicy(name: "MyAllowSpecificOrigins", policy =>
                 {
                     policy.AllowAnyHeader().AllowAnyOrigin().AllowAnyMethod();
  
                 });
            });
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options => {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = Configuration["Jwt:Issuer"],
                    ValidAudience = Configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"]))
                };
            });
            services.AddDistributedMemoryCache();
            services.AddSession(options =>
            {
                options.Cookie.Name = ".AdventureWorks.Session";
                options.IdleTimeout = TimeSpan.FromDays(1);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });
            services.AddControllers().AddNewtonsoftJson(options =>
    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
).AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
                options.JsonSerializerOptions.PropertyNamingPolicy = null;
            });

            services.AddTransient<ShoesEcommereContext>();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "API_DSCS2_WEBBANGIAY", Version = "v1" });
            });
            services.AddTransient<IMailService, MailService>();
            services.Configure<MailSettings>(Configuration.GetSection("MailSettings"));
            services.AddSignalR();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "API_DSCS2_WEBBANGIAY v1"));
            }
           
            app.UseHttpsRedirection();
           
            app.UseRouting();
            app.UseFileServer(new FileServerOptions
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot")),
                RequestPath = "/wwwroot"
            });
            app.UseSession();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseStaticFiles();
           
            app.UseCors("MyAllowSpecificOrigins");
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
          name: "areas",
          pattern: "{area:exists}/{controller=HomeAdmin}/{action=Index}/{id?}"
        );
                endpoints.MapControllers();
                endpoints.MapHub<RoomMessageHub>("/ChatRoom");

            });
        }
    }
}
