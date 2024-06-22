using PassionProject1N01659860.Migrations;
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
    public class CommentsController : Controller
    {
        private static readonly HttpClient client;
        private JavaScriptSerializer jss = new JavaScriptSerializer();

        static CommentsController()
        {
            client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:44350/api/");
        }

        // GET: Comments/List
        public ActionResult List()
        {
            // OBJECTIVE: Communication with the comments data API to retrieve a list of comments.
            // curl https://localhost:44350/api/commentsdata/listcomments
            string url = "commentsdata/listcomments";
            HttpResponseMessage response = client.GetAsync(url).Result;

            // Deserialize the JSON response into an IEnumerable of Comments objects.
            IEnumerable<CommentsDto> comments = response.Content.ReadAsAsync<IEnumerable<CommentsDto>>().Result;

            // Pass the list of comments to the view for rendering.
            return View(comments);
        }

        // GET: Comments/Details/5
        public ActionResult Details(int id)
        {
            // OBJECTIVE: Communication with the comments data API to retrieve a specific comment.
            // curl https://localhost:44350/api/commentsdata/findcomment/{id}
            string url = "commentsdata/findcomment/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;

            // Deserialize the JSON response into a Comments object.
            CommentsDto SelectedComment = response.Content.ReadAsAsync<CommentsDto>().Result;

            // Pass the comment object to the view for rendering.
            return View(SelectedComment);
        }

        public ActionResult Error()
        {
            return View();
        }

        // GET: Art/Create
        public ActionResult New()
        {
            return View();
        }

        // POST: Comments/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Comments comments)
        {
            // Define the API endpoint for adding a new comment.
            string url = "commentsdata/addcomment";

            // Serialize the comments object to JSON format.
            string jsonpayload = jss.Serialize(comments);
            Debug.WriteLine(jsonpayload);

            // Create a new HttpContent object for the JSON payload.
            HttpContent content = new StringContent(jsonpayload);
            content.Headers.ContentType.MediaType = "application/json";

            // Send the POST request to the API endpoint.
            HttpResponseMessage response = client.PostAsync(url, content).Result;
            if (response.IsSuccessStatusCode)
            {
                // If the request is successful, redirect to the commented art.
                return RedirectToAction("Details", "Art", new { id = comments.ArtID }); // Redirect to Art Details view
            }
            else
            {
                // If the request fails, redirect to the error view.
                return RedirectToAction("Error");
            }
        }

        // GET: Comments/Edit/5
        public ActionResult Edit(int id)
        {
            // Define the API endpoint for finding a comment by ID.
            string url = "commentsdata/findcomment/" + id;
            HttpResponseMessage responseMessage = client.GetAsync(url).Result;

            // Deserialize the JSON response into a Comments object.
            CommentsDto SelectedComment = responseMessage.Content.ReadAsAsync<CommentsDto>().Result;

            // Pass the comment object to the view for editing.
            return View(SelectedComment);
        }

        // POST: Comments/Update/5
        [HttpPost]
        public ActionResult Update(int id, Comments comments)
        {
            // Define the API endpoint for updating a comment.
            string url = "commentsdata/updatecomment/" + id;

            // Serialize the comments object to JSON format.
            string jsonpayload = jss.Serialize(comments);

            // Create a new HttpContent object for the JSON payload.
            HttpContent content = new StringContent(jsonpayload);
            content.Headers.ContentType.MediaType = "application/json";

            // Send the POST request to the API endpoint.
            HttpResponseMessage response = client.PostAsync(url, content).Result;
            Debug.WriteLine(content);

            if (response.IsSuccessStatusCode)
            {
                // If the request is successful, redirect to the commented art.
                return RedirectToAction("Details", "Art", new { id = comments.ArtID }); // Redirect to Art Details view
            }
            else
            {
                // If the request fails, redirect to the error view.
                return RedirectToAction("Error");
            }
        }

        // GET: Comments/DeleteConfirm/5
        public ActionResult DeleteConfirm(int id)
        {
            // Define the API endpoint for finding a comment by ID.
            string url = "commentsdata/findcomment/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;

            // Deserialize the JSON response into a Comments object.
            CommentsDto SelectedComment = response.Content.ReadAsAsync<CommentsDto>().Result;

            if (SelectedComment == null)
            {
                // If the comment is not found, redirect to the error view.
                return RedirectToAction("Error");
            }

            // Pass the comment object to the view for deletion confirmation.
            return View(SelectedComment);
        }

        // POST: Comments/Delete/5
        [HttpPost]
        public ActionResult Delete(int id)
        {
            // Define the API endpoint for deleting a comment by ID.
            string url = "commentsdata/deletecomment/" + id;

            // Create an empty HttpContent object since the delete request doesn't require a body.
            HttpContent content = new StringContent("");
            content.Headers.ContentType.MediaType = "application/json";

            // Send the POST request to the API endpoint.
            HttpResponseMessage response = client.PostAsync(url, content).Result;

            if (response.IsSuccessStatusCode)
            {
                // If the request is successful, redirect to the comments list.
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
