using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebServerProject.Model
{
    public interface IBookService
    {
        List<string> GetAllBooks();
        void AddBook(string title);
    }
}
