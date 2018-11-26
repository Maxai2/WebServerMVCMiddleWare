using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebServerProject.Controllers
{
    class PhoneController
    {
        public string Buy(string name, int quantity)
        {
            return $"<h3>Name: {name}<br>Quantity: {quantity}</h3>";
        }

        public string Info(int id)
        {
            var items = new[]
            {
                "Lumia",
                "LG",
                "Sony"
            };

            if (id < 0 || id >= items.Length)
                return "Poco";

            return $"<h1>{items[id]}</h1>";
        }

        public string All()
        {
            return @"
                    <ul>
                        <li>iPhone</li>
                        <li>Nokia</li>
                        <li>Sony</li>
                        <li><strike>LG</strike></li>
                    </ul>
                    ";
        }
    }
}
