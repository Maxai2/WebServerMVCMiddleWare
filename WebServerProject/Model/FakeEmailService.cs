using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebServerProject.Model
{
    class FakeEmailService : IEmailservice
    {
        public void SendEmail(string address, string text)
        {
            Console.WriteLine($"FES: send {text} to {address}");
        }
    }
}
