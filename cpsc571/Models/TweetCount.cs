using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Bson;

namespace cpsc571.Models
{
    public class TweetCount
    {
        public ObjectId _id { get; set; }
        public int Key { get; set; }
        public int Count { get; set; }
    }
}