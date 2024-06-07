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
            // OBJECTIVE: Communication with the art data api to retrieve a list of comments
            //curl https://localhost:44350/api/commentsdata/listcomments
            string url = "commentsdata/listcomments";
            HttpResponseMessage response = client.GetAsync(url).Result;

            IEnumerable<Comments> comments = response.Content.ReadAsAsync<IEnumerable<Comments>>().Result;

            return View(comments);
        }

        // GET: Comments/Details/5
        public ActionResult Details(int id)
        {
            // OBJECTIVE: Communication with the comments data api to retrieve a one comment
            //curl https://localhost:44350/api/commentsdata/findcomments/{id}
            string url = "commentsdata/findcomments/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;

            Comments SelectedComment = response.Content.ReadAsAsync<Comments>().Result;

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

        // GET: Comments/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Comments/Create
        [HttpPost]
        public ActionResult Create(Comments comments)
        {
            string url = "commentsdata/addcomments";

            string jsonpayload = jss.Serialize(comments);
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

        // GET: Comments/Edit/5
        public ActionResult Edit(int id)
        {
            string url = "commentsdata/addcomments" + id;
            HttpResponseMessage responseMessage = client.GetAsync(url).Result;
            Comments SelectedComment = responseMessage.Content.ReadAsAsync<Comments>().Result;
            return View(SelectedComment);
        }

        // POST: Comments/Update/5
        [HttpPost]
        public ActionResult Update(int id, Comments comments)
        {
            string url = "commentsdata/addcomments" + id;
            string jsonpayload = jss.Serialize(comments);
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

        // GET: Comments/DeleteConfirm/5
        public ActionResult DeleteConfirm(int id)
        {
            string url = "commentsdata/findcomment/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            Comments comment = response.Content.ReadAsAsync<Comments>().Result;

            if (comment == null)
            {
                return RedirectToAction("Error");
            }

            return View(comment);
        }

        // POST: Comments/Delete/5
        [HttpPost]
        public ActionResult Delete(int id)
        {
            string url = "commentsdata/deletecomments/" + id;
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
