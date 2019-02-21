using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tweetinvi;
using Tweetinvi.Parameters;
namespace cpsc571.Controllers
{
    public class HomeController : Controller
    {
        private static string _consumerKey = "";
        private static string _consumerSecret = "";
        private static string _accessToken = "";
        private static string _accessTokenSecret = "";

        // GET: Home
        [HttpGet]
        public ActionResult Index() {
            //TweetinviConfig.CurrentThreadSettings.TweetMode = TweetMode.Extended;
            //Auth.SetUserCredentials(_consumerKey, _consumerSecret, _accessToken, _accessTokenSecret);
            //var tweets = Search.SearchTweets("dog");
            //foreach (Tweetinvi.Models.ITweet tweet in tweets)
            //{
            //    Console.WriteLine(tweet.FullText);
            //}
            //List<Tweetinvi.Models.ITweet> model = new List<Tweetinvi.Models.ITweet>(tweets);
            //return View(model);
            Helpers.TweetParser tp = new Helpers.TweetParser();
            tp.run();
            return null;

        }
    }
}