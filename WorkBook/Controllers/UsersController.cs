using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace WorkBook.Controllers
{
    public class UsersController : Controller
    {
        private WorkBookDatabaseEntities db = new WorkBookDatabaseEntities();
        private readonly string apiBaseUrl = "http://localhost:56199/api/UserAPI/";

        // GET: Users
        public async Task<ActionResult> Index() //int? page
        {
            IEnumerable<User> users = null;
            using (var client = new HttpClient())
            {
                try 
                {
                    var responseTask = await client.GetAsync($"{apiBaseUrl}getuserapi");
                    if(responseTask.IsSuccessStatusCode)
                    {
                        users = await responseTask.Content.ReadAsAsync<IList<User>>();
                        System.Diagnostics.Debug.WriteLine("Users count: " + users.Count());
                    }
                    else
                    {
                        users = Enumerable.Empty<User>();
                        ModelState.AddModelError(string.Empty, "Server error.");
                    }
                }
                catch(Exception ex)
                {
                    ModelState.AddModelError(string.Empty, $"Error: {ex.Message}");
                }
            }
            return View(users);
        }

        // GET: Users/Details/5
        public ActionResult Details(int? id)
        {
            User user = null;
            using (var client = new HttpClient())
            {
                var responseTask = client.GetAsync($"{apiBaseUrl}getuseridapi/{id}");
                responseTask.Wait();
                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<User>();
                    readTask.Wait();
                    user = readTask.Result;
                }
                else
                {
                    user = new User();
                }
            }
            return View(user);
        }

        // GET: Users/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Users/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Name,Email")] User user)
        {
            if (ModelState.IsValid)
            {
                using (var client = new HttpClient())
                {
                    var postTask = client.PostAsJsonAsync<User>($"{apiBaseUrl}postuserapi", user);
                    postTask.Wait();
                    var result = postTask.Result;
                    if(result.IsSuccessStatusCode)
                    {
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        var error = result.Content.ReadAsStringAsync().Result;
                        ModelState.AddModelError(string.Empty, error);
                    }
                }
            }
            return View(user);
        }

        // GET: Users/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = null;
            using (var client = new HttpClient())
            {
                var responseTask = client.GetAsync($"{apiBaseUrl}getuseridapi/{id}");
                responseTask.Wait();
                var result = responseTask.Result;
                if(result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<User>();
                    readTask.Wait();
                    user = readTask.Result;
                }
                else
                {
                    var error = result.Content.ReadAsStringAsync().Result;
                    ModelState.AddModelError(string.Empty, error);
                }
            }
            return View(user);
        }

        // POST: Users/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Name,Email")] User user)
        {
            if (ModelState.IsValid)
            {
                using (var client = new HttpClient())
                {
                    var editTask = client.PutAsJsonAsync<User>($"{apiBaseUrl}putuserapi/{user.ID}", user);
                    editTask.Wait();
                    var result = editTask.Result;
                    if(result.IsSuccessStatusCode)
                    {
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        var error = result.Content.ReadAsStringAsync().Result;
                        ModelState.AddModelError(string.Empty, error);
                    }
                }
            }
            return View(user);
        }

        // GET: Users/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = null;
            using (var client = new HttpClient())
            {
                var responseTask = client.GetAsync($"{apiBaseUrl} getuseridapi/{id}");
                responseTask.Wait();
                var result = responseTask.Result;
                if(result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<User>();
                    readTask.Wait();
                    user = readTask.Result;
                }
                else
                {
                    user = new User();
                }
            }
            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
           using (var client = new HttpClient())
            {
                var deleteTask = client.DeleteAsync($"{apiBaseUrl} deleteuserapi/{id}");
                deleteTask.Wait();
                var result = deleteTask.Result;
                if(result.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
                return RedirectToAction("Delete");
            }
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
