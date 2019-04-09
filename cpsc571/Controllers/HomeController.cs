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
        private int COUNTERKEY = 2120151928;
        private static MongoClient mongoClient = new MongoClient("mongodb://localhost");
        private static IMongoDatabase _db = mongoClient.GetDatabase("cpsc571");
        // GET: Home

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Query()
        {
            return View();
        }

        public void GetTweets(int threshold)
        {
            IMongoCollection<Models.Tweet> collection = _db.GetCollection<Models.Tweet>("tweets");
            string jsonTweets = new JavaScriptSerializer().Serialize(collection.Find(t => t.Count >= threshold).ToList());
            string path = Server.MapPath("~/Data_Files/");
            System.IO.File.WriteAllText(path + "tweets.json", jsonTweets);
        }

        public JsonResult GetTweetCount()
        {
            IMongoCollection<Models.TweetCount> counterCollection = _db.GetCollection<Models.TweetCount>("tweetcount");
            Models.TweetCount counter = counterCollection.Find(t => t.Key.Equals(COUNTERKEY)).First();
            return Json(counter.Count);
        }

        //public void Stop()
        //{
        //    stream.StopStream();
        //}

        public JsonResult Restart()
        {
            System.Web.HttpRuntime.UnloadAppDomain();
            return Json(true);
        }

        [HttpPost]
        public void SubmitForm(string query, int threshold)
        {
            TwitterStream stream = new TwitterStream(query);
            stream.SetupStream();
            ThreadStart job = new ThreadStart(stream.StartStream);
            Thread thread = new Thread(job);
            thread.Start();
        }
    }
}