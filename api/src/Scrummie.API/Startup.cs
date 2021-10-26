using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

namespace Scrummie.API
{
    public class Startup
    {
        private readonly GraphLogger logger;

        private readonly SampleObserver observer;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Startup"/> class.
        /// </summary>
        /// <param name="configuration">The configuration interface.</param>
        public Startup(IConfiguration configuration)
        {
            this.Configuration = configuration;
            this.logger = new GraphLogger(typeof(Startup).Assembly.GetName().Name);
            this.observer = new SampleObserver(this.logger);
        }

        public IConfiguration Configuration { get; }

        /// <summary>
        /// Gets the configuration.
        /// </summary>
        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Scrummie.API v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app">The application builder.</param>
        /// <param name="env">The hosting environment.</param>
        /// <param name="loggerFactory">The logger of ILogger instance.</param>
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            this.logger.BindToILoggerFactory(loggerFactory);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();

            app.UseMiddleware<CallAffinityMiddleware>();

            // bypass the user-auth middleware for the incoming request of calls.
            app.UseWhen(
                context => !context.Request.Path.StartsWithSegments(new Microsoft.AspNetCore.Http.PathString(HttpRouteConstants.OnIncomingRequestRoute)),
                appBuilder => appBuilder.UseAuthentication());

            app.UseMvc();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Scrummie.API", Version = "v1" });
            });
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="services">The service collection.</param>
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddSingleton(this.observer)
                .AddSingleton<IGraphLogger>(this.logger)
                .AddAuthentication(sharedOptions =>
                {
                    sharedOptions.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddAzureAdBearer(options => this.Configuration.Bind("AzureAd", options));

            services
                .AddBot(options => this.Configuration.Bind("Bot", options))
                .AddMvc();
        }
    }
}