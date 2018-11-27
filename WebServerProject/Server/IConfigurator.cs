using Autofac;
using WebServerProject.Controllers;
using WebServerProject.Model;

namespace WebServerProject.Server
{
    public interface IConfigurator
    {
        void ConfigureMiddleware(MiddlewareBuilder builder);
        void ConfigureDependencies(ContainerBuilder builder);
    }

    public class Configurator : IConfigurator
    {
        public void ConfigureDependencies(ContainerBuilder builder)
        {
            builder.RegisterType<BookController>();
            builder.RegisterType<HomeController>();

            builder.RegisterType<FakeEmailService>().As<IEmailservice>();
            builder.RegisterType<BookService>().As<IBookService>();
        }

        public void ConfigureMiddleware(MiddlewareBuilder builder)
        {
            builder.Use<Middleware1>().Use<StaticFilesMiddleware>().Use<MvcMiddleware>();
        }
    }
}
