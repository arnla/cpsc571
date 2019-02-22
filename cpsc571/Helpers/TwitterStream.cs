using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading;
using System.Diagnostics;
using Tweetinvi;

namespace cpsc571.Helpers
{
    public class TwitterStream
    {

        public List<Tweetinvi.Models.ITweet> tweetsList = new List<Tweetinvi.Models.ITweet>();
        private Tweetinvi.Streaming.IFilteredStream stream;

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
        }

        public void StopStream()
        {
            stream.StopStream();
        }

        public List<Tweetinvi.Models.ITweet> GetListTweets()
        {
            List<Tweetinvi.Models.ITweet> returnTweets = new List<Tweetinvi.Models.ITweet>(tweetsList);
            tweetsList.Clear();
            return returnTweets;
        }
    }
}