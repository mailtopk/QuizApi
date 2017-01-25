using System;
using System.Net;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using TopicRepositoryLib;
using DataEntity;
using QuizDataAccess;
using Microsoft.Extensions.Caching.Distributed;
using QuizCaching;

namespace QuizSvc
{
    public class AppStart
    {
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddMvc(options => options.RespectBrowserAcceptHeader = true);

            // Swagger
            services.AddSwaggerGen(
                option => new Swashbuckle.Swagger.Model.Info
                {
                    Version = "v1",
                    Title = "Quiz web Api",
                    Description = "Build Quiz web services",
                    TermsOfService = "None"
                }
            );

            // Redis
            // Redis can not use host name - this is workaround
            // https://github.com/StackExchange/StackExchange.Redis/issues/410
            services.AddDistributedRedisCache(
                options => options.Configuration = GetRedisContainerIPAddress());
            
            // Data Access Layer
            services.AddTransient<IQuizDataAccess<Topic>>(p => new QuizDataAccess<Topic>());

            var serviceProvider = services.BuildServiceProvider();
            
            // Cache
            services.AddTransient<IQuizCache<Topic>>( 
                p => new QuizCache<Topic>(serviceProvider.GetService<IDistributedCache>()) );

            serviceProvider = services.BuildServiceProvider(); // TODO - why do i need to call this again ?
            // Topic Repository
            services.AddTransient<ITopicRepository>(p =>
               new TopicRepository(
                       serviceProvider.GetService<IQuizDataAccess<Topic>>(),
                       serviceProvider.GetService<IQuizCache<Topic>>() ));

        }

        private string GetRedisContainerIPAddress()
        {
            try
            {
                // TODO - fix the blocking call
                var iphostEntry = Dns.GetHostEntryAsync("cachingservice").GetAwaiter().GetResult().AddressList;
                Console.WriteLine($"[DEBUG] : IP Address of redis : {iphostEntry.FirstOrDefault()}");
                return iphostEntry.FirstOrDefault().ToString();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] : Connecting to redis {ex.Message}");
            }
            return string.Empty;
        }
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseSwagger();
            app.UseSwaggerUi();
            app.UseMvcWithDefaultRoute();

        }
    }
}