using System.Linq;
using System.Net;
using System.Web.Http;

namespace WorkBook.API
{
    public class UserAPIController : ApiController
    {
        private WorkBookDatabaseEntities db = new WorkBookDatabaseEntities();
        [HttpGet]
        [Route("api/UserAPI/testusers")]
        public IHttpActionResult TestUsers()
        {
            var users = db.Users.ToList();
            return Json(users);
        }

        // GET: api/UserAPI
        [HttpGet]
        [Route("api/UserAPI/getuserapi")]
        public IHttpActionResult Get()
        {
            return Ok(db.Users.ToList());
        }

        // GET: api/UserAPI/5
        [HttpGet]
        [Route("api/UserAPI/getuseridapi/{id}")]
        public IHttpActionResult Get(int id)
        {
            User user = db.Users.Find(id);
            if(user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }

        // POST: api/UserAPI
        [HttpPost]
        [Route("api/UserAPI/postuserapi")]
        public IHttpActionResult Post(User user)
        {
            if(ModelState.IsValid)
            {
                if (db.Users.Any(u => u.Email == user.Email))
                {
                    return Content (HttpStatusCode.BadRequest, "Email already exists.");
                }

                db.Users.Add(user);
                db.SaveChanges();
                return Ok(200);
            }
            return BadRequest(ModelState);
        }

        // PUT: api/UserAPI/5
        [HttpPut]
        [Route("api/UserAPI/putuserapi/{id}")]
        public IHttpActionResult Put(int id, User user)
        {
            if (id != user.ID)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                var existingUser = db.Users.FirstOrDefault(u => u.ID == id);
                if (existingUser == null)
                {
                    return NotFound();
                }
                if (existingUser.Email != user.Email)
                {
                    if (db.Users.Any(u => u.Email == user.Email))
                    {
                        return Content(HttpStatusCode.BadRequest, "Email already exists.");
                    }
                }
                db.Entry(existingUser).CurrentValues.SetValues(user);
                db.SaveChanges();
                return StatusCode(HttpStatusCode.NoContent);
            }
            return BadRequest(ModelState);
        }


        // DELETE: api/UserAPI/5
        [HttpDelete]
        [Route("api/UserAPI/deleteuserapi/{id}")]
        public IHttpActionResult Delete(int id)
        {
            User user = db.Users.Find(id);
            if(user == null)
            {
                return NotFound();
            }
            db.Users.Remove(user);
            db.SaveChanges();
            return Ok(user);
        }
    }
}
