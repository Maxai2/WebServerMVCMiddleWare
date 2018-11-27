using Autofac;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace WebServerProject.Server
{
    public class MyWebServer
    {
        private string domain;
        private string port;

        private HttpListener httpListener;

        private HttpDelegate firstMiddleware;

        public static IContainer IOC { get; private set; }

        public MyWebServer(string domain, string port)
        {
            this.domain = domain;
            this.port = port;
            httpListener = new HttpListener();
            httpListener.Prefixes.Add($"{domain}:{port}/");
        }

        public MyWebServer Configure<T>() where T : IConfigurator, new()
        {
            IConfigurator configurator = new T();

            var builder = new MiddlewareBuilder();
            configurator.ConfigureMiddleware(builder);
            firstMiddleware = builder.Build();

            var depBuilder = new ContainerBuilder();
            configurator.ConfigureDependencies(depBuilder);
            IOC = depBuilder.Build();

            return this;
        }

        public void Run()
        {
            httpListener.Start();
            while(true)
            {
                HttpListenerContext context = httpListener.GetContext();
                Task.Run(() => { Process(context); });
            }
        }

        private void Process(HttpListenerContext context)
        {
            try
            {
                firstMiddleware.Invoke(context, new Dictionary<string, object>());              
            }
            catch (Exception ex)
            {
                context.Response.StatusCode = 500;
                context.Response.ContentType = "text/plain";
                using (StreamWriter writer = new StreamWriter(context.Response.OutputStream))
                {
                    writer.Write($"Error on MyWebServer: {ex.Message}");
                }
            }

        }
    }
}
