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
    public class UserDataController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        /// <summary>
        /// Retrieves a list of all users in the system.
        /// </summary>
        /// <returns>
        /// HEADER: 200 (OK)
        /// CONTENT: A list of all users in the database.
        /// </returns>
        /// <example>
        /// GET: api/UserData/ListUsers
        /// </example>
        [HttpGet]
        public IQueryable<User> ListUsers()
        {
            // Returns an IQueryable object representing all users in the database.
            return db.Users;
        }

        /// <summary>
        /// Finds a specific user by their ID.
        /// </summary>
        /// <param name="id">The primary key of the user to find.</param>
        /// <returns>
        /// HEADER: 200 (OK)
        /// CONTENT: The user corresponding to the provided ID.
        /// or
        /// HEADER: 404 (NOT FOUND) if the user is not found.
        /// </returns>
        /// <example>
        /// GET: api/UserData/FindUser/5
        /// </example>
        [ResponseType(typeof(User))]
        [HttpGet]
        public IHttpActionResult FindUser(int id)
        {
            // Finds the user by their ID.
            User user = db.Users.Find(id);
            if (user == null)
            {
                // Returns a 404 Not Found if the user does not exist.
                return NotFound();
            }

            // Returns the user in the response body.
            return Ok(user);
        }

        /// <summary>
        /// Updates the details of a specific user.
        /// </summary>
        /// <param name="id">The primary key of the user to update.</param>
        /// <param name="user">Updated user data in JSON format.</param>
        /// <returns>
        /// HEADER: 204 (No Content) if the update is successful.
        /// HEADER: 400 (Bad Request) if the provided data is invalid or the IDs do not match.
        /// HEADER: 404 (Not Found) if the user does not exist.
        /// </returns>
        /// <example>
        /// POST: api/UserData/UpdateUser/5
        /// FORM DATA: User JSON Object
        /// </example>
        [ResponseType(typeof(void))]
        [HttpPost]
        public IHttpActionResult UpdateUser(int id, User user)
        {
            // Checks if the model state is valid.
            if (!ModelState.IsValid)
            {
                // Returns a 400 Bad Request with the model state errors.
                return BadRequest(ModelState);
            }

            // Ensures the ID in the URL matches the ID of the user object.
            if (id != user.UserID)
            {
                // Returns a 400 Bad Request if there is an ID mismatch.
                return BadRequest();
            }

            // Marks the user entity as modified.
            db.Entry(user).State = EntityState.Modified;

            try
            {
                // Attempts to save the changes to the database.
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                // If there is a concurrency issue or the user does not exist, handle the exception.
                if (!UserExists(id))
                {
                    // Returns a 404 Not Found if the user does not exist.
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
        /// Adds a new user to the database.
        /// </summary>
        /// <param name="user">New user data in JSON format.</param>
        /// <returns>
        /// HEADER: 201 (Created) if the user is successfully added.
        /// HEADER: 400 (Bad Request) if the provided data is invalid.
        /// </returns>
        /// <example>
        /// POST: api/UserData/AddUser
        /// FORM DATA: User JSON Object
        /// </example>
        [ResponseType(typeof(User))]
        [HttpPost]
        public IHttpActionResult AddUser(User user)
        {
            // Checks if the model state is valid.
            if (!ModelState.IsValid)
            {
                // Returns a 400 Bad Request with the model state errors.
                return BadRequest(ModelState);
            }

            // Adds the new user to the database.
            db.Users.Add(user);
            db.SaveChanges();

            // Returns a 201 Created response with the newly added user.
            return CreatedAtRoute("DefaultApi", new { id = user.UserID }, user);
        }

        /// <summary>
        /// Deletes a user from the database by their ID.
        /// </summary>
        /// <param name="id">The primary key of the user to delete.</param>
        /// <returns>
        /// HEADER: 200 (OK) if the user is successfully deleted.
        /// HEADER: 404 (NOT FOUND) if the user is not found.
        /// </returns>
        /// <example>
        /// POST: api/UserData/DeleteUser/5
        /// FORM DATA: (empty)
        /// </example>
        [ResponseType(typeof(User))]
        [HttpPost]
        public IHttpActionResult DeleteUser(int id)
        {
            // Finds the user by their ID.
            User user = db.Users.Find(id);
            if (user == null)
            {
                // Returns a 404 Not Found if the user does not exist.
                return NotFound();
            }

            // Removes the user from the database.
            db.Users.Remove(user);
            db.SaveChanges();

            // Returns a 200 OK response with the deleted user.
            return Ok(user);
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
        /// Checks if a user with the specified ID exists.
        /// </summary>
        /// <param name="id">The ID of the user to check for.</param>
        /// <returns>True if the user exists, otherwise false.</returns>
        private bool UserExists(int id)
        {
            // Returns true if a user with the specified ID exists in the database.
            return db.Users.Count(e => e.UserID == id) > 0;
        }
    }
}
