using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;

namespace WorkBook.API
{
    public class BooksAPIController : ApiController
    {
        private readonly WorkBookDatabaseEntities db = new WorkBookDatabaseEntities();

        // GET: api/BooksAPI/getbooksbyuser/{userID}
        [HttpGet]
        [Route("api/BooksAPI/getbooksbyuser/{userID}")]
        public async Task<IHttpActionResult> GetBooksByUser(int userID)
        {
            var books = await db.Books.Where(b => b.UserID == userID).ToListAsync();
            return Ok(books);
        }

        // GET: api/BooksAPI/getbookidapi/{id}
        [HttpGet]
        [Route("api/BooksAPI/getbookidapi/{id}")]
        public async Task<IHttpActionResult> Get(int id)
        {
            var book = await db.Books.FindAsync(id);
            if(book == null)
            {
                return NotFound();
            }
            return Ok(book);
        }

        // POST: api/BooksAPI/postbookapi
        [HttpPost]
        [Route("api/BooksAPI/postbookapi")]
        public async Task<IHttpActionResult> Post([FromBody] Book book)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            db.Books.Add(book);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = book.BookID }, book);
        }

        // PUT: api/BooksAPI/putbookapi/{id}
        [HttpPut]
        [Route("api/BooksAPI/putbookapi/{id}")]
        public async Task<IHttpActionResult> Put(int id, [FromBody]Book book)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if(id != book.BookID)
            {
                return BadRequest();
            }

            db.Entry(book).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch(Exception ex)
            {
                if(!BookExists(id))
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

        // DELETE: api/BooksAPI/deletebookapi/{id}
        [HttpDelete]
        [Route("api/BooksAPI/deletebookapi/{id}")]
        public async Task<IHttpActionResult> Delete(int id)
        {
            var book = await db.Books.FindAsync(id);
            if(book == null)
            {
                return NotFound();
            }
            db.Books.Remove(book);
            await db.SaveChangesAsync();

            return Ok(book);
        }
        //Check whether the book exists or not.
        private bool BookExists(int id)
        {
            return db.Books.Count(e => e.BookID == id) > 0;
        }
    }
}
