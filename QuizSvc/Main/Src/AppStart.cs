using System;
using System.Net;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using DataEntity;
using QuizDataAccess;
using Microsoft.Extensions.Caching.Distributed;
using QuizCaching;
using QuizRepository;
using Swashbuckle.Swagger.Model;
using QuizSwagger;
using QuizManager;
using Microsoft.Extensions.PlatformAbstractions;
using System.IO;

namespace QuizSvc
{
    public class AppStart
    {
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddMvc(options => options.RespectBrowserAcceptHeader = true);

            
            services.AddSwaggerGen ( options =>  options.SingleApiVersion(new Info
                {
                    Version = "v1",
                    Title = "Flash Card/Quiz Web API",
                    TermsOfService = "Contact ppkumar.email@gmail.com",

                }));
            var basePath = PlatformServices.Default.Application.ApplicationBasePath;
            var xmlFile = Path.Combine(basePath, "Main.xml");
            services.ConfigureSwaggerGen( swaggerConfig => { 
                swaggerConfig.IncludeXmlComments(xmlFile);
                swaggerConfig.OperationFilter<QuizSwaggerFilter>();
                });


            // Redis
            // Redis can not use host name - this is workaround
            // https://github.com/StackExchange/StackExchange.Redis/issues/410
            services.AddDistributedRedisCache(
                options => options.Configuration = GetRedisContainerIPAddress());
            
            // Data Access Layer
            services.AddTransient<IQuizDataAccess<Topic>>(t => new QuizDataAccess<Topic>());
            services.AddTransient<IQuizDataAccess<DataEntity.Question>>(q => new QuizDataAccess<DataEntity.Question>());
            services.AddTransient<IQuizDataAccess<DataEntity.Answer>>( a  => new QuizDataAccess<DataEntity.Answer>());
            
            CacheingDI(services);
            RepositorysDI(services);
        }

        private void CacheingDI(IServiceCollection services)
        {
            var serviceProvider = services.BuildServiceProvider();
            
            // Topic Cache
            services.AddTransient<IQuizCache<Topic>>( 
                tc => new QuizCache<Topic>(serviceProvider.GetService<IDistributedCache>()));
            // Question Cache
            services.AddTransient<IQuizCache<DataEntity.Question>>(
                pc => new QuizCache<DataEntity.Question>(serviceProvider.GetService<IDistributedCache>()));
            // Answer Cache
            services.AddTransient<IQuizCache<DataEntity.Answer>>(
                ac => new QuizCache<DataEntity.Answer>(serviceProvider.GetService<IDistributedCache>()));
        }

        private void RepositorysDI(IServiceCollection services)
        {
            var serviceProvider = services.BuildServiceProvider(); // TODO - why do i need to call this again ?

            // Topic Repository
            services.AddTransient<ITopicRepository>( tr =>
               new TopicRepository(
                       serviceProvider.GetService<IQuizDataAccess<Topic>>(),
                       serviceProvider.GetService<IQuizCache<Topic>>() ));
            
            // Question Repository
            services.AddTransient<IQuestionRepository>( 
                pr => new QuestionRepository(
                    serviceProvider.GetService<IQuizDataAccess<DataEntity.Question>>(),
                    serviceProvider.GetService<IQuizCache<DataEntity.Question>>()));

            // Answer Repository
            services.AddTransient<IAnswerRepository>( 
                ar => new AnswerRepository(
                    serviceProvider.GetService<IQuizDataAccess<DataEntity.Answer>>(),
                    serviceProvider.GetService<IQuizCache<DataEntity.Answer>>()));

            // QuizManager
            var topicRepoInstance = new TopicRepository(
                       serviceProvider.GetService<IQuizDataAccess<Topic>>(),
                       serviceProvider.GetService<IQuizCache<Topic>>());
            var questionRepoInstance = new QuestionRepository(
                    serviceProvider.GetService<IQuizDataAccess<DataEntity.Question>>(),
                    serviceProvider.GetService<IQuizCache<DataEntity.Question>>());
            var answerRepoInstance = new AnswerRepository(
                    serviceProvider.GetService<IQuizDataAccess<DataEntity.Answer>>(),
                    serviceProvider.GetService<IQuizCache<DataEntity.Answer>>());

            services.AddTransient<IQuizManager>( 
                qm => new QuizManager.QuizManager( topicRepoInstance, questionRepoInstance,  answerRepoInstance));
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