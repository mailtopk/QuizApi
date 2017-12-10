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
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using NLog.Web;

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
            services.AddDistributedRedisCache(
                options =>  {options.Configuration =  "cachingservice"; 
                options.InstanceName = "QuizAPI"; });
            
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

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            var applicationPath = PlatformServices.Default.Application.ApplicationBasePath;
            var configFile = Path.Combine(applicationPath, "Config/nlog.config");
            loggerFactory.AddNLog();
            app.AddNLogWeb();
            env.ConfigureNLog(configFile);
            app.UseSwagger();
            app.UseSwaggerUi();
            app.UseMvcWithDefaultRoute();
        }
    }
}