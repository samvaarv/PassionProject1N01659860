﻿using PassionProject1N01659860.Models;
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
            // OBJECTIVE: Communication with the art data api to retrieve a list of arts
            //curl https://localhost:44350/api/artdata/listarts
            string url = "artdata/listarts";
            HttpResponseMessage response = client.GetAsync(url).Result;

            IEnumerable<Art> arts = response.Content.ReadAsAsync<IEnumerable<Art>>().Result;

            return View(arts);
        }

        // GET: Art/Details/5
        public ActionResult Details(int id)
        {
            // OBJECTIVE: Communication with the art data api to retrieve a one art
            //curl https://localhost:44350/api/artdata/findart/{id}

            ArtDetailsViewModel ViewModel = new ArtDetailsViewModel();

            string url = "artdata/findart/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;

            Art SelectedArt = response.Content.ReadAsAsync<Art>().Result;

            ViewModel.Art = SelectedArt;

            //showcase information about animals related to this species
            //send a request to gather information about animals related to a particular species ID
            url = "commentsdata/ListCommentsForArt/" + id;
            response = client.GetAsync(url).Result;
            IEnumerable<CommentsDto> RelatedComments = response.Content.ReadAsAsync<IEnumerable<CommentsDto>>().Result;

            ViewModel.Comments = RelatedComments;


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
            string url = "artdata/addart";

            string jsonpayload = jss.Serialize(art);
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

        // GET: Art/Edit/5
        public ActionResult Edit(int id)
        {
            string url = "artdata/findart/" + id;
            HttpResponseMessage responseMessage = client.GetAsync(url).Result;
            Art SelectedArt = responseMessage.Content.ReadAsAsync<Art>().Result;
            return View(SelectedArt);
        }

        // POST: Art/Edit/5
        [HttpPost]
        public ActionResult Update(int id, Art art)
        {
            string url = "artdata/updateart/" + id;
            string jsonpayload = jss.Serialize(art);
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

        // GET: Art/Delete/5
        public ActionResult DeleteConfirm(int id)
        {
            string url = "artdata/findart/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            Art SelectedArt = response.Content.ReadAsAsync<Art>().Result;
            return View(SelectedArt);
        }

        // POST: Art/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            string url = "artdata/deleteart/" + id;
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
