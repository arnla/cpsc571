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
        private static Helpers.TweetParser tweetParser;

        public void SetupStream()
        {
            tweetParser = new TweetParser();
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
                String textToParse;
                if(t.IsRetweet)
                {
                    textToParse = t.RetweetedTweet.FullText;
                } else
                {
                    textToParse = t.FullText;
                }
                CountTweetWords(textToParse);
            }
        }

        private static void CountTweetWords(string tweetText)
        {
            List<String> tweetWords = tweetParser.ParseTweet(tweetText);
            foreach (String word in tweetWords)
            {
                using (var context = new TwitterDbContext())
                {
                    Models.Tweet dbTweet = context.Tweets.SingleOrDefault(t => t.Keyword == word);
                    if(dbTweet == null)
                    {
                        dbTweet = new Models.Tweet();
                        dbTweet.Keyword = word;
                        dbTweet.Count = 1;
                        context.Tweets.Add(dbTweet);
                    } else
                    {
                        dbTweet.Count += 1;
                    }
                    context.SaveChanges();
                }
                
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