using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace WebServerProject.Server
{
    class AuthorizeMiddleWare : IMiddleware
    {
        private readonly HttpDelegate next;

        public AuthorizeMiddleWare(HttpDelegate next)
        {
            this.next = next;
        }

        public async Task InvokeAsync(HttpListenerContext context, Dictionary<string, object> data)
        {
            //js in Console
            //document.cookie = 'token=qwerty'
            //document.cookie = 'role=User'

            var token = context.Request.Cookies["token"];

            if (token != null)
                data.Add("isAuth", true);
            else
                data.Add("isAuth", false);

            var role = context.Request.Cookies["role"];
            data.Add("Role", role?.Value ?? "Guest");

            await next.Invoke(context, data);
        }
    }
}
