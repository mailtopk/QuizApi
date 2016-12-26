

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace QuizSvc
{
    public class AppStart
    {
        public void ConfigureServices(IServiceCollection services)
        {
           /* services.AddTransient<IQuestionRepository>( p => new QuestionRepository());
            services.AddMvc();
            services.AddSwaggerGen(
                option => new Swashbuckle.Swagger.Model.Info
                {
                    Version = "v1",
                    Title = "Demo sample application",
                    Description = "API Sample made for PBC",
                    TermsOfService = "None"
                }
            );*/
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseSwagger();
            app.UseSwaggerUi();
            app.UseMvcWithDefaultRoute();

        }
    }
}