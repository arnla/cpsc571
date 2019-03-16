using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading;
using System.Diagnostics;
using Tweetinvi;
using cpsc571.Helpers;
using cpsc571.Models;
using MongoDB.Driver;
using System.Web.Script.Serialization;

namespace cpsc571.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            return View();
        }

        public void GetTweets()
        {
            MongoClient mongoClient = new MongoClient("mongodb://localhost");
            IMongoDatabase _db = mongoClient.GetDatabase("cpsc571");
            IMongoCollection<Models.Tweet> collection = _db.GetCollection<Models.Tweet>("tweets");
            string jsonTweets = new JavaScriptSerializer().Serialize(collection.Find(_ => true).ToList());
            string path = Server.MapPath("~/Data_Files/");
            System.IO.File.WriteAllText(path + "tweets.json", jsonTweets);
        }

        //public void Stop()
        //{
        //    stream.StopStream();
        //}

        [HttpPost]
        public void SubmitForm(FormCollection form)
        {
            TwitterStream stream = new TwitterStream(form.Get("InputQuery"));
            stream.SetupStream();
            ThreadStart job = new ThreadStart(stream.StartStream);
            Thread thread = new Thread(job);
            thread.Start();
        }
    }
}