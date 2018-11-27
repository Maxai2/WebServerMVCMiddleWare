using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebServerProject.Model;

namespace WebServerProject.Controllers
{
    public class HomeController
    {
        private readonly IEmailservice emailservice;

        public HomeController(IEmailservice emailservice)
        {
            this.emailservice = emailservice;
        }

        public string Email(string address, string text)
        {
            emailservice.SendEmail(address, text);
            return "Email sent";
        }

        public string Index()
        {
            return "Home/Index";
        }

        public string About()
        {
            return "Home/About";
        }
    }
}
