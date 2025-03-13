using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;

namespace WorkBook.API
{
    public class PagesAPIController : ApiController
    {
        private readonly WorkBookDatabaseEntities db = new WorkBookDatabaseEntities();

        // GET: api/PagesAPI/getpagesbybook/{bookID}
        [HttpGet]
        [Route("api/PagesAPI/getpagesbybook/{bookID}")]
        public async Task<IHttpActionResult> GetPagesByBook(int bookID)
        {
            var pages = await db.Pages.Where(p => p.PBookID == bookID).ToListAsync();
            return Ok(pages);
        }

        // GET: api/PagesAPI/getpageidapi/{id}
        [HttpGet]
        [Route("api/PagesAPI/getpageidapi/{id}")]
        public async Task<IHttpActionResult> Get(int id)
        {
            var page = await db.Pages.FindAsync(id);
            if(page == null)
            {
                return NotFound();
            }
            return Ok(page);
        }

        // POST: api/PagesAPI/postpageapi
        [HttpPost]
        [Route("api/PagesAPI/postpageapi")]
        public async Task<IHttpActionResult> Post([FromBody] Page page)
        {
            if(!ModelState.IsValid)
            {
                //
                var errors = ModelState.Values.SelectMany(v => v.Errors);
                foreach (var error in errors)
                {
                    System.Diagnostics.Debug.WriteLine(error.ErrorMessage);
                }
                //
                return BadRequest(ModelState);
            }
                db.Pages.Add(page);
                await db.SaveChangesAsync();
                return CreatedAtRoute("DefaultApi", new { id = page.PageID }, page);
        }

        // PUT: api/PagesAPI/putpageapi/{id}
        [HttpPut]
        [Route("api/PagesAPI/putpageapi/{id}")]
        public async Task<IHttpActionResult> Put(int id, [FromBody] Page page)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if(id != page.PageID)
            {
                return BadRequest();
            }

            db.Entry(page).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch(Exception ex)
            {
                if(!PageExists(id))
                {
                    return NotFound();
                }
                else
                {
                    return InternalServerError(ex);
                }
            }
            return StatusCode(HttpStatusCode.NoContent);
        }

        // DELETE: api/PagesAPI/deletepageapi/{id}
        [HttpDelete]
        [Route("api/PagesAPI/deletepageapi/{id}")]
        public async Task<IHttpActionResult> Delete(int id)
        {
            var page = await db.Pages.FindAsync(id);
            if(page == null)
            {
                return NotFound();
            }
            db.Pages.Remove(page);
            await db.SaveChangesAsync();

            return Ok(page);
        }
        private bool PageExists(int id)
        {
            return db.Pages.Count(e => e.PageID == id) > 0;
        }
    }
}
