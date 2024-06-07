using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using PassionProject1N01659860.Models;

namespace PassionProject1N01659860.Controllers
{
    public class CommentsDataController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: api/CommentsData/ListComments
        [HttpGet]
        public IQueryable<Comments> ListComments()
        {
            return db.Comments;
        }

        // GET: api/CommentsData/5
        [ResponseType(typeof(Comments))]
        public IHttpActionResult GetComments(int id)
        {
            Comments comments = db.Comments.Find(id);
            if (comments == null)
            {
                return NotFound();
            }

            return Ok(comments);
        }

        // PUT: api/CommentsData/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutComments(int id, Comments comments)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != comments.CommentID)
            {
                return BadRequest();
            }

            db.Entry(comments).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CommentsExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/CommentsData
        [ResponseType(typeof(Comments))]
        public IHttpActionResult PostComments(Comments comments)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Comments.Add(comments);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = comments.CommentID }, comments);
        }

        // DELETE: api/CommentsData/5
        [ResponseType(typeof(Comments))]
        public IHttpActionResult DeleteComments(int id)
        {
            Comments comments = db.Comments.Find(id);
            if (comments == null)
            {
                return NotFound();
            }

            db.Comments.Remove(comments);
            db.SaveChanges();

            return Ok(comments);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool CommentsExists(int id)
        {
            return db.Comments.Count(e => e.CommentID == id) > 0;
        }
    }
}