﻿using System;
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
        private static List<Tweetinvi.Models.ITweet> tweetsList = new List<Tweetinvi.Models.ITweet>();
        private static Tweetinvi.Streaming.IFilteredStream stream;
        private static Helpers.TweetParser tweetParser;
        private string query;
        private static MongoClient mongoClient;
        private static IMongoDatabase _db;
        private IMongoCollection<Models.Tweet> collection;
        private int ctr = 1;

        public TwitterStream(String query)
        {
            this.query = query;
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
            stream.PauseStream();
            List<Tweetinvi.Models.ITweet> listToDb = new List<Tweetinvi.Models.ITweet>(tweetsList);
            tweetsList.Clear();
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
            stream.ResumeStream();
        }

        private void CountTweetWords(string tweetText)
        {
            List<String> tweetWords = tweetParser.ParseTweet(tweetText);
            foreach (String word in tweetWords)
            {
                //using (var context = new TwitterDbContext())
                //{
                //    Models.Tweet dbTweet = context.Tweets.SingleOrDefault(t => t.Keyword == word);
                //    if(dbTweet == null)
                //    {
                //        dbTweet = new Models.Tweet();
                //        dbTweet.Keyword = word;
                //        dbTweet.Count = 1;
                //        context.Tweets.Add(dbTweet);
                //    } else
                //    {
                //        dbTweet.Count += 1;
                //    }
                //    context.SaveChanges();
                //}

                var dbTweet = collection.Find(t => t.Keyword == word).ToList();
                if (dbTweet.Count < 1)
                {
                    Models.Tweet newTweet = new Models.Tweet();
                    newTweet.Keyword = word;
                    newTweet.Count = 1;
                    newTweet.Id = ctr;
                    collection.InsertOne(newTweet);
                }
                else
                {
                    var filter = Builders<Models.Tweet>.Filter.Eq(t => t.Keyword, word);
                    var update = Builders<Models.Tweet>.Update.Inc(t => t.Count, 1);
                    collection.FindOneAndUpdate(filter, update);
                }
                ctr++;
            }
        }

        public List<Tweetinvi.Models.ITweet> GetListTweets()
        {
            List<Tweetinvi.Models.ITweet> returnTweets = new List<Tweetinvi.Models.ITweet>(tweetsList);
            tweetsList.Clear();
            return returnTweets;
        }

        private void SetupDb()
        {
            //mongoClient = new MongoClient("mongodb+srv://user:test@school-hjytx.mongodb.net/test?retryWrites=true");
            //_db = mongoClient.GetDatabase("cpsc571");
            //collection = _db.GetCollection<Models.Tweet>("tweets");

            mongoClient = new MongoClient("mongodb://localhost");
            _db = mongoClient.GetDatabase("cpsc571");
            collection = _db.GetCollection<Models.Tweet>("tweets");

            //Models.Tweet newTweet = new Models.Tweet();
            //newTweet.tweet = "pls work";
            //newTweet.count = 6;
            //collection.InsertOne(newTweet);
        }
    }
}