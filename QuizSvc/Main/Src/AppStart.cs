

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using TopicRepositoryLib;
namespace QuizSvc
{
    public class AppStart
    {
        public void ConfigureServices(IServiceCollection services)
        {
           services.AddTransient<ITopicRepository>( p => new TopicRepository());
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