﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebServerProject.Model;
using WebServerProject.Server.Attributes;

namespace WebServerProject.Controllers
{
    public class BookController
    {
        private IBookService bookService;

        public BookController(IBookService bookService)
        {
            this.bookService = bookService;
        }

        [Authorize("User,Admin")] // [Authorize]
        [HttpMethod("GET")]
        public string All()
        {
            List<string> books = bookService.GetAllBooks();
            StringBuilder resp = new StringBuilder()
                .Append("<html><body>")
                .Append("<h3>All books:</h3>")
                .Append("<ul>");
            foreach (string b in books)
            {
                resp.Append($"<li>{b}</li>");
            }
            resp.Append("</ul>")
                .Append("<br/>")
                .Append("<form action='/book/add' method='POST'>")
                .Append("<input type='text' name='title' />")
                .Append("<input type='submit' value='Add book' />")
                .Append("</form>")
                .Append("</body></html>");
            return resp.ToString();
        }

        [Authorize("Admin")]
        [HttpMethod("POST")]
        public string Add(string title)
        //public string PostAdd(string title)
        {
            bookService.AddBook(title);
            List<string> books = bookService.GetAllBooks();
            StringBuilder resp = new StringBuilder()
                .Append("<html><body>")
                .Append("<h3>All books:</h3>")
                .Append("<ul>");
            foreach (string b in books)
            {
                resp.Append($"<li>{b}</li>");
            }
            resp.Append("</ul>")
                .Append("<br/>")
                .Append("<form action='/book/add' method='POST'>")
                .Append("<input type='text' name='title' />")
                .Append("<input type='submit' value='Add book' />")
                .Append("</form>")
                .Append("</body></html>");
            return resp.ToString();
        }
    }
}
