using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace WebServerProject.Server
{
    public class MVCMiddleWare : IMiddleware
    {
        private readonly HttpDelegate next;

        public MVCMiddleWare(HttpDelegate next)
        {
            this.next = next;
        }

        public async Task InvokeAsync(HttpListenerContext context, Dictionary<string, object> data)
        {
            var responce = context.Response;
            var writer = new StreamWriter(responce.OutputStream);

            var result = Execute(context.Request);

            if (result != null)
            {
                responce.ContentType = "text/html";
                responce.StatusCode = 200;
                await writer.WriteAsync(result);
                writer.Close();
            }
            else
                await next.Invoke(context, data);
        }

        private string Execute(HttpListenerRequest request)
        {
            var urlParts = request.Url.PathAndQuery.Split(new[] { '/', '\\', '?' }, StringSplitOptions.RemoveEmptyEntries);

            if (urlParts.Length < 2)
                return null;

            var controller = urlParts[0];
            var action = urlParts[1];
            

            var ctrlName = $"WebServerProject.Controllers.{controller}Controller";
            var asm = Assembly.GetExecutingAssembly();
            var controllerType = asm.GetType(ctrlName, false, true); // last Capitalize

            if (controllerType is null)
                return null;

            var actionMethod = controllerType.GetMethod(action, BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);

            if (actionMethod is null)
                return null;

            var controllerInstance = Activator.CreateInstance(controllerType); // new PhonesController

            var args = new List<object>();
            NameValueCollection queryParams = null;

            if (request.HttpMethod == HttpMethod.Get.Method) // url
            {
                if (urlParts.Length == 2 && actionMethod.GetParameters().Length != 0)
                {
                    return null;
                }

                if (urlParts.Length > 2)
                {
                    queryParams = HttpUtility.ParseQueryString(urlParts[2]);
                }
            }
            else if (request.HttpMethod == HttpMethod.Post.Method) // form
            {
                using (StreamReader sr = new StreamReader(request.InputStream))
                {
                    var data = sr.ReadToEnd();
                    queryParams = HttpUtility.ParseQueryString(data);
                }
            }

            var parameters = actionMethod.GetParameters();

            foreach (var param in parameters)
            {
                var item = queryParams[param.Name];
                var arg = Convert.ChangeType(item, param.ParameterType);
                args.Add(arg);
            }

            var r = (string)actionMethod.Invoke(controllerInstance, args.ToArray());

            return r;
        }
    }
}
