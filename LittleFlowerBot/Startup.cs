using System.Linq;
using LittleFlowerBot.DbContexts;
using LittleFlowerBot.Models.Caches;
using LittleFlowerBot.Models.Game;
using LittleFlowerBot.Models.Game.BoardGame.ChessGames.ChineseChess;
using LittleFlowerBot.Models.Game.BoardGame.KiGames.Gomoku;
using LittleFlowerBot.Models.Game.BoardGame.KiGames.TicTacToe;
using LittleFlowerBot.Models.Game.GuessNumber;
using LittleFlowerBot.Models.Message;
using LittleFlowerBot.Models.Renderer;
using LittleFlowerBot.Repositories;
using LittleFlowerBot.Services.EventHandler;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace LittleFlowerBot
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            WebHostEnvironment = env;
            Configuration = configuration;
        }

        public IWebHostEnvironment WebHostEnvironment;
        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<LittleFlowerBotContext>(options =>
            {
                options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection"));
            });

            services.AddDistributedRedisCache(options =>
            {
                options.InstanceName = "LittleFlowerBot";
                options.Configuration = Configuration["Redis:Host"];
            });
            
            services.AddMemoryCache();
            services.AddHttpClient();
            services.AddControllers();

            services.AddSingleton<RegistrationCache>();
            services.AddSingleton<GameStateCache>();
            services.AddScoped<GameFactory>();

            services.AddScoped<IBoardGameResultsRepository, BoardGameResultsRepository>();
            services.AddScoped<ISubscriptionRepository, SubscriptionRepository>();
            
            services.AddScoped<TicTacToeGame>();
            services.AddScoped<GomokuGame>();
            services.AddScoped<ChineseChessGame>();
            services.AddScoped<GuessNumberGame>();

            services.AddScoped<IEventHandler, GameHandler>();
            services.AddScoped<IEventHandler, RecordHandler>();
            services.AddScoped<GameHandler, GameHandler>();
            services.AddScoped<IEventHandler, RegistrationHandler>();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

            services.AddHealthChecks()
                .AddNpgSql(Configuration["ConnectionStrings:DefaultConnection"], name: "PostgreSQL")
                .AddRedis(Configuration["Redis:Host"], name: "Redis");
            
            
            if (WebHostEnvironment.IsDevelopment())
            {
                services.AddScoped<ITextRenderer, ConsoleRenderer>();
                services.AddScoped<ILineNotify, ConsoleRenderer>();
                services.AddScoped<IMessage, ConsoleRenderer>();
            }
            else
            {
                services.AddScoped<ITextRenderer, LineNotify>();
                services.AddScoped<ILineNotify, LineNotify>();
                services.AddScoped<IMessage, LineMessage>();
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
            
            app.UseHealthChecks("/health", new HealthCheckOptions
            {
                Predicate = _ => true,
                ResponseWriter = async (context, report) =>
                {
                    var allHealthReport = report.Entries.Aggregate("",
                        (healthReport, pair) => healthReport + $"{pair.Key}: {pair.Value.Status.ToString()}\n");
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsync(allHealthReport);
                }
            });
            
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
            ;
            
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetRequiredService<LittleFlowerBotContext>();

                context.Database.Migrate();
            }
        }
    }
}