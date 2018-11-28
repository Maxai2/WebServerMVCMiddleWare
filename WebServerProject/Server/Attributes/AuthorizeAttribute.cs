using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebServerProject.Server.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class AuthorizeAttribute : Attribute
    {
        public string Roles { get; set; }

        public AuthorizeAttribute()
        {
            Roles = null;
        }

        public AuthorizeAttribute(string roles)
        {
            Roles = roles;
        }
    }
}
