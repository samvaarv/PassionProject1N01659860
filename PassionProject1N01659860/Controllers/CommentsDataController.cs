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

        /// <summary>
        /// Retrieves a list of all comments in the system.
        /// </summary>
        /// <returns>
        /// HEADER: 200 (OK)
        /// CONTENT: A list of all comments in the database.
        /// </returns>
        /// <example>
        /// GET: api/CommentsData/ListComments
        /// </example>
        // GET: api/CommentsData/ListComments
        [HttpGet]
        [Route("api/CommentsData/ListComments")]
        [ResponseType(typeof(CommentsDto))]
        public IHttpActionResult ListComments()
        {
            List<Comments> Comments = db.Comments.ToList();
            List<CommentsDto> CommentDtos = new List<CommentsDto>();

            Comments.ForEach(a => CommentDtos.Add(new CommentsDto()
            {
                CommentID = a.CommentID,
                CommentText = a.CommentText,
                DateCommented = a.DateCommented,
                UserID = a.User.UserID,
                ArtID = a.Art.ArtID
            }));

            return Ok(CommentDtos);
        }

        [HttpGet]
        [ResponseType(typeof(CommentsDto))]
        public IHttpActionResult ListCommentsForArt(int id)
        {
            //SQL Equivalent:
            //Select * from comments where comments.artid = {id}
            List<Comments> Comments = db.Comments.Where(c => c.ArtID == id).ToList();
            List<CommentsDto> CommentsDto = new List<CommentsDto>();

            Comments.ForEach(c => CommentsDto.Add(new CommentsDto()
            {
                CommentID = c.CommentID,
                CommentText = c.CommentText,
                DateCommented = c.DateCommented,
                UserID = c.User.UserID,
                ArtID = c.Art.ArtID
            }));

            return Ok(CommentsDto);
        }

        [HttpGet]
        [ResponseType(typeof(CommentsDto))]
        public IHttpActionResult ListCommentsForUser(int id)
        {
            //SQL Equivalent:
            //Select * from comments where comments.artid = {id}
            List<Comments> Comments = db.Comments.Where(c => c.UserID == id).ToList();
            List<CommentsDto> CommentsDto = new List<CommentsDto>();

            Comments.ForEach(c => CommentsDto.Add(new CommentsDto()
            {
                CommentID = c.CommentID,
                CommentText = c.CommentText,
                DateCommented = c.DateCommented,
                UserID = c.User.UserID,
                ArtID = c.Art.ArtID
            }));

            return Ok(CommentsDto);
        }

        /// <summary>
        /// Finds a specific comment by its ID.
        /// </summary>
        /// <param name="id">The primary key of the comment to find.</param>
        /// <returns>
        /// HEADER: 200 (OK)
        /// CONTENT: The comment corresponding to the provided ID.
        /// or
        /// HEADER: 404 (NOT FOUND) if the comment is not found.
        /// </returns>
        /// <example>
        /// GET: api/CommentsData/findComments/5
        /// </example>
        [ResponseType(typeof(CommentsDto))]
        [HttpGet]
        ///[Route("api/CommentsData/findComments/")]
        public IHttpActionResult FindComments(int id)
        {
            Comments Comments = db.Comments.Find(id);
            CommentsDto CommentsDto = new CommentsDto()
            {
                CommentID = Comments.CommentID,
                CommentText = Comments.CommentText,
                DateCommented = Comments.DateCommented,
                UserID = Comments.User.UserID,
                ArtID = Comments.Art.ArtID
            };
            if (Comments == null)
            {
                return NotFound();
            }

            return Ok(CommentsDto);
        }

        /// <summary>
        /// Updates the details of a specific comment.
        /// </summary>
        /// <param name="id">The primary key of the comment to update.</param>
        /// <param name="comments">Updated comment data in JSON format.</param>
        /// <returns>
        /// HEADER: 204 (No Content) if the update is successful.
        /// HEADER: 400 (Bad Request) if the provided data is invalid or the IDs do not match.
        /// HEADER: 404 (Not Found) if the comment does not exist.
        /// </returns>
        /// <example>
        /// POST: api/CommentsData/UpdateComments/5
        /// FORM DATA: Comments JSON Object
        /// </example>
        [ResponseType(typeof(void))]
        [HttpPost]
        public IHttpActionResult UpdateComments(int id, Comments comments)
        {
            // Checks if the model state is valid.
            if (!ModelState.IsValid)
            {
                // Returns a 400 Bad Request with the model state errors.
                return BadRequest(ModelState);
            }

            // Ensures the ID in the URL matches the ID of the comments object.
            if (id != comments.CommentID)
            {
                // Returns a 400 Bad Request if there is an ID mismatch.
                return BadRequest();
            }

            // Marks the comments entity as modified.
            db.Entry(comments).State = EntityState.Modified;

            try
            {
                // Attempts to save the changes to the database.
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                // If there is a concurrency issue or the comment does not exist, handle the exception.
                if (!CommentsExists(id))
                {
                    // Returns a 404 Not Found if the comment does not exist.
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
        /// Adds a new comment to the database.
        /// </summary>
        /// <param name="comments">New comment data in JSON format.</param>
        /// <returns>
        /// HEADER: 201 (Created) if the comment is successfully added.
        /// HEADER: 400 (Bad Request) if the provided data is invalid.
        /// </returns>
        /// <example>
        /// POST: api/CommentsData/AddComments
        /// FORM DATA: Comments JSON Object
        /// </example>
        [ResponseType(typeof(Comments))]
        [HttpPost]
        public IHttpActionResult AddComments(Comments comments)
        {
            // Checks if the model state is valid.
            if (!ModelState.IsValid)
            {
                // Returns a 400 Bad Request with the model state errors.
                return BadRequest(ModelState);
            }

            // Adds the new comment to the database.
            db.Comments.Add(comments);
            db.SaveChanges();

            // Returns a 201 Created response with the newly added comment.
            return CreatedAtRoute("DefaultApi", new { id = comments.CommentID }, comments);
        }

        /// <summary>
        /// Deletes a comment from the database by its ID.
        /// </summary>
        /// <param name="id">The primary key of the comment to delete.</param>
        /// <returns>
        /// HEADER: 200 (OK) if the comment is successfully deleted.
        /// HEADER: 404 (NOT FOUND) if the comment is not found.
        /// </returns>
        /// <example>
        /// POST: api/CommentsData/DeleteComments/5
        /// FORM DATA: (empty)
        /// </example>
        [ResponseType(typeof(Comments))]
        [HttpPost]
        public IHttpActionResult DeleteComments(int id)
        {
            // Finds the comment by its ID.
            Comments comments = db.Comments.Find(id);
            if (comments == null)
            {
                // Returns a 404 Not Found if the comment does not exist.
                return NotFound();
            }

            // Removes the comment from the database.
            db.Comments.Remove(comments);
            db.SaveChanges();

            // Returns a 200 OK response with the deleted comment.
            return Ok(comments);
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
        /// Checks if a comment with the specified ID exists.
        /// </summary>
        /// <param name="id">The ID of the comment to check for.</param>
        /// <returns>True if the comment exists, otherwise false.</returns>
        private bool CommentsExists(int id)
        {
            // Returns true if a comment with the specified ID exists in the database.
            return db.Comments.Count(e => e.CommentID == id) > 0;
        }
    }
}
