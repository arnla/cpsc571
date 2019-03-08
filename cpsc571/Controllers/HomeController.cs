using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading;
using System.Diagnostics;
using Tweetinvi;
using cpsc571.Helpers;
using cpsc571.DAL;
using cpsc571.Models;

namespace cpsc571.Controllers
{
    public class HomeController : Controller
    {
        private Helpers.TweetParser tweetParser;
        private Dictionary<string, int> tweetCount;
        private TwitterStream stream;
        private TwitterDbContext _db = new TwitterDbContext();

        private void CountTweetWords(List<String> words)
        {
            foreach(string word in words)
            {
                try
                {
                    tweetCount[word]++;
                }
                catch(KeyNotFoundException)
                {
                    tweetCount[word] = 1;
                }
            }
        }

        // GET: Home
        public ActionResult Index()
        {
            

            List<Models.Tweet> model = new List<Models.Tweet>();
            //var query = from t in _db.Tweets select t;
            //foreach (Models.Tweet t in query.ToList())
            //{
            //    model.Add(t);
            //}

            return View(model);
        }

        public JsonResult GetTweets()
        {
            return Json(stream.GetListTweets());
        }

        public void Stop()
        {
            stream.StopStream();
        }

        public void Tester(FormCollection form)
        {
            stream = new TwitterStream(form.Get("InputQuery"));
            stream.SetupStream();
            ThreadStart job = new ThreadStart(stream.StartStream);
            Thread thread = new Thread(job);
            thread.Start();
        }
    }
}