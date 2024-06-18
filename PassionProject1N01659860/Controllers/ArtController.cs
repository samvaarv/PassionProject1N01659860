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
    public class ArtController : Controller
    {
        private static readonly HttpClient client;
        private JavaScriptSerializer jss = new JavaScriptSerializer();

        static ArtController()
        {
            client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:44350/api/");
        }

        // GET: Art/List
        public ActionResult List()
        {
            // OBJECTIVE: Communication with the art data API to retrieve a list of art pieces.
            // curl https://localhost:44350/api/artdata/listarts
            string url = "artdata/listarts";
            HttpResponseMessage response = client.GetAsync(url).Result;

            // Deserialize the JSON response into an IEnumerable of Art objects.
            IEnumerable<Art> arts = response.Content.ReadAsAsync<IEnumerable<Art>>().Result;


            // Pass the list of art pieces to the view for rendering.
            return View(arts);
        }

        // GET: Art/Details/5
        public ActionResult Details(int id)
        {
            // OBJECTIVE: Communication with the art data API to retrieve a specific art piece.
            // curl https://localhost:44350/api/artdata/findart/{id}

            ArtDetailsViewModel ViewModel = new ArtDetailsViewModel();

            string url = "artdata/findart/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;

            // Deserialize the JSON response into an Art object.
            Art SelectedArt = response.Content.ReadAsAsync<Art>().Result;

            ViewModel.Art = SelectedArt;

            // Retrieve related comments for the art piece.
            url = "commentsdata/ListCommentsForArt/" + id;
            response = client.GetAsync(url).Result;
            IEnumerable<CommentsDto> RelatedComments = response.Content.ReadAsAsync<IEnumerable<CommentsDto>>().Result;

            ViewModel.Comments = RelatedComments;

            // Pass the art piece and related comments to the view for rendering.
            return View(ViewModel);
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

        // POST: Art/Create
        [HttpPost]
        public ActionResult Create(Art art)
        {
            // Define the API endpoint for adding a new art piece.
            string url = "artdata/addart";

            // Serialize the art object to JSON format.
            string jsonpayload = jss.Serialize(art);
            Debug.WriteLine(jsonpayload);

            // Create a new HttpContent object for the JSON payload.
            HttpContent content = new StringContent(jsonpayload);
            content.Headers.ContentType.MediaType = "application/json";

            // Send the POST request to the API endpoint.
            HttpResponseMessage response = client.PostAsync(url, content).Result;
            if (response.IsSuccessStatusCode)
            {
                // If the request is successful, redirect to the art list.
                return RedirectToAction("List");
            }
            else
            {
                // If the request fails, redirect to the error view.
                return RedirectToAction("Error");
            }
        }

        // GET: Art/Edit/5
        public ActionResult Edit(int id)
        {
            // Define the API endpoint for finding an art piece by ID.
            string url = "artdata/findart/" + id;
            HttpResponseMessage responseMessage = client.GetAsync(url).Result;

            // Deserialize the JSON response into an Art object.
            Art SelectedArt = responseMessage.Content.ReadAsAsync<Art>().Result;

            // Pass the art piece object to the view for editing.
            return View(SelectedArt);
        }

        // POST: Art/Edit/5
        [HttpPost]
        public ActionResult Update(int id, Art art)
        {
            // Define the API endpoint for updating an art piece.
            string url = "artdata/updateart/" + id;

            // Serialize the art object to JSON format.
            string jsonpayload = jss.Serialize(art);

            // Create a new HttpContent object for the JSON payload.
            HttpContent content = new StringContent(jsonpayload);
            content.Headers.ContentType.MediaType = "application/json";

            // Send the POST request to the API endpoint.
            HttpResponseMessage response = client.PostAsync(url, content).Result;
            Debug.WriteLine(content);

            if (response.IsSuccessStatusCode)
            {
                // If the request is successful, redirect to the art list.
                return RedirectToAction("List");
            }
            else
            {
                // If the request fails, redirect to the error view.
                return RedirectToAction("Error");
            }
        }

        // GET: Art/Delete/5
        public ActionResult DeleteConfirm(int id)
        {
            // Define the API endpoint for finding an art piece by ID.
            string url = "artdata/findart/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;

            // Deserialize the JSON response into an Art object.
            Art SelectedArt = response.Content.ReadAsAsync<Art>().Result;

            // Pass the art piece object to the view for deletion confirmation.
            return View(SelectedArt);
        }

        // POST: Art/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            // Define the API endpoint for deleting an art piece by ID.
            string url = "artdata/deleteart/" + id;

            // Create an empty HttpContent object since the delete request doesn't require a body.
            HttpContent content = new StringContent("");
            content.Headers.ContentType.MediaType = "application/json";

            // Send the POST request to the API endpoint.
            HttpResponseMessage response = client.PostAsync(url, content).Result;

            if (response.IsSuccessStatusCode)
            {
                // If the request is successful, redirect to the art list.
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
