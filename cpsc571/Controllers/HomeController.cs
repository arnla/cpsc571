using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TweetSharp;

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
            TwitterService twitterService = new TwitterService(_consumerKey, _consumerSecret);
            twitterService.AuthenticateWith(_accessToken, _accessTokenSecret);

            int tweetcount = 1;
            var tweets_search = twitterService.Search(new SearchOptions { Q = "school", Resulttype = TwitterSearchResultType.Popular, Count = 100 });
            //Resulttype can be TwitterSearchResultType.Popular or TwitterSearchResultType.Mixed or TwitterSearchResultType.Recent  
            List<TwitterStatus> model = new List<TwitterStatus>(tweets_search.Statuses);
            //foreach (var tweet in tweets_search.Statuses)
            //{
            //    try
            //    {
            //        //tweet.User.ScreenName;  
            //        //tweet.User.Name;   
            //        //tweet.Text; // Tweet text  
            //        //tweet.RetweetCount; //No of retweet on twitter  
            //        //tweet.User.FavouritesCount; //No of Fav mark on twitter  
            //        //tweet.User.ProfileImageUrl; //Profile Image of Tweet  
            //        //tweet.CreatedDate; //For Tweet posted time  
            //        //"https://twitter.com/intent/retweet?tweet_id=" + tweet.Id;  //For Retweet  
            //        //"https://twitter.com/intent/tweet?in_reply_to=" + tweet.Id; //For Reply  
            //        //"https://twitter.com/intent/favorite?tweet_id=" + tweet.Id; //For Favorite  

            //        //Above are the things we can also get using TweetSharp.  
            //        Console.WriteLine("Sr.No: " + tweetcount + "\n" + tweet.User.Name + "\n" + tweet.User.ScreenName + "\n" + "https://twitter.com/intent/retweet?tweet_id=" + tweet.Id);
            //        tweetcount++;
            //    }
            //    catch { }
            //}
            return View(model);
        }
    }
}