

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using TopicRepositoryLib;
using DataEntity;
using QuizDataAccess;

namespace QuizSvc
{
    public class AppStart
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<IQuizDataAccess<Topic>>( p => new QuizDataAccess<Topic>() );
            services.AddTransient<ITopicRepository>( p => new TopicRepository(new QuizDataAccess<Topic>()));
            
            services.AddMvc( options => options.RespectBrowserAcceptHeader = true );
            
            services.AddSwaggerGen(
                option => new Swashbuckle.Swagger.Model.Info
                {
                    Version = "v1",
                    Title = "Quiz web Api",
                    Description = "Help in building web services",
                    TermsOfService = "None"
                }
            );
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseSwagger();
            app.UseSwaggerUi();
            app.UseMvcWithDefaultRoute();

        }
    }
}