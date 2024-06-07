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
            // OBJECTIVE: Communication with the user data api to retrieve a list of users
            //curl https://localhost:44350/api/userdata/listusers
            string url = "userdata/listusers";
            HttpResponseMessage response = client.GetAsync(url).Result;

            IEnumerable<User> users = response.Content.ReadAsAsync<IEnumerable<User>>().Result;

            return View(users);
        }

        // GET: User/Details/5
        public ActionResult Details(int id)
        {
            // OBJECTIVE: Communication with the user data api to retrieve a one user
            //curl https://localhost:44350/api/userdata/finduser/{id}

            ArtDetailsViewModel ViewModel = new ArtDetailsViewModel();

            string url = "userdata/finduser/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;

            User SelectedUser = response.Content.ReadAsAsync<User>().Result;

            ViewModel.User = SelectedUser;

            //showcase information about animals related to this species
            //send a request to gather information about animals related to a particular species ID
            url = "commentsdata/ListCommentsForUser/" + id;
            response = client.GetAsync(url).Result;
            IEnumerable<CommentsDto> RelatedComments = response.Content.ReadAsAsync<IEnumerable<CommentsDto>>().Result;

            ViewModel.Comments = RelatedComments;


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
            string url = "userdata/adduser";

            string jsonpayload = jss.Serialize(user);
            Debug.WriteLine(jsonpayload);

            HttpContent content = new StringContent(jsonpayload);
            content.Headers.ContentType.MediaType = "application/json";

            HttpResponseMessage response = client.PostAsync(url, content).Result;
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("List");
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        // GET: User/Edit/5
        public ActionResult Edit(int id)
        {
            string url = "userdata/finduser/" + id;
            HttpResponseMessage responseMessage = client.GetAsync(url).Result;
            User selectedUser = responseMessage.Content.ReadAsAsync<User>().Result;
            return View(selectedUser);
        }

        // POST: User/Edit/5
        [HttpPost]
        public ActionResult Update(int id, User user)
        {
            string url = "userdata/updateuser/" + id;
            string jsonpayload = jss.Serialize(user);
            HttpContent content = new StringContent(jsonpayload);
            content.Headers.ContentType.MediaType = "application/json";
            HttpResponseMessage response = client.PostAsync(url, content).Result;
            Debug.WriteLine(content);
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("List");
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        // GET: User/Delete/5
        public ActionResult DeleteConfirm(int id)
        {
            string url = "userdata/finduser/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            User selectedUser = response.Content.ReadAsAsync<User>().Result;
            return View(selectedUser);
        }

        // POST: User/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            string url = "userdata/deleteuser/" + id;
            HttpContent content = new StringContent("");
            content.Headers.ContentType.MediaType = "application/json";
            HttpResponseMessage response = client.PostAsync(url, content).Result;

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("List");
            }
            else
            {
                return RedirectToAction("Error");
            }
        }
    }
}
