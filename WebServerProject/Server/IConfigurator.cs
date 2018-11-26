namespace WebServerProject.Server
{
    public interface IConfigurator
    {
        void ConfigureMiddleware(MiddlewareBuilder builder);
    }

    public class Configurator : IConfigurator
    {
        public void ConfigureMiddleware(MiddlewareBuilder builder)
        {
            builder.Use<MVCMiddleWare>();
        }
    }
}