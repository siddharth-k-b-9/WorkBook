using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace WorkBook.Controllers
{
    public class BooksController : Controller
    {
        private WorkBookDatabaseEntities db = new WorkBookDatabaseEntities();
        private readonly string apiBaseUrl = "http://localhost:56199/api/BooksAPI/";

        // GET: Books
        public async Task<ActionResult> Index(int userID)
        {
            IEnumerable<Book> books = null;
            using (var client = new HttpClient())
            {
                var responseTask = await client.GetAsync($"{apiBaseUrl}getbooksbyuser/{userID}");
                if(responseTask.IsSuccessStatusCode)
                {
                    books = await responseTask.Content.ReadAsAsync<IList<Book>>();
                }
                else
                {
                    books = new List<Book>();
                    ModelState.AddModelError(string.Empty, "Server Error.");
                }
            }
            ViewBag.UserID = userID;
            return View(books);
        }

        // GET: Books/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Book book = null;
            using (var client = new HttpClient())
            {
                var responseTask = await client.GetAsync($"{apiBaseUrl}getbookidapi/{id}");
                if(responseTask.IsSuccessStatusCode)
                {
                    book = await responseTask.Content.ReadAsAsync<Book>();
                }
                else
                {
                    book = new Book();
                    ModelState.AddModelError(string.Empty, "Server Error.");
                }
            }
            ViewBag.UserID = book.UserID;
            return View(book);
        }

        // GET: Books/Create
        public ActionResult Create(int userID) 
        {
            ViewBag.UserID = userID;
            return View();
        }

        // POST: Books/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "BookName,UserID")] Book book)
        {
            if (ModelState.IsValid)
            {
                using (var client = new HttpClient())
                {
                    var postTask = await client.PostAsJsonAsync($"{apiBaseUrl}postbookapi", book);
                    if(postTask.IsSuccessStatusCode)
                    {
                        return RedirectToAction("Index", new { userID = book.UserID });
                    }

                }
            }
            ViewBag.UserID = book.UserID;
            return View(book);
        }

        // GET: Books/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Book book = await db.Books.FindAsync(id);
            if(book == null)
            {
                return HttpNotFound();
            }
            ViewBag.UserID = new SelectList(db.Users, "ID", "Name", book.UserID);
            return View(book);
        }

        // POST: Books/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "BookID,BookName,UserID")] Book book)
        {
            if (ModelState.IsValid)
            {
                using (var client = new HttpClient())
                {
                    var editTask = await client.PutAsJsonAsync($"{apiBaseUrl}putbookapi/{book.BookID}", book);
                    if(editTask.IsSuccessStatusCode)
                    {
                        return RedirectToAction("Index", new { userID = book.UserID });
                    }
                }
            }
            ViewBag.UserID = book.UserID;
            return View(book);
        }

        // GET: Books/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Book book = null;
            using (var client = new HttpClient())
            {
                var responseTask = await client.GetAsync($"{apiBaseUrl}getbookidapi/{id}");
                if(responseTask.IsSuccessStatusCode)
                {
                    book = await responseTask.Content.ReadAsAsync<Book>();
                }
                else
                {
                    book = new Book();
                }
            }
            return View(book);
        }

        // POST: Books/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            using (var client = new HttpClient())
            {
                var deleteTask = await client.DeleteAsync($"{apiBaseUrl}deletebookapi/{id}");
                if(deleteTask.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index", new { userID = (await deleteTask.Content.ReadAsAsync<Book>()).UserID});
                }
            }
            return RedirectToAction("Delete", new { id });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
