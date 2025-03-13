using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace WorkBook.Controllers
{
    public class PagesController : Controller
    {
        private readonly string apiBaseUrl = "http://localhost:56199/api/PagesAPI/";
        private WorkBookDatabaseEntities db = new WorkBookDatabaseEntities();

        // GET: Pages
        public async Task<ActionResult> Index(int bookID)
        {
            IEnumerable<Page> pages = null;
            using (var client = new HttpClient())
            {
                var response = await client.GetAsync($"{apiBaseUrl}getpagesbybook/{bookID}");
                if(response.IsSuccessStatusCode)
                {
                    pages = await response.Content.ReadAsAsync<IList<Page>>();
                }
                else
                {
                    pages = new List<Page>();
                }
            }
            ViewBag.PBookID = bookID;
            return View(pages);
        }

        // GET: Pages/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Page page = null;
            using(var client = new HttpClient())
            {
                var response = await client.GetAsync($"{apiBaseUrl}getpageidapi/{id}");
                if(response.IsSuccessStatusCode)
                {
                    page = await response.Content.ReadAsAsync<Page>();
                }
                else
                {
                    page = new Page();
                }
            }
            ViewBag.PBookID = page.PBookID;
            return View(page);
        }

        // GET: Pages/Create
        public ActionResult Create(int bookID)
        {
            ViewBag.PBookID = bookID;
            
            return View();
        }

        // POST: Pages/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "PContent,PBookID")] Page page)
        {
            if (ModelState.IsValid)
            {
                using (var client = new HttpClient())
                {                  
                    var response = await client.PostAsJsonAsync($"{apiBaseUrl}postpageapi", page);
                    if(response.IsSuccessStatusCode)
                    {
                        return RedirectToAction("Index", new { bookID = page.PBookID });
                    }
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Model state invalid.");
            }    
            ViewBag.PBookID = page.PBookID;
            return View(page);
        }

        // GET: Pages/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Page page = await db.Pages.FindAsync(id);
            if (page == null)
            {
                return HttpNotFound();
            }
            ViewBag.PBookID = page.PBookID;
            return View(page);
        }

        // POST: Pages/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "PageID,PContent,PBookID")] Page page)
        {
            if (ModelState.IsValid)
            {
                using(var client = new HttpClient())
                {
                    var response = await client.PutAsJsonAsync($"{apiBaseUrl}putpageapi/{page.PageID}", page);
                    if(response.IsSuccessStatusCode)
                    {
                        return RedirectToAction("Index", new { bookID = page.PBookID });
                    }
                }
            }
            ViewBag.PBookID = page.PBookID;
            return View(page);
        }

        // GET: Pages/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Page page = null;
            using(var client = new HttpClient())
            {
                var response = await client.GetAsync($"{apiBaseUrl}getpageidapi/{id}");
                if(response.IsSuccessStatusCode)
                {
                    page = await response.Content.ReadAsAsync<Page>();
                }
                else
                {
                    page = new Page();
                }
            }
            ViewBag.PBookID = page.PBookID;
            return View(page);
        }

        // POST: Pages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            using (var client = new HttpClient())
            {
                var response = await client.DeleteAsync($"{apiBaseUrl}deletepageapi/{id}");
                if(response.IsSuccessStatusCode)
                {
                    var page = await response.Content.ReadAsAsync<Page>();
                    return RedirectToAction("Index", new { bookID = page.PBookID });
                }
            }
            return RedirectToAction("Delete", new {id});
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
