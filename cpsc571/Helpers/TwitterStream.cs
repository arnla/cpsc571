using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading;
using System.Timers;
using System.Diagnostics;
using Tweetinvi;
using cpsc571.DAL;

namespace cpsc571.Helpers
{
    public class TwitterStream
    {
        private static string _consumerKey = "";
        private static string _consumerSecret = "";
        private static string _accessToken = "";
        private static string _accessTokenSecret = "";
        private static List<Tweetinvi.Models.ITweet> tweetsList = new List<Tweetinvi.Models.ITweet>();
        private static Tweetinvi.Streaming.IFilteredStream stream;

        public void SetupStream()
        {
            TweetinviConfig.CurrentThreadSettings.TweetMode = TweetMode.Extended;
            Auth.SetUserCredentials(_consumerKey, _consumerSecret, _accessToken, _accessTokenSecret);
            stream = Stream.CreateFilteredStream();
            stream.AddTrack("dog");

            stream.MatchingTweetReceived += (sender, args) =>
            {
                tweetsList.Add(args.Tweet);
                var tweet = args.Tweet;
                var matchingTracks = args.MatchingTracks;
                var matchedOn = args.MatchOn;
                Debug.WriteLine(args.Tweet);
            };
        }

        public void StartStream()
        {
            stream.StartStreamMatchingAllConditionsAsync();
            System.Timers.Timer dbTimer = new System.Timers.Timer();
            dbTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            dbTimer.Interval = 5000;
            dbTimer.Enabled = true;
        }

        public void StopStream()
        {
            stream.StopStream();
        }

        private static void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            stream.PauseStream();
            List<Tweetinvi.Models.ITweet> listToDb = new List<Tweetinvi.Models.ITweet>(tweetsList);
            tweetsList.Clear();
            TwitterDbContext _db = new TwitterDbContext();
            foreach(Tweetinvi.Models.ITweet t in listToDb)
            {
                Models.Tweet tweet = new Models.Tweet();
                if(t.IsRetweet)
                {
                    tweet.FullText = t.RetweetedTweet.FullText;
                } else
                {
                    tweet.FullText = t.FullText;
                }
                _db.Tweets.Add(tweet);
                _db.SaveChanges();
            }
        }

        public List<Tweetinvi.Models.ITweet> GetListTweets()
        {
            List<Tweetinvi.Models.ITweet> returnTweets = new List<Tweetinvi.Models.ITweet>(tweetsList);
            tweetsList.Clear();
            return returnTweets;
        }
    }
}