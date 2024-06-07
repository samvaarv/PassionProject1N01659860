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
    public class ArtDataController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: api/ArtData/ListArts
        [HttpGet]
        public IQueryable<Art> ListArts()
        {
            return db.Arts;
        }

        // GET: api/ArtData/FindArt/5
        [ResponseType(typeof(Art))]
        [HttpGet]
        public IHttpActionResult FindArt(int id)
        {
            Art art = db.Arts.Find(id);
            if (art == null)
            {
                return NotFound();
            }

            return Ok(art);
        }

        // POST: api/ArtData/UpdateArt/5
        [ResponseType(typeof(void))]
        [HttpPost]
        public IHttpActionResult UpdateArt(int id, Art art)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != art.ArtID)
            {
                return BadRequest();
            }

            db.Entry(art).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ArtExists(id))
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

        // POST: api/ArtData/AddArt
        [ResponseType(typeof(Art))]
        [HttpPost]
        public IHttpActionResult AddArt(Art art)

        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Arts.Add(art);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = art.ArtID }, art);
        }

        // POST: api/ArtData/DeleteArt/5
        [ResponseType(typeof(Art))]
        [HttpPost]
        public IHttpActionResult DeleteArt(int id)
        {
            Art art = db.Arts.Find(id);
            if (art == null)
            {
                return NotFound();
            }

            db.Arts.Remove(art);
            db.SaveChanges();

            return Ok(art);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ArtExists(int id)
        {
            return db.Arts.Count(e => e.ArtID == id) > 0;
        }
    }
}