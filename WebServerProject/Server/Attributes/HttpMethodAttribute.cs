using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebServerProject.Server.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class HttpMethodAttribute : Attribute
    {
        public string Method { get; private set; }

        public HttpMethodAttribute(string method)
        {
            if (String.Compare(method, "Post", true) != 0 && String.Compare(method, "Get", true) != 0)
            {
                throw new ArgumentException($"{nameof(method)} must be GET ot POST");
            }

            Method = method;
        }
    }
}
