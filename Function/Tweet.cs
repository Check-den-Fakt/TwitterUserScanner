using System;
using System.Collections.Generic;
using System.Text;

namespace TwitterToCosmos
{
    public class Tweet
    {
        public string content { get; set; }

        public string username { get; set; }

        public string id { get; set; }

        public DateTimeOffset createdAt { get; set; }

        public bool isRetweet { get; set; }
    }
}
