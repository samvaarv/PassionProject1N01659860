using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using PassionProject1N01659860.Models;

namespace PassionProject1N01659860.Controllers
{
    public class ArtDataController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        /// <summary>
        /// Retrieves a list of all artworks in the system.
        /// </summary>
        /// <returns>
        /// HEADER: 200 (OK)
        /// CONTENT: A list of all artworks in the database.
        /// </returns>
        /// <example>
        /// GET: api/ArtData/ListArts
        /// </example>
        [HttpGet]
        public IQueryable<Art> ListArts()
        {
            // Returns an IQueryable object representing all artworks in the database.
            return db.Arts;
        }

        /// <summary>
        /// Finds a specific artwork by its ID.
        /// </summary>
        /// <param name="id">The primary key of the artwork to find.</param>
        /// <returns>
        /// HEADER: 200 (OK)
        /// CONTENT: The artwork corresponding to the provided ID.
        /// or
        /// HEADER: 404 (NOT FOUND) if the artwork is not found.
        /// </returns>
        /// <example>
        /// GET: api/ArtData/FindArt/5
        /// </example>
        [ResponseType(typeof(Art))]
        [HttpGet]
        public IHttpActionResult FindArt(int id)
        {
            // Finds the artwork by its ID.
            Art art = db.Arts.Find(id);
            if (art == null)
            {
                // Returns a 404 Not Found if the artwork does not exist.
                return NotFound();
            }

            // Returns the artwork in the response body.
            return Ok(art);
        }

        /// <summary>
        /// Updates the details of a specific artwork.
        /// </summary>
        /// <param name="id">The primary key of the artwork to update.</param>
        /// <param name="art">Updated artwork data in JSON format.</param>
        /// <returns>
        /// HEADER: 204 (No Content) if the update is successful.
        /// HEADER: 400 (Bad Request) if the provided data is invalid or the IDs do not match.
        /// HEADER: 404 (Not Found) if the artwork does not exist.
        /// </returns>
        /// <example>
        /// POST: api/ArtData/UpdateArt/5
        /// FORM DATA: Artwork JSON Object
        /// </example>
        [ResponseType(typeof(void))]
        [HttpPost]
        public IHttpActionResult UpdateArt(int id, Art art)
        {
            // Checks if the model state is valid.
            if (!ModelState.IsValid)
            {
                // Returns a 400 Bad Request with the model state errors.
                return BadRequest(ModelState);
            }

            // Ensures the ID in the URL matches the ID of the artwork object.
            if (id != art.ArtID)
            {
                // Returns a 400 Bad Request if there is an ID mismatch.
                return BadRequest();
            }

            // Marks the artwork entity as modified.
            db.Entry(art).State = EntityState.Modified;
            // Picture update is handled by another method
            db.Entry(art).Property(a => a.ImageURL).IsModified = false;
            db.Entry(art).Property(a => a.PicExtension).IsModified = false;

            try
            {
                // Attempts to save the changes to the database.
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                // If there is a concurrency issue or the artwork does not exist, handle the exception.
                if (!ArtExists(id))
                {
                    // Returns a 404 Not Found if the artwork does not exist.
                    return NotFound();
                }
                else
                {
                    // Throws the exception if it's not a known concurrency issue.
                    throw;
                }
            }

            // Returns a 204 No Content response indicating successful update.
            return StatusCode(HttpStatusCode.NoContent);
        }

        /// <summary>
        /// Receives art picture data, uploads it to the webserver and updates the art's HasPic option
        /// </summary>
        /// <param name="id">the art id</param>
        /// <returns>status code 200 if successful.</returns>
        /// <example>
        /// curl -F artpic=@file.jpg "https://localhost:xx/api/artdata/uploadartpic/2"
        /// POST: api/artData/UpdateartPic/3
        /// HEADER: enctype=multipart/form-data
        /// FORM-DATA: image
        /// </example>
        /// https://stackoverflow.com/questions/28369529/how-to-set-up-a-web-api-controller-for-multipart-form-data

        [HttpPost]
        public IHttpActionResult UploadArtPic(int id)
        {

            bool haspic = false;
            string picextension;
            if (Request.Content.IsMimeMultipartContent())
            {
                Debug.WriteLine("Received multipart form data.");

                int numfiles = HttpContext.Current.Request.Files.Count;
                Debug.WriteLine("Files Received: " + numfiles);

                //Check if a file is posted
                if (numfiles == 1 && HttpContext.Current.Request.Files[0] != null)
                {
                    var artPic = HttpContext.Current.Request.Files[0];
                    //Check if the file is empty
                    if (artPic.ContentLength > 0)
                    {
                        //establish valid file types (can be changed to other file extensions if desired!)
                        var valtypes = new[] { "jpeg", "jpg", "png", "gif" };
                        var extension = Path.GetExtension(artPic.FileName).Substring(1);
                        //Check the extension of the file
                        if (valtypes.Contains(extension))
                        {
                            try
                            {
                                //file name is the id of the image
                                string fn = id + "." + extension;

                                //get a direct file path to ~/Content/arts/{id}.{extension}
                                string path = Path.Combine(HttpContext.Current.Server.MapPath("~/Content/Images/Arts/"), fn);

                                //save the file
                                artPic.SaveAs(path);

                                //if these are all successful then we can set these fields
                                haspic = true;
                                picextension = extension;

                                //Update the art haspic and picextension fields in the database
                                Art Selectedart = db.Arts.Find(id);
                                Selectedart.ImageURL = haspic;
                                Selectedart.PicExtension = extension;
                                db.Entry(Selectedart).State = EntityState.Modified;

                                db.SaveChanges();

                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine("Art Image was not saved successfully.");
                                Debug.WriteLine("Exception:" + ex);
                                return BadRequest();
                            }
                        }
                    }

                }

                return Ok();
            }
            else
            {
                //not multipart form data
                return BadRequest();

            }

        }


        /// <summary>
        /// Adds a new artwork to the database.
        /// </summary>
        /// <param name="art">New artwork data in JSON format.</param>
        /// <returns>
        /// HEADER: 201 (Created) if the artwork is successfully added.
        /// HEADER: 400 (Bad Request) if the provided data is invalid.
        /// </returns>
        /// <example>
        /// POST: api/ArtData/AddArt
        /// FORM DATA: Artwork JSON Object
        /// </example>
        [ResponseType(typeof(Art))]
        [HttpPost]
        public IHttpActionResult AddArt(Art art)
        {
            // Checks if the model state is valid.
            if (!ModelState.IsValid)
            {
                // Returns a 400 Bad Request with the model state errors.
                return BadRequest(ModelState);
            }

            // Adds the new artwork to the database.
            db.Arts.Add(art);
            db.SaveChanges();

            // Returns a 201 Created response with the newly added artwork.
            return CreatedAtRoute("DefaultApi", new { id = art.ArtID }, art);
        }

        /// <summary>
        /// Deletes an artwork from the database by its ID.
        /// </summary>
        /// <param name="id">The primary key of the artwork to delete.</param>
        /// <returns>
        /// HEADER: 200 (OK) if the artwork is successfully deleted.
        /// HEADER: 404 (NOT FOUND) if the artwork is not found.
        /// </returns>
        /// <example>
        /// POST: api/ArtData/DeleteArt/5
        /// FORM DATA: (empty)
        /// </example>
        [ResponseType(typeof(Art))]
        [HttpPost]
        public IHttpActionResult DeleteArt(int id)
        {
            // Finds the artwork by its ID.
            Art art = db.Arts.Find(id);
            if (art == null)
            {
                // Returns a 404 Not Found if the artwork does not exist.
                return NotFound();
            }

            if (art.ImageURL && art.PicExtension != "")
            {
                //also delete image from path
                string path = HttpContext.Current.Server.MapPath("~/Content/Images/Arts/" + id + "." + art.PicExtension);
                if (System.IO.File.Exists(path))
                {
                    Debug.WriteLine("File exists... preparing to delete!");
                    System.IO.File.Delete(path);
                }
            }

            // Removes the artwork from the database.
            db.Arts.Remove(art);
            db.SaveChanges();

            // Returns a 200 OK response with the deleted artwork.
            return Ok(art);
        }

        /// <summary>
        /// Disposes of the database context when the controller is disposed.
        /// </summary>
        /// <param name="disposing">True to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Disposes the database context.
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// Checks if an artwork with the specified ID exists.
        /// </summary>
        /// <param name="id">The ID of the artwork to check for.</param>
        /// <returns>True if the artwork exists, otherwise false.</returns>
        private bool ArtExists(int id)
        {
            // Returns true if an artwork with the specified ID exists in the database.
            return db.Arts.Count(e => e.ArtID == id) > 0;
        }
    }
}
