using PassionProject1N01659860.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace PassionProject1N01659860.Controllers
{
    public class UserController : Controller
    {
        private static readonly HttpClient client;
        private JavaScriptSerializer jss = new JavaScriptSerializer();

        static UserController()
        {
            client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:44350/api/");
        }

        // GET: User/List
        public ActionResult List()
        {
            // OBJECTIVE: Communication with the user data API to retrieve a list of users.
            // curl https://localhost:44350/api/userdata/listusers
            string url = "userdata/listusers";
            HttpResponseMessage response = client.GetAsync(url).Result;

            // Deserialize the JSON response into an IEnumerable of User objects.
            IEnumerable<User> users = response.Content.ReadAsAsync<IEnumerable<User>>().Result;

            // Pass the list of users to the view for rendering.
            return View(users);
        }

        // GET: User/Details/5
        public ActionResult Details(int id)
        {
            // Create a new view model instance to hold the user and their related comments.
            ArtDetailsViewModel ViewModel = new ArtDetailsViewModel();

            // OBJECTIVE: Communication with the user data API to retrieve a specific user.
            // curl https://localhost:44350/api/userdata/finduser/{id}
            string url = "userdata/finduser/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;

            // Deserialize the JSON response into a User object.
            User SelectedUser = response.Content.ReadAsAsync<User>().Result;

            // Assign the user to the view model.
            ViewModel.User = SelectedUser;

            // Send a request to gather information about comments related to this user.
            url = "commentsdata/ListCommentsForUser/" + id;
            response = client.GetAsync(url).Result;
            IEnumerable<CommentsDto> RelatedComments = response.Content.ReadAsAsync<IEnumerable<CommentsDto>>().Result;

            // Assign the comments to the view model.
            ViewModel.Comments = RelatedComments;

            // Pass the view model to the view for rendering.
            return View(ViewModel);
        }

        public ActionResult Error()
        {

            return View();
        }

        // GET: User/Create
        public ActionResult New()
        {
            return View();
        }

        // POST: User/Create
        [HttpPost]
        public ActionResult Create(User user)
        {
            // Define the API endpoint for adding a new user.
            string url = "userdata/adduser";

            // Serialize the user object to JSON format.
            string jsonpayload = jss.Serialize(user);
            Debug.WriteLine(jsonpayload);

            // Create a new HttpContent object for the JSON payload.
            HttpContent content = new StringContent(jsonpayload);
            content.Headers.ContentType.MediaType = "application/json";

            // Send the POST request to the API endpoint.
            HttpResponseMessage response = client.PostAsync(url, content).Result;
            if (response.IsSuccessStatusCode)
            {
                // If the request is successful, redirect to the user list.
                return RedirectToAction("List");
            }
            else
            {
                // If the request fails, redirect to the error view.
                return RedirectToAction("Error");
            }
        }

        // GET: User/Edit/5
        public ActionResult Edit(int id)
        {
            // Define the API endpoint for finding a user by ID.
            string url = "userdata/finduser/" + id;
            HttpResponseMessage responseMessage = client.GetAsync(url).Result;

            // Deserialize the JSON response into a User object.
            User selectedUser = responseMessage.Content.ReadAsAsync<User>().Result;

            // Pass the user object to the view for editing.
            return View(selectedUser);
        }

        // POST: User/Edit/5
        [HttpPost]
        public ActionResult Update(int id, User user)
        {
            // Define the API endpoint for updating a user.
            string url = "userdata/updateuser/" + id;

            // Serialize the user object to JSON format.
            string jsonpayload = jss.Serialize(user);

            // Create a new HttpContent object for the JSON payload.
            HttpContent content = new StringContent(jsonpayload);
            content.Headers.ContentType.MediaType = "application/json";

            // Send the POST request to the API endpoint.
            HttpResponseMessage response = client.PostAsync(url, content).Result;
            Debug.WriteLine(content);

            if (response.IsSuccessStatusCode)
            {
                // If the request is successful, redirect to the user list.
                return RedirectToAction("List");
            }
            else
            {
                // If the request fails, redirect to the error view.
                return RedirectToAction("Error");
            }
        }

        // GET: User/Delete/5
        public ActionResult DeleteConfirm(int id)
        {
            // Define the API endpoint for finding a user by ID.
            string url = "userdata/finduser/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;

            // Deserialize the JSON response into a User object.
            User selectedUser = response.Content.ReadAsAsync<User>().Result;

            // Pass the user object to the view for deletion confirmation.
            return View(selectedUser);
        }

        // POST: User/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            // Define the API endpoint for deleting a user by ID.
            string url = "userdata/deleteuser/" + id;

            // Create an empty HttpContent object since the delete request doesn't require a body.
            HttpContent content = new StringContent("");
            content.Headers.ContentType.MediaType = "application/json";

            // Send the POST request to the API endpoint.
            HttpResponseMessage response = client.PostAsync(url, content).Result;

            if (response.IsSuccessStatusCode)
            {
                // If the request is successful, redirect to the user list.
                return RedirectToAction("List");
            }
            else
            {
                // If the request fails, redirect to the error view.
                return RedirectToAction("Error");
            }
        }
    }
}
