using Autofac;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using WebServerProject.Server.Attributes;

namespace WebServerProject.Server
{
    public class MvcMiddleware : IMiddleware
    {
        private HttpDelegate next;

        public MvcMiddleware(HttpDelegate next)
        {
            this.next = next;
        }

        public async Task InvokeAsync(HttpListenerContext context, Dictionary<string, object> data)
        {
            HttpListenerResponse response = context.Response;
            StreamWriter writer = new StreamWriter(response.OutputStream);
            try
            {
                string resp = FindControllerAction(context.Request, data);
                if (resp != null)
                {
                    response.StatusCode = 200;
                    response.ContentType = "text/html";
                    writer.Write(resp);
                }
                else
                {
                    await next.Invoke(context, data);
                }
            }
            catch (Exception ex)
            {
                response.StatusCode = 500;
                response.ContentType = "text/plain";
                writer.Write(ex.Message);
            }
            finally
            {
                writer.Close();
            }            
        }

        private string FindControllerAction(HttpListenerRequest request, Dictionary<string, object> data)
        {
            string[] urlparts = request.Url.PathAndQuery.Split(new char[] { '/', '\\', '?' }, StringSplitOptions.RemoveEmptyEntries);
            if (urlparts.Length < 2) return null;

            string controller = urlparts[0];
            string action = urlparts[1];

            Assembly curAssembly = Assembly.GetExecutingAssembly();

            Type controllerType = curAssembly.GetType($"WebServerProject.Controllers.{controller}Controller", false, true);
            if (controllerType == null) return null;

            //var httpMethod = request.HttpMethod;
            //MethodInfo actionMethod = controllerType.GetMethod($"{httpMethod}{action}", BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);

            MethodInfo actionMethod = controllerType.GetMethod(action, BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);
            if (actionMethod == null) return null;

            var attr = actionMethod.GetCustomAttribute<HttpMethodAttribute>();

            if (attr != null)
            {
                if (String.Compare(attr.Method, request.HttpMethod, true) != 0)
                {
                    Console.WriteLine($"{attr.Method} - {request.HttpMethod}");
                    return null;
                }
            }

            var attrAuth = actionMethod.GetCustomAttribute<AuthorizeAttribute>();

            if (attrAuth != null)
            {
                if ((bool)data["isAuth"] == false)
                {
                    return "HTTP ERROR 401: Not authorizade";
                }

                if (attrAuth.Roles != null)
                {
                    var roles = attrAuth.Roles.Split(',');
                    if (!roles.Contains(data["Role"]))
                    {
                        return "HTTP ERROR 401: Access Denied!";
                    }
                }
            }

            List<object> paramsToMethod = new List<object>();
            NameValueCollection coll = null;

            if (request.HttpMethod == "GET")
            {
                if (urlparts.Length == 2 && actionMethod.GetParameters().Length != 0) return null;
                if (urlparts.Length > 2)
                {
                    coll = System.Web.HttpUtility.ParseQueryString(urlparts[2]);
                }
            }
            else if (request.HttpMethod == "POST")
            {
                string body;
                using (StreamReader reader = new StreamReader(request.InputStream))
                {
                    body = reader.ReadToEnd();
                }
                coll = System.Web.HttpUtility.ParseQueryString(body);
            }
            else { return null; }

            ParameterInfo[] parameters = actionMethod.GetParameters();
            foreach (ParameterInfo pi in parameters)
            {
                paramsToMethod.Add(Convert.ChangeType(coll[pi.Name], pi.ParameterType));
            }
            if (paramsToMethod.Count != actionMethod.GetParameters().Length) return null;

            //var _this = Activator.CreateInstance(controllerType);
            //var args = paramsToMethod.ToArray();
            //var res = actionMethod.Invoke(_this, args);

            //string resp = (string)actionMethod.Invoke(Activator.CreateInstance(controllerType), paramsToMethod.ToArray());

            var _this = MyWebServer.IOC.Resolve(controllerType);
            var args = paramsToMethod.ToArray();
            var res = actionMethod.Invoke(_this, args);

            return res as string;
        }
    }
}
