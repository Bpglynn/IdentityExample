using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity.UI.Services;
using IdentityExample.Services;
using IdentityExample.Data;
using IdentityExample.Models;

namespace IdentityExample
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
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddDbContext<ApplicationDbContext>(options => options.UseMySql(Configuration["DBInfo:ConnectionString"]));

            services.AddDefaultIdentity<ApplicationUser>(config =>
            {
                //config.SignIn.RequireConfirmedEmail = true;
                config.User.RequireUniqueEmail = true;
            })
                .AddRoles<IdentityRole>()
                .AddDefaultUI(UIFramework.Bootstrap4)
                .AddDefaultTokenProviders()
                .AddEntityFrameworkStores<ApplicationDbContext>();

            services.Configure<IdentityOptions>(options =>
            {
                // Password settings
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 8;
                options.Password.RequiredUniqueChars = 1;
                // Lockout settings
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 3;
                options.Lockout.AllowedForNewUsers = true;
                // User settings
                options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                options.User.RequireUniqueEmail = true;
            });

            services.ConfigureApplicationCookie(options =>
            {
                // Cookie settings
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(10);
                options.LoginPath = "/Identity/Account/Login";
                options.AccessDeniedPath = "/Identity/Account/AccessDenied";
                options.SlidingExpiration = true;
            });

            //E-mail configuration
            services.AddTransient<IEmailSender, EmailSender>();
            services.Configure<AuthMessageSenderOptions>(Configuration.GetSection("EmailSettings"));

            //External authentication configuration
            services.AddAuthentication().AddMicrosoftAccount(microsoftOptions =>
                {
                    microsoftOptions.ClientId = Configuration["ExternalLogin:Microsoft:AppId"];
                    microsoftOptions.ClientSecret = Configuration["ExternalLogin:Microsoft:AppSecret"];
             //   })
             //   .AddFacebook(facebookOptions =>
             //   {
             //       facebookOptions.AppId = Configuration["ExternalLogin:Facebook:AppId"];
             //       facebookOptions.AppSecret = Configuration["ExternalLogin:Facebook:AppId"];
                });

            services.AddMvc(config =>
            {
                // Set default policy to require authentication
                // Use [AllowAnonymous] attribute to override this behavior where desired
                var policy = new AuthorizationPolicyBuilder()
                                .RequireAuthenticatedUser()
                                .Build();
                config.Filters.Add(new AuthorizeFilter(policy));
            })
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddAuthorization(options =>
            {
                options.AddPolicy("PolicyAdmin", policy => policy.RequireRole("Admin"));
                options.AddPolicy("PolicyEditContent", policy => policy.RequireRole("Admin", "Editor"));
                options.AddPolicy("PolicyFriendsFamily", policy => policy.RequireRole("Friends", "Family"));
                options.AddPolicy("PolicyFamily", policy => policy.RequireRole("Family"));
                options.AddPolicy("PolicyeFriends", policy => policy.RequireRole("Friends"));
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IServiceProvider services)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            CreateUserRoles(services).Wait();
        }

        // Create identity roles
        private async Task CreateUserRoles(IServiceProvider serviceProvider)
        {
            var RoleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var UserManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            //Check if roles exist
            var roleCheckAdmin = await RoleManager.RoleExistsAsync("Admin");
            //Create the roles and add them to the database if not found
            if (!roleCheckAdmin)
            {
                await RoleManager.CreateAsync(new IdentityRole("Admin"));
            }
            //Assign Admin role to specified user in appsettings.json
            ApplicationUser userAdmin = await UserManager.FindByEmailAsync(Configuration["UserSetup:Admin"]);
            if (userAdmin != null)
            {
                await UserManager.AddToRoleAsync(userAdmin, "Admin");
            }
        }
    }
}
