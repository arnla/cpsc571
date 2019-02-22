using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading;
using System.Diagnostics;
using Tweetinvi;
using cpsc571.Helpers;

namespace cpsc571.Controllers
{
    public class HomeController : Controller
    {
        private TwitterStream stream;

        // GET: Home
        public ActionResult Index()
        {
            stream = new TwitterStream();
            stream.SetupStream();
            ThreadStart job = new ThreadStart(stream.StartStream);
            Thread thread = new Thread(job);
            thread.Start();

            return View();
        }

        public JsonResult GetTweets()
        {
            return Json(stream.GetListTweets());
        }

        public void Stop()
        {
            stream.StopStream();
        }
    }
}