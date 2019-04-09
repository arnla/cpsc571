using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading;
using System.Timers;
using System.Diagnostics;
using Tweetinvi;
using MongoDB.Driver;
using MongoDB.Bson;

namespace cpsc571.Helpers
{
    public class TwitterStream
    {
        private static string _consumerKey = "";
        private static string _consumerSecret = "";
        private static string _accessToken = "";
        private static string _accessTokenSecret = "";
        private int COUNTERKEY = 2120151928;
        private static List<Tweetinvi.Models.ITweet> tweetsList = new List<Tweetinvi.Models.ITweet>();
        private static Tweetinvi.Streaming.IFilteredStream stream;
        private static Helpers.TweetParser tweetParser;
        private string query;
        private static MongoClient mongoClient;
        private static IMongoDatabase _db;
        private IMongoCollection<Models.Tweet> collection;
        private IMongoCollection<Models.TweetCount> tweetCounterCollection;
        private int ctr = 1;
        private int tweetsRetrieved;

        public TwitterStream(String query)
        {
            this.query = query;
            tweetsRetrieved = 0;
        }

        public void SetupStream()
        {
            SetupDb();
            tweetParser = new TweetParser();
            TweetinviConfig.CurrentThreadSettings.TweetMode = TweetMode.Extended;
            Auth.SetUserCredentials(_consumerKey, _consumerSecret, _accessToken, _accessTokenSecret);
            stream = Stream.CreateFilteredStream();
            stream.AddTrack(query);

            stream.MatchingTweetReceived += (sender, args) =>
            {
                tweetsList.Add(args.Tweet);
                var tweet = args.Tweet;
                var matchingTracks = args.MatchingTracks;
                var matchedOn = args.MatchOn;
                Debug.WriteLine(args.Tweet);
            };

            Models.TweetCount counter = new Models.TweetCount();
            counter.Key = COUNTERKEY;
            counter.Count = 0;
            tweetCounterCollection.InsertOne(counter);
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
        
        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            List<Tweetinvi.Models.ITweet> listToDb = new List<Tweetinvi.Models.ITweet>(tweetsList);
            UpdateCount();
            tweetsList.Clear();
            foreach(Tweetinvi.Models.ITweet t in listToDb)
            {
                tweetsRetrieved++;
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

        private void UpdateCount()
        {
            var filter = Builders<Models.TweetCount>.Filter.Eq(c => c.Key, COUNTERKEY);
            var update = Builders<Models.TweetCount>.Update.Set(c => c.Count, tweetsRetrieved);
            tweetCounterCollection.FindOneAndUpdate(filter, update);
        }

        private void CountTweetWords(string tweetText)
        {
            List<String> tweetWords = tweetParser.ParseTweet(tweetText);
            foreach (String word in tweetWords)
            {

                var lowerWord = word.ToLower();
                if (lowerWord == this.query.ToLower())
                    continue;

                var dbTweet = collection.Find(t => t.Keyword == lowerWord).ToList();
                if (dbTweet.Count < 1)
                {
                    Models.Tweet newTweet = new Models.Tweet();
                    newTweet.Keyword = lowerWord;
                    newTweet.Count = 1;
                    collection.InsertOne(newTweet);
                    ctr++;
                }
                else
                {
                    var filter = Builders<Models.Tweet>.Filter.Eq(t => t.Keyword, lowerWord);
                    var update = Builders<Models.Tweet>.Update.Inc(t => t.Count, 1);
                    collection.FindOneAndUpdate(filter, update);
                }
            }
        }

        private void SetupDb()
        {
            mongoClient = new MongoClient("mongodb://localhost");
            mongoClient.DropDatabase("cpsc571");
            _db = mongoClient.GetDatabase("cpsc571");
            collection = _db.GetCollection<Models.Tweet>("tweets");
            tweetCounterCollection = _db.GetCollection<Models.TweetCount>("tweetcount");
        }
    }
}